////////////////////////////////////////////////////////////////////////////////
//
//  $license
//
////////////////////////////////////////////////////////////////////////////////

using merapi.messages;

namespace Merapi.Handlers
{
    /**
     *  The <code>IEventHandler</code> interface defines the methods for receiving a 
     *  <code>MerapiEvent</code> from the Bridge.
     * 
     *  @see Merapi.Bridge
     */
    public interface IMessageHandler
    {
        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------

        /**
         *  Handles an <code>IMessage</code> dispatched from the Bridge.
         */
        void HandleMessage( IMessage message );
    }
}
