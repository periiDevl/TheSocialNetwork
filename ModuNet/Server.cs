using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModuNet
{
    internal class Server
    {
        PacketHandler hanlerPkt;
        public ModulesManager modules;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private const int port = 8888;
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream stream;
        public bool running = true;


        //Streams
        ServerRequestPacketStream serverRequestPacketStream = new ServerRequestPacketStream();
        public Server()
        {
            hanlerPkt = new PacketHandler();

        }
        public async Task start()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Server started on port:{port}");
                client = await listener.AcceptTcpClientAsync();
                client.NoDelay = true;
                stream = client.GetStream();
                Console.WriteLine("Client connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex}");
                throw;
            }
        }
        public async Task mainServer()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Server started");

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    client.NoDelay = true;

                    int newId = clients.Count + 1;
                    ClientHandler handler = new ClientHandler(client, newId);
                    clients.Add(handler);

                    Console.WriteLine($"Client {newId} connected.");
                    _ = Task.Run(() => Communicate(handler));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex}");
            }
        }

        private async Task Communicate(ClientHandler handl)
        {

            while (running)
            {
                try
                {
                    Packet recived = await handl.recivePacket();
                    serverRequestPacketStream.recive(recived);

                    Console.WriteLine($"Text from {handl.clientID} :{recived.payload}");
                    Packet pktSend = new Packet();
                    Console.WriteLine($"Client interacted with section [{recived.section}]");
                    //Check if packet is valid
                    if (recived.section == 0)
                    {
                        if (hanlerPkt.getCleanPayload(ref recived).Equals("?"))
                        {

                            pktSend = hanlerPkt.create(0, 1, 2, 3, 0);
                            hanlerPkt.addPayload(modules.containsWhat(), ref pktSend);
                            await handl.sendPacket(pktSend);
                            Console.WriteLine("Send client modules data");
                        }
                    }

                    for (int i = 0; i < modules.getModules().Count; i++)
                    {
                        UInt16 id = (UInt16)(i + 1);
                        if (recived.section == id)
                        {
                            modules.getModules()[i].hndlServer(handl, hanlerPkt, pktSend, serverRequestPacketStream, id);
                        }
                        serverRequestPacketStream.flush();
                    }
                }


                catch (Exception ex)
                {
                    Console.WriteLine($"Error{ex}");
                    running = false;
                }
                await Task.Delay(0);
            }


        }
    }
}