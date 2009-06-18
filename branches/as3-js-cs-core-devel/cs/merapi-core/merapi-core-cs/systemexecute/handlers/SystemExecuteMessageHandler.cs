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

using Merapi.Handlers;
using merapi.messages;
using merapi.systemexecute.messages;
using System;

namespace merapi.systemexecute.handlers
{

    public class SystemExecuteMessageHandler : MessageHandler
    {
        //--------------------------------------------------------------------------
        //
        //  Constructors
        //
        //--------------------------------------------------------------------------

        /**
         *  The default constructor
         */
        public SystemExecuteMessageHandler()
             : base( SystemExecuteMessage.SYSTEM_EXECUTE ) {}
       

        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Handles an <code>IMessage</code> dispatched from the Bridge.
         */
        public void handleMessage( IMessage message )
        {
            if ( message is SystemExecuteMessage )
            {
                SystemExecuteMessage sem = (SystemExecuteMessage)message;

                //  Use the args passed in the message to do a shell exec
                try
                {
                    string[] args = sem.args;
                    if ( args.Length > 1 )
                    {
                        System.Diagnostics.Process.Start( args[ 0 ], args[ 1 ] );
                    }
                    else if ( args.Length == 1 )
                    {
                        System.Diagnostics.Process.Start( args[ 0 ] );
                    }
                }
                catch ( Exception e )
                {
                    System.Console.WriteLine( e );
                }
            }
        }

    }
}
