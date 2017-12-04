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
        public string receive;
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

            backgroundWorker1.RunWorkerAsync(); // Start reiceving Data in the background
            backgroundWorker2.WorkerSupportsCancellation = true; // Ability to cancel this thread


        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) // receive data
        {
            while (client.Connected)
            {
                try
                {
                    receive = STR.ReadLine();
                    this.TxtStatus.Invoke(new MethodInvoker(delegate () { TxtStatus.AppendText("You : " + receive + "\n"); }));
                    receive = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) // send data
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);
                this.TxtSend.Invoke(new MethodInvoker(delegate () { TxtSend.AppendText("Me : " + text_to_send + "\n"); }));

            }
            else
            {
                MessageBox.Show("Send failed!");
            }
            backgroundWorker2.CancelAsync();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(TxtIPClient.Text), int.Parse(TxtClientPort.Text));

            try
            {
                client.Connect(iPEndPoint);
                if (client.Connected)
                {
                    TxtStatus.AppendText("Connected to Server" + "\n");
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync(); // Start reiceving Data in the background
                    backgroundWorker2.WorkerSupportsCancellation = true; // Ability to cancel this thread
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                throw;
            }
        }

        private void BtnSend_Click(object sender, EventArgs e) // Send Button
        {
            if (TxtSend.Text != "")
            {
                text_to_send = TxtSend.Text;
                backgroundWorker2.RunWorkerAsync();
            }

            TxtSend.Text = "";
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
