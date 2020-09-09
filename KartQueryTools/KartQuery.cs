using KartQueryTools.Packets;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using static KartQueryTools.Packets.packettype_t;

namespace KartQueryTools
{
    /// <summary>
    /// Provides methods to query individual SRB2Kart servers, as well as the HTTP Master Server.
    /// </summary>
    public static class KartQuery
    {
        private const int DUMMY_CHECKSUM_START = 0x1234567;

        /// <summary>
        /// The default port SRB2Kart and SRB2 use for hosting.
        /// </summary>
        public const int DEFAULT_SRB2KART_PORT = 5029;

        /// <summary>
        /// The url the server list is to be fetched from.
        /// </summary>
        public const string HTTP_MASTER_SERVER_LIST = "https://mb.srb2.org/MS/0/servers";

        private static readonly HttpClient Http = new HttpClient();

        /// <summary>
        /// Gets or sets how long a query should wait before timing out waiting for a response, in milliseconds.
        /// <para>Defaults to 10,000 (10 seconds).</para>
        /// </summary>
        public static uint QueryTimeout { get; set; } = 10_000;

        /// <summary>
        /// Attempts to query an SRB2Kart server for information.
        /// </summary>
        /// <param name="hostname">The hostname or IP address of the server.</param>
        /// <param name="port">The port the server is running on.</param>
        /// <returns>A <see cref="KartServer"/> object representing server query information.</returns>
        public static KartServer QueryServer(string hostname, int port = DEFAULT_SRB2KART_PORT)
        {
            if (IPAddress.TryParse(hostname, out var address))
                return QueryServer(new IPEndPoint(address, port));

            var dnsEntries = Dns.GetHostAddresses(hostname);
            return QueryServer(new IPEndPoint(dnsEntries[0], port));
        }

        /// <summary>
        /// Attempts to query an SRB2Kart server for information.
        /// </summary>
        /// <param name="address">The IP address of the server.</param>
        /// <param name="port">The port the server is running on.</param>
        /// <returns>A <see cref="KartServer"/> object representing server query information.</returns>
        public static KartServer QueryServer(IPAddress address, int port = DEFAULT_SRB2KART_PORT)
            => QueryServer(new IPEndPoint(address, port));

        /// <summary>
        /// Attempts to query an SRB2Kart server for information.
        /// </summary>
        /// <param name="endpoint">The endpoint (IP address and port) of the server.</param>
        /// <returns>A <see cref="KartServer"/> object representing server query information.</returns>
        public static KartServer QueryServer(IPEndPoint endpoint)
        {
            using var client = new UdpClient(endpoint.Port) {Client = {ReceiveTimeout = (int) QueryTimeout}};

            client.Connect(endpoint);

            // ask for server info
            SendPacket(client, (byte) PT_ASKINFO, new byte[] { 0, 0, 0, 0, 0 });
            var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

            var start = 0u;
            var morePackets = true;
            var srv = new serverinfo_pak();
            var players = new List<plrinfo>();
            var neededFiles = new List<(string Filename, byte[] Md5)>();

            while (morePackets)
            {
                var received = client.Receive(ref remoteEndpoint);
                var stream = new MemoryStream(received);
                var header = ReadPacketComponent<doomdata_t>(stream);

                var type = (packettype_t) header.packettype;

                var timeout = 0;

                switch (type)
                {
                    case PT_SERVERINFO: // 13
                        srv = ReadPacketComponent<serverinfo_pak>(stream);
                        break;
                    case PT_PLAYERINFO: // 14
                        for (var i = 0; i < 16; i++)
                        {
                            players.Add(ReadPacketComponent<plrinfo>(stream));
                        }

                        SendPacket(client, (byte) PT_TELLFILESNEEDED, BitConverter.GetBytes(start));

                        break;
                    case PT_MOREFILESNEEDED:
                        var filesNeeded = ReadPacketComponent<filesneededconfig_pak>(stream);
                        start += filesNeeded.num;

                        neededFiles.AddRange(GetFilesNeeded(received));
                        if (filesNeeded.more == 1)
                        {
                            // ask for more files
                            SendPacket(client, (byte) PT_TELLFILESNEEDED, BitConverter.GetBytes(start));
                        }
                        else
                        {
                            morePackets = false;
                        }
                        break;
                    default:
                        if (++timeout == 5) // 5 invalid packets in a row
                            throw new Exception("Received 5 invalid response packets in a row.");

                        break;
                }
            }

            return new KartServer(srv, players, neededFiles);
        }

        /// <summary>
        /// Fetches all servers via the HTTP Master Server.
        /// </summary>
        /// <returns>A collection of server entries. May not ALL be SRB2Kart servers, check their <see cref="KartServerListEntry.GameVersion"/> property.</returns>
        public static ImmutableArray<KartServerListEntry> QueryMasterServer()
        {
            // ew, sync
            var response = Http.GetStringAsync(HTTP_MASTER_SERVER_LIST).GetAwaiter().GetResult();
            var reader = new StringReader(response);
            var servers = new List<KartServerListEntry>();

            var currentRoom = 0;
            var currentLine = reader.ReadLine();

            do
            {
                if (string.IsNullOrWhiteSpace(currentLine))
                {
                    // empty line between groups of servers/rooms
                    continue;
                }

                if (int.TryParse(currentLine, out var nextRoom))
                {
                    currentRoom = nextRoom;
                    continue;
                }

                var split = currentLine.Split(' ');
                if (split.Length != 4)
                    throw new Exception($"{currentLine} was supposed to have 4 arguments but has {split.Length}.");

                // Format is: ADDR PORT NAME VER
                servers.Add(new KartServerListEntry(new IPEndPoint(IPAddress.Parse(split[0]), int.Parse(split[1])),
                    split[2], split[3], currentRoom));

            } while ((currentLine = reader.ReadLine()) != null);

            return servers.ToImmutableArray();
        }

        // Credit to Tyron for the original method on Hyuuseeker
        private static T ReadPacketComponent<T>(Stream stream)
            where T : unmanaged
        {
            var size = Marshal.SizeOf(typeof(T));
            Span<byte> buffer = stackalloc byte[size];
            stream.Read(buffer);
            return MemoryMarshal.Cast<byte, T>(buffer)[0];
        }

        // Credit to Tyron for the original method on Hyuuseeker
        private static void SendPacket(UdpClient client, byte type, byte[] payload)
        {
            byte[] header = {0, 0, type, 0};
            var contents = header.Concat(payload).ToArray();
            var bytesToSend = BitConverter.GetBytes(CalculateChecksum(contents)).Concat(contents).ToArray();
            client.Send(bytesToSend, bytesToSend.Length);
        }

        // Credit to Tyron for the original method on Hyuuseeker
        private static int CalculateChecksum(Span<byte> data)
        {
            var c = DUMMY_CHECKSUM_START;
            for (var i = 0; i < data.Length; i++)
                c += data[i] * (i + 1);
            return c;
        }

        // Credit to Tyron for the original method on Hyuuseeker
        private static List<(string Filename, byte[] Md5)> GetFilesNeeded(byte[] packet)
        {
            var filesNeeded = new List<(string Filename, byte[] Md5)>();

            var reader = new BinaryReader(new MemoryStream(packet));

            // Skips the header (8 bytes) and an extra 4 + 2 bytes?
            // Dude I'm just the messenger don't shoot me I don't understand how this works
            reader.ReadBytes(14); 

            while (reader.PeekChar() != -1)
            {
                // Skips the status (1 byte) and size (4 bytes)
                reader.ReadBytes(5);

                var builder = new StringBuilder();

                char currentChar;
                while ((currentChar = reader.ReadChar()) != '\0')
                {
                    builder.Append(currentChar);
                }

                var md5 = reader.ReadBytes(16);
                filesNeeded.Add((builder.ToString(), md5));
                builder.Clear();
            }

            return filesNeeded;
        }
    }
}
