using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

IPAddress serverIp = IPAddress.Parse("127.0.0.1");
int serverPort = 9000;

Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

bool run = true;

try
{
    clientSocket.Connect(new IPEndPoint(serverIp, serverPort));
}
catch(Exception ex)
{ 
    Console.WriteLine(ex.Message);
    run = false; 
}

int counter = 1;
string name = string.Empty;

if (run)
{
    Console.Write("Please enter your name: ");
    name = Console.ReadLine() ?? "Unknown user";
    Thread receiverThread = new Thread(new ThreadStart(()=> { Receiver(); }));
    receiverThread.Start();
}

while (run)
{
    Console.WriteLine("Enter a message to send to server");

    string text = Console.ReadLine() ?? "";

    if (text.Equals("Close"))
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();

        Console.WriteLine("Connection was closed");

        break;
    }


    byte[] bytesData = Encoding.UTF8.GetBytes($"{name}: {text}");

    clientSocket.Send(bytesData);
}

void Receiver()
{
    StringBuilder sb = new StringBuilder();

    while (true)
    {
        do
        {
            byte[] buffer = new byte[1024];
            int bytesReceived = clientSocket.Receive(buffer);

            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));

        } while (clientSocket.Available > 0);

        Console.WriteLine(sb.ToString());

        sb.Clear();
    }
}


