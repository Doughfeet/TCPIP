using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerClient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string reiceive;
        public String text_to_send;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName()); //get my own IP

            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    TxtIPServer.Text = address.ToString();
                }
            }
        }

        private void BtnStart_Click(object sender, EventArgs e) //Start server
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(TxtServerPort.Text));
            listener.Start();

            client = listener.AcceptTcpClient();

            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

        }
    }
}
