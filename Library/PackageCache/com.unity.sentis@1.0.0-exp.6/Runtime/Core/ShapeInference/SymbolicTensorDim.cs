using System;
using UnityEngine;

namespace Unity.Sentis
{
    /// <summary>
    /// Types of `SymbolicTensorShape` dimension.
    /// </summary>
    [Serializable]
    public enum DimType
    {
        /// <summary>
        /// The tensor dimension is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The tensor dimension is fixed.
        /// </summary>
        Value,

        /// <summary>
        /// The tensor dimension is dynamic.
        /// </summary>
        Param
    }

    /// <summary>
    /// Represents a single dimension of a `SymbolicTensorShape`.
    /// </summary>
    [Serializable]
    public struct SymbolicTensorDim
    {
        DimType m_DimType;
        char m_Param;
        int m_Value;

        internal static SymbolicTensorDim Unknown => new SymbolicTensorDim();

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorDim` of fixed type, with an integer value.
        /// </summary>
        public SymbolicTensorDim(int value)
        {
            Logger.AssertIsTrue(value >= 0, "Dim value cannot be negative");
            m_DimType = DimType.Value;
            m_Param = default;
            m_Value = value;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorDim` of dynamic type, with a character value. The character value maps to a string in the `Model` class.
        /// </summary>
        public SymbolicTensorDim(char param)
        {
            Logger.AssertIsTrue(param >= 0, "Dim param cannot be negative");
            m_DimType = DimType.Param;
            m_Param = param;
            m_Value = default;
        }

        internal bool isUnknown => m_DimType == DimType.Unknown;

        /// <summary>
        /// Whether the dimension is fixed. If the value is `true`, you can use `.value` to return the value.
        /// </summary>
        public bool isValue => m_DimType == DimType.Value;

        /// <summary>
        /// Whether the dimension is dynamic. If the value is `true`, you can use `.param` to return the value as a character.
        /// </summary>
        public bool isParam => m_DimType == DimType.Param;

        internal static SymbolicTensorDim Zero => new SymbolicTensorDim(0);
        internal static SymbolicTensorDim One => new SymbolicTensorDim(1);

        /// <summary>
        /// The value of the dimension. You can only call this method if `.isValue` is true.
        /// </summary>
        public int value
        {
            get
            {
                Logger.AssertIsTrue(m_DimType == DimType.Value, "Cannot get value of dim with type != DimType.Value");
                return m_Value;
            }
        }

        /// <summary>
        /// The value of the dimension. You can only call this method if `.isParam` is true.
        /// </summary>
        public char param
        {
            get
            {
                Logger.AssertIsTrue(m_DimType == DimType.Param, "Cannot get param of dim with type != DimType.Param");
                return m_Param;
            }
        }

        /// <summary>
        /// Returns a string that represents the `SymbolicTensorDim`.
        /// </summary>
        public override string ToString()
        {
            return m_DimType switch
            {
                DimType.Unknown => "?",
                DimType.Value => value.ToString(),
                DimType.Param => param.ToString(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        internal bool Equals(SymbolicTensorDim other)
        {
            return m_DimType == other.m_DimType && m_Value == other.m_Value && m_Param == other.m_Param;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current `SymbolicTensorDim`.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is SymbolicTensorDim other && Equals(other);
        }

        public static bool operator ==(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.m_DimType != b.m_DimType)
                return false;
            if (a.m_Value != b.m_Value)
                return false;
            if (a.m_Param != b.m_Param)
                return false;
            return true;
        }

        public static bool operator !=(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Whether symbolic dims a and b could be referring to the same underlying value
        /// </summary>
        internal static bool IsCompatible(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            return !a.isValue || !b.isValue || a == b;
        }

        /// <summary>
        /// Adds two `SymbolicTensorDim` dimensions.
        /// </summary>
        public static SymbolicTensorDim operator +(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.isValue)
                return a.value + b;
            if (b.isValue)
                return b.value + a;
            return Unknown;
        }

        ///   | 0   1   A   ?
        /// --|-----------------
        /// 0 | 0   4   A   ?
        /// 3 | 3   4   ?   ?
        /// <summary>
        /// Adds a `SymbolicTensorDim` to an `int`.
        /// </summary>
        public static SymbolicTensorDim operator +(int a, SymbolicTensorDim b)
        {
            if (b.isValue)
                return new SymbolicTensorDim(a + b.value);
            if (a == 0)
                return b;
            return Unknown;
        }

        ///   | 0   1
        /// --|--------
        /// 0 | 0   1
        /// 3 | 3   4
        /// A | A   ?
        /// ? | ?   ?
        /// <summary>
        /// Adds an `int` to a `SymbolicTensorDim`.
        /// </summary>
        public static SymbolicTensorDim operator +(SymbolicTensorDim a, int b)
        {
            return b + a;
        }

        ///   | 0   1   A   B   ?
        /// --|---------------------
        /// 3 | 3   2   ?   ?   ?
        /// A | A   ?   0   ?   ?
        /// ? | ?   ?   ?   ?   ?
        /// <summary>
        /// Subtracts a `SymbolicTensorDim` from another `SymbolicTensorDim`.
        /// </summary>
        public static SymbolicTensorDim operator -(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.isValue)
                return a.value - b;
            if (b.isValue)
                return a - b.value;
            if (a.isParam && b.isParam && a.param == b.param)
                return Zero;
            return Unknown;
        }

        ///   | 0   1   A   B   ?
        /// --|---------------------
        /// 3 | 3   2   ?   ?   ?
        /// <summary>
        /// Subtracts a `SymbolicTensorDim` from an `int`.
        /// </summary>
        public static SymbolicTensorDim operator -(int a, SymbolicTensorDim b)
        {
            if (b.isValue)
                return new SymbolicTensorDim(a - b.value);
            return Unknown;
        }

        ///   | 0   1
        /// --|---------
        /// 3 | 3   2
        /// A | A   ?
        /// ? | ?   ?
        /// <summary>
        /// Subtracts an `int` from a `SymbolicTensorDim`.
        /// </summary>
        public static SymbolicTensorDim operator -(SymbolicTensorDim a, int b)
        {
            if (a.isValue)
                return new SymbolicTensorDim(a.value - b);
            if (b == 0)
                return a;
            return Unknown;
        }

        ///   | 0   1   3   A   B   ?
        /// --|-----------------------
        /// 0 | 0   0   0   0   0   0
        /// 2 | 0   2   6   ?   ?   ?
        /// A | 0   A   ?   ?   ?   ?
        /// ? | 0   ?   ?   ?   ?   ?
        /// <summary>
        /// Multiplies two `SymbolicTensorDim` dimensions.
        /// </summary>
        public static SymbolicTensorDim operator *(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.isValue)
                return a.value * b;
            if (b.isValue)
                return b.value * a;
            return Unknown;
        }

        ///   | 1   3   A   B   ?
        /// --|--------------------
        /// 0 | 0   0   0   0   0
        /// 2 | 2   6   ?   ?   ?
        /// <summary>
        /// Multiplies an `int` by a `SymbolicTensorDim`.
        /// </summary>
        public static SymbolicTensorDim operator *(int a, SymbolicTensorDim b)
        {
            if (b.isValue)
                return new SymbolicTensorDim(a * b.value);
            if (a == 1)
                return b;
            if (a == 0)
                return Zero;
            return Unknown;
        }

        ///   | 0   1   3
        /// --|-----------
        /// 2 | 0   2   6
        /// A | 0   A   ?
        /// ? | 0   ?   ?
        /// <summary>
        /// Multiplies a `SymbolicTensorDim` by an `int`.
        /// </summary>
        public static SymbolicTensorDim operator *(SymbolicTensorDim a, int b)
        {
            return b * a;
        }

        ///   | 1   2   3   A   B   ?
        /// --|-----------------------
        /// 0 | 0   0   0   0   0   0
        /// 2 | 2   1  Err  ?   ?   ?
        /// A | A   3   ?   1   ?   ?
        /// ? | ?   ?   ?   ?   ?   ?
        /// <summary>
        /// Divides two `SymbolicTensorDim` dimensions a whole number of times. The method throws an error if the result has a remainder.
        /// </summary>
        public static SymbolicTensorDim operator /(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.isValue)
                return a.value / b;
            if (b.isValue)
                return a / b.value;
            if (a.isParam && b.isParam && a.param == b.param)
                return One;
            return Unknown;
        }

        ///   | 1   2   3   A   ?
        /// --|--------------------
        /// 0 | 0   0   0   0   0
        /// 2 | 2   1  Err  ?   ?
        /// <summary>
        /// Divides an `int` by a `SymbolicTensorDim` a whole number of times. The method throws an error if the result has a remainder.
        /// </summary>
        public static SymbolicTensorDim operator /(int a, SymbolicTensorDim b)
        {
            if (a == 0)
                return Zero;
            if (b.isValue)
            {
                Logger.AssertIsTrue(b.value != 0, "ValueError: cannot divide by dim of size 0");
                Logger.AssertIsTrue(a % b.value == 0, "ValueError: cannot divide SymbolicTensorDims exactly");
                return new SymbolicTensorDim(a / b.value);
            }

            return Unknown;
        }

        ///   | 1   2   3
        /// --|------------
        /// 2 | 2   1  Err
        /// A | A   3   ?
        /// ? | ?   ?   ?
        /// <summary>
        /// Divides a `SymbolicTensorDim` by an `int` a whole number of times. The method throws an error if the result has a remainder.
        /// </summary>
        public static SymbolicTensorDim operator /(SymbolicTensorDim a, int b)
        {
            if (a.isValue)
            {
                Logger.AssertIsTrue(b != 0, "ValueError: cannot divide by dim of size 0");
                Logger.AssertIsTrue(a.value % b == 0, "ValueError: cannot divide SymbolicTensorDims exactly");
                return new SymbolicTensorDim(a.value / b);
            }
            if (b == 1)
                return a;
            return Unknown;
        }

        /// with rounding direction = 1
        ///   |  0   1   2
        /// --|-------------
        /// 0 | Err  0   0
        /// 1 | Err  1   1
        /// 2 | Err  2   1
        /// A | Err  A   ?
        /// ? | Err  ?   ?
        /// <summary>
        /// Divides a `SymbolicTensorDim` by a `float` to return a rounded `SymbolicTensorDim`.
        /// rounding direction greater than 0 = ceiling
        /// rounding direction less than 0 = floor
        /// rounding direction equals 0 = round
        /// </summary>
        internal SymbolicTensorDim DivideWithRounding(int b, int roundingDirection)
        {
            if (b == 1)
                return this;

            Logger.AssertIsTrue(b != 0, "ValueError: cannot divide by dim of size 0");

            if (!isValue)
                return Unknown;

            var v = value / (float)b;
            if (roundingDirection > 0)
                return new SymbolicTensorDim(Mathf.CeilToInt(v));
            if (roundingDirection < 0)
                return new SymbolicTensorDim(Mathf.FloorToInt(v));
            return new SymbolicTensorDim(Mathf.RoundToInt(v));
        }

        ///   | 2   3   A   B   ?
        /// --|-------------------
        /// 2 | 2  Err  2   2   2
        /// A | 2   3   A   A   A
        /// ? | 2   3   A   B   ?
        /// <summary>
        /// Returns the better known of two `SymbolicTensorDim` dimensions known to be equal. The method throws an error if both dimensions are values and not equal.
        /// </summary>
        internal static SymbolicTensorDim MaxDefinedDim(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a.isUnknown)
                return b;
            if (b.isUnknown)
                return a;
            if (b.isValue)
            {
                Logger.AssertIsTrue(!a.isValue || b == a, "ValueError: value dims must be equal");
                return b;
            }

            return a;
        }

        ///   | 1   3   A   B   ?
        /// --|-----------------
        /// 1 | 1   3   A   B   ?
        /// 2 | 2  Err  2   2   2
        /// A | A   3   A   ?   ?
        /// ? | ?   3   ?   ?   ?
        /// <summary>
        /// Broadcasts two `SymbolicTensorDim` dimensions using a broadcast rule where a dimension of size 1 can broadcast with any other dimension.
        /// </summary>
        internal static SymbolicTensorDim Broadcast(SymbolicTensorDim a, SymbolicTensorDim b)
        {
            if (a == One)
                return b;
            if (b == One)
                return a;
            if (a == b)
                return a;
            if (a.isValue && b.isValue)
                Logger.AssertIsTrue(a == b, "ValueError: broadcast dims must be equal or 1");
            if (a.isValue)
                return a;
            if (b.isValue)
                return b;
            return Unknown;
        }

        ///   | 1   3   B   ?
        /// --|-----------------
        /// 1 | 1   3   B   ?
        /// 2 | 2  Err  2   2
        /// A | A   3   ?   ?
        /// ? | ?   3   ?   ?
        /// <summary>
        /// Broadcasts the `SymbolicTensorDim` with another `SymbolicTensorDim` using a broadcast rule where a dimension of size 1 can broadcast with any other dimension.
        /// </summary>
        internal SymbolicTensorDim Broadcast(SymbolicTensorDim other)
        {
            return Broadcast(this, other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            return m_DimType.GetHashCode() ^ m_Param.GetHashCode() ^ m_Value.GetHashCode();
        }
    }
}
