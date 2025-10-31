using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuNet
{
    public abstract class Streams
    {
        protected string stream;
        public Streams() { }
        public virtual string getStream() { return stream; }
        public abstract void recive(string stream);
        public virtual void flush()
        {
            stream = "";
        }
    }
    public class ServerRequestPacketStream : Streams
    {
        public Packet pkt;
        public override void recive(string stream)
        {
            this.stream = stream;
        }
        public void recive(Packet packet)
        {
            this.pkt = packet;
            this.stream = packet.payload;
        }

    }
}
