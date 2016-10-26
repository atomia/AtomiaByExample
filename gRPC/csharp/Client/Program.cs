using Atomia.Grpc.Billing;
using Gnu.Getopt;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using static Atomia.Grpc.Billing.AtomiaGrpcBilling;

namespace Client
{
    class Program
    {
        // Authentication function
        static string auth(string hostname, string username, string password)
        {
            // Prepare identity url
            var identity_url = $"https://{hostname}/LoginGetJwt";

            // Prepare post data
            var post_data = new System.Collections.Specialized.NameValueCollection()
            {
                { "username", username },
                { "password", password }
            };

            // Validate credentials and receive the token
            var client = new WebClient();
            var response = client.UploadValues(identity_url, post_data);
            var data = JObject.Parse(Encoding.UTF8.GetString(response));

            // The token that is retrieved is valid for 24 hours
            // In a real world application it is recommended that the
            // token is cached and reused
            var token = string.Empty;

            if (data["success"].Value<bool>())
            {
                token = data["data"].Value<string>();
            }

            return token;
        }

        // Add the token to the metadata that is sent to the API endpoint
        static AsyncAuthInterceptor authInterceptor(string token)
        {
            return new AsyncAuthInterceptor(async (context, metadata) =>
            {
                await Task.Delay(0);
                metadata.Add(new Metadata.Entry("authorization", $"bearer {token}"));
            });
        }

        // Sends an echo request to the API
        static string echo_req(string hostname, string port, string token, string cert, string message)
        {
            // Read the cert contents
            var cert_data = File.ReadAllText(cert);

            // Create a SSL credentials object
            var ssl_creds = new SslCredentials(cert_data);

            // Create a call credentials object
            var call_creds = CallCredentials.FromInterceptor(authInterceptor(token));
            var call_opts = new CallOptions(credentials: call_creds);

            // Create a new channel and client
            var channel = new Channel($"{hostname}:{port}", ssl_creds);
            var client = new AtomiaGrpcBillingClient(channel);

            // Call the echo method on the API
            var echo_request = new EchoRequest { Message = message };
            var echo_reply = client.Echo(echo_request, call_opts);

            return echo_reply.Message;
        }

        static void usage()
        {
            Console.WriteLine("Usage: client.exe [OPTION]");
            Console.WriteLine();
            Console.WriteLine("  -a, --api=ARG       hostname of the Billing server");
            Console.WriteLine("  -p, --port=ARG      port of the Billing server");
            Console.WriteLine("  -c, --cert=ARG      path to the certificate");
            Console.WriteLine("  -i, --identity=ARG  hostname of the Identity server");
            Console.WriteLine("  -u, --username=ARG  username");
            Console.WriteLine("  -w, --password=ARG  password");
            Console.WriteLine("  -m, --message=ARG   message to echo");
            Console.WriteLine("  -h, --help          display this help");
        }

        static void Main(string[] args)
        {
            // Parse commands
            var c = 0;
            var short_opts = "a:p:c:i:u:w:m:h";
            var long_opts = new List<LongOpt>()
            {
                new LongOpt("api", Argument.Required, null, 'a'),
                new LongOpt("port", Argument.Required, null, 'p'),
                new LongOpt("cert", Argument.Required, null, 'c'),
                new LongOpt("identity", Argument.Required, null, 'i'),
                new LongOpt("username", Argument.Required, null, 'u'),
                new LongOpt("password", Argument.Required, null, 'w'),
                new LongOpt("message", Argument.Required, null, 'm'),
                new LongOpt("help", Argument.No, null, 'h')
            };

            var opts = new Getopt("Client", args, short_opts, long_opts.ToArray());
            opts.Opterr = false;

            // We need all parameters to be set
            var hostname = string.Empty;
            var port = string.Empty;
            var cert = string.Empty;
            var identity = string.Empty;
            var username = string.Empty;
            var password = string.Empty;
            var message = string.Empty;
            var help = false;

            while ((c = opts.getopt()) != -1)
            {
                switch (c)
                {
                    case 'a':
                        hostname = opts.Optarg;
                        break;

                    case 'p':
                        port = opts.Optarg;
                        break;

                    case 'c':
                        cert = opts.Optarg;
                        break;

                    case 'i':
                        identity = opts.Optarg;
                        break;

                    case 'u':
                        username = opts.Optarg;
                        break;

                    case 'w':
                        password = opts.Optarg;
                        break;

                    case 'm':
                        message = opts.Optarg;
                        break;

                    case 'h':
                        help = true;
                        break;

                    default:
                        Console.WriteLine("error: unsupported option");
                        break;
                }
            }

            if (help ||
                string.IsNullOrEmpty(hostname) ||
                string.IsNullOrEmpty(port) ||
                string.IsNullOrEmpty(cert) ||
                string.IsNullOrEmpty(identity) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(message))
            {
                usage();
                Environment.Exit(1);
            }

            // Check if the username and password is correct
            var token = auth(identity, username, password);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("error: invalud username and/or password");
                Environment.Exit(1);
            }

            // Call the echo method on the Billing API server
            var ret = echo_req(hostname, port, token, cert, message);
            Console.WriteLine(ret);
        }
    }
}
