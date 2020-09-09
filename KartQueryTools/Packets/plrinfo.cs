using System.Runtime.InteropServices;

namespace KartQueryTools.Packets
{
    // https://github.com/STJr/Kart-Public/blob/master/src/d_clisrv.h#L421
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct plrinfo
    {
        public const int MAX_PLAYER_NAME_LENGTH = 21;
        public const int ADDRESS_LENGTH = 4;

		public byte node;
        public fixed byte name[MAX_PLAYER_NAME_LENGTH + 1];
        public fixed byte address[ADDRESS_LENGTH];
        public byte team;
        public byte skin;
        public byte data;
        public uint score;
        public ushort timeinserver;
    }
}