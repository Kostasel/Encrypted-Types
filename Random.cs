//Copyright 2023 Kostasel
//See license.txt for license details

using System.Threading;

namespace EncryptedTypes
{
    internal readonly ref struct Random
    {
        private static readonly System.Random rnd  = new (System.DateTime.Now.Millisecond);

        /// <summary>
        /// Calculates a random number from 1 to 1000.
        /// </summary>
        /// <param name="result">A reference to a variable to write the result</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void GetNew(ref int result)
        {
            Interlocked.Exchange(ref result, rnd.Next(1, 1000));
        }

        /// <summary>
        /// Calculates a random number from min to max.
        /// </summary>
        /// <param name="min">A reference to a variable containing the lowest random number.</param>
        /// <param name="max">A reference to a variable containing the highest random number.</param>
        /// <param name="result">A reference to a variable to write the result</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void GetNew(ref int min, ref int max, ref int result)
        {
            Interlocked.Exchange(ref result, rnd.Next(min, max));
        }

        /// <summary>
        /// Calculates a random number from min to max.
        /// </summary>
        /// <param name="min">The lowest random number</param>
        /// <param name="max">The highest random number</param>
        /// <param name="result">A variable to write the result</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static void GetNew(int min, int max, ref int result)
        {
            Interlocked.Exchange(ref result, rnd.Next(min, max));
        }
    }
}
