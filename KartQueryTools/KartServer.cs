using KartQueryTools.Packets;
using KartQueryTools.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KartQueryTools
{
    /// <summary>
    /// Represents an individual SRB2Kart server.
    /// </summary>
    public sealed class KartServer
    {
        internal KartServer(serverinfo_pak srv, IEnumerable<plrinfo> players, IEnumerable<(string Filename, byte[] Md5)> neededFiles)
        {
            unsafe
            {
                Application = Utils.DecodeString(srv.application, serverinfo_pak.MAX_APPLICATION_LENGTH);
                Name = Utils.DecodeString(srv.servername, serverinfo_pak.MAX_SERVER_NAME_LENGTH);
                HttpSource = Utils.DecodeString(srv.httpsource, serverinfo_pak.MAX_MIRROR_LENGTH);

                if (string.IsNullOrWhiteSpace(HttpSource))
                    HttpSource = null; // Just in case the query returns empty spaces
            }

            MajorVersion = srv.version;
            MinorVersion = srv.subversion;
            CurrentPlayerCount = srv.numberofplayer;
            MaxPlayerCount = srv.maxplayer;
            GameType = srv.gametype;
            ModifiedGame = srv.modifiedgame == 0 || srv.modifiedgame == 1
                ? srv.modifiedgame == 1
                : throw new ArgumentOutOfRangeException(nameof(srv.modifiedgame));
            CheatsEnabled = srv.cheatsenabled == 0 || srv.cheatsenabled == 1
                ? srv.cheatsenabled == 1
                : throw new ArgumentOutOfRangeException(nameof(srv.cheatsenabled));
            KartVars = srv.kartvars;
            NumberOfFilesNeeded = srv.fileneedednum;
            CurrentMap = new KartMap(srv);
            CurrentPlayers = players.Where(x => x.node != 255).Select(x => new KartPlayer(x)).ToImmutableArray();
            NeededFiles = neededFiles.ToImmutableArray();
        }

        /// <summary>
        /// The application of this server. Will most likely be SRB2Kart, or possibly SRB2 if querying a vanilla SRB2 server.
        /// </summary>
        public string Application { get; }

        /// <summary>
        /// The major version number of this server. For SRB2Kart version 1.2, this would be 1.
        /// </summary>
        public byte MajorVersion { get; }

        /// <summary>
        /// The minor version number of this server. For SRB2Kart version 1.2, this would be 2.
        /// </summary>
        public byte MinorVersion { get; }

        /// <summary>
        /// The current number of players on this server, as returned by the server query.
        /// </summary>
        public byte CurrentPlayerCount { get; }

        /// <summary>
        /// The maximum number of players allowed on this server at a time.
        /// </summary>
        public byte MaxPlayerCount { get; }

        /// <summary>
        /// A collection of the current players on this server.
        /// </summary>
        public ImmutableArray<KartPlayer> CurrentPlayers { get; }

        /// <summary>
        /// The internal game type of this server.
        /// </summary>
        public byte GameType { get; }

        /// <summary>
        /// If true, this server is using addons/is modded.
        /// </summary>
        public bool ModifiedGame { get; }

        /// <summary>
        /// If true, this server has one or more cheats enabled.
        /// </summary>
        public bool CheatsEnabled { get; }

        /// <summary>
        /// Internal Kart related variables for this server.
        /// </summary>
        public byte KartVars { get; }

        /// <summary>
        /// The total number of files needed to connect to this server.
        /// </summary>
        public byte NumberOfFilesNeeded { get; }

        /// <summary>
        /// The display name of this server, without any color formatting codes.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The map currently being played on this server.
        /// </summary>
        public KartMap CurrentMap { get; }

        /// <summary>
        /// The <c>"http_source"</c> value on this server (if set), otherwise <see langword="null"/>.
        /// </summary>
        public string HttpSource { get; }
        
        /// <summary>
        /// A collection of file names and MD5s needed to join this server.
        /// </summary>
        public ImmutableArray<(string Filename, byte[] Md5)> NeededFiles { get; }

        /// <inheritdoc />
        public override string ToString()
            => Name;
    }
}