using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBotter
{
    public static class ProxyHandler
    {
        public static async Task<TcpClient> ConnectSocks5Async(IPEndPoint proxy, string targetHost, int targetPort)
        {
            var client = new TcpClient();
            await client.ConnectAsync(proxy.Address, proxy.Port);
            var stream = client.GetStream();
            
            // SOCKS5 handshake
            byte[] authReq = { 0x05, 0x01, 0x00 };
            await stream.WriteAsync(authReq, 0, authReq.Length);
            
            byte[] response = new byte[2];
            await stream.ReadAsync(response, 0, 2);
            if (response[0] != 0x05) throw new Exception("SOCKS5 not supported");
            
            // Connection request
            byte[] connectReq = new byte[4 + 1 + 1 + 2 + targetHost.Length];
            connectReq[0] = 0x05;
            connectReq[1] = 0x01;
            connectReq[2] = 0x00;
            connectReq[3] = 0x03;
            connectReq[4] = (byte)targetHost.Length;
            Encoding.ASCII.GetBytes(targetHost, 0, targetHost.Length, connectReq, 5);
            connectReq[5 + targetHost.Length] = (byte)(targetPort >> 8);
            connectReq[6 + targetHost.Length] = (byte)(targetPort & 0xFF);
            await stream.WriteAsync(connectReq, 0, connectReq.Length);
            
            byte[] resp = new byte[10];
            await stream.ReadAsync(resp, 0, resp.Length);
            if (resp[1] != 0x00) throw new Exception("SOCKS5 connection failed");
            
            return client;
        }
    }
}