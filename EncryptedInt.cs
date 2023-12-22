﻿//Copyright 2023 Kostasel
//See license.txt for license details

using System;
using System.Threading;

namespace EncryptedTypes
{
    public struct EncryptedInt: IComparable<int>, IComparable<EncryptedInt>, IEquatable<EncryptedInt>, IEquatable<int>
    {
        [NonSerialized]
        private int EncryptionKey;
        [NonSerialized]
        private int EncryptedValue;

        private byte initialized;
         
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private EncryptedInt(ref int value)
        {
            EncryptionKey = CreateNewEncryptionKey();
            EncryptedValue = EncryptValue(ref value, ref EncryptionKey);
            initialized = 1;
        }
 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Encrypts the value with the given key.
        /// </summary>
        public static int EncryptValue(ref int value, ref int key)
        {
            return ProccessValue(ref value, ref key, 0);
        }
 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Decrypts the value with the given key.
        /// </summary>
        public static int DecryptValue(ref int value, ref int key)
        {
            return ProccessValue(ref value, ref key, 1);
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static int ProccessValue(ref int value, ref int key, byte mode)
        {
            //encrypt mode
            if (mode == 0)
            {
                int origkey = key ^ ((0x1505 << 33) + 0x1505);
                return (BitConverter.ToInt32(Transforms.TransformValueArray(value))) ^ origkey;
            }
            //decrypt mode
            else if (mode == 1)
            {
                int origkey = key ^ ((0x1505 << 33) + 0x1505);
                int decryptvalue = (value ^ origkey);
                return BitConverter.ToInt32(Transforms.InvertTransformValueArray(decryptvalue));
            }
            return 0;
        }
 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        /// <summary>
        /// Get a new key for encryption.
        /// </summary>
        private static int CreateNewEncryptionKey()
        {
            //Get some random bytes for the key
            int result = 0;
            Random.GetNew(1000, 10000,ref result);
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

                RandomData[i] = Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) i)));
                RandomData[i + 1] = Transforms.Transform((byte) (RandomData[i] ^ Transforms.Transform((byte) i)));
            }

            //Scramble the key before writing to variable
            return (BitConverter.ToInt32(RandomData.ToArray(), 0)) ^ ((0x1505 << 33) + 0x1505);
        }
 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public int GetDecryptedValue()
        {
            return DecryptedValue;
        }

        private int DecryptedValue
        {
            get { return DecryptValue(); }
        }
 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private int DecryptValue()
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

        public static implicit operator EncryptedInt(int value) => new(ref value);

        public static implicit operator int(EncryptedInt value)
        {
            return value.DecryptValue();
        }

        public static EncryptedInt operator ++(EncryptedInt input)
        {
            return IncreaseValue(input);
        }

        public static EncryptedInt operator --(EncryptedInt input)
        {
            return DecreaseValue(input);
        }

        public static EncryptedInt operator +(EncryptedInt input, int value)
        {
            return AddValue(input, value);
        }

        public static EncryptedInt operator -(EncryptedInt input, int value)
        {
            return SubValue(input, value);
        }

        public static bool operator <(EncryptedInt left, EncryptedInt right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedInt left, EncryptedInt right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedInt left, EncryptedInt right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedInt left, EncryptedInt right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(EncryptedInt left, int right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(EncryptedInt left, int right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(EncryptedInt left, int right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(EncryptedInt left, int right)
        {
            return left.CompareTo(right) >= 0;
        }


        public static bool operator ==(EncryptedInt left, EncryptedInt right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedInt left, EncryptedInt right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(EncryptedInt left, int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EncryptedInt left, int right)
        {
            return !left.Equals(right);
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedInt IncreaseValue(EncryptedInt value)
        {
            int tmp = value.DecryptValue();
            Interlocked.Increment(ref tmp);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedInt DecreaseValue(EncryptedInt value)
        {
            int tmp = value.DecryptValue();
            Interlocked.Decrement(ref tmp);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedInt AddValue(EncryptedInt value, int amount)
        {
            int tmp = value.DecryptValue();
            Interlocked.Add(ref tmp, amount);
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        private static EncryptedInt SubValue(EncryptedInt value, int amount)
        {
            int tmp = value.DecryptValue();
            Interlocked.Exchange(ref tmp, (tmp - amount));
            value.EncryptedValue = EncryptValue(ref tmp, ref value.EncryptionKey);

            return value;
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public override bool Equals(object obj)
        {
            return obj is EncryptedInt && Equals((EncryptedInt) obj);
        }

 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

        public bool Equals(EncryptedInt obj)
        {
            if (EncryptionKey == obj.EncryptionKey)
            {
                return EncryptedValue.Equals(obj.EncryptedValue);
            }

            return DecryptValue(ref EncryptedValue, ref EncryptionKey).Equals(DecryptValue(ref obj.EncryptedValue, ref obj.EncryptionKey));
        }

        public bool Equals(int other)
        {
            return DecryptedValue.Equals(other);
        }

        public int CompareTo(EncryptedInt other)
        {
            return DecryptValue().CompareTo(other.DecryptValue());
        }

        public int CompareTo(int other)
        {
            return DecryptValue().CompareTo(other);
        }

        public override int GetHashCode()
        {
            return DecryptValue().GetHashCode();
        }

        public override string ToString()
        {
            return DecryptedValue.ToString();
        }
    }
}