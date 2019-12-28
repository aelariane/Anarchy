using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Authentication;


public class WebSocket
{
    private string error = null;
    private bool isConnected = false;
    Queue<byte[]> messages = new Queue<byte[]>();
    private Uri mUrl;
    WebSocketSharp.WebSocket socket;
    private string protocols = "GpBinaryV16";

    public WebSocket(Uri url, string protocols = null)
    {
        this.mUrl = url;
        if (protocols != null)
        {
            this.protocols = protocols;
        }

        string protocol = mUrl.Scheme;
        if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            throw new ArgumentException("Unsupported protocol: " + protocol);
    }

    public void SendString(string str)
    {
        Send(Encoding.UTF8.GetBytes(str));
    }

    public string RecvString()
    {
        byte[] retval = Recv();
        if (retval == null)
            return null;
        return Encoding.UTF8.GetString(retval);
    }

    public void Connect()
    {
        socket = new WebSocketSharp.WebSocket(mUrl.ToString(), new string[] { this.protocols });
        socket.SslConfiguration.EnabledSslProtocols = socket.SslConfiguration.EnabledSslProtocols | (SslProtocols)(3072 | 768);
        socket.OnMessage += (sender, e) => messages.Enqueue(e.RawData);
        socket.OnOpen += (sender, e) => isConnected = true;
        socket.OnError += (sender, e) => error = e.Message + (e.Exception == null ? "" : " / " + e.Exception);
        socket.ConnectAsync();
    }

    public bool Connected { get { return isConnected; } }


    public void Send(byte[] buffer)
    {
        socket.Send(buffer);
    }

    public byte[] Recv()
    {
        if (messages.Count == 0)
            return null;
        return messages.Dequeue();
    }

    public void Close()
    {
        socket.Close();
    }

    public string Error
    {
        get
        {
            return error;
        }
    }
}