////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace merapi.messages
{
    /**
     *  The <code>IMessage</code> interface describes a 'message' sent from the Merapi Flex bridge.
     * 
     *  @see Merapi.BridgeInstance;
     *  @see Merapi.Bridge;
     *  @see Merapi.messages.Message;
     */
    public interface IMessage
    {
        //--------------------------------------------------------------------------
        //
        //  Properties
        //
        //--------------------------------------------------------------------------

        //----------------------------------
        //  type
        //----------------------------------

        /**
         *  The message type.
         */
        String type
        {
            get;
            set;
        }

        //----------------------------------
        //  data
        //----------------------------------

        /**
         *  The data of the message.
         */
        Object data
        {
            get;
            set;
        }

    }
}