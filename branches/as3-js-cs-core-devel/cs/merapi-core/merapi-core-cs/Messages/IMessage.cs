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