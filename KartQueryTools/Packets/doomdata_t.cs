using System.Runtime.InteropServices;

namespace KartQueryTools.Packets
{
    // https://github.com/STJr/Kart-Public/blob/master/src/d_clisrv.h#L455
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct doomdata_t
    {
        public int checksum;
        public byte ack;
        public byte ackreturn;
        public byte packettype;
        public byte reserved;
        // union type is ignored as it's covered by other packet structs
    }
}