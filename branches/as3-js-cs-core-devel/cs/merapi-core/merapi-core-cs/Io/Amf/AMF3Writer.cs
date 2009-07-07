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
using Merapi.Io.Writer;
using merapi.messages;
using FluorineFx.IO;
using System.IO;
using log4net;
using merapi_core_cs;

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
        //  Constructor
        //
        //--------------------------------------------------------------------------

        /**
         *  Constructor.
         */	
        public AMF3Writer() : base()
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
         *  Serializes <code>message</code>.
         */		
	    public byte[] write( IMessage message ) 
        {
            __logger.Debug( LoggingConstants.METHOD_BEGIN );
            __logger.Debug( "message: " + message );

            MemoryStream ms = new MemoryStream();

            ms.WriteByte( (byte)10 );

            AMFWriter writer = new AMFWriter( ms );
            writer.WriteAMF3Object( message );

            byte[] bytes = ms.ToArray();

            ms = null;
            writer = null;

            __logger.Debug( "Encoded " + bytes.Length + " bytes." );
            __logger.Debug( LoggingConstants.METHOD_END );

            return bytes;
        }
    }
} 