using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using Binarysharp.MemoryManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EHMLauncher
{
    public partial class fmMain : Form
    {
        public static Process ehm = null;
        public const string JSON_FILE = "servers.json";


        public fmMain()
        {
            InitializeComponent();
            notifyicon.BalloonTipText = "My application still working...";
            notifyicon.BalloonTipTitle = "My Sample Application";
            notifyicon.BalloonTipIcon = ToolTipIcon.Info;
            lbServers.DisplayMember = "display";
            LoadServers();
            GenerateContext();

        }

        public void LoadServers()
        {
            string[] rawJson = File.ReadAllText(JSON_FILE).Split('\n');
            foreach (string str in rawJson)
            {
                var srv = JsonConvert.DeserializeObject<Server>(str);
                if (srv == null) continue;
                lbServers.Items.Add(new Server(srv.ip, srv.desc));
            }
        }

        public static void InputIp(string ip)
        {
            try
            {
                MessageBox.Show(ip);
                ehm = Process.GetProcessesByName("ehm").First();
                MemorySharp mem = new MemorySharp(ehm);
                IntPtr ip1 = new IntPtr(0xAFCD20);
                IntPtr ip2 = new IntPtr(0xCAEC14);
                mem[ip1].WriteString(ip);
                mem[ip2].WriteString(ip);
            }
            catch { }
        }

        private void fmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyicon.Visible = true;
                Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyicon.Visible = false;
            }
        }

        private void notifyicon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbIP.Text) && !string.IsNullOrEmpty(tbDesc.Text))
            {
                lbServers.Items.Add(new Server(tbIP.Text, tbDesc.Text));
                var list = new List<Server>();
                foreach (Server srv in lbServers.Items)
                {
                    list.Add(srv);
                }
                json.WriteJson(list);
                GenerateContext();
            }
        }

        public class Server
        {
            public string ip { get; set; }
            public string desc { get; set; }
            public string display { get; set; }

            public Server(string ip, string desc)
            {
                this.ip = ip;
                this.desc = desc;
                display = desc + " - " + ip;
            }

            public Server() { }
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (lbServers.SelectedIndex > -1)
            {
                lbServers.Items.RemoveAt(lbServers.SelectedIndex);
                var list = new List<Server>();
                foreach (Server srv in lbServers.Items)
                {
                    list.Add(srv);
                }
                json.WriteJson(list);
                GenerateContext();
            }
        }

        public void GenerateContext()
        {
            contextMenu.Items.Clear();
            foreach (Server srv in lbServers.Items)
            {
                var item = contextMenu.Items.Add(srv.desc);
                item.Click += new EventHandler((sender, e) => InputIp(srv.ip));
            }
        }
    }
}
