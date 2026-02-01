using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using u8 = System.Text.Encoding;

const string _okStatusLine = "HTTP/1.1 200 OK\r\n";
const string _notFound = "HTTP/1.1 404 Not Found\r\n\r\n";
const string _crlf = "\r\n";
ConcurrentDictionary<string, string> requestHeaders = new();

// You can use print statements as follows for debugging, they'll be visible
// when running tests.
Console.WriteLine("Logs from your program will appear here!");
Console.WriteLine("Starting..."); 
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while (true)
{
    Console.WriteLine("Why wont you work");
    TcpClient client = await server.AcceptTcpClientAsync();
    _ = Process(client);
}



async Task Process(TcpClient client)
{
    using (client)
    await using (NetworkStream stream = client.GetStream())
    {
        byte[] buffer = new byte[1024];

        StringBuilder builder = new();

        int data = await stream.ReadAsync(buffer, 0, buffer.Length);

        string request = u8.UTF8.GetString(buffer, 0, data);
        
        //debug
        //request = @"GET / HTTP/1.1\r\nHost: localhost:4221\r\n\r\n";
        Console.WriteLine("request");
        string[] reqArr = request.Split("\r\n\r\n");
        string reqLine = reqArr[0];        
        string reqBody = reqArr.Length == 2 ? reqArr[1] : "";

        var reqLineWithHeaderArr = reqLine.Split("\r\n");

        for (int i = 1; i < reqLineWithHeaderArr.Length; i++)
        {
            Console.WriteLine("Heddys");

            string[] headerArr = reqLineWithHeaderArr[i].Split(" ");
            string key = headerArr[0];
            string val = headerArr[1];

            requestHeaders.TryAdd(key, val);
        }

        string[] reqLineArr = reqLineWithHeaderArr[0].Split(" ");

        if ((reqLineArr?.Length < 3))
        {
            Console.WriteLine("shit");

            await stream.WriteAsync(u8.UTF8.GetBytes(builder.ToString()));
            await stream.FlushAsync();
            return;

        }

        string method = reqLineArr[0];
        string target = reqLineArr[1];
        string httpVersion = reqLineArr[2];

        var targetArr = target.Split("/");

        string endpoint = target == "/" ? "/" : targetArr[1];

        Console.WriteLine("do the thing");

        switch (endpoint)
        {
            case "/":
                builder.Append($"{_okStatusLine}{_crlf}");
                break;
            case "echo":
                builder.Append(_okStatusLine);

                //Headers
                builder.Append($"Content-Type: text/plain{_crlf}");
                builder.Append($"Content-Length: {targetArr[2].Length.ToString()}{_crlf}");
                builder.Append(_crlf);

                builder.Append(targetArr[2]);
                break;
            case "user-agent":
                if (requestHeaders.TryGetValue("User-Agent:", out string responseBody))
                {
                    builder.Append(_okStatusLine);

                    //Headers
                    builder.Append($"Content-Type: text/plain{_crlf}");
                    builder.Append($"Content-Length: {responseBody.Length.ToString()}{_crlf}");
                    builder.Append(_crlf);

                    builder.Append(responseBody);
                }
                break;
            default:
                builder.Append(_notFound);
                break;
        }

        Console.WriteLine(builder.ToString());

        await stream.WriteAsync(u8.UTF8.GetBytes(builder.ToString()));

        await stream.FlushAsync();
    }
}