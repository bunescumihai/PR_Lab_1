using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 9000;
bool run = true;

try
{
    serverSocket.Bind(new IPEndPoint(ipAddress, port));
}
catch(SocketException ex)
{
    Console.WriteLine(ex.ToString());
    Console.WriteLine(ex.Message);
    Console.WriteLine($"The port {port} is busy");
    run = false;
}


int clientsConnected = 0;

if (run)
{
    Console.WriteLine("Server is starting");
    Console.WriteLine("Waiting for connections...");
    serverSocket.Listen(5);
}

while (run)
{
    Socket connection = serverSocket.Accept();
    Console.Write("Accepted a connection");
    ThreadStart threadStart = new ThreadStart(() => ServeConnection(connection));
    Thread thread = new Thread(threadStart);
    thread.Start();
    
}

void ServeConnection(Socket connection)
{
    clientsConnected++;

    int clientNumber = clientsConnected;

    Console.WriteLine("A client has connected");
    Console.WriteLine($"Total: {clientsConnected} connections");

    StringBuilder sb = new StringBuilder();

    while (connection.Connected)
    {
        do
        {
            byte[] buffer = new byte[1024];

            try
            {
                int bytesReceived = connection.Receive(buffer);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (connection.Available > 0);

        try
        {
            connection.Send(Encoding.UTF8.GetBytes("Server receive your message"));
        }
        catch (Exception ex) { }

        Console.WriteLine(sb.ToString());
        sb = sb.Clear();

    }

    clientsConnected--;
    Console.WriteLine("A client has deconnected");
    Console.WriteLine($"Total: {clientsConnected} connections");
}
