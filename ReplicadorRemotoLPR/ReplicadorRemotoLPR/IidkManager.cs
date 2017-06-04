using System;
using System.Runtime.InteropServices;

namespace ISS.Net
{
    public delegate void IidkCallback([MarshalAs(UnmanagedType.LPStr)] string msg);
    //
    public class IidkWrapper
    {
        //C:\Program Files\ISS\SecurOS\
        [DllImport(@"C:\Program Files (x86)\ISS\SecurOS\iidk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Connect([MarshalAs(UnmanagedType.LPStr)] string address,
            [MarshalAs(UnmanagedType.LPStr)] string port,
            [MarshalAs(UnmanagedType.LPStr)] string id,
            IidkCallback callback
            );
        [DllImport(@"C:\Program Files (x86)\ISS\SecurOS\iidk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int SendMsg([MarshalAs(UnmanagedType.LPStr)] string id,
            [MarshalAs(UnmanagedType.LPStr)] string msg);
        [DllImport(@"C:\Program Files (x86)\ISS\SecurOS\iidk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Disconnect([MarshalAs(UnmanagedType.LPStr)] string id);
    }
    //
    public delegate void MessagesListener(Message msg);
    public delegate void ConnectionStateListener(bool connected);
    //
    public class IidkManager
    {
        private string selfId;
        private bool connected;
        private IidkCallback callback;
        public string IpAdress;
        public string ConnectionPort;
        //
        public event MessagesListener OnSecurOSMessage;
        public event ConnectionStateListener OnConnectionStateChanged;
        //
        public IidkManager()
        {
            connected = false;
            callback = new IidkCallback(ProcessSecurOSMessage);
        }
        //
        public void Connect(string address, string port, string id)
        {
            selfId = id;
            IidkWrapper.Connect(address, port, id, callback);
        }
        public void Disconnect()
        {
            if (!connected)
            {
                throw new System.Exception("Invalid connection state");
            }
            IidkWrapper.Disconnect(selfId);
        }
        public bool IsConnected()
        {
            return connected;
        }
        public void SendMessage(string msg)
        {
            if (!connected)
            {
                //throw new System.Exception("Invalid connection state");
                IidkWrapper.Connect(IpAdress, ConnectionPort, "", callback);
            }
            //  IidkWrapper.Connect("127.0.0.1", "1030", "1", callback);
            IidkWrapper.SendMsg(selfId, msg);
        }
        //
        private void ProcessSecurOSMessage(string incomingMsg)
        {
            bool bConnectedEvent = incomingMsg.Equals("CONNECTED");
            bool bDisconnectedEvent = incomingMsg.Equals("DISCONNECTED");
            if (bConnectedEvent || bDisconnectedEvent)
            {
                connected = bConnectedEvent;
                if (null != OnConnectionStateChanged)
                {
                    OnConnectionStateChanged(connected);
                }
            }
            else if (null != OnSecurOSMessage)
            {
                OnSecurOSMessage(new Message(incomingMsg));
            }
        }
    }

}
