using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
class Program
{
    static void Main()
    {
        try
        {
            IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(serverPoint);
            serverSocket.Listen(1000);
            serverSocket.BeginAccept(AcceptConnectionCallback, serverSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadLine();
    }

    static void AcceptConnectionCallback(IAsyncResult result)
    {
        if (result.AsyncState != null)
        {
            byte[] data = new byte[1024];
            Socket server = (Socket)result.AsyncState;
            Socket client = server.EndAccept(result);
            Console.WriteLine($"Connection has occurred: {client.RemoteEndPoint} in {DateTime.Now}");
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, ClientReciveMessageCallback, new ClientMessage(client, data));
            server.BeginAccept(AcceptConnectionCallback, server);
        }
    }

    static void ClientReciveMessageCallback(IAsyncResult result)
    {
        if (result.AsyncState != null)
        {
            ClientMessage clientMessage = (ClientMessage)result.AsyncState;
            Socket client = clientMessage.GetClient();
            byte[] data = clientMessage.GetData();
            client.EndReceive(result);
            if (data[0] > 0)
            {
                string message = Encoding.UTF8.GetString(data);
                if(message != "<Decimal><Decimal>")
                Console.WriteLine(message);
            }
            client.BeginReceive(data, 0, data.Length, SocketFlags.None, ClientReciveMessageCallback, new ClientMessage(client, data));
        }
    }
}

class ClientMessage
{
    Socket _client;
    byte[] _message = new byte[1024];
    public ClientMessage(Socket client, byte[] message)
    {
        _client = client;
        _message = message;
    }
    public Socket GetClient()
    {
        return _client;
    }
    public byte[] GetData()
    {
        return _message;
    }
}