<?php
require dirname(__FILE__).'/vendor/autoload.php';
require dirname(__FILE__).'/AtomiaGrpcBaseTypes.php';
require dirname(__FILE__).'/AtomiaGrpcBilling.php';

// Store the token inside this global
$token = null;

// Authentication function
function auth($hostname, $username, $password) {
    // Prepare identity url
    $identity_url = 'https://' . $hostname . '/LoginGetJwt';

    // Prepare post data
    $post_data = array(
        'username' => $username,
        'password' => $password
    );

    // Prepare curl
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $identity_url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($post_data));
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, 0);

    // Validate credentials and receive the token
    $data = json_decode(curl_exec($ch));

    // The token that is retrieved is valid for 24 hours
    // In a real world application it is recommended that the
    // token is cached and reused
    $token = null;

    if ($data->success) {
        $token = $data->data;
    }

    return $token;
}

// Add the token to the metadata that is sent to the API endpoint
function auth_interceptor($context) {
    return [ 'authorization' => [ 'bearer ' . $GLOBALS['token'] ] ];
}

// Sends an echo request to the API
function echo_req($hostname, $port, $cert, $message) {
    // Read the cert contents
    $cert_data = file_get_contents($cert);

    // Create a SSL credentials object
    $ssl_creds = Grpc\ChannelCredentials::createSsl($cert_data);

    // Create a call credentials object
    $call_creds = Grpc\CallCredentials::createFromPlugin('auth_interceptor');

    // Combine the SSL and call credentials objects
    $chan_creds = Grpc\ChannelCredentials::createComposite($ssl_creds,
        $call_creds);
    $opts = [
        'credentials' => $chan_creds,
    ];

    // Create a new client object
    $server = $hostname . ':' . $port;
    $client = new AtomiaGrpcBilling\AtomiaGrpcBillingClient($server, $opts);

    // Call the echo method on the API
    $echo_request = new AtomiaGrpcBilling\EchoRequest();
    $echo_request->setMessage($message);
    list($reply, $status) = $client->Echo($echo_request)->wait();

    return $reply->message;
}

function usage() {
    echo "Usage: php client.php [OPTION]\n" .
         "\n" .
         "  -a, --api=ARG       hostname of the Billing server\n" .
         "  -p, --port=ARG      port of the Billing server\n" .
         "  -c, --cert=ARG      path to the certificate\n" .
         "  -i, --identity=ARG  hostname of the Identity server\n" .
         "  -u, --username=ARG  username\n" .
         "  -w, --password=ARG  password\n" .
         "  -m, --message=ARG   message to echo\n" .
         "  -h, --help          display this help\n";
}

function main() {
    // Parse commands
    $short_opts = 'a:p:c:i:u:w:m:h';

    $long_opts = array(
        'api:',
        'port:',
        'cert:',
        'identity:',
        'username:',
        'password:',
        'message:',
        'help'
    );

    $opts = getopt($short_opts, $long_opts);

    // We need all parameters to be set
    $hostname = null;
    $port = null;
    $cert = null;
    $identity = null;
    $username = null;
    $password = null;
    $message = null;

    foreach (array_keys($opts) as $opt) {
        switch ($opt) {
            case 'a':
            case 'api':
                $hostname = $opts[$opt];
                break;

            case 'p':
            case 'port':
                $port = $opts[$opt];
                break;

            case 'c':
            case 'cert':
                $cert = $opts[$opt];
                break;

            case 'i':
            case 'identity':
                $identity = $opts[$opt];
                break;

            case 'u':
            case 'username':
                $username = $opts[$opt];
                break;

            case 'w':
            case 'password':
                $password = $opts[$opt];
                break;

            case 'm':
            case 'message':
                $message = $opts[$opt];
                break;

            case 'h':
            case 'help':
                usage();
                exit(1);

            default:
                echo "error: unsupported option\n";
                exit(1);
        }
    }

    if ($hostname == null ||
        $port == null ||
        $cert == null ||
        $identity == null ||
        $username == null ||
        $password == null ||
        $message == null) {
        print_help();
        exit(1);
    }

    // Check if the username and password is correct
    $token = auth($identity, $username, $password);

    if ($token == null) {
        echo "error: invalid username and/or password\n";
        exit(1);
    }

    $GLOBALS['token'] = $token;

    // Call the echo method on the Billing API server
    $ret = echo_req($hostname, $port, $cert, $message);
    echo $ret . "\n";
}

main();

