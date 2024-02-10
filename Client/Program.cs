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
}

while (run)
{
    Console.WriteLine("Enter a message to send to server");

    counter ++;

    string text = Console.ReadLine() ?? "";

    if (text.Equals("Close"))
    {
        clientSocket.Close();

        Console.WriteLine("Connection was closed");

        break;
    }


    byte[] bytesData = Encoding.UTF8.GetBytes($"{name}: Message {counter}");

    clientSocket.Send(bytesData);

    string receivedText = string.Empty;

    do
    {
        byte[] buffer = new byte[1024];
        int bytesReceived = clientSocket.Receive(buffer);

        receivedText += Encoding.UTF8.GetString(buffer, 0, bytesReceived);

    } while (clientSocket.Available > 0);

    Console.WriteLine($"Server: {receivedText}");
}


