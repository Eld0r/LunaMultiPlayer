﻿using LunaCommon.Enums;
using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Handshake
{
    public class HandshakeRequestMsgData : HandshakeBaseMsgData
    {
        public override HandshakeMessageType HandshakeMessageType => HandshakeMessageType.REQUEST;

        //Nothing here
    }
}