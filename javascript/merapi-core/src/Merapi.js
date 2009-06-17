////////////////////////////////////////////////////////////////////////////////////
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
////////////////////////////////////////////////////////////////////////////////////

/**
 *  The <code>Message</code> class is a 'message' sent or received by the Merapi 
 *  UI layer.
 * 
 *  @see merapi.handlers.MessageHandler;
 */
//public class merapi.messages.Message
//{
	
	//--------------------------------------------------------------------------
	//
	//  Constructor
	//
	//--------------------------------------------------------------------------
	
	/**
	 *  Constructor.
	 */		
	function Message( type /*String*/, data /*Object*/ ) /* -> void */
	{
		//  Properties
		this.type   = type;
		this.data   = data;
	}
	
	
	//--------------------------------------------------------------------------
	//
	//  Properties
	//
	//--------------------------------------------------------------------------
	
	//----------------------------------
	//  uid
	//----------------------------------
	
	/**
	 *  A unique ID for the message.
	 */		
	/*public*/ var uid; /* String */
	
	//----------------------------------
	//  type
	//----------------------------------
	
	/**
	 *  The type of the message.
	 */		
	/*public*/ var type; /* String */
	
	//----------------------------------
	//  data
	//----------------------------------
	
	/**
	 *  The data carried by this message.
	 */		
	/*public*/ var data; /* Object */
	
	
	//--------------------------------------------------------------------------
	//
	//  Methods
	//
	//--------------------------------------------------------------------------
	
	/**
	 *  Sends the event across the Merapi bridge.
	 */
	/*public function send() -> void */
	Message.prototype.send = function()
	{
		bridge.sendMessage( this );
	}

//} end class

/**
 *  The <code>MessageHandler</code> class defines the methods for receiving a 
 *  <code>Message</code> from the Bridge.
 * 
 *  @see merapi.messages.Message
 */
//public class merapi.handlers.MessageHandler 
//{                                                                                                           
    //--------------------------------------------------------------------------
    //
    //  Constructors
    //
    //--------------------------------------------------------------------------
	
	/**
	 *  Automatically registers the handler for message type <code>type</code>.
	 *  
	 *  @param type The type of message to handle.
	 */
	/*public*/ function MessageHandler() /* -> void */
	{
	}
	
	
	//--------------------------------------------------------------------------
	//
	//  Methods
	//
	//--------------------------------------------------------------------------
	
	/**
	 *  Handles a message dispatched from the Bridge. This method should
	 *  be overridden by MessageHandler sub class definitions / instances.
	 */
	/*public function handleMessage( message : Message ) -> void */
	MessageHandler.prototype.handleMessage = function( message )
	{
	}

	/**
	 *  Adds another message type to be listend for by this instance of MessageHandler.
	 */
	/*public function addMessageType( type : String ) -> void */
	MessageHandler.prototype.addMessageType = function( type )
	{
		//  Registers the type w/ the bridge. If the type has
		//  been registered, it is ignored on the flash side
		bridge.registerMessageHandler( type );
		
		//  If this is the first handler registerd for the type,
		//  create an Array to store the handlers in the map
		if ( handlersMap[ type ] == null )
		{
			handlersMap[ type ] = new Array();
		}

		// Map the message type -> [ ..., handler, ... ]
		handlersMap[ type ].push( this );
	}
	
	/**
	 *  Removes the handling of message type <code>type</code>.
	 */
	/*public function removeMessageType( type : String ) -> void */
	MessageHandler.prototype.removeMessageType = function( type )
	{
		//  true if there is another listener still listening to
		//  this message type
		var stillListening = false;
		
		//  The list of handlers registered for Message type <type>
		var list = handlersMap[ type ];
		
		if ( list != null )
		{
			//  Find the handler in list that matches this 
			//  instance and remove it
			for ( i=0; i<list.length; i++ )
			{
				//  if list[ i ] matches this listener, remove it
				if ( list[ i ] == this )
				{
					list[ i ] = null;
				}
				//  if list[ i ] is not null, then there are still
				//  handlers listening to this type
				else if ( list[ i ] != null )
				{
					stillListening = true;
				}
			}
		}
		
		//  if there are still handlers listening to the type,
		//  don't unRegister with Flash.. otherwise if all handlers
		//  for this type are removed, then we can unregister the 
		//  type from the bridge
		if ( stillListening == false )
		{
			bridge.unRegisterMessageHandler( type );
		}
	}
	
//} end class  


//--------------------------------------------------------------------------
//
//  Static methods
//
//--------------------------------------------------------------------------

/**
 *  Connects the Merapi client/ui Bridge to the native Bridge.
 */
/*public static*/ function connectMerapi( port /* int */, host /* String */ ) /* -> void */
{
	bridge.connectMerapi( port, host );
}

/**
 *  Disconnects the Merapi client/ui Bridge to the native Bridge.
 */
/*public static*/ function disconnectMerapi() /* -> void */
{
	bridge.disconnectMerapi();
}

/**
 *  Performs a systemExecute via a proxy call to the Flash swf.
 */
/*public static*/ function systemExecute( args /* Array */ ) /* -> void */
{
	bridge.systemExecute( args );
}

/**
 *  The external callback for any messages that have a type that
 *  was registered. The flash side of Merapi calls handleMerapiMessage()
 *  for any type that has been registered... The list of handlers
 *  that have registered is looked up in handlersMap and handleMessage()
 *  is called on each registered handler.
 */
/*public static*/ function handleMerapiMessage( message )
{
	// The list of handlers registered for messsage.type
	var list = handlersMap[ message.type ];
	
	//  If the list is null there's nothing to do
	if ( list != null )
	{
		// Call handleMessage() on each handler registered
		// for message.type
		for( i=0; i<list.length; i++ )
		{
			if ( list[ i ] != null )
			{
				list[ i ].handleMessage( message );
			}
		}
	}
}


//--------------------------------------------------------------------------
//
//  Static properties
//
//--------------------------------------------------------------------------

/**
 *  Maps registered message types to a list of the 
 *  associated handlers registered for the type.
 * 
 *  ( type -> [ handler, handler, handler, ... ] )
 */
/*public static*/ var handlersMap = new Object();

