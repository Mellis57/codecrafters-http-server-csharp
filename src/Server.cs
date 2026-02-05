using codecrafters_http_server.src;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using u8 = System.Text.Encoding;

Dictionary<string, string> argDict = new();

// Should bet set in debug cli commands or through your_program.sh
if (args.Length == 2)
    argDict.TryAdd(args[0], args[1]);

// You can use print statements as follows for debugging, they'll be visible
// when running tests.
Console.WriteLine("Logs from your program will appear here!");
Console.WriteLine("Starting..."); 
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while (true)
{
    TcpClient client = await server.AcceptTcpClientAsync();
    RequestProcessor processor = new(client, argDict);
    _ = processor.Process();
}



