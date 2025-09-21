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
    public enum Flags
    {
        NF = 0, //No flag
        DSBLE = 0x2, //disable Module
        URGNT = 0x4, //force command only for priv users
        PRRQ = 0x8, //resuest privlage
    }
    public struct Packet
    {
        public UInt16 section; //What Module to interact with
        public UInt16 version;//version
        public UInt32 opr;//operation add, regist..
        public UInt32 flagNresurve; // byte flag, 3 bytes resurve
        public string payload; //payload, text maybe
    }
    public class PacketHandler
    {
        public Packet create(
            UInt16 sec,
            UInt16 ver,
            UInt32 op,
            byte flag,
            UInt32 resurve // Last byte will be ignored
            )
        {
            Packet packet = new Packet();
            packet.section = sec;
            packet.version = ver;
            packet.opr = op;
            packet.flagNresurve = ((uint)flag << 24) | (resurve & 0x00FFFFFF);
            return packet;
        }
        public Packet bytesToPacket(byte[] b)
        {
            Packet packet = new Packet();

            packet.section = (UInt16)(b[0] | (b[1] << 8));

            packet.version = (UInt16)(b[2] | (b[3] << 8));

            packet.opr = (UInt32)(b[4]
                       | (b[5] << 8)
                       | (b[6] << 16)
                       | (b[7] << 24));

            packet.flagNresurve = (UInt32)(b[8]
                               | (b[9] << 8)
                               | (b[10] << 16)
                               | (b[11] << 24));

            if (b.Length > 12)
                packet.payload = System.Text.Encoding.UTF8.GetString(b, 12, b.Length - 12);
            else
                packet.payload = string.Empty;

            return packet;
        }
        public byte[] packetToBytes(Packet pkt)
        {
            List<byte> bytes = new List<byte>();

            bytes.Add((byte)(pkt.section & 0xFF));
            bytes.Add((byte)((pkt.section >> 8) & 0xFF));

            bytes.Add((byte)(pkt.version & 0xFF));
            bytes.Add((byte)((pkt.version >> 8) & 0xFF));

            bytes.Add((byte)(pkt.opr & 0xFF));
            bytes.Add((byte)((pkt.opr >> 8) & 0xFF));
            bytes.Add((byte)((pkt.opr >> 16) & 0xFF));
            bytes.Add((byte)((pkt.opr >> 24) & 0xFF));

            bytes.Add((byte)(pkt.flagNresurve & 0xFF));
            bytes.Add((byte)((pkt.flagNresurve >> 8) & 0xFF));
            bytes.Add((byte)((pkt.flagNresurve >> 16) & 0xFF));
            bytes.Add((byte)((pkt.flagNresurve >> 24) & 0xFF));

            if (!string.IsNullOrEmpty(pkt.payload))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes(pkt.payload));
            }

            return bytes.ToArray();
        }

        public byte getFlag(Packet pkt)
        {
            return (byte)(pkt.flagNresurve >> 24);
        }
        
        public void addPayload(string data, ref Packet pkt)
        {
            pkt.payload = data;
        }
        public string getCleanPayload(ref Packet pkt)
        {
            return pkt.payload.Trim('\0', ' ', '\t', '\r', '\n');
        }
    }
}
