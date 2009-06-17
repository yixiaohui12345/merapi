////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Merapi.Io.Writer;
using Merapi.Messages;
using merapi.messages;
using FluorineFx.IO;
using System.IO;

namespace Merapi.Io.Amf
{

    /**
     *  The <code>AMF3Writer</code> class serializes a C# object into AMF 3 encoded binary data.
     * 
     *  @see Merapi.Io.Amf.IReader;
     */
    public class AMF3Writer : IWriter
    {
        //--------------------------------------------------------------------------
        //
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */	
        public AMF3Writer() : base()
        {
        }

	    //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Serializes <code>message</code>.
         */		
	    public byte[] write( IMessage message ) 
        {
            MemoryStream ms = new MemoryStream();

            ms.WriteByte( (byte)10 );

            AMFWriter writer = new AMFWriter( ms );
            writer.WriteAMF3Object( message );

            byte[] bytes = ms.ToArray();

            ms = null;
            writer = null;

            return bytes;
        }
    }
} 