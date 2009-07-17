/////////////////////////////////////////////////////////////////////////////////////
//
//  This program is free software; you can redistribute it and/or modify 
//  it under the terms of the GNU Lesser General Public License as published 
//  by the Free Software Foundation; either version 3 of the License, or (at 
//  your option) any later version.
//
//  This program is distributed in the hope that it will be useful, but 
//  WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
//  or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License 
//  along with this program; if not, see <http://www.gnu.org/copyleft/lesser.html>.
//
/////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Merapi.Io.Amf;
using System.Net.Sockets;
using Merapi.Io.Writer;
using Merapi.Io.Reader;
using System.Net;
using merapi.messages;
using System.IO;
using FluorineFx.IO;
using Merapi.Handlers;
using log4net.Repository.Hierarchy;
using log4net;
using merapi.systemexecute.handlers;
using merapi_core_cs;

namespace Merapi
{
    /**
     *  The <code>Bridge</code> class is a singleton gateway to the Flex Merapi bridge. 
     *  IMessages are exchanged between this C# bridge object and the Flex bridge object.
     * 
     *  @see merapi.events.MerapiEvent
     *  @see Merapi.Messages.IMessage;
     */
    public class Bridge
    {
	    //--------------------------------------------------------------------------
	    //
	    //  Class Constants
	    //
	    //--------------------------------------------------------------------------
    	
	    public const String CONFIG_PATH 		= "./config/merapi-native-config.xml";
	    public const String READER_STRING 		= "reader";
	    public const String WRITER_STRING 		= "writer";
	    public const String PORT_STRING	 		= "port";
        

	    //--------------------------------------------------------------------------
	    //
	    //  Class Properties
	    //
	    //--------------------------------------------------------------------------

	    /**
	     * The port used to connect Merapi
	     */
	    public static int PORT                 = 12345;
    	

   	    //--------------------------------------------------------------------------
	    //
	    //  Class Methods
	    //
	    //--------------------------------------------------------------------------
    	
	    /**
	     *  The single instance of the Merapi <code>Bridge</code>
	     */	
        public static Bridge GetInstance()
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

    	    if ( Instance == null )
    	    {
			    Instance = new Bridge();
			    Instance.RegisterHandlers();
    	    }

            __logger.Debug( LoggingConstants.METHOD_END );

            return Instance;
        }

        /**
         *  Opens the Merapi server socket
         */
        public static void Open() 
        {
    	    if ( Thread == null )
    	    {
    		    Bridge.Thread 			= new Thread( Bridge.GetInstance().Run );
    		    Bridge.Thread.Start();
        		
    		    Bridge.IsRunning		= true;
    	    }
        }	
        
        /**
         *  Closes the Merapi server socket
         */    
        public static void Close()
        {
    	    Bridge.IsRunning			= false;
    	    Bridge.Thread 				= null;
        	
    	    try
    	    {
                Instance.__server.Close();
    	    }
    	    catch ( IOException exception )
    	    {
    		    __logger.Error( exception );
    	    }
        }
        
	    //--------------------------------------------------------------------------
	    //
	    //  Class Variables
	    //
	    //--------------------------------------------------------------------------
    	
        private static Bridge 	Instance	= null;
        private static Thread	Thread		= null;
        private static bool	    IsRunning 	= true;
        private static ILog     __logger    = LogManager.GetLogger( typeof( Bridge ) );
        

	    //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         * Constructor.
         */
        private Bridge() 
        {
    	    ReadConfig();
            __handlers = new Dictionary<String, List<IMessageHandler>>();
        }


        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------
    	
        /**
         *  The run method of the thread
         */
	    public void Run()
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

		    try 
		    {
			    __server = new Socket( AddressFamily.InterNetwork, SocketType.Stream, 
                                       ProtocolType.Tcp );
                __server.Bind( new IPEndPoint( IPAddress.Any, PORT ) );
                __server.Listen( 4 );

			    __logger.Info( "Merapi started on port: " + PORT );
    			
			    while( true )
			    {
				    //  Get the first client that connects
				    Socket temp = __server.Accept();
				    __client    = temp;

                    __logger.Info( "Acceptted a connection: " + __client );

				    //  Instantiate a listener thread that will listen and read the input stream
				    //  of the first client that connects and start the thread
				    __clientListenerThread = new BridgeListenerThread( __client, __reader );
                    new Thread( __clientListenerThread.Run ).Start();
			    }
		    }
		    catch ( Exception e )
		    {
			    __logger.Error( e );
		    }

            __logger.Debug( LoggingConstants.METHOD_END );
	    }

	    /**
	     *  Dispatches an <code>IMessage</code> to registered listeners.
	     */
	    public void DispatchMessage( IMessage message )
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

            List<IMessageHandler> list = null;

		    //  Get the list of handlers registered for the event type
            if ( __handlers.ContainsKey(message.type) == false )
            {
                __logger.Debug( "No handlers registered, exiting method." );
                __logger.Debug( LoggingConstants.METHOD_END );
                return;
            }

            list = __handlers[ message.type ];
    		
		    //  If the list is not null and not empty notify the registered event handlers 
		    if ( list != null && list.Count > 0 )
		    {
			    foreach ( IMessageHandler handler in list )
			    {
				    handler.HandleMessage( message );
			    }
		    }

            __logger.Debug( LoggingConstants.METHOD_END );
	    }
    	
	    /**
	     *  Registers an <code>IMessageHandler</code> to receive notifications when a certain 
	     *  message type is dispatccmhed.
	     */		
	    public void RegisterMessageHandler( String type, IMessageHandler handler )
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "type: \"" + type + "\"" );
            __logger.Debug( "handler: " + handler );

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

            __logger.Debug( LoggingConstants.METHOD_END );
	    }	

        /**
         *  Sends a <code>message</code> to the Flex side of the Merapi bridge.
         */		
	    public void SendMessage( IMessage message )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

		    if ( __client == null || __client.Connected == false ) return;
    		
		    byte[] bytes = __writer.write( message );

		    //  Send the length of the message first
            byte[] lenBytes = new byte[ 1 ];
            lenBytes[ 0 ] = (byte)bytes.Length;
            __client.Send( lenBytes );

            __logger.Debug( "Sending " + bytes.Length + " bytes." );

		    //  Send the message
            __client.Send( bytes );

            __logger.Debug( LoggingConstants.METHOD_END );
	    }	
    	
	    /**
	     *  Unregisters a given handler.
	     */		
	    public void UnRegisterMessageHandler( String type, IMessageHandler handler )
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

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

            __logger.Debug( LoggingConstants.METHOD_END );
	    }

        /**
	     *  @protected
	     *  
	     *  Instantiates the framework <code>IMessageHandlers</code>.
	     */
	    protected void RegisterHandlers()
	    {
		    //  Registers SystemExecuteHandler as the IMessageHandler of the 
		    //  SystemExecuteMessage.SYSTEM_EXECUTE message type.
		    new SystemExecuteMessageHandler();
	    }
    	
        /**
         *  @protected
         *  
         *  Reads a config file and applies the settings
         */
	    protected void ReadConfig() 
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

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

            __logger.Debug( LoggingConstants.METHOD_END );
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
	    private IWriter 				__writer 				= new AMF3Writer();
    	
	    /**
	     *  @private
         *  
	     *  The <code>IReader</code> used to deserialize data that comes across the bridge from Flex.
	     */
	    private IReader 				__reader 				= new AMF3Reader();

	    /**
	     *  @private
         *  
	     *  A Dictionary of registered event handlers.
	     */
	    private Dictionary<String, List<IMessageHandler>> __handlers = null;
    }	
}
