using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using u8 = System.Text.Encoding;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// TODO: Uncomment the code below to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

using TcpClient handler = server.AcceptTcpClient();
using NetworkStream stream = handler.GetStream();

//Create a buffer
byte[] buffer = new byte[1024];

int data = stream.Read(buffer, 0, buffer.Length);

string request = u8.UTF8.GetString(buffer,0,data);

var reqArr = request.Split("\r\n");

var requestLine = reqArr[0].Split(" ");

if(requestLine.Length == 3)
{
    string method = requestLine[0];
    string target = requestLine[1].Split("/")[1];
    string httpVersion = requestLine[2];

    //string[] targetArr = target.Split("/");
    if(target == "")
    {
        stream.Socket.Send(u8.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n"));
    }
    else
    {
        stream.Socket.Send(u8.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n"));
    }

}
else
{
    stream.Socket.Send(u8.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n"));
}


