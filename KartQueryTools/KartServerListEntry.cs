using System.Net;

namespace KartQueryTools
{
    /// <summary>
    /// Represents a server listed on the HTTP Master Server.
    /// </summary>
    public sealed class KartServerListEntry
    {
        internal KartServerListEntry(IPEndPoint endpoint, string contact)
        {
            Endpoint = endpoint;
            Contact = contact;
        }

        /// <summary>
        /// The address of this server.
        /// </summary>
        public IPEndPoint Endpoint { get; }

        /// <summary>
        /// If set, the contact information for this server's owner(s).
        /// </summary>
        public string Contact { get; }
    }
}