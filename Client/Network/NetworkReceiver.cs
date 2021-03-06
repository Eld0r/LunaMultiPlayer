﻿using System;
using System.Threading;
using LunaClient.Systems.Admin;
using LunaClient.Systems.Chat;
using LunaClient.Systems.ColorSystem;
using LunaClient.Systems.CraftLibrary;
using LunaClient.Systems.Flag;
using LunaClient.Systems.Handshake;
using LunaClient.Systems.KerbalSys;
using LunaClient.Systems.Lock;
using LunaClient.Systems.ModApi;
using LunaClient.Systems.Motd;
using LunaClient.Systems.PlayerConnection;
using LunaClient.Systems.Scenario;
using LunaClient.Systems.SettingsSys;
using LunaClient.Systems.Status;
using LunaClient.Systems.TimeSyncer;
using LunaClient.Systems.VesselProtoSys;
using LunaClient.Systems.VesselRemoveSys;
using LunaClient.Systems.VesselUpdateSys;
using LunaClient.Systems.Warp;
using LunaCommon.Enums;
using LunaCommon.Message.Data.Vessel;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using Lidgren.Network;
using LunaClient.Systems.VesselChangeSys;
using LunaClient.Systems.VesselFlightStateSys;
using LunaClient.Systems.VesselPositionSys;
using UnityEngine;

namespace LunaClient.Network
{
    public class NetworkReceiver
    {
        /// <summary>
        /// Main receiveing thread
        /// </summary>
        public static void ReceiveMain()
        {
            try
            {
                while (!MainSystem.Singleton.Quit)
                {
                    NetIncomingMessage msg;
                    while (NetworkMain.ClientConnection.ReadMessage(out msg))
                    {
                        NetworkStatistics.LastReceiveTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        switch (msg.MessageType)
                        {
                            case NetIncomingMessageType.NatIntroductionSuccess:
                                NetworkServerList.HandleNatIntroduction(msg);
                                break;
                            case NetIncomingMessageType.ConnectionLatencyUpdated:
                                NetworkStatistics.PingMs = (float)TimeSpan.FromSeconds(msg.ReadFloat()).TotalMilliseconds;
                                break;
                            case NetIncomingMessageType.UnconnectedData:
                                NetworkServerList.HandleServersList(msg);
                                break;
                            case NetIncomingMessageType.Data:
                                try
                                {
                                    var deserializedMsg = NetworkMain.SrvMsgFactory.Deserialize(msg.ReadBytes(msg.LengthBytes), DateTime.UtcNow.Ticks);
                                    EnqueueMessageToSystem(deserializedMsg as IServerMessageBase);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError("[LMP]: Error deserializing message! {e}");
                                    NetworkMain.HandleDisconnectException(e);
                                }
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                switch ((NetConnectionStatus)msg.ReadByte())
                                {
                                    case NetConnectionStatus.Disconnected:
                                        var reason = msg.ReadString();
                                        NetworkConnection.Disconnect(reason);
                                        break;
                                }
                                break;
                            default:
                                Debug.Log("[LMP]: LIDGREN: " + msg.MessageType + "-- " + msg.PeekString());
                                break;
                        }
                    }

                    Thread.Sleep(SettingsSystem.CurrentSettings.SendReceiveMsInterval);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[LMP]: Receive message thread error: " + e);
                NetworkMain.HandleDisconnectException(e);
            }
        }
        
        /// <summary>
        /// Enqueues the received message to the correct system
        /// </summary>
        /// <param name="msg"></param>
        private static void EnqueueMessageToSystem(IServerMessageBase msg)
        {
            switch (msg.MessageType)
            {
                case ServerMessageType.HANDSHAKE:
                    HandshakeSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.CHAT:
                    ChatSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.SETTINGS:
                    SettingsSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.PLAYER_STATUS:
                    StatusSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.PLAYER_COLOR:
                    PlayerColorSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.PLAYER_CONNECTION:
                    PlayerConnectionSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.SCENARIO:
                    ScenarioSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.KERBAL:
                    KerbalSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.VESSEL:
                    switch (((VesselBaseMsgData)msg.Data).VesselMessageType)
                    {
                        case VesselMessageType.UPDATE:
                            VesselUpdateSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                        case VesselMessageType.POSITION:
                            VesselPositionSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                        case VesselMessageType.FLIGHTSTATE:
                            VesselFlightStateSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                        case VesselMessageType.CHANGE:
                            VesselChangeSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                        case VesselMessageType.LIST_REPLY:
                        case VesselMessageType.VESSELS_REPLY:
                        case VesselMessageType.PROTO:
                            VesselProtoSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                        case VesselMessageType.REMOVE:
                            VesselRemoveSystem.Singleton.EnqueueMessage(msg.Data);
                            break;
                    }
                    break;
                case ServerMessageType.CRAFT_LIBRARY:
                    CraftLibrarySystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.FLAG:
                    FlagSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.SYNC_TIME:
                    TimeSyncerSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.MOTD:
                    MotdSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.WARP:
                    WarpSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.ADMIN:
                    AdminSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.LOCK:
                    LockSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                case ServerMessageType.MOD:
                    ModApiSystem.Singleton.EnqueueMessage(msg.Data);
                    break;
                default:
                    Debug.LogError("[LMP]: Unhandled Message type " + msg.MessageType);
                    break;
            }
        }
    }
}