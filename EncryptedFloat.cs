//Copyright 2023 Kostasel
//See license.txt for license details

using System;
using System.Threading;

namespace EncryptedTypes
{
    public struct EncryptedFloat : IComparable<float>, IComparable<EncryptedFloat>, IEquatable<EncryptedFloat>, IEquatable<float>
    {
        [System.NonSerialized]
        private int EncryptionKey;
        [System.NonSerialized]
        private int EncryptedValue;
        private byte initialized;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EncryptedFloat(ref float value)
        {
            EncryptionKey = CreateNewEncryptionKey();
            EncryptedValue = EncryptValue(value, EncryptionKey);
            initialized = 1;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        private static int EncryptValue(float value, int key)
        {
            return ComputeValue(ref value, ref key, 0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        private static int EncryptValue(ref float value, ref int key)
        {
            return ComputeValue(ref value, ref key, 0);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        public static float DecryptValue(int value, int key)
        {
            return ComputeValue(ref value, ref key, 1);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static int ComputeValue(ref float value, ref int key, byte mode)
        {
            //encrypt
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            int data = STransform(ref value);
            return (Transforms.TransformValue(data) ^ origkey);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static float ComputeValue(ref int value, ref int key, byte mode)
        {
            //decrypt
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            int decryptvalue = (value ^ origkey);
            return InvertSTransform(Transforms.InvertTransformValue(decryptvalue));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static int STransform(ref float value)
        {
            return BitConverter.SingleToInt32Bits(value);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static float InvertSTransform(int value)
        {
            return InvertSTransform(ref value);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static float InvertSTransform(ref int value)
        {
            return BitConverter.Int32BitsToSingle(value);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Span<byte> TransformData(int value)
        {
            Span<byte> data = BitConverter.GetBytes(value).AsSpan();
            for (int i = 0; i < 4; i += 2)
            {
                data[i] = Transforms.TransformByte(ref data[i]);
                data[i + 1] = Transforms.TransformByte(ref data[i + 1]);
            };
            return data;
        }
        /// <summary>
        /// Get a new key for encryption.
        /// </summary>
        private static int CreateNewEncryptionKey()
        {
            //Get some random bytes for the key
            int result = 0;
            Random.GetNew(1000, 10000, ref result);
            Span<byte> RandomData = TransformData(result);
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

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public float DecryptValue()
        {
            if (initialized == 0)
            {
                EncryptionKey = CreateNewEncryptionKey();
                EncryptedValue = EncryptValue(0, EncryptionKey);
                initialized = 1;

                return 0;
            }
            return ComputeValue(ref EncryptedValue, ref EncryptionKey, 1);
        }

        public static implicit operator EncryptedFloat(float value) => new(ref value);

        public static implicit operator float(EncryptedFloat value)
        {
            return value.DecryptValue();
        }

        public static EncryptedFloat operator ++(EncryptedFloat input)
        {
            return IncreaseValue(input);
        }

        public static EncryptedFloat operator --(EncryptedFloat input)
        {
            return DecreaseValue(input);
        }

        public static EncryptedFloat operator +(EncryptedFloat input, float value)
        {
            return AddValue(input, value);
        }

        public static EncryptedFloat operator -(EncryptedFloat input, float value)
        {
            return SubValue(input, value);
        }

        public static EncryptedFloat operator +(EncryptedFloat input, int value)
        {
            return AddValue(input, value);
        }

        public static EncryptedFloat operator -(EncryptedFloat input, int value)
        {
            return SubValue(input, value);
        }

        public static bool operator <(EncryptedFloat left, EncryptedFloat right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedFloat left, EncryptedFloat right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedFloat left, EncryptedFloat right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedFloat left, EncryptedFloat right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(EncryptedFloat left, float right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedFloat left, float right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedFloat left, float right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedFloat left, float right)
        {
            return left.CompareTo(right) >= 0;
        }


        public static bool operator ==(EncryptedFloat left, EncryptedFloat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedFloat left, EncryptedFloat right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(EncryptedFloat left, float right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedFloat left, float right)
        {
            return !left.Equals(right);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat IncreaseValue(EncryptedFloat value)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, tmp++);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat DecreaseValue(EncryptedFloat value)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, tmp--);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat AddValue(EncryptedFloat value, float amount)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp + amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat SubValue(EncryptedFloat value, float amount)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp - amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat AddValue(EncryptedFloat value, int amount)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp + amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedFloat SubValue(EncryptedFloat value, int amount)
        {
            float tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp - amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public override bool Equals(object obj)
        {
            return obj is EncryptedFloat && Equals((EncryptedFloat) obj);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public bool Equals(EncryptedFloat obj)
        {
            if (EncryptionKey == obj.EncryptionKey)
            {
                return EncryptedValue.Equals(obj.EncryptedValue);
            }

            return DecryptValue(EncryptedValue, EncryptionKey).Equals(DecryptValue(obj.EncryptedValue, obj.EncryptionKey));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public bool Equals(float other)
        {
            return DecryptValue().Equals(other);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public int CompareTo(EncryptedFloat other)
        {
            return DecryptValue().CompareTo(other.DecryptValue());
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public int CompareTo(float other)
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