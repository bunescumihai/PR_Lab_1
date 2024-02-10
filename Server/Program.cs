using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 9000;
bool run = true;

List <Socket> connections = new List <Socket>();
List <Thread> threads = new List <Thread>();

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
    Thread controlConnections = new Thread(new ThreadStart(ControlConnections));
    Thread controlThreads = new Thread(new ThreadStart(ControlThreads));
    controlConnections.Start();
    controlThreads.Start();

    Socket connection = serverSocket.Accept();
    connections.Add(connection);

    clientsConnected++;

    Console.WriteLine("A client has connected");
    Console.WriteLine($"Total: {clientsConnected} connections");

}

void ServeConnection(Socket connection)
{

    StringBuilder sb = new StringBuilder();

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
    catch { }

    Console.WriteLine(sb.ToString());
    sb = sb.Clear();

}

void ControlConnections()
{
    while (true)
    {
        Thread.Sleep(100);
        try
        {
            foreach (Socket conn in connections)
            {
                if (!conn.Connected)
                {
                    clientsConnected--;

                    Console.WriteLine("A client has deconnected");
                    Console.WriteLine($"Total: {clientsConnected} connections");

                    connections.Remove(conn);

                    continue;
                }

                if (conn.Available > 0)
                {
                    while (threads.Count > 10)
                        Thread.Sleep(50);

                    Thread thread = new Thread(new ThreadStart(() => ServeConnection(conn)));
                    threads.Add(thread);
                    thread.Start();

                }
            }
        }
        catch { }
    }
}


void ControlThreads()
{
    while (true)
    {
        Thread.Sleep(100);
        try
        {
            foreach (Thread th in threads)
            {
                if (!th.IsAlive)
                    threads.Remove(th);
            }
        }
        catch { }
    }
}