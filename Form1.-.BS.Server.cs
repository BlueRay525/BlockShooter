using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace BlockShooter___Server
{
    public partial class Form1 : Form
    {
        Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        const int ArrSize = 1024;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartServer(48248);
        }
        void StartServer(int Port)
        {
            Server.Bind(new IPEndPoint(IPAddress.Any, Port));
            Server.Listen(100);
            Server.BeginAccept(new AsyncCallback(Accept), null);
        }
        void Accept(IAsyncResult Result)
        {
            Socket User = Server.EndAccept(Result);
            byte[] Data = new byte[ArrSize];
            User.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(Receive), new StateObj(Data, User));
            Server.BeginAccept(new AsyncCallback(Accept), null);
            Debug.WriteLine("New connection: " + User.RemoteEndPoint);
        }
        void Receive(IAsyncResult Result)
        {
            Socket User = ((StateObj)Result.AsyncState).User;
            byte[] Data = ((StateObj)Result.AsyncState).Data;
            int Bytes = User.EndReceive(Result);
            Debug.WriteLine("New message");
            Data = new byte[ArrSize]; // flush stream
            User.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(Receive), new StateObj(Data, User));
        }
    }
    class StateObj
    {
        StateObj Obj;
        public byte[] Data;
        public Socket User;
        public StateObj(byte[] Data, Socket User)
        {
            Obj = new StateObj(Data, User);
            this.Data = Data;
            this.User = User;
        }
    }
}
