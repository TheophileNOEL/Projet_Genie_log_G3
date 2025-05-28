using System.Net.Sockets;
using System.Text;

public class SocketClient
{
    private TcpClient _client;
    private NetworkStream _stream;

    public bool Connect(string host = "127.0.0.1", int port = 5000)
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(host, port);
            _stream = _client.GetStream();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SendCommand(string command)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(command);
        _stream.Write(buffer, 0, buffer.Length);
    }

    public string ReadResponse()
    {
        byte[] buffer = new byte[4096];
        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}
