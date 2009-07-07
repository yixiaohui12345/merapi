////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using merapi;
using merapi.messages;
using Merapi.Handlers;
using log4net;

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
            __logger.Debug( typeof( Bootstrap ) + ".Main() started" );
            System.Console.WriteLine( "Bootstrap" );

            PolicyServer.Start();
            Bridge b = Bridge.GetInstance();

            HelloWorldListener hwl = new HelloWorldListener();
        }


        /**
         *  @private 
         * 
         *  An instance of the log4net logger to handle the logging.
         */
        private static ILog __logger = LogManager.GetLogger( typeof( Bootstrap ) );
    }

    class HelloWorldListener : MessageHandler
    {
        public HelloWorldListener()
            : base( "hwType" )
        {
        }
            

        public override void HandleMessage( IMessage message )
        {
            System.Console.WriteLine( "Recv'd message: " + message.data );

            Message m = new Message();
            m.type = message.type;
            m.data = message.data;
            m.send();
        }
    }
}
