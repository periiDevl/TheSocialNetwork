using Avalonia.Controls.Platform;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModuNet
{
    class MainClass
    {
        public static ModulesManager manager = new ModulesManager();
        public static UInt16 currectSECTION = 0;
        public static void man()
        {
            Console.WriteLine(@"
Welcome to ModuNet! a program to make servers easy. here, you are welcomed with an empty server.
to give the server functionality you need to add MODULES.
to add, write -include <mod name>.
here are the modules that you can use:
UserDatabase

");
        }
        public static void runManager(Server server, string awn)
        {
            while (true)
            {
                
                awn = Console.ReadLine();
                
                if (awn.StartsWith("mount"))
                {
                    Console.WriteLine("Detected mount.");
                    awn = awn.Substring(6).Trim('\0', ' ', '\t', '\r', '\n');
                    Console.WriteLine("COMMAND = " + awn);
                    if (awn == "UserDatabase")
                    {
                        UserDatabase datab = new UserDatabase();
                        Console.WriteLine("Adding Database...");
                        datab.manual();
                        datab.create("Database.db");
                        Console.WriteLine("Created DATABASE SQL file");
                        datab.showUsers();
                        datab.who();
                        manager.add(datab);
                        Console.WriteLine("Manager knows about the DATABASE");
                        Console.WriteLine("DATABASE MODULE IS OPEN FOR ALL USERS.");

                        Console.WriteLine("DATABASE moudle added.");
                    }
                    //datab.close();
                }
                else if (awn.StartsWith("demount"))
                {
                    Console.WriteLine("Demount active");
                    awn = awn.Substring(8).Trim('\0', ' ', '\t', '\r', '\n');

                    Console.WriteLine("Iterating...");
                    UserDatabase dbb = (UserDatabase)manager.getModules()[0];
                    dbb.close();
                    Module empty = new Module();
                    manager.getModules()[0] = empty;
                    Console.WriteLine("DONE.");
                }
                if (manager.getModules()[0] is UserDatabase && awn.Contains("DATABASE"))
                {
                    if (awn == "SHOW DATABASE")
                    {
                        UserDatabase d = (UserDatabase)manager.getModules()[0];
                        d.showUsers();
                    }
                    if (awn == "CLOSE DATABASE")
                    {
                        UserDatabase d = (UserDatabase)manager.getModules()[0];
                        d.close();
                        Console.WriteLine("DONE.");
                    }
                }
                
            }
        }
        public static string InputChecker(string awn)
        {
            if (currectSECTION >= 0)
            {
                //ASSUMING DATABASE

            }
            if (awn == "DESELECT")
            {
                Console.WriteLine("[+] REMOVING SELECTION.");
                currectSECTION = 0;
            }
            else if (awn.StartsWith("SELECT"))
            {
                Console.WriteLine("[+] SELECT");
                awn = awn.Substring(7);
                if (awn.StartsWith("SECTION"))
                {
                    Console.WriteLine("[+] SELECTION");
                    awn = awn.Substring(8);
                    currectSECTION = UInt16.Parse(awn);
                    Console.WriteLine("[+] THE SELECTION IS " + currectSECTION);
                    return null;
                }
                return awn;
            }
            else if (awn.StartsWith("login"))
            {
                string[] info = awn.Split('<');
                string username = info[1].Split('>')[0];
                string password = info[2].Split('>')[0];
                awn = $"login <{username}> <{Encyptions.ComputeSha256Hash(password)}>";
            }
            string[] awnSplit = awn.Split('/');
            if (awnSplit.Length == 4) //And in database section
            {
                if (awnSplit.Length == 4)
                {
                    awnSplit[1] = Encyptions.ComputeSha256Hash(awnSplit[1]);
                }
                // USERNAME/PASSWORD/EMAIL/PHONE
                awn = $"{awnSplit[0]}/{awnSplit[1]}/{awnSplit[2]}/{awnSplit[3]}";
            }
            return awn;
        }
        public static async Task Main(string[] args)
        {
            Encyptions enc = new Encyptions();
            PacketHandler pktHandler = new PacketHandler();
            Console.WriteLine("Choose if you are a host or a client:");
            Console.WriteLine("C/H");
            string awn = Console.ReadLine();
            awn = awn.ToLower();
            if (awn == "h")
            {
                man();
                Server server = new Server();
                server.modules = manager;
                Task serverTask = server.mainServer();
                runManager(server, awn);
                await Task.WhenAny(serverTask);
            }


            if (awn == "c")
            {
                var client = new Client();
                await client.connect();
                
                Task.Run(async () =>
                {
                    while (true)
                    {
                        string input = Console.ReadLine();
                        input = InputChecker(input);

                        if (input == null)
                            continue;

                        Packet packet = pktHandler.create(currectSECTION, 1, 0, 0, 0);
                        pktHandler.addPayload(input, ref packet);
                        await client.sendPacket(packet);

                        Console.WriteLine($"Sent: {input}");
                    }
                });
                while (true)
                {
                    if (client.dataInStream())
                    {
                        Packet response = await client.recivePacket();
                        Console.WriteLine($"Reply: {response.payload}");
                    }

                    await Task.Delay(1); // prevent 100% CPU usage
                }
            }

        }
    }
}