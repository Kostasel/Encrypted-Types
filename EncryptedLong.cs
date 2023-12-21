//Copyright 2023 Kostasel
//See license.txt for license details

using System;
using System.Threading;


namespace EncryptedTypes
{
    public struct EncryptedLong : IComparable<long>, IComparable<EncryptedLong>, IEquatable<EncryptedLong>, IEquatable<long>
    {
        [System.NonSerialized]
        private int EncryptionKey;
        [System.NonSerialized]
        private long EncryptedValue;
        private byte initialized;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EncryptedLong(ref long value)
        {
            EncryptionKey = CreateNewEncryptionKey();
            EncryptedValue = EncryptValue(ref value, ref EncryptionKey);
            initialized = 1;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        internal static long EncryptValue(long value, int key)
        {
            return ComputeEncryptedValue(ref value, ref key, 0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        internal static long DecryptValue(long value, int key)
        {
            return ComputeDecryptedValue(ref value, ref key, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        private static long EncryptValue(ref long value, ref int key)
        {
            return ComputeEncryptedValue(ref value, ref key, 0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        private static long DecryptValue(ref long value, ref int key)
        {
            return ComputeDecryptedValue(ref value, ref key, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static long ComputeEncryptedValue(ref long value, ref int key, byte mode)
        {
            //encrypt
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            return (Transforms.TransformValue(value)) ^ origkey;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static long ComputeDecryptedValue(ref long value, ref int key, byte mode)
        {
            //decrypt
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            long decryptvalue = (value ^ origkey);
            return Convert.ToInt64(Transforms.InvertTransformValue(decryptvalue));
        }

        /// <summary>
        /// Get a new key for encryption.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static int CreateNewEncryptionKey()
        {
            //Get some random bytes for the key
            int result = 0;
            Random.GetNew(1000, 10000, ref result);
            Span<byte> RandomData = Transforms.TransformValueArray(result);
            byte a = 33;
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = (byte) (((a << 5) + a) + RandomData[i]);
                RandomData[i + 1] = (byte) (((a << 5) + a) + RandomData[i + 1]);
            }
            //Shuffle the key bytes
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) (i))));
                RandomData[i + 1] = Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) (i + 1))));
            }

            //Scramble the key before writing to variable
            return (BitConverter.ToInt32(RandomData.ToArray(), 0)) ^ ((0x1505 << 33) + 0x1505);
        }

        /// <summary>
        /// Decrypt the value encrypted.
        /// </summary>
        /// <returns>The decrypted value.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long DecryptValue()
        {
            if (initialized == 0)
            {
                EncryptionKey = CreateNewEncryptionKey();
                EncryptedValue = EncryptValue(0, EncryptionKey);
                initialized = 1;

                return 0;
            }
            return ComputeDecryptedValue(ref EncryptedValue, ref EncryptionKey, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator EncryptedLong(long value) => new(ref value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(EncryptedLong value)
        {
            return value.DecryptValue();
        }

        public static EncryptedLong operator ++(EncryptedLong input)
        {
            return IncreaseValue(input);
        }

        public static EncryptedLong operator --(EncryptedLong input)
        {
            return DecreaseValue(input);
        }

        public static EncryptedLong operator +(EncryptedLong input, long value)
        {
            return AddValue(input, value);
        }

        public static EncryptedLong operator -(EncryptedLong input, long value)
        {
            return SubValue(input, value);
        }

        public static EncryptedLong operator +(EncryptedLong input, int value)
        {
            return AddValue(input, value);
        }

        public static EncryptedLong operator -(EncryptedLong input, int value)
        {
            return SubValue(input, value);
        }

        public static bool operator <(EncryptedLong left, EncryptedLong right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedLong left, EncryptedLong right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedLong left, EncryptedLong right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedLong left, EncryptedLong right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(EncryptedLong left, long right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedLong left, long right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedLong left, long right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedLong left, long right)
        {
            return left.CompareTo(right) >= 0;
        }


        public static bool operator ==(EncryptedLong left, EncryptedLong right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedLong left, EncryptedLong right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(EncryptedLong left, long right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedLong left, long right)
        {
            return !left.Equals(right);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong IncreaseValue(EncryptedLong value)
        {
            long tmp = value.DecryptValue();
            Interlocked.Increment(ref tmp);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong DecreaseValue(EncryptedLong value)
        {
            long tmp = value.DecryptValue();
            Interlocked.Decrement(ref tmp);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong AddValue(EncryptedLong value, long amount)
        {
            long tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp + amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong SubValue(EncryptedLong value, long amount)
        {
            long tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp - amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong AddValue(EncryptedLong value, int amount)
        {
            long tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp + amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedLong SubValue(EncryptedLong value, int amount)
        {
            long tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp - amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is EncryptedLong && Equals((EncryptedLong) obj);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(long other)
        {
            return DecryptValue().Equals(other);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(EncryptedLong obj)
        {
            if (EncryptionKey == obj.EncryptionKey)
            {
                return EncryptedValue.Equals(obj.EncryptedValue);
            }

            return DecryptValue(ref EncryptedValue, ref EncryptionKey).Equals(DecryptValue(ref obj.EncryptedValue, ref obj.EncryptionKey));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(EncryptedLong other)
        {
            return DecryptValue().CompareTo(other.DecryptValue());
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(long other)
        {
            return DecryptValue().CompareTo(other);
        }

        public override int GetHashCode()
        {
            return DecryptValue().GetHashCode();
        }

        public override string ToString()
        {
            return DecryptValue().ToString();
        }
    }
}