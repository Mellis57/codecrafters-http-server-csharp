using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using u8 = System.Text.Encoding;

const string _okStatusLine = "HTTP/1.1 200 OK\r\n";
const string _notFound = "HTTP/1.1 404 Not Found\r\n\r\n";
const string _crlf = "\r\n";

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// TODO: Uncomment the code below to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

using TcpClient handler = server.AcceptTcpClient();
using NetworkStream stream = handler.GetStream();

//Create a buffer
byte[] buffer = new byte[1024];

StringBuilder builder = new();

int data = stream.Read(buffer, 0, buffer.Length);

string request = u8.UTF8.GetString(buffer,0,data);

var reqArr = request.Split("\r\n");

var requestLine = reqArr[0]?.Split(" ");

if (!(requestLine?.Length == 3))
{
    stream.Socket.Send(u8.UTF8.GetBytes(_notFound));
    return;

}

string method = requestLine[0];
string target = requestLine[1];
string httpVersion = requestLine[2];

var targetArr = target.Split("/");

if(targetArr.Length == 3)
{
    string endpoint = targetArr[1];
    string responseBody = targetArr[2];
    builder.Append(_okStatusLine);
    //Headers
    builder.Append($"Content-Type: text/plain{_crlf}");
    builder.Append($"Content-Length: {responseBody.Length.ToString()}{_crlf}");
    builder.Append(_crlf);

    //Body
    builder.Append(responseBody);

    Console.WriteLine(builder.ToString());
    stream.Socket.Send(u8.UTF8.GetBytes(builder.ToString()));
    return;
}

//If we got here it's something else

switch (target)
{
    case "/":
        builder.Append($"{_okStatusLine}{_crlf}");
        break;
    default:
        builder.Append(_notFound);
        break;
}

Console.WriteLine(builder.ToString());

stream.Socket.Send(u8.UTF8.GetBytes(builder.ToString()));


