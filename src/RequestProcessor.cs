using codecrafters_http_server.src.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using u8 = System.Text.Encoding;

namespace codecrafters_http_server.src
{
    sealed public class RequestProcessor
    {
        const string _okStatusLine = "HTTP/1.1 200 OK\r\n";
        const string _okCreated = "HTTP/1.1 201 Created\r\n\r\n";
        const string _notFound = "HTTP/1.1 404 Not Found\r\n\r\n";
        const string _crlf = "\r\n";
        private int _timeout = 10000;

        Dictionary<string, string> requestHeaders = new();
        Dictionary<string, string> argDict = new();

        private TcpClient _client;
        public RequestProcessor(TcpClient client, Dictionary<string, string> args)
        {
            _client = client;
            argDict = args;
        }

        public async Task Process()
        {
            using (_client)
            await using (NetworkStream stream = _client.GetStream())
            {
                while(true)
                {
                    byte[] buffer = new byte[1024];

                    StringBuilder builder = new();

                    int data;
                    try
                    {
                        using var cts = new CancellationTokenSource(_timeout);
                        data = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Connection timed out.");
                        break;
                    }

                    if (data == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    string request = u8.UTF8.GetString(buffer, 0, data);

                    string[] reqArr = request.Split("\r\n\r\n");
                    string reqLine = reqArr[0];
                    string reqBody = reqArr.Length == 2 ? reqArr[1] : "";

                    var reqLineWithHeaderArr = reqLine.Split("\r\n");

                    for (int i = 1; i < reqLineWithHeaderArr.Length; i++)
                    {
                        string[] headerArr = reqLineWithHeaderArr[i].Split(": ");
                        string key = headerArr[0].Trim('[',']');
                        string val = headerArr[1];

                        requestHeaders.TryAdd(key, val);
                    }

                    string[] reqLineArr = reqLineWithHeaderArr[0].Split(" ");

                    if ((reqLineArr?.Length < 3))
                    {
                        await stream.WriteAsync(u8.UTF8.GetBytes(builder.ToString()));
                        await stream.FlushAsync();
                        return;

                    }

                    Enum.TryParse<Method>(reqLineArr[0], false, out Method httpMethod);
                    string target = reqLineArr[1];
                    string httpVersion = reqLineArr[2];

                    var targetArr = target.Split("/");

                    string endpoint = target == "/" ? "/" : targetArr[1];

                    switch (endpoint)
                    {
                        case "/":
                            builder.Append($"{_okStatusLine}{_crlf}");
                            break;
                        case "echo":
                            requestHeaders.TryGetValue(RequestHeaders.AcceptEncoding, out string encoding);

                            string message = targetArr[2];

                            string? gzip = encoding?.Split(",")                        
                                ?.FirstOrDefault(s => s.Trim() == "gzip");

                            builder.Append(_okStatusLine);

                            //Headers
                            builder.Append($"{ResponseHeaders.ContentType} text/plain{_crlf}");

                            if (!string.IsNullOrWhiteSpace(gzip))
                            {
                                var messageBytes = u8.UTF8.GetBytes(message);
                                byte[] compressed;

                                using (MemoryStream memoryStream = new())
                                {
                                    using (GZipStream compressor = new(memoryStream, CompressionMode.Compress, leaveOpen: true))
                                    {
                                        await compressor.WriteAsync(messageBytes, 0, messageBytes.Length);
                                    }
                                    // GZipStream is now disposed, footer has been written
                                    compressed = memoryStream.ToArray();
                                }

                                builder.Append($"{ResponseHeaders.ContentEncoding} gzip{_crlf}");
                                builder.Append($"{ResponseHeaders.ContentLength} {compressed.Length}{_crlf}");
                                builder.Append(_crlf);

                                Console.WriteLine(builder.ToString());

                                await stream.WriteAsync(u8.UTF8.GetBytes(builder.ToString()));
                                await stream.WriteAsync(compressed);
                                await stream.FlushAsync();
                                return;
                            }
                            else
                            {
                                builder.Append($"{ResponseHeaders.ContentLength} {message.Length.ToString()}{_crlf}");
                                builder.Append(_crlf);

                                builder.Append(targetArr[2]);

                            }

                            break;
                        case "user-agent":
                            if (requestHeaders.TryGetValue(RequestHeaders.UserAgent, out string userAgentBody))
                            {
                                builder.Append(_okStatusLine);

                                //Headers
                                builder.Append($"{ResponseHeaders.ContentType} text/plain{_crlf}");
                                builder.Append($"{ResponseHeaders.ContentLength} {userAgentBody.Length.ToString()}{_crlf}");
                                builder.Append(_crlf);

                                builder.Append(userAgentBody);
                            }
                            break;
                        case "files":
                            string fileName = targetArr[2];

                            argDict.TryGetValue("debugDir", out string debugDir);

                            if (!argDict.TryGetValue("--directory", out string dir))
                            {
                                Console.WriteLine("--directory key not found in dictionary");
                                builder.Append(_notFound);
                                break;
                            }

                            Console.WriteLine($"Directory: {dir}");
                            Console.WriteLine($"File: {fileName}");

                            if (!Directory.Exists(dir))
                            {
                                Console.WriteLine("Dir not found");
                                builder.Append(_notFound);
                                break;
                            }

                            string filePath = Path.Combine(dir, fileName);

                            Console.WriteLine($"{nameof(httpMethod)} {filePath}");

                            if (httpMethod == Method.GET)
                            {

                                if (!File.Exists(filePath))
                                {
                                    Console.WriteLine("File not found");
                                    builder.Append(_notFound);
                                    break;

                                }

                                var fileData = await File.ReadAllTextAsync(filePath);
                                // Status Line
                                builder.Append(_okStatusLine);

                                //Headers
                                builder.Append($"{ResponseHeaders.ContentType} application/octet-stream{_crlf}");
                                builder.Append($"{ResponseHeaders.ContentLength} {fileData.Length}{_crlf}");
                                builder.Append(_crlf);

                                // Body
                                builder.Append(fileData);

                            }
                            else if (httpMethod == Method.POST)
                            {
                                try
                                {
                                    await File.WriteAllBytesAsync(filePath, u8.UTF8.GetBytes(reqBody));

                                    builder.Append(_okCreated);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to write file at {filePath}. ex: {ex.Message} {ex.InnerException?.ToString() ?? string.Empty}");
                                }
                            }
                            break;
                        default:
                            builder.Append(_notFound);
                            break;
                    }

                    Console.WriteLine(builder.ToString());

                    await stream.WriteAsync(u8.UTF8.GetBytes(builder.ToString()));                

                    await stream.FlushAsync();

                    if(requestHeaders.TryGetValue(RequestHeaders.Connection, out string connection)
                        && connection.ToLower() == "close")
                        break;

                    requestHeaders.Clear();
                }
            }
        }
    }

    public enum Method
    {
        GET,
        POST,
        PUT,
        PATCH,
        UPDATE,
        DELETE
    }
}
