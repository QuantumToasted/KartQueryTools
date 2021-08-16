using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KartQueryTools
{
    /// <summary>
    /// KartQuery-related utilities.
    /// </summary>
    public static class KartUtils
    {
        static KartUtils()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        
        /// <summary>
        /// The number of game tics that occur per second in SRB2's engine, effectively the base unit of time.
        /// </summary>
        public const int SRB2_TICRATE = 35; // Tics per second

        // Credit to Tyron for the original method on Hyuuseeker
        internal static unsafe string DecodeString(byte* bytes, int fullLength)
        {
            var span = new Span<byte>(bytes, fullLength);
            var nullIndex = span.IndexOf((byte) 0);
            var relevant = span.Slice(0, nullIndex == -1 ? fullLength : nullIndex);

            fixed (byte* pointer = &MemoryMarshal.GetReference(relevant))
                return Encoding.GetEncoding(1252).GetString(pointer, relevant.Length);
        }
        
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> into a number of SRB2 tics representing it, rounded down.
        /// </summary>
        /// <param name="ts">A duration of time.</param>
        /// <returns>The number of SRB2 tics that would have elapsed in that time.</returns>
        public static int ToTics(this TimeSpan ts)
            => (int) ts.TotalSeconds * SRB2_TICRATE; // Floor the result. Who cares if we're off by one?
    }
}