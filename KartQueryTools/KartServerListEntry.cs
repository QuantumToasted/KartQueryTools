using System.Net;

namespace KartQueryTools
{
    /// <summary>
    /// Represents a server listed on the HTTP Master Server.
    /// </summary>
    public sealed class KartServerListEntry
    {
        internal KartServerListEntry(IPEndPoint endpoint, string name, string gameVersion, int room)
        {
            Endpoint = endpoint;
            Name = name;
            GameVersion = gameVersion;
            Room = room;
        }

        /// <summary>
        /// The address of this server.
        /// </summary>
        public IPEndPoint Endpoint { get; }

        /// <summary>
        /// The raw display name of this server. URL-encoded, and contains color escape codes.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The version of the game this server is running. May vary depending on the game or mod.
        /// </summary>
        public string GameVersion { get; }

        /// <summary>
        /// The room number this server is in.
        /// </summary>
        public int Room { get; }

        /// <inheritdoc />
        public override string ToString()
            => Name;
    }
}