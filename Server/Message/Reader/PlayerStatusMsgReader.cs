﻿using System.Linq;
using LunaCommon.Enums;
using LunaCommon.Message.Data;
using LunaCommon.Message.Data.PlayerStatus;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Server;
using LunaCommon.Message.Types;
using LunaServer.Client;
using LunaServer.Message.Reader.Base;
using LunaServer.Server;

namespace LunaServer.Message.Reader
{
    public class PlayerStatusMsgReader : ReaderBase
    {
        public override void HandleMessage(ClientStructure client, IMessageData messageData)
        {
            var message = messageData as PlayerStatusBaseMsgData;
            switch (message?.PlayerStatusMessageType)
            {
                case PlayerStatusMessageType.REQUEST:
                    SendOtherPlayerStatusToNewPlayer(client);
                    break;
                case PlayerStatusMessageType.SET:
                    var data = (PlayerStatusSetMsgData)message;
                    if (data.PlayerName == client.PlayerName)
                    {
                        client.PlayerStatus.VesselText = data.VesselText;
                        client.PlayerStatus.StatusText = data.StatusText;
                    }
                    MessageQueuer.RelayMessage<PlayerStatusSrvMsg>(client, data);
                    break;
            }
        }

        private static void SendOtherPlayerStatusToNewPlayer(ClientStructure client)
        {
            var otherClients = ClientRetriever.GetAuthenticatedClients().Where(c => c != client).ToArray();

            var otherPlayerStatusMsgData = new PlayerStatusReplyMsgData
            {
                PlayerName = otherClients.Select(c=> c.PlayerName).ToArray(),
                StatusText = otherClients.Select(c => c.PlayerStatus.StatusText).ToArray(),
                VesselText = otherClients.Select(c => c.PlayerStatus.VesselText).ToArray()
            };

            MessageQueuer.SendToClient<PlayerStatusSrvMsg>(client, otherPlayerStatusMsgData);
        }
    }
}