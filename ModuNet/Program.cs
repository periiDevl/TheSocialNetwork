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
        public static async Task runManager(Server server, string awn)
        {
            while (true)
            {
                
                awn = Console.ReadLine();
                
                if (awn.StartsWith("include"))
                {
                    Console.WriteLine("Detected include.");
                    awn = awn.Substring(7).Trim('\0', ' ', '\t', '\r', '\n');
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
                if (manager.getModules()[0] is UserDatabase)
                {
                    if (awn == "SHOW DATABASE")
                    {
                        UserDatabase d = (UserDatabase)manager.getModules()[0];
                        d.showUsers();
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
            return awn;
        }
        public static async Task Main(string[] args)
        {
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
                Task managerTask = runManager(server, awn);

                await Task.WhenAny(serverTask, managerTask);
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