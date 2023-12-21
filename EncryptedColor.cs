//Copyright 2023 Kostasel
//See license.txt for license details

using System;
using UnityEngine;

namespace EncryptedTypes
{
    public struct EncryptedColor : IEquatable<EncryptedColor>, IEquatable<Color>
    {
        [System.NonSerialized]
        private int EncryptionKey;
        [System.NonSerialized]
        private int[] EncryptedValue;

        private byte initialized;

        private EncryptedColor(ref Color value)
        {
            EncryptionKey = GetNewEncryptionKey();
            EncryptedValue = EncryptValue(value, EncryptionKey);
            initialized = 1;
        }

        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        private static int[] EncryptValue(Color value, int key)
        {
            return ComputeEncryptedValue(ref value, ref key, 0);
        }

        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        private static Color DecryptValue(int[] value, int key)
        {
            return ComputeDecryptedValue(ref value, ref key, 1);
        }

        private static int[] ComputeEncryptedValue(ref Color value, ref int key, byte mode = 0)
        {
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            Span<int> data = stackalloc int[4];
            data[0] = Transforms.TransformValue(BitConverter.SingleToInt32Bits(value.r));
            data[0] = data[0] ^ origkey;
            data[1] = Transforms.TransformValue(BitConverter.SingleToInt32Bits(value.g));
            data[1] = data[1] ^ origkey;
            data[2] = Transforms.TransformValue(BitConverter.SingleToInt32Bits(value.b));
            data[2] = data[2] ^ origkey;
            data[3] = Transforms.TransformValue(BitConverter.SingleToInt32Bits(value.a));
            data[3] = data[3] ^ origkey;
            return data.ToArray();
        }

        private static Color ComputeDecryptedValue(ref int[] value, ref int key, byte mode = 1)
        {
            int origkey = key ^ ((0x1505 << 33) + 0x1505);
            Span<float> result = stackalloc float[4];
            int decrypted;
            for (int i = 0; i < 4; i++)
            {
                decrypted = (value[i] ^ origkey);
                result[i] = BitConverter.Int32BitsToSingle(Transforms.InvertTransformValue(decrypted));
            }
            return new Color(result[0], result[1], result[2], result[3]);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Get a new key for encryption.
        /// </summary>
        private static int GetNewEncryptionKey()
        {
            //Get some random bytes for the key.
            int result = 0;
            Random.GetNew(1000, 10000, ref result);
            Span<byte> RandomData = Transforms.TransformValueArray(result);
            byte random = 33;
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = (byte) (((random << 5) + random) + RandomData[i]);
                RandomData[i + 1] = (byte) (((random << 5) + random) + RandomData[i + 1]);
            }
            //Shuffle the key bytes
            for (int i = 0; i < 4; i += 2)
            {
                RandomData[i] = (Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) i))));
                RandomData[i + 1] = (Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) (i + 1)))));
            }

            //Scramble the key before writing to variable
            return (BitConverter.ToInt32(RandomData.ToArray(), 0)) ^ ((0x1505 << 33) + 0x1505);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Color DecryptValue()
        {
            if (initialized == 0)
            {
                EncryptionKey = GetNewEncryptionKey();
                EncryptedValue = EncryptValue(Color.white, EncryptionKey);
                initialized = 1;

                return Color.white;
            }
            return DecryptValue(EncryptedValue, EncryptionKey);
        }

        public static implicit operator EncryptedColor(Color value) => new(ref value);

        public static implicit operator Color(EncryptedColor value)
        {
            return value.DecryptValue();
        }

        public static bool operator ==(EncryptedColor left, EncryptedColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedColor left, EncryptedColor right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(EncryptedColor left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedColor left, Color right)
        {
            return !left.Equals(right);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is EncryptedColor && Equals((EncryptedColor) obj);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(EncryptedColor obj)
        {
            if (EncryptionKey == obj.EncryptionKey)
            {
                return EncryptedValue.Equals(obj.EncryptedValue);
            }

            return DecryptValue(EncryptedValue, EncryptionKey).Equals(DecryptValue(obj.EncryptedValue, obj.EncryptionKey));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Equals(Color other)
        {
            return DecryptValue().Equals(other);
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