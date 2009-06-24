using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace merapi
{
    /**
     * Class PolicyServer
     * Starts a PolicyServer on the specified port.
     * Can be started as main class, passing the port number as the first command line argument
     * @author Thomas Meyer, Less Rain (thomas@lessrain.com)
     *
     */
    public class PolicyServer
    {
        /**
	     * If no argument is passed the server will listen on this port for connections
	     */
        public const int DEFAULT_PORT = 12344;
        public static String[] DEFAULT_POLICY = new String[] { "*" };

        /**
         * The character sequence sent by the Flash Player to request a _policy file
         */
        public const String POLICY_REQUEST = "<policy-file-request/>";

        public const bool DEBUG = true;

        /**
         * @param args	Use the first command line argument to set the port the server will listen on for connections
         */
        public static void Start()
        {
            int port = DEFAULT_PORT;

            // Start the PolicyServer
            PolicyServer server = new PolicyServer( port, new String[] { "*:80" } );
            new Thread( server.Run ).Start();
        }


        /*
         * PolicyServer class variables
         */
        private int _port;
        private bool _listening;
        private Socket _socketServer;
        private static String _policy;


        /**
         * PolicyServer constructor
         * @param port_	Sets the port that the PolicyServer listens on
         */
        public PolicyServer( int port_, String[] allowedHosts_ )
        {
            _port = port_;
            _listening = true;
            if ( allowedHosts_ == null ) allowedHosts_ = DEFAULT_POLICY;
            _policy = buildPolicy( allowedHosts_ );
        }

        /**
         *  @private
         *  
         *  Constructs the cross domain policy string for the Flash Player
         */
        private String buildPolicy( String[] allowedHosts_ )
        {
            StringBuilder policyBuffer = new StringBuilder();

            policyBuffer.Append( "<?xml version=\"1.0\"?><cross-domain-policy>" );
            policyBuffer.Append( "<site-control permitted-cross-domain-policies=\"by-content-type\"/>" );

            for ( int i = 0; i < allowedHosts_.Length; i++ )
            {
                String[] hostInfo = allowedHosts_[ i ].Split( new char[] { ':' } );
                String hostname = hostInfo[ 0 ];
                String ports;
                if ( hostInfo.Length > 1 ) ports = hostInfo[ 1 ];
                else ports = "*";

                policyBuffer.Append( "<allow-access-from domain=\"" + hostname + "\" to-ports=\"12345\" />" );
            }
            policyBuffer.Append( "</cross-domain-policy>" );

            return policyBuffer.ToString();
        }

        /**
         * Thread run method, accepts incoming connections and creates SocketConnection objects to handle requests
         */
        public void Run()
        {
            try
            {
                _listening = true;

                // Start listening for connections
                _socketServer = new Socket( AddressFamily.InterNetwork, SocketType.Stream, 
                                            ProtocolType.Tcp );
                _socketServer.Bind( new IPEndPoint( IPAddress.Any, _port ) );
               
                // start listening...
                _socketServer.Listen( 4 );

                if ( DEBUG ) System.Console.WriteLine( "PolicyServer listening on port " + _port );

                while ( _listening )
                {
                    // Wait for a connection and accept it

                    System.Console.WriteLine( "1" );

                    Socket socket = _socketServer.Accept();

                    System.Console.WriteLine( "2" );

                    try
                    {
                        if ( DEBUG ) System.Console.WriteLine( "PolicyServer got a connection on port " + _port );
                        // Start a new connection thread
                        new Thread( new SocketConnection( socket ).Run ).Start();
                    }
                    catch ( Exception e )
                    {
                        if ( DEBUG ) System.Console.WriteLine( "Exception: " + e.ToString() );
                    }

                    // Wait for a sec until a new connection is accepted to avoid flooding
                    //Thread.Sleep( 1000 );
                }
            }
            catch ( IOException e )
            {
                if ( DEBUG ) System.Console.WriteLine( "IO Exception: " + e.ToString() );
            }
        }

        /**
         * Local class SocketConnection
         * For every accepted connection one SocketConnection is created.
         * It waits for the _policy file request, returns the _policy file and closes the connection immediately
         * @author Thomas Meyer, Less Rain (thomas@lessrain.com)
         */
        class SocketConnection
        {
            private Socket _socket;
            private StreamReader _socketIn;
            private StreamWriter _socketOut;

            /**
             * Constructor takes the Socket object for this connection
             * @param socket_ Socket connection to a client created by the PolicyServer main thread
             */
            public SocketConnection( Socket socket_ )
            {
                _socket = socket_;
            }

            /**
             * Thread run method waits for the _policy request, returns the poilcy file and closes the connection
             */
            public void Run()
            {
                readPolicyRequest();
            }

            /**
             * Wait for and read the _policy request sent by the Flash Player
             * Return the _policy file and close the Socket connection
             */
            private void readPolicyRequest()
            {
                try
                {
                    // Read the request and compare it to the request string defined in the constants.
                    // If the proper _policy request has been sent write out the _policy file
                    String readData = read();
                    if ( POLICY_REQUEST.Equals( readData ) ) Write( _policy );
                }
                catch ( Exception e )
                {
                    if ( DEBUG ) System.Console.WriteLine( "PolicyServer.readPolicyRequest() " + e.ToString() );
                }
                Close();
            }

            /**
             * Read until a zero character is sent or a maximum of 100 character
             * @return The character sequence read
             * @throws IOException
             * @throws EOFException
             * @throws InterruptedIOException
             */
            private String read()
            {
                byte[] buffer = new byte[ _socket.ReceiveBufferSize ];
                _socket.Receive( buffer );

                StringBuilder strb = new StringBuilder();
                foreach ( byte byt in buffer )
                {
                    if ( byt == (byte)0 ) break;

                    strb.Append( (char)byt );
                }

                return strb.ToString();
            }

            /**
             * Writes a String to the client
             * @param msg	Text to be sent to the client (_policy file)
             */
            public void Write( String msg )
            {
                msg += "\u0000";
                
                char[] chars = msg.ToCharArray();
                byte[] buffer = new byte[ chars.Length ];
                int idx = 0;
                
                foreach ( char chr in chars )
                {
                    buffer[ idx ] = (byte)chr;
                    idx++;
                }

                _socket.Send( buffer );

                if ( DEBUG ) System.Console.WriteLine( "Wrote: " + msg );
            }

            /**
             * Close the Socket connection an set everything to null. Prepared for garbage collection
             */
            public void Close()
            {
                try
                {
                    if ( _socket != null ) _socket.Close();
                    if ( _socketOut != null ) _socketOut.Close();
                    if ( _socketIn != null ) _socketIn.Close();
                }
                catch ( IOException e ) { System.Console.WriteLine( e ); }

                _socketIn = null;
                _socketOut = null;
                _socket = null;
            }

        }

    }
}