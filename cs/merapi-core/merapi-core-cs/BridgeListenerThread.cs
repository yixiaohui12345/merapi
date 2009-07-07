////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Merapi.Io.Reader;
using System.Net.Sockets;
using merapi.messages;
using System.Threading;
using log4net;
using merapi_core_cs;

namespace Merapi
{
    /**
     *  The <code>BridgeListenerThread</code> listens for messages from a client socket and notifies
     *  the bridge when a message is received. 
     * 
     *  @see merapi.Bridge;
     *  @see merapi.messages.IMessage;
     */
	public class BridgeListenerThread
	{
        private static readonly ILog __logger = LogManager.GetLogger( typeof( BridgeListenerThread ) );
 
	    //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */
	    public BridgeListenerThread( Socket client, IReader reader ) : base()
	    {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

		    __client 	    = client;
		    __reader      	= reader;

            __logger.Debug( LoggingConstants.METHOD_END );
        }

    	
	    //--------------------------------------------------------------------------
	    //
	    //  Methods
	    //
	    //--------------------------------------------------------------------------
    	
	    /**
	     *  The run method of the thread that reads data from the input stream. When a messaged is
	     *  deserialized it is broadcasted from the bridge via <code>Bridge.dispatchEvent</code>. 
	     *  Registered listeners are notified of the event.
	     */
	    public void Run() 
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

            byte[] bytes = null;
            bool firstRead = true;


		    //  When the first byte returns is equal to -1, the socket has been disconnected
		    //  we let the thread end at this point.
            while ( firstRead == true || ( bytes != null && bytes.Length > 0 && 
                    bytes[ 0 ] != Int32.Parse( "-1" ) ) )
		    {
                firstRead = false;

                //  not ideal.. fix this
                while ( __client.Available == 0 )
                {
                    Thread.Sleep( 500 );
                }

                __logger.Debug( __client.Available + " bytes recv'd." );
                
                try 
			    {
                    bytes = new byte[ __client.Available ];
                    __client.Receive( bytes );

				    List<IMessage> messages = __reader.read( bytes );

                    if ( bytes != null && bytes.Length > 0 )
                    {
                        __logger.Debug( "First byte: " + bytes[ 0 ] );
                    }

                    if ( messages != null && messages.Count > 0 )
                    {
                        foreach ( IMessage message in messages )
                        {
                            //  Broadcast the Message from the Bridge
                            Bridge.GetInstance().DispatchMessage( message );
                        }
                    }
                    else
                    {
                        bytes = null;
                    }
			    }
    			
			    catch ( Exception exception )
			    {
                    __logger.Error( exception.ToString() );
                    bytes = null;
			    }
		    }

            __logger.Info( "BridgeListenerThread stopped running." );
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
	     *  The client connected from the UI side of the bridge.
	     */
	    private Socket      __client = null;
    	
	    /**
	     *  @private
         *  
	     *  The reader used to deserialize Objects sent across the bridge.
	     */
	    private IReader 	__reader = null;	
    	
    }
}