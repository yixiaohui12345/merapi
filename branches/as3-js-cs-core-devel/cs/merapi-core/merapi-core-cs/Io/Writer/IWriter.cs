////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using merapi.messages;

namespace Merapi.Io.Writer
{
    /**
     *  The <code>IWriter</code> interface describes a class for reading binary data from the Merapi
     *  bridge.
     * 
     *  @see Merapi.BridgeInstance;
     *  @see Merapi.Bridge;
     *  @see Merapi.Io.AMF3Writer;
     */
    public interface IWriter
    {
        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Serializes the <code>message</code> and returns a the data as an Array of bytes.
         */
        byte[] write(IMessage message);
    }
}
