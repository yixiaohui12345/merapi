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
		    __client 	    = client;
		    __reader      	= reader;
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
            System.Console.WriteLine( "BridgeListenerThread.Run()" );

            byte[] bytes = new byte[ __client.ReceiveBufferSize ];
            __client.Receive( bytes );

		    //  When the first byte returns is equal to -1, the socket has been disconnected
		    //  we let the thread end at the point.
            while ( bytes != null && bytes.Length > 0 && bytes[ 0 ] != -1 ) 
		    {
			    try 
			    {
				    List<IMessage> messages = __reader.read( bytes );

                    if ( messages != null && messages.Count > 0 )
                    {

                        foreach ( IMessage message in messages )
                        {
                            //  Broadcast the Message from the Bridge
                            Bridge.Instance.DispatchMessage( message );
                        }

                        bytes = new byte[ __client.ReceiveBufferSize ];
                        __client.Receive( bytes );
                    }
                    else
                    {
                        bytes = null;
                    }
			    }
    			
			    catch ( Exception exception )
			    {
                    System.Console.Write( "BridgeListenerThread.Run(): " + exception.ToString() );
                    bytes = null;
			    }
		    }
    		
		    System.Console.WriteLine( "BridgeListenerThread stopped running." );
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