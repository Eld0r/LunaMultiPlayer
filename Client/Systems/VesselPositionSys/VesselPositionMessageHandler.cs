﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaCommon.Message.Data.Vessel;
using LunaCommon.Message.Interface;

namespace LunaClient.Systems.VesselPositionSys
{
    public class VesselPositionMessageHandler : SubSystem<VesselPositionSystem>, IMessageHandler
    {
        public ConcurrentQueue<IMessageData> IncomingMessages { get; set; } = new ConcurrentQueue<IMessageData>();

        public void HandleMessage(IMessageData messageData)
        {
            var msgData = messageData as VesselPositionMsgData;

            if (msgData == null || !System.PositionUpdateSystemBasicReady || VesselCommon.UpdateIsForOwnVessel(msgData.VesselId))
            {
                return;
            }

            var update = new VesselPositionUpdate(msgData);

            if (!System.ReceivedUpdates.ContainsKey(update.VesselId))
            {
                System.ReceivedUpdates.Add(update.VesselId, new Queue<VesselPositionUpdate>());
            }

            if (System.ReceivedUpdates[update.VesselId].Count + 1 > VesselPositionInterpolationSystem.MaxTotalUpdatesInQueue)
                System.ReceivedUpdates[update.VesselId].Dequeue();

            System.ReceivedUpdates[update.VesselId].Enqueue(update);
        }
    }
}
