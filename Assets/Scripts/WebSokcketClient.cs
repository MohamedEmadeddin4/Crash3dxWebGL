using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient
{
    private ClientWebSocket ws = new ClientWebSocket();

    public async Task Connect(string uri)
    {
        await ws.ConnectAsync(new Uri(uri), CancellationToken.None);
        Console.WriteLine("Connected!");

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine("Received: " + message);
        }
        while (!result.CloseStatus.HasValue);

        await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

}
