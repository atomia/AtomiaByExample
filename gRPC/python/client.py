#!/usr/bin/env python2.7

import os
import sys
import grpc
import getopt
import requests
import AtomiaGrpcBilling_pb2

# Store the token inside this global
token = None

def auth(hostname, username, password):
    # Prepare identity url
    identity_url = 'https://{}/LoginGetJwt'.format(hostname)

    # Prepare post data
    post_data = {
        'username': username,
        'password': password
    }

    # Allow self signed certificates
    requests.packages.urllib3.disable_warnings()

    # Validate credentials and receive the token
    response = requests.post(identity_url, data=post_data, verify=False)
    data = response.json()

    # The token that is retrieved is valid for 24 hours
    # In a real world application it is recommended that the
    # token is cached and reused
    token = None

    if data['success']:
        token = data['data']

    return token

# Add the token to the metadata that is sent to the API endpoint
def auth_interceptor(context, callback):
    global token
    callback([('authorization', 'bearer {}'.format(token))], None)

# Sends an echo request to the API
def echo_req(hostname, port, cert, message):
    # Read the cert contents
    cert_data = open(cert).read()

    # Create a SSL credentials object
    ssl_creds = grpc.ssl_channel_credentials(cert_data)

    # Create a call credentials object
    call_creds = grpc.metadata_call_credentials(auth_interceptor)

    # Combine the SSL and call credentials objects
    chan_creds = grpc.composite_channel_credentials(ssl_creds, call_creds)

    # Create a new channel and client
    server = '{}:{}'.format(hostname, port)
    channel = grpc.secure_channel(server, chan_creds)
    client = AtomiaGrpcBilling_pb2.AtomiaGrpcBillingStub(channel)

    # Call the echo method on the API
    echo_request = AtomiaGrpcBilling_pb2.EchoRequest(message=message)
    echo_response = client.Echo(echo_request)

    return echo_response.message

# Displays the help message
def usage():
    print """Usage: python client.py [OPTION]

  -a, --api=ARG       hostname of the Billing server
  -p, --port=ARG      port of the Billing server
  -c, --cert=ARG      path to the certificate
  -i, --identity=ARG  hostname of the Identity server
  -u, --username=ARG  username
  -w, --password=ARG  password
  -m, --message=ARG   message to echo
  -h, --help          display this help"""

def main():
    # Parse commands
    short_opts = 'a:p:c:i:u:w:m:h'
    long_opts = [
        'api=',
        'port=',
        'cert=',
        'identity=',
        'username=',
        'password=',
        'message=',
        'help'
    ]

    try:
        opts, args = getopt.getopt(sys.argv[1:], short_opts, long_opts)
    except getopt.GetoptError:
        print 'error: unsupported option'
        sys.exit(1)

    # We need all parameters to be set
    hostname = None
    port = None
    cert = None
    identity = None
    username = None
    password = None
    message = None

    for opt, arg in opts:
        if opt in ('-a', '--api'):
            hostname = arg
        elif opt in ('-p', '--port'):
            port = arg
        elif opt in ('-c', '--cert'):
            cert = arg
        elif opt in ('-i', '--identity'):
            identity = arg
        elif opt in ('-u', '--username'):
            username = arg
        elif opt in ('-w', '--password'):
            password = arg
        elif opt in ('-m', '--message'):
            message = arg
        elif opt in ('-h', '--help'):
            usage()
            sys.exit(1)

    if (hostname == None or
        port == None or
        cert == None or
        identity == None or
        username == None or
        password == None or
        message == None):
        usage()
        sys.exit(1)

    # Check if the username and password is correct
    global token
    token = auth(identity, username, password)

    if token == None:
        print 'error: invalid username and/or password'
        sys.exit(1)

    # Call the echo method on the Billing API server
    ret = echo_req(hostname, port, cert, message)
    print ret

if __name__ == '__main__':
    main()

