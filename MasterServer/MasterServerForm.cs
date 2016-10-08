﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterServer
{
    public partial class MasterServerForm : Form
    {
        public MasterServerForm()
        {
            InitializeComponent();
        }
        
        public void WriteLine(string line)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(WriteLine), line);
                return;
            }
            tbStatus.Text += line + Environment.NewLine;
        }
        
        private void MasterServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MasterServer.RunServer = false;
            Thread.Sleep(5000);
        }

        private void MasterServerForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                Hide();
            }
            else if (WindowState == FormWindowState.Normal)
            {
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void tbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                var value = int.Parse(tbPort.Text + char.GetNumericValue(e.KeyChar));
                if (value > ushort.MaxValue)
                    e.Handled = true;
            }
            e.Handled |= !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MasterServer.Port = ushort.Parse(tbPort.Text);
            tbPort.ReadOnly = true;
            btnStart.Enabled = false;
            Task.Run(()=> MasterServer.Start(this));
        }
        
        public void UpdateServerList(IEnumerable<string> servers)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<string>>(UpdateServerList), servers);
                return;
            }
            serverList.Items.Clear();
            foreach (var server in servers)
            {
                serverList.Items.Add(server);
            }
        }
    }
}