////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Merapi.Messages;
using System.Threading;
using Merapi.Io.Amf;
using System.Net.Sockets;
using Merapi.Io.Writer;
using Merapi.Io.Reader;
using System.Net;
using merapi.messages;
using System.IO;
using FluorineFx.IO;

namespace Merapi
{
    /**
     *  The <code>Bridge</code> class is a singleton gateway to the Flex Merapi bridge. 
     *  IMessages are exchanged between this C# bridge object and the Flex bridge object.
     * 
     *  @see merapi.events.MerapiEvent
     *  @see Merapi.Messages.IMessage;
     */
    public class Bridge : IMessageHandler
    {

	    //--------------------------------------------------------------------------
	    //
	    //  Constants
	    //
	    //--------------------------------------------------------------------------
    	
	    /**
	     *  The port used to connect Merapi
	     */	
	    public static int PORT = 12345; 
    	

        //--------------------------------------------------------------------------
	    //
	    //  Class Methods
	    //
	    //--------------------------------------------------------------------------
    	
	    /**
	     *  The single instance of the Merapi <code>Bridge</code>
	     */	
        public static Bridge Instance
        {
            get
            {
                System.Console.WriteLine( "Bridge.Instance" );
    	        if ( __instance == null )
    	        {
			        __instance = new Bridge();

                    new Thread( __instance.Run ).Start();
    	        }
        	    return __instance;
            }
        }
        private static Bridge __instance;
        
        
	    //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */
	    private Bridge()
	    {
		    ReadConfig();
    		
		    __reader 		= new AMF3Reader(); // Using AMF3Reader by default, @todo make this configurable
		    __writer 		= new AMF3Writer(); // Using AMF3Writer by default, @todo make this configurable
		    __handlers 		= new Dictionary<String, List<IMessageHandler>>();
    		
		    //  Handle system execute message in the bridge
		    //RegisterMessageHandler( Message.SYSTEM_EXECUTE, this );
	    }


        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  @protected
         *  
         *  Reads a config file and applies the settings
         */
	    protected void ReadConfig() 
	    {
            System.Console.WriteLine( "Bridge.ReadConfigs()" );

            /* TODO : Port code
		    try 
		    {
			    FileInputStream fis = new FileInputStream( "./config/merapi-config.xml" );
			    Properties config = new Properties();
			    config.loadFromXML( fis );
			    fis.close();

			    PORT = Integer.parseInt( config.getProperty( "port" ) );
		    } 
		    catch ( Exception e ) { }
    		*/
	    }
    	
        /**
         *  The run method of the thread
         */
	    public void Run()
	    {
            System.Console.WriteLine( "Bridge.Run()" );

		    try 
		    {
			    __server = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                __server.Bind( new IPEndPoint( IPAddress.Any, PORT ) );
                __server.Listen( 4 );

			    System.Console.WriteLine( "Merapi started on port: " + PORT );
    			
			    while( true )
			    {
				    //  Get the first client that connects
				    Socket temp = __server.Accept();
				    __client    = temp;

                    System.Console.WriteLine( "Acceptted a connection: " + __client );

				    //  Instantiate a listener thread that will listen and read the input stream
				    //  of the first client that connects and start the thread
				    __clientListenerThread = new BridgeListenerThread( __client, __reader );
                    new Thread( __clientListenerThread.Run ).Start();
			    }
		    }
		    catch ( Exception e )
		    {
			    System.Console.WriteLine( "Bridge.Run(): " + e );
		    }
	    }

	    /**
	     *  Dispatches an <code>IMessage</code> to registered listeners.
	     */
	    public void DispatchMessage( IMessage message )
	    {
            System.Console.WriteLine( "Bridge.DispatchMessage()" );

            List<IMessageHandler> list = null;

		    //  Get the list of handlers registered for the event type
            if ( __handlers.ContainsKey( message.type ) == false ) return;
            list = __handlers[ message.type ];
    		
		    //  If the list is not null and not empty notify the registered event handlers 
		    if ( list != null && list.Count > 0 )
		    {
			    foreach ( IMessageHandler handler in list )
			    {
				    handler.HandleMessage( message );
			    }
		    }
	    }
    	
