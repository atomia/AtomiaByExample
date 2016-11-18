'use strict';

var fs = require('fs');
var grpc = require('grpc');
var request = require('request');
var Getopt = require('node-getopt');

var base_proto_path = __dirname + '/../proto/AtomiaGrpcBaseTypes.proto';
var base_proto = grpc.load(base_proto_path).AtomiaGrpcBaseTypes;
var account_proto_path = __dirname + '/../proto/AtomiaGrpcAccount.proto';
var account_proto = grpc.load(account_proto_path).AtomiaGrpcBilling;
var billing_proto_path = __dirname + '/../proto/AtomiaGrpcBilling.proto';
var billing_proto = grpc.load(billing_proto_path).AtomiaGrpcBilling;

// Authentication function
var auth = function(hostname, username, password, callback) {
    // Prepare identity url
    var identity_url = 'https://' + hostname + '/LoginGetJwt';

    // Prepare post data
    var post_data = {
        formData: {
            username: username,
            password: password
        }
    };

    // Allow self signed certs
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

    // Validate credentials and receive the token
    request.post(identity_url, post_data, function(err, response, body) {
        var data = JSON.parse(body);

        if (err || !data.success) {
            callback('error: invalid username and/or password', null);
        } else {
            // The token that is retrieved is valid for 24 hours
            // In a real world application it is recommended that the
            // token is cached and reused
            callback(null, data.data);
        }
    });
}

// Sends an echo request to the API
var echo_req = function(hostname, port, cert, token, message, callback) {
    // Fetch the cert data
    var cert_data = fs.readFileSync(cert);

    // Create a SSL credentials object
    var ssl_creds = grpc.credentials.createSsl(cert_data);

    // Add the token to the metadata that is sent to the API endpoint
    var auth_interceptor = function(service_url, callback) {
        var metadata = new grpc.Metadata();
        metadata.set('authorization', 'bearer ' + token);

        callback(null, metadata);
    }

    // Create a call credentials object
    var call_creds =
        grpc.credentials.createFromMetadataGenerator(auth_interceptor);

    // Combine the SSL and call credentials objects
    var chan_creds = grpc.credentials.combineChannelCredentials(ssl_creds,
        call_creds)

    // Create a new client object
    var server = hostname + ':' + port;
    var client = new billing_proto.AtomiaGrpcBilling(server, chan_creds);

    // Call the echo method on the API
    var echo_request = { message: message };
    client.echo(echo_request, function(err, echo_response) {
        if (err) {
            callback(err, null);
        } else {
            callback(null, echo_response.message);
        }
    });
}

function main() {
    // Parse commands
    var getopt = new Getopt([
        [ 'a', 'api=ARG', 'hostname of the Billing server' ],
        [ 'p', 'port=ARG', 'port of the Billing server' ],
        [ 'c', 'cert=ARG', 'path to the certificate' ],
        [ 'i', 'identity=ARG', 'hostname of the Identity server' ],
        [ 'u', 'username=ARG', 'username' ],
        [ 'w', 'password=ARG', 'password' ],
        [ 'm', 'message=ARG', 'message to echo' ],
        [ 'h', 'help', 'display this help' ]
    ]);

    getopt.setHelp(
        "Usage: node app.js [OPTION]\n" +

        "\n" +
        "[[OPTIONS]]"
    );

    var opts = getopt.parse(process.argv.slice(2));

    // We need all parameters to be set
    var hostname = null;
    var port = null;
    var cert = null;
    var identity = null;
    var username = null;
    var password = null;
    var message = null;

    for (var opt in opts.options) {
        switch (opt) {
            case 'api':
                hostname = opts.options[opt];
                break;

            case 'port':
                port = opts.options[opt];
                break;

            case 'cert':
                cert = opts.options[opt];
                break;

            case 'identity':
                identity = opts.options[opt];
                break;

            case 'username':
                username = opts.options[opt];
                break;

            case 'password':
                password = opts.options[opt];
                break;

            case 'message':
                message = opts.options[opt];
                break;

            case 'help':
                getopt.showHelp();
                return 1;

            default:
                console.log('error: unsupported option\n');
                return 1;
        }
    }

    if (hostname === null ||
        port === null ||
        cert === null ||
        identity === null ||
        username === null ||
        password === null ||
        message === null) {
        getopt.showHelp();
        return 1;
    }

    // Check if the username and password is correct
    auth(identity, username, password, function(err, token) {
        if (err) {
            console.log(err);
        } else {
            // Call the echo method on the Billing API server
            echo_req(hostname, port, cert, token, message, function(err, msg) {
                if (err) {
                    console.log(err);
                } else {
                    console.log(msg);
                }
            });
        }
    });
}

main();

