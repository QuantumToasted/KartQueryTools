using System.Runtime.InteropServices;

namespace KartQueryTools.Packets
{
    // https://github.com/STJr/Kart-Public/blob/master/src/d_clisrv.h#L372
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct serverinfo_pak
    {
        public const int MAX_APPLICATION_LENGTH = 16;
        public const int MAX_SERVER_NAME_LENGTH = 32;
        public const int MAX_MAP_NAME_LENGTH = 8;
        public const int MAX_MAP_TITLE_LENGTH = 33;
        public const int MAP_MD5_LENGTH = 16;
        public const int MAX_MIRROR_LENGTH = 256;
        public const int MAX_FILE_NEEDED = 915;

        public byte _255;
        public byte packetversion;
        public fixed byte application[MAX_APPLICATION_LENGTH];
        public byte version;
        public byte subversion;
        public byte numberofplayer;
        public byte maxplayer;
        public byte gametype;
        public byte modifiedgame;
        public byte cheatsenabled;
        public byte kartvars;
        public byte fileneedednum;
        public uint time;
        public uint leveltime;
        public fixed byte servername[MAX_SERVER_NAME_LENGTH];
        public fixed byte mapname[MAX_MAP_NAME_LENGTH];
        public fixed byte maptitle[MAX_MAP_TITLE_LENGTH];
        public fixed byte mapmd5[MAP_MD5_LENGTH];
        public byte actnum;
        public byte iszone;
        public fixed byte httpsource[MAX_MIRROR_LENGTH];
        public fixed byte fileneeded[MAX_FILE_NEEDED];
    }
}