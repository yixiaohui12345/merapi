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
using merapi;
using merapi.messages; 
using Merapi.Handlers;
using log4net;
using merapi_core_cs;

namespace Merapi
{
    class Bootstrap
    {
        //--------------------------------------------------------------------------
        //
        //  Class Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  The main method that starts the bridge.
         */	
        static void Main(string[] args)
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );

            PolicyServer.Start();
            Bridge b = Bridge.GetInstance();

            HelloWorldListener hwl = new HelloWorldListener();

            __logger.Debug( LoggingConstants.METHOD_END );
        }


        //--------------------------------------------------------------------------
        //
        //  Class Variables
        //
        //--------------------------------------------------------------------------

        /**
         *  @private 
         * 
         *  An instance of the log4net logger to handle the logging.
         */
        private static ILog __logger = LogManager.GetLogger( typeof( Bootstrap ) );
    }

    class HelloWorldListener : MessageHandler
    {
        public const String HELLO_WORLD = "hwType";

        //--------------------------------------------------------------------------
        //
        //  Class Variables
        //
        //--------------------------------------------------------------------------

        /**
         *  @private 
         * 
         *  An instance of the log4net logger to handle the logging.
         */
        private static ILog __logger = LogManager.GetLogger( typeof( HelloWorldListener ) );


        public HelloWorldListener()
            : base( HELLO_WORLD )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( LoggingConstants.METHOD_END );
        }
            

        public override void HandleMessage( IMessage message )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "Recv'd message: " + message.data );

            Message m = new Message();
            m.type = message.type;
            m.data = message.data;
            m.send();

            __logger.Debug( LoggingConstants.METHOD_END );
        }

    }
}
