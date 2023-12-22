//Copyright 2023 Kostasel
//See license.txt for license details

using System;

namespace EncryptedTypes
{
    public struct EncryptedUInt : IComparable<uint>, IComparable<EncryptedUInt>, IEquatable<EncryptedUInt>, IEquatable<uint>
    {
        [System.NonSerialized]
        private uint EncryptionKey;
        [System.NonSerialized]
        private uint EncryptedValue;

        private byte initialized;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EncryptedUInt(ref uint value)
        {
            EncryptionKey = CreateNewEncryptionKey();
            EncryptedValue = EncryptValue(ref value, ref EncryptionKey);
            initialized = 1;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        public static uint EncryptValue(ref uint value, ref uint key)
        {
            return ProccessValue(ref value, ref key, 0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        public static uint DecryptValue(ref uint value, ref uint key)
        {
            return ProccessValue(ref value, ref key, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static uint ProccessValue(ref uint value, ref uint key, byte mode)
        {
            uint origkey = key ^ ((0x1505 << 33) + 0x1505);
            uint decryptvalue;
            //encrypt
            if (mode == 0)
            {
                return (BitConverter.ToUInt32(Transforms.TransformValueArray(value))) ^ origkey;
            }
            //decrypt
            else if (mode == 1)
            {
                decryptvalue = (value ^ origkey);
                return BitConverter.ToUInt32(Transforms.InvertTransformValueArray(decryptvalue));
            }
            return 0;
        }

        /// <summary>
        /// Get a new key for encryption.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static uint CreateNewEncryptionKey()
        {
            //Get some random bytes for the key
            int result = 0;
            Random.GetNew(1000, 10000, ref result);
            byte[] RandomData = Transforms.TransformValueArray((uint) result).ToArray();
            byte a = 33;
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = (byte) (((a << 5) + a) + RandomData[i]);
                RandomData[i + 1] = (byte) (((a << 5) + a) + RandomData[i + 1]);
            }
            //Shuffle the key bytes
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = (Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) i))));
                RandomData[i + 1] = (Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) (i + 1)))));
            }

            //Scramble the key before writing to variable
            return (BitConverter.ToUInt32(RandomData, 0)) ^ ((0x1505 << 33) + 0x1505);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private uint DecryptValue()
        {
            if (initialized == 0)
            {
                EncryptionKey = 0;
                EncryptedValue = 0;
                initialized = 1;

                return 0;
            }
            return ProccessValue(ref EncryptedValue, ref EncryptionKey, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator EncryptedUInt(uint value) => new(ref value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(EncryptedUInt value)
        {
            return value.DecryptValue();
        }

        public static EncryptedUInt operator ++(EncryptedUInt input)
        {
            return IncreaseValue(input);
        }

        public static EncryptedUInt operator --(EncryptedUInt input)
        {
            return DecreaseValue(input);
        }

        public static EncryptedUInt operator +(EncryptedUInt input, uint value)
        {
            return AddValue(input, ref value);
        }

        public static EncryptedUInt operator -(EncryptedUInt input, uint value)
        {
            return SubValue(input, ref value);
        }

        public static EncryptedUInt operator +(EncryptedUInt input, int value)
        {
            return AddValue(input, ref value);
        }

        public static EncryptedUInt operator -(EncryptedUInt input, int value)
        {
            return SubValue(input, ref value);
        }
        public static bool operator <(EncryptedUInt left, EncryptedUInt right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedUInt left, EncryptedUInt right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedUInt left, EncryptedUInt right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedUInt left, EncryptedUInt right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(EncryptedUInt left, uint right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedUInt left, uint right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedUInt left, uint right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedUInt left, uint right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator ==(EncryptedUInt left, EncryptedUInt right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedUInt left, EncryptedUInt right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(EncryptedUInt left, uint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedUInt left, uint right)
        {
            return !left.Equals(right);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt IncreaseValue(EncryptedUInt value)
        {
            uint tmp = value.DecryptValue() + 1;
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt DecreaseValue(EncryptedUInt value)
        {
            uint tmp = value.DecryptValue() - 1;
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt AddValue(EncryptedUInt value, ref uint amount)
        {
            uint tmp = value.DecryptValue() + amount;
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt SubValue(EncryptedUInt value, ref uint amount)
        {
            uint tmp = value.DecryptValue() - amount;
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt AddValue(EncryptedUInt value, ref int amount)
        {
            uint tmp = (uint) (value.DecryptValue() + amount);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static EncryptedUInt SubValue(EncryptedUInt value, ref int amount)
        {
            uint tmp = (uint) (value.DecryptValue() - amount);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is EncryptedUInt && Equals((EncryptedUInt) obj);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(EncryptedUInt obj)
        {
            if (EncryptionKey == obj.EncryptionKey)
            {
                return EncryptedValue.Equals(obj.EncryptedValue);
            }

            return DecryptValue(ref EncryptedValue, ref EncryptionKey).Equals(DecryptValue(ref obj.EncryptedValue, ref obj.EncryptionKey));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(uint other)
        {
            return DecryptValue().Equals(other);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(EncryptedUInt other)
        {
            return DecryptValue().CompareTo(other.DecryptValue());
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CompareTo(uint other)
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