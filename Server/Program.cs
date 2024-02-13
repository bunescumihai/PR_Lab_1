using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 9000;
bool run = true;


Queue<Job> jobQueue = new Queue<Job>();
Mutex mutex = new Mutex();
Mutex queueMutex = new Mutex();


ConnectionsHandler connectionsHandler = new ConnectionsHandler();
ThreadsHandler threadsHandler = new ThreadsHandler();

MessagesSender messagesSender = new MessagesSender(connectionsHandler);
const int threadsNumber = 10;

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



if (run)
{
    Console.WriteLine("Server is starting");
    Console.WriteLine("Waiting for connections...");

    for (int i = 0; i < threadsNumber; i++)
        threadsHandler.Add(new Thread(new ThreadStart(() => { ThreadWork(); })));

    Thread controlConnectionsThread = new Thread(new ThreadStart(() => {  ControlConnections(); }));
    controlConnectionsThread.Start();

    serverSocket.Listen(5);
}

while (run)
{
    Socket connection = serverSocket.Accept();
    mutex.WaitOne();
    connectionsHandler.Add(new Connection(connection));
    mutex.ReleaseMutex();
}

void ServeConnection(Connection connection)
{
    connection.InServing = true;

    StringBuilder sb = new StringBuilder();

    do
    {
        byte[] buffer = new byte[1024];

        try
        {
            int bytesReceived = connection.Conn.Receive(buffer);
            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
        }
        catch (Exception ex)
        {
            throw new ReceiveException();
        }

    } while (connection.Conn.Available > 0);

    try
    {
        messagesSender.SendToAll(connection, sb.ToString());
    }
    catch {
        throw new SendException();
    }

    connection.InServing = false;

    if(sb.Length > 0)
        Console.WriteLine(sb.ToString());

    sb = sb.Clear();
}



void ControlConnections()
{
    while (true)
    {
        Connection connection = connectionsHandler.NeedToServe();
        jobQueue.Enqueue(new Job(() => { ServeConnection(connection); }));
    }
}


void ThreadWork()
{
    while (true)
    {
        queueMutex.WaitOne();
        if(jobQueue.Count> 0)
        {
            Job job = jobQueue.Dequeue();
            queueMutex.ReleaseMutex();
            try
            {
                job.Execute();
            }
            catch(ReceiveException ex) 
            {
                Console.WriteLine(ex.Message);
            }
            catch (SendException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            queueMutex.ReleaseMutex();
            Thread.Sleep(100);
        }

    }
}
