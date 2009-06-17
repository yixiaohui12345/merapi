////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using merapi.messages;

namespace Merapi.Handlers
{
    public class MessageHandler : IMessageHandler
    {

        //--------------------------------------------------------------------------
        //
        //  Constructors
        //
        //--------------------------------------------------------------------------

        /**
         *   The default constructor
         */
        public MessageHandler()
        {
        }

        /**
         *  Automatically registers the handler for message type <code>type</code>.
         */
        public MessageHandler( String type )
        {
            AddMessageType( type );
        }


        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Handles an <code>IMessage</code> dispatched from the Bridge.
         */
        public virtual void HandleMessage( IMessage message )
        {
        }

        /**
         *  Adds another message type to be listend for by this instance of MessageHandler.
         */
        public void AddMessageType( String type )
        {
            Bridge.Instance.RegisterMessageHandler( type, this );
        }

        /**
         *  Removes the handling of message type <code>type</code>.
         */
        public void RemoveMessageType( String type )
        {
            Bridge.Instance.UnRegisterMessageHandler( type, this );
        }
    }
}

