using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketServer
{
    private TcpListener _listener;
    private Thread _serverThread;

    public void Start(int port = 5000)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _serverThread = new Thread(ListenForClients);
        _serverThread.IsBackground = true;
        _serverThread.Start();
        Console.WriteLine("Serveur TCP lancé sur le port " + port);
    }

    private void ListenForClients()
    {
        while (true)
        {
            TcpClient client = _listener.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClientComm(client));
            clientThread.Start();
        }
    }

    private void HandleClientComm(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];

        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Message reçu : " + clientMessage);

            string response = ProcessCommand(clientMessage);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        client.Close();
    }

    private string ProcessCommand(string msg)
    {
        if (msg.StartsWith("pause:"))
        {
            string scenarioName = msg.Substring(6);
            // À compléter plus tard avec ta logique métier
            return $"Scénario {scenarioName} mis en pause.";
        }
        return "Commande inconnue";
    }
}
