using KartQueryTools.Packets;
using System;

namespace KartQueryTools
{
    /// <summary>
    /// Represents a map currently being played on a server.
    /// </summary>
    public sealed class KartMap
    {
        internal KartMap(serverinfo_pak srv)
        {
            unsafe
            {
                InternalName = KartUtils.DecodeString(srv.mapname, serverinfo_pak.MAX_MAP_NAME_LENGTH);
                
                Title = KartUtils.DecodeString(srv.maptitle, serverinfo_pak.MAX_MAP_TITLE_LENGTH);

                MD5 = new byte[16];
                for (var i = 0; i < 16; i++)
                    MD5[i] = srv.mapmd5[i];
            }

            ActNumber = srv.actnum;
            IsZone = srv.iszone == 0 || srv.iszone == 1
                ? srv.iszone == 1
                : throw new ArgumentOutOfRangeException(nameof(srv.iszone));
            TimeElapsed = TimeSpan.FromSeconds(srv.leveltime / (double) KartUtils.SRB2_TICRATE);
        }

        /// <summary>
        /// The internal game ID/name for this map.
        /// </summary>
        public string InternalName { get; }

        /// <summary>
        /// The display name of this map. May be UNKNOWN if it is a hidden/"Hell" map.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The MD5 hash for this map. Always 16 bytes long.
        /// </summary>
        public byte[] MD5 { get; }
        
        /// <summary>
        /// The internal act number of this map.
        /// </summary>
        public byte ActNumber { get; }

        /// <summary>
        /// If true, this map is internally listed as a Zone.
        /// </summary>
        public bool IsZone { get; }

        /// <summary>
        /// The amount of time elapsed since this map started play.
        /// </summary>
        public TimeSpan TimeElapsed { get; }

        /// <inheritdoc />
        public override string ToString()
            => Title;
    }
}