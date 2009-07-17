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
using Merapi;
using log4net;
using merapi_core_cs;

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
        //  Static Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Convience method to check if a message matches a type.
         */
        public static bool IsType( IMessage message, string type )
        {
            return type.Equals( message.type );
        }


        //--------------------------------------------------------------------------
        //
        //  Static variables
        //
        //--------------------------------------------------------------------------

        /**
         *  @private 
         * 
         *  An instance of the log4net logger to handle the logging.
         */
        private static readonly ILog __logger = LogManager.GetLogger( typeof( Message ) );

        
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
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

            this.uid = System.Guid.NewGuid().ToString();

            __logger.Debug( "uid: " + uid );
            __logger.Debug( LoggingConstants.METHOD_END );
        }

        /**
         *  Constructor.
         */
        public Message( String type )
            : this()
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "type: \"" + type + "\"" );

            this.type = type;

            __logger.Debug( LoggingConstants.METHOD_END );
        }

        /**
         *  Constructor.
         */
        public Message( String type, Object data )
            : this( type )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "data: " + data );

            this.data = data;

            __logger.Debug( LoggingConstants.METHOD_END );
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
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

            try
            {
                Bridge.GetInstance().SendMessage( this );
            }
            catch ( Exception e )
            {
                __logger.Error( e );
            }

            __logger.Debug( LoggingConstants.METHOD_END );
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