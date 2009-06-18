////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Merapi;

namespace merapi.messages
{
    /**
     *  The <code>Message</code> class implements IMessage, a 'message' sent from the Merapi 
     *  Flex bridge.
     * 
     *  @see merapi.Bridge;
     *  @see merapi.messages.IMessage;
     */
    public class Message : IMessage
    {
        //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */
        public Message()
        {
            this.uid = System.Guid.NewGuid().ToString();
        }

        /**
         *  Constructor.
         */
        public Message( String type )
            : this()
        {
            this.type = type;
        }

        /**
         *  Constructor.
         */
        public Message( String type, Object data )
            : this( type )
        {
            this.data = data;
        }


        //--------------------------------------------------------------------------
        //
        //  Properties
        //
        //--------------------------------------------------------------------------

        /**
         *  The type of the message.
         */
        public String type
        {
            get { return __type; }
            set { __type = value; }
        }

        /**
         *  A unique ID for this message.
         */
        public String uid
        {
            get { return __uid; }
            set { __uid = value; }
        }

        /**
         *  The data carried by this message.
         */
        public Object data
        {
            get { return __data; }
            set { __data = value; }
        }


        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Sends this message across the bridge.
         */
        public void send()
        {
            try
            {
                Bridge.GetInstance().SendMessage( this );
            }
            catch ( Exception e )
            {
                System.Console.Write( "Message.send(): " + e.ToString() );
            }
        }


        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------

        /**
         *  Used by the getters/setters.
         */
        private String __type = null;
        private String __uid = null;
        private Object __data = null;
                
    }
}