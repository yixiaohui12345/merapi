////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Merapi.Io.Reader;
using Merapi.Messages;
using FluorineFx.IO;
using System.IO;
using merapi.messages;
using FluorineFx;

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
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */
        public AMF3Reader()
            : base()
        {
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
            MemoryStream ms = new MemoryStream();

            foreach ( byte b in bytes )
            {
                if ( b == 0 ) break;
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
                System.Console.WriteLine( "AMF3Reader.read() [1]: " + exception );
                return null;
            }

            IMessage message = null;
            List<IMessage> messages = new List<IMessage>();

            while ( decoded != null )
            {
                if ( decoded is IMessage )
                {
                    message = decoded as IMessage;
                }
                else if ( decoded is ASObject )
                {
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
                    System.Console.WriteLine( "AMF3Reader.read() [2]: " + exception );
                    decoded = null;
                }
            }

            return messages;
        }

    }
}