	    /**
	     *  Registers an <code>IMessageHandler</code> to receive notifications when a certain 
	     *  message type is dispatccmhed.
	     */		
	    public void RegisterMessageHandler( String type, IMessageHandler handler )
	    {
            System.Console.WriteLine( "Bridge.RegisterMessageHandler()" );

		    //  Get the list of handlers registered for the event type
            List<IMessageHandler> list = null;
            if ( __handlers.ContainsKey( type ) )
            {
                list = __handlers[ type ];
            }
            else
            {
    		    //  If the list is null, create a new list to add 'handler' to
			    list = new List<IMessageHandler>();
			    __handlers.Add( type, list );
		    }
    		
		    //  Add the handler to the list
		    list.Add( handler );
	    }	

        /**
         *  Sends a <code>message</code> to the Flex side of the Merapi bridge.
         */		
	    public void SendMessage( IMessage message )
        {
            System.Console.WriteLine( "Bridge.SendMessage()" );

		    if ( __client == null || __client.Connected == false ) return;
    		
		    byte[] bytes = __writer.write( message );

		    //  Send the length of the message first
            byte[] lenBytes = new byte[ 1 ];
            lenBytes[ 0 ] = (byte)bytes.Length;
            __client.Send( lenBytes );

            System.Console.WriteLine( "Sending " + bytes.Length + " bytes." );

		    //  Send the message
            __client.Send( bytes );
	    }	
    	
	    /**
	     *  Unregisters a given handler.
	     */		
	    public void UnRegisterMessageHandler( String type, IMessageHandler handler )
	    {
            System.Console.WriteLine( "Bridge.UnRegisterMessageHandler()" );

		    //  Get the list of handlers registered for the event type
		    List<IMessageHandler> list = __handlers[ type ];
    		
		    //  If the list is not null and not empty, look for handler in the list and remove it
		    //  if a match is found
		    if ( list != null && list.Count > 0 )
		    {
			    foreach ( IMessageHandler activeHandler in list )
			    {
				    if ( activeHandler == handler )
				    {
					    list.Remove( handler );
				    }
			    }
		    }
	    }
    	
	    /**
	     *  Handles the <code>Merapi.Messages.Message.SYSTEM_EXECUTE</code> message type.
	     */		
	    public void HandleMessage( IMessage message )
	    {
            /* TODO : Port code
		    //  All arrays are sent from Flex as type Object
		    //  Need to convert to an Array of Strings for exec()
		    Object[] data = (Object[])( (Message)message ).getData();
		    String[] args = new String[ data.length ];
		    for ( int i = 0; i < data.length; i++ )
		    {
			    args[ i ] = (String)data[ i ];
		    }
    		
		    //  Use the args passed in the message to do a shell exec
		    try 
		    {
			    Runtime.getRuntime().exec( args );
		    }
		    catch ( Exception e )
		    {
			    System.Console.WriteLine( Bridge.class );
			    e.printStackTrace();
		    }
            */
	    }
    	
    	
	    //--------------------------------------------------------------------------
	    //
	    //  Variables
	    //
	    //--------------------------------------------------------------------------
    	
	    /**
	     *  @private
         *  
	     *  The socket that open connections to the Flex Merapi bridge.
	     */
	    private Socket 		        	__server 				= null;
    	
	    /**
	     *  @private
         *  
	     *  The socket that connected to the Flex Merapi bridge.
	     */
	    private Socket 					__client 				= null;
    		
	    /**
	     *  @private
         *  
	     *  The thread that listens for messages from the Flex client socket.
	     */
	    private BridgeListenerThread 	__clientListenerThread 	= null;
    		
	    /**
	     *  @private
         *  
	     *  The <code>IWriter</code> used to serialize data sent across the bridge to Flex.
	     */
	    private IWriter 				__writer 				= null;
    	
	    /**
	     *  @private
         *  
	     *  The <code>IReader</code> used to deserialize data that comes across the bridge from Flex.
	     */
	    private IReader 				__reader 				= null;
    	
	    /**
	     *  @private
         *  
	     *  A Dictionary of registered event handlers.
	     */
	    private Dictionary<String, List<IMessageHandler>> __handlers = null;
    }	
}