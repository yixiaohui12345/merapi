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
using merapi.messages;
using log4net;
using merapi_core_cs;

namespace Merapi.Handlers
{
    public class MessageHandler : IMessageHandler
    {
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
        private static readonly ILog __logger = LogManager.GetLogger( typeof( MessageHandler ) );


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
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( LoggingConstants.METHOD_END );
        }

        /**
         *  Automatically registers the handler for message type <code>type</code>.
         */
        public MessageHandler( String type )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "type: \"" + type + "\"" );

            AddMessageType( type );

            __logger.Debug( LoggingConstants.METHOD_END );
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
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "message: " + message );
            __logger.Debug( LoggingConstants.METHOD_END );
        }

        /**
         *  Removes all of the type registrations, cleaning up any
         *  external references held by the Bridge to this Handler
         */
        public virtual void UnregisterAllTypes()
        {
            String[] types = Types.ToArray();

            foreach ( String type in types )
            {
                RemoveMessageType( type );
            }
        }

        /**
         *  Adds another message type to be listend for by this instance of MessageHandler.
         */
        public void AddMessageType( String type )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "type: \"" + type + "\"" );

            Types.Add( type );
            Bridge.GetInstance().RegisterMessageHandler( type, this );

            __logger.Debug( LoggingConstants.METHOD_END );
        }

        /**
         *  Removes the handling of message type <code>type</code>.
         */
        public void RemoveMessageType( String type )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "type: \"" + type + "\"" );

            Types.Remove( type );
            Bridge.GetInstance().UnRegisterMessageHandler( type, this );

            __logger.Debug( LoggingConstants.METHOD_END );
        }

        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------

        /**
         *  @protected
         *  
         *  The types this handler is registered to listen for
         */
        protected List<string> Types = new List<string>();
    }
}

