﻿using System;
using System.Collections.Generic;
using LunaClient.Systems.ColorSystem;
using LunaClient.Systems.Toolbar;
using UnityEngine;

namespace LunaClient.Systems.SettingsSys
{
    [Serializable]
    public class SettingStructure
    {
        public string PlayerName { get; set; } = "Player";
        public int CacheSize { get; set; } = 100;
        public int ConnectionTries { get; set; } = 3;
        public int InitialConnectionMsTimeout { get; set; } = 5000;
        public int SendReceiveMsInterval { get; set; } = 5;

#if DEBUG
        public int ConnectionMsTimeout { get; set; } = 120000;
#else
        public int ConnectionMsTimeout { get; set; } = 20000;
#endif

        public int MsBetweenConnectionTries { get; set; } = 3000;
        public int HearbeatMsInterval { get; set; } = 2000;
        public int MtuSize { get; set; } = 1408;
        public int SyncTimeRequestMsInterval { get; set; } = 1000;
        public bool CompressionEnabled { get; set; } = true;
        public bool DisclaimerAccepted { get; set; } = false;
        public Color PlayerColor { get; set; } = PlayerColorSystem.GenerateRandomColor();
        public KeyCode ChatKey { get; set; } = KeyCode.BackQuote;
        public string SelectedFlag { get; set; } = "Squad/Flags/default";
        public LmpToolbarType ToolbarType { get; set; } = LmpToolbarType.BlizzyIfInstalled;
        public List<ServerEntry> Servers { get; set; } = new List<ServerEntry>();
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public int PlayerStatusCheckMsInterval { get; set; } = 500;
        public int InitialConnectionSyncTimeRequests { get; set; } = 10;
        public bool RevertEnabled { get; set; }
        public bool InterpolationEnabled { get; set; } = true;
        public bool PositionFudgeEnable { get; set; } = false;
        public bool PackOtherControlledVessels { get; set; } = false;

#if DEBUG
        public bool Debug1 { get; set; } = false;
        public bool Debug2 { get; set; } = false;
        public bool Debug3 { get; set; } = false;
        public bool Debug4 { get; set; } = false;
        public bool Debug5 { get; set; } = false;
        public bool Debug6 { get; set; } = false;
        public bool Debug7 { get; set; } = false;
        public bool Debug8 { get; set; } = false;
        public bool Debug9 { get; set; } = false;
#endif

    }

    [Serializable]
    public class ServerEntry
    {
        public int Port;
        public string Name { get; set; }
        public string Address { get; set; }
    }
}