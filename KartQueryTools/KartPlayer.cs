using KartQueryTools.Packets;
using System;

namespace KartQueryTools
{
    /// <summary>
    /// Represents a player currently playing on a server.
    /// </summary>
    public sealed class KartPlayer
    {
        internal KartPlayer(plrinfo plr)
        {
            Node = plr.node;

            unsafe
            {
                Name = Utilities.Utils.DecodeString(plr.name, plrinfo.MAX_PLAYER_NAME_LENGTH);
            }

            Team = plr.team;
            Skin = plr.skin;
            Data = plr.data;
            Score = plr.score;
            TimeInServer = TimeSpan.FromSeconds(plr.timeinserver);
        }

        /// <summary>
        /// The node number of the player as returned by the <c>"nodes"</c> command in-game.
        /// </summary>
        public byte Node { get; }

        /// <summary>
        /// The in-game name the player has set.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The internal team the player is on.
        /// </summary>
        public byte Team { get; }

        /// <summary>
        /// The skin the player is using.
        /// </summary>
        public byte Skin { get; }

        /// <summary>
        /// Internal server-related data for the player.
        /// </summary>
        public byte Data { get; }

        /// <summary>
        /// The player's current in-game score.
        /// </summary>
        public uint Score { get; }

        /// <summary>
        /// The amount of time the player has been on the server.
        /// </summary>
        public TimeSpan TimeInServer { get; }

        /// <inheritdoc />
        public override string ToString()
            => Name;
    }
}
