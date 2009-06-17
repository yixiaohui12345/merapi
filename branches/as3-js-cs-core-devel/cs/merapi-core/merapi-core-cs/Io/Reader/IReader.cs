////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using merapi.messages;

namespace Merapi.Io.Reader
{
    /**
     *  The <code>IWriter</code> interface describes a class for reading binary data from the Merapi
     *  bridge.
     * 
     *  @see Merapi.BridgeInstance;
     *  @see Merapi.Bridge;
     *  @see Merapi.Io.AMF3Writer;
     */
    public interface IReader
    {
        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Deserializes the binary data <code>bytes</code> and returns a the data as an Object
         */
        List<IMessage> read(byte[] bytes);
    }
}
