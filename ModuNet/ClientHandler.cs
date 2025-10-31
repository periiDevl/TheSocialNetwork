using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ModuNet
{
    public class ClientHandler
    {
        PacketHandler hanlerPkt;
        public TcpClient tcpClient { get; private set; }
        public NetworkStream stream { get; private set; }
        public int clientID { get; private set; }
        public string indentfier { get;set; }
        public ClientHandler(TcpClient c, int ID)
        {
            hanlerPkt = new PacketHandler();
            tcpClient = c;
            stream = tcpClient.GetStream();
            clientID = ID;
            var endpoint = (IPEndPoint)c.Client.RemoteEndPoint;
            indentfier = $"{endpoint.Address},{endpoint.Port}";
        }
        public async Task sendText(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine($"Error:{ex} ClientID{clientID}");
                    throw;
                }
            }
        }
        public async Task sendData(byte[] data)
        {
            try
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine($"Error:{ex} ClientID{clientID}");
                }
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
                {
                    Console.WriteLine($"Error:{ex} ClientID{clientID}");
                }
            }

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
                Console.WriteLine($"Error:{ex} from {clientID}");
            }
            Packet pkt = new Packet();
            return pkt;
        }

        public async Task<byte[]> reciveData()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex} from {clientID}");
            }
            return null;
        }
        public async Task<string> reciveText()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message} from {clientID}");
                throw;
            }
            return null;
        }
    }

}
