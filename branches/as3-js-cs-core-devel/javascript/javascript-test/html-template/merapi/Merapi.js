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
		
		//  Methods
		this.send 	= send;
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
	/*public*/ function send() /* -> void */
	{
		flex.sendMessage( this );
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
	function MessageHandler( type /* String */ ) /* -> void */
	{
		//  Methods
		this.addMessageType = addMessageType;
		
		if ( type != null )
		{
			addMessageType( type );
		}
	}
	
	
	//--------------------------------------------------------------------------
	//
	//  Methods
	//
	//--------------------------------------------------------------------------
	
	/**
	 *  Handles a message dispatched from the Bridge. This method should
	 *  be overridden by MessageHandler sub class definitions.
	 */
	/*public*/ function handleMessage( message /* Message */ ) /* -> void */ 
	{
	}

	/**
	 *  Adds another message type to be listend for by this instance of MessageHandler.
	 */
	/*public*/ function addMessageType( type /* String */ ) /* -> void */
	{
		//Bridge.getInstance().registerMessageHandler( type, this );
	}
	
	/**
	 *  Removes the handling of message type <code>type</code>.
	 */
	/*public*/ function removeMessageType( type /* String */ ) /* -> void */
	{
		//Bridge.getInstance().unRegisterMessageHandler( type, this );
	}
	
//} end class  

