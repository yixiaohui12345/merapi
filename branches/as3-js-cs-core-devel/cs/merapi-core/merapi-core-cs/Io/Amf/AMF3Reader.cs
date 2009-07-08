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
using Merapi.Io.Reader;
using FluorineFx.IO;
using System.IO;
using merapi.messages;
using FluorineFx;
using log4net;
using merapi_core_cs;

namespace Merapi.Io.Amf
{

    /**
     *  The <code>AMF3Reader</code> class deserializes AMF 3 encoded binary data into an 
     *  <code>IMessage</code>. When a message has been received from the Flex bridge.
     * 
     *  @see merapi.io.reader.IReader;
     */
    public class AMF3Reader : IReader
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
        private static readonly ILog __logger = LogManager.GetLogger( typeof( AMF3Reader ) );


        //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */
        public AMF3Reader()
            : base()
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( LoggingConstants.METHOD_END );
        }


        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  @return Reads the binary data <code>bytes</code> and deserializes it into an 
         *  <code>IMessage</code>.
         */
        public List<IMessage> read( byte[] bytes )
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "bytes.length: " + bytes.Length );

            MemoryStream ms = new MemoryStream();

            foreach ( byte b in bytes )
            {
                ms.WriteByte( b );
            }

            ms.Position = 0;

            AMFReader amfReader = new AMFReader( ms );

            object decoded = null;
            try
            {
                decoded = amfReader.ReadAMF3Data();
            }
            catch ( Exception exception )
            {
                __logger.Error( exception );
                __logger.Debug( LoggingConstants.METHOD_END );
                return null;
            }

            IMessage message = null;
            List<IMessage> messages = new List<IMessage>();

            while ( decoded != null )
            {
                if ( decoded is IMessage )
                {
                    __logger.Debug( "Decoded message is an IMessage." );
                    message = decoded as IMessage;
                }
                else if ( decoded is ASObject )
                {
                    __logger.Debug( "Decoded message is not an IMessage... converting." );
                    
                    ASObject aso = decoded as ASObject;
                    
                    Message m = new Message();
                    m.type = aso[ "type" ] as string;
                    m.data = aso[ "data" ];
                    m.uid = aso[ "uid" ] as string;

                    message = m;
                }

                messages.Add( message );

                try
                {
                    if ( ms.Position < ms.Length && ms.Length > 0 )
                    {
                        decoded = amfReader.ReadAMF3Data();
                    }
                    else
                    {
                        decoded = null;
                    }
                }
                catch ( Exception exception )
                {
                    __logger.Error( exception );
                    decoded = null;
                }
            }

            __logger.Debug( "Decoded " + messages.Count + " messages." );
            __logger.Debug( LoggingConstants.METHOD_END );

            return messages;
        }

    }
}

