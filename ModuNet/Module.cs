using System;
namespace ModuNet
{
    public class Module
    {
        public Module()
        {
        }
        public virtual void manual()
        {
            string man = "Basic manual,\n Hello World! ";
            Console.WriteLine(man);
        }
        public virtual void who()
        {
            Console.WriteLine("Empty Module");
        }
        public virtual async void hndlServer(
            ClientHandler handl,
            PacketHandler hanlerPkt,
            Packet pktSend,
            ServerRequestPacketStream serverRequestPacketStream,
            UInt16 ID

            )
        {
            Console.WriteLine($"ID{ID} moudle = null error");
            throw new NotImplementedException();
        }
    }
}
