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
            try
            {
                while (running)
                {
                    Packet recived = await handl.recivePacket();

                    Console.WriteLine($"Text from {handl.clientID} :{recived.payload}");
                    Packet pktSend = new Packet();
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
                    else if (recived.section == 1)
                    {
                        UserDatabase datab = (UserDatabase)modules.getModules()[0];
                        pktSend = hanlerPkt.create(1, 1, 2, 3, 0);
                        if (hanlerPkt.getCleanPayload(ref recived).Equals("register"))
                        {
                            
                            string message = @"Register in this format: \n USERNAME/PASSWORD/EMAIL/PHONE";
                            hanlerPkt.addPayload(message, ref pktSend);
                            await handl.sendPacket(pktSend);
                            recived = await handl.recivePacket();
                            string[] info = recived.payload.Split('/');
                            
                            datab.register(info[0], info[1], info[2], info[3]);
                            Console.WriteLine($@"Created user with :
USERNAME = {info[0]}
EMAIL = {info[1]}
");
                            message = "[+]USER CREATED!";
                            hanlerPkt.addPayload(message, ref pktSend);
                            await handl.sendPacket(pktSend);

                        }
                        if (hanlerPkt.getCleanPayload(ref recived).StartsWith("login"))
                        {
                            string loginMessage = hanlerPkt.getCleanPayload(ref recived);
                            //login <USERNAME><PASSWORD>
                            string[] info = loginMessage.Split('<');
                            string username = info[1].Split('>')[0];
                            string password = info[2].Split('>')[0];
                            bool exists = datab.login(username, password);
                            string message = "This user does not exist.";
                            if (exists)
                            {
                                message = "You are logged in as : " + username;
                            }
                            hanlerPkt.addPayload(message, ref pktSend);
                            await handl.sendPacket(pktSend);
                        }
                    }



                    await Task.Delay(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error{ex}");
                running = false;
            }
        }
    }
}
