﻿using System.Collections.Generic;

namespace LunaServer.Command.Command.Base
{
    public abstract class AdminCommand : HandledCommand
    {
        //This way, all admin Commands share the same Items list
        private static List<string> _sharedItems = new List<string>();

        //Share the lock between all instances of admin as the file is the same
        private static readonly object _commandLock = new object();
        protected override string FileName => "LMPAdmins.txt";
        protected override object CommandLock => _commandLock;

        protected override List<string> Items
        {
            get { return _sharedItems; }
            set { _sharedItems = value; }
        }
    }
}