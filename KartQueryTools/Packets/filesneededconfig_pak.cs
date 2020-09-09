using System.Runtime.InteropServices;

namespace KartQueryTools.Packets
{
    // https://github.com/STJr/Kart-Public/blob/master/src/d_clisrv.h#L444
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct filesneededconfig_pak
    {
        public uint first;
        public byte num;
        public byte more;
        public fixed byte files[serverinfo_pak.MAX_FILE_NEEDED]; // unused
    }
}