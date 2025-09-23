using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModuNet
{
    internal class Client
    {
        PacketHandler hanlerPkt;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private string hostIP;
        private int port;
        private bool isConnected =false;

        public Client(string hp = "127.0.0.1", int prt = 8888)
        {
            hanlerPkt = new PacketHandler();
            hostIP = hp;
            port = prt;
            /*
            await Task.WhenAll(
                Communicate(handler1)
            );
            */
        }
        public bool dataInStream()
        {
            return stream.DataAvailable;
        }
        public async Task connect()
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(hostIP, port);
                tcpClient.NoDelay = true;
                stream = tcpClient.GetStream();
                isConnected = true;
                Console.WriteLine($"Connected to {hostIP}:{port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed... {ex}");

                throw;
            }
        }
        public async Task<string> reciveText()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, read);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in receiving text: {ex.Message}");
            }

            return string.Empty;

        }
        public async Task<Packet> recivePacket()
        {
            byte[] buffer = new byte[1024];

            try
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    return hanlerPkt.bytesToPacket(buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex} reciving packet");
            }
            Packet pkt = new Packet();
            return pkt;
        }
        public async Task sendText(string text)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                Console.WriteLine("Error in sending Text");
                throw;
            }
        }
        public async Task sendPacket(Packet pkt)
        {
            try
            {
                byte[] buffer = hanlerPkt.packetToBytes(pkt);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sending Text");
                throw;
            }

        }
    }
}
