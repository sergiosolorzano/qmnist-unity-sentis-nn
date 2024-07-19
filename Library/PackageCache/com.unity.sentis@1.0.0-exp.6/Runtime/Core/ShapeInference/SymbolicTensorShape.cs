using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Object = System.Object;

namespace Unity.Sentis
{
    /// <summary>
    /// Represents the shape of an input tensor, or the predicted shape of a tensor before Sentis executes.
    /// </summary>
    [Serializable]
    public unsafe struct SymbolicTensorShape
    {
        // disable warning: Field 'field' is never assigned to, and will always have its default value 'value'
        // fields are accessed using unsafe code (ptr)
        // SymbolicTensorShapes are 0 padded from right to left to match SymbolicTensorDim slices
        // Ex:
        // (5)       -> 0,0,0,5
        // (3,5)     -> 0,0,3,5
        // (7,3,5)   -> 0,7,3,5
        // (6,7,3,5) -> 6,7,3,5
        // ...
        SymbolicTensorDim m_D7;
        SymbolicTensorDim m_D6;
        SymbolicTensorDim m_D5;
        SymbolicTensorDim m_D4;
        SymbolicTensorDim m_D3;
        SymbolicTensorDim m_D2;
        SymbolicTensorDim m_D1;
        SymbolicTensorDim m_D0;
        bool m_IsRankUnknown;
        int m_Rank;

        internal bool hasRank => !m_IsRankUnknown;

        /// <summary>
        /// The rank of a `SymbolicTensorShape`, For example, a tensor of shape (5) has a rank of 1. A tensor of shape (7, 3, 5) has a rank of 3.
        /// </summary>
        public int rank => m_Rank;

        /// <summary>
        /// Gets or sets the tensor shape at a given axis.
        /// Ex:
        /// shape  (3, 4, 5, 6)
        /// index   0, 1, 2, 3
        ///        -4,-3,-2,-1
        /// shape  (7, 3, 2)
        /// index   0, 1, 2
        ///        -3,-2,-1
        /// </summary>
        public SymbolicTensorDim this[int axis]
        {
            get
            {
                if (!hasRank)
                    return SymbolicTensorDim.Unknown;

                axis = Axis(axis);

                fixed (SymbolicTensorDim* shape = &m_D7)
                {
                    return shape[(TensorShape.maxRank - rank) + axis];
                }
            }

            set
            {
                if (!hasRank)
                    return;

                axis = Axis(axis);

                fixed (SymbolicTensorDim* shape = &m_D7)
                {
                    shape[(TensorShape.maxRank - rank) + axis] = value;
                }
            }
        }

        /// <summary>
        /// Checks if the `SymbolicTensorShape` is fully defined and converted to a `TensorShape`.
        /// </summary>
        public bool IsFullyKnown()
        {
            if (!hasRank)
                return false;

            for (var i = 0; i < rank; i++)
            {
                if (!this[i].isValue)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// The length of the tensor shape as a symbolic tensor dimension.
        /// </summary>
        internal SymbolicTensorDim Length()
        {
            if (!hasRank)
                return SymbolicTensorDim.Unknown;

            var length = SymbolicTensorDim.One;

            for (var i = 0; i < rank && length != SymbolicTensorDim.Unknown && length != SymbolicTensorDim.Zero; i++)
                length *= this[i];

            return length;
        }

        /// <summary>
        /// Converts the `SymbolicTensorShape` to a `TensorShape`. You should call `IsFullyKnown` before you call this method.
        /// </summary>
        public TensorShape ToTensorShape()
        {
            Assert.IsTrue(hasRank, "ValueError: Cannot convert tensor of unknown rank to TensorShape");

            var shapeOut = TensorShape.Ones(rank);
            for (var i = 0; i < rank; i++)
            {
                shapeOut[i] = this[i].value;
            }

            return shapeOut;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 8: (d7, d6, d5, d4, d3, d2, d1, d0).
        ///
        /// For example (2, 3, 4, 5, 6, 7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d7, SymbolicTensorDim d6, SymbolicTensorDim d5, SymbolicTensorDim d4, SymbolicTensorDim d3, SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = d7;
            m_D6 = d6;
            m_D5 = d5;
            m_D4 = d4;
            m_D3 = d3;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 8;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 7: (d6, d5, d4, d3, d2, d1, d0).
        ///
        /// For example (3, 4, 5, 6, 7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d6, SymbolicTensorDim d5, SymbolicTensorDim d4, SymbolicTensorDim d3, SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = d6;
            m_D5 = d5;
            m_D4 = d4;
            m_D3 = d3;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 7;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 6: (d5, d4, d3, d2, d1, d0).
        ///
        /// For example (4, 5, 6, 7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d5, SymbolicTensorDim d4, SymbolicTensorDim d3, SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = d5;
            m_D4 = d4;
            m_D3 = d3;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 6;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 5: (d4, d3, d2, d1, d0).
        ///
        /// For example (5, 6, 7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d4, SymbolicTensorDim d3, SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = SymbolicTensorDim.Unknown;
            m_D4 = d4;
            m_D3 = d3;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 5;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 4: (d3, d2, d1, d0).
        ///
        /// For example (6, 7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d3, SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = SymbolicTensorDim.Unknown;
            m_D4 = SymbolicTensorDim.Unknown;
            m_D3 = d3;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 4;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 3: (d2, d1, d0).
        ///
        /// For example (7, 8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d2, SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = SymbolicTensorDim.Unknown;
            m_D4 = SymbolicTensorDim.Unknown;
            m_D3 = SymbolicTensorDim.Unknown;
            m_D2 = d2;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 3;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 2: (d1, d0).
        ///
        /// For example (8, 9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d1, SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = SymbolicTensorDim.Unknown;
            m_D4 = SymbolicTensorDim.Unknown;
            m_D3 = SymbolicTensorDim.Unknown;
            m_D2 = SymbolicTensorDim.Unknown;
            m_D1 = d1;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 2;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a rank of 1: (d0).
        ///
        /// For example (9).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim d0)
        {
            m_D7 = SymbolicTensorDim.Unknown;
            m_D6 = SymbolicTensorDim.Unknown;
            m_D5 = SymbolicTensorDim.Unknown;
            m_D4 = SymbolicTensorDim.Unknown;
            m_D3 = SymbolicTensorDim.Unknown;
            m_D2 = SymbolicTensorDim.Unknown;
            m_D1 = SymbolicTensorDim.Unknown;
            m_D0 = d0;

            m_IsRankUnknown = false;
            m_Rank = 1;
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` of unknown rank.
        /// </summary>
        internal static SymbolicTensorShape UnknownShape
        {
            get
            {
                var shape = new SymbolicTensorShape();
                shape.m_IsRankUnknown = true;
                return shape;
            }
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a given shape, and no unknown or dynamic dimensions. For example: `SymbolicTensorShape(new [] {3, 4, 5, 6})` returns a symbolic tensor shape of (3, 4, 5, 6).
        /// </summary>
        public SymbolicTensorShape(int[] shape)
            : this()
        {
            Logger.AssertIsTrue(shape.Length <= TensorShape.maxRank, "ValueError: SymbolicTensorShape are capped to rank=8, cannot create SymbolicTensorShape of rank {0}", shape.Length);

            m_IsRankUnknown = false;
            m_Rank = shape.Length;

            for (var i = 0; i < shape.Length; i++)
            {
                this[i] = new SymbolicTensorDim(shape[i]);
            }
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a given shape, including unknown or dynamic dimensions. For example: `SymbolicTensorShape(new [] { new SymbolicTensorDim(3), new SymbolicTensorDim('A'), SymbolicTensorDim.Unknown } )` returns a symbolic tensor shape of (3, 'A', ?).
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorDim[] shape)
            : this()
        {
            Logger.AssertIsTrue(shape.Length <= TensorShape.maxRank, "ValueError: SymbolicTensorShape are capped to rank=8, cannot create SymbolicTensorShape of rank {0}", shape.Length);

            m_IsRankUnknown = false;
            m_Rank = shape.Length;

            for (var i = 0; i < shape.Length; i++)
            {
                this[i] = shape[i];
            }
        }

        /// <summary>
        /// Initializes and returns an instance of `SymbolicTensorShape` with a given `TensorShape`, and no unknown or dynamic dimensions. For example: `SymbolicTensorShape(new TensorShape(3, 4, 5, 6))` returns a symbolic tensor shape of (3, 4, 5, 6).
        /// </summary>
        public SymbolicTensorShape(TensorShape other)
        {
            m_Rank = other.rank;
            m_IsRankUnknown = false;

            m_D7 = m_Rank > 7 ? new SymbolicTensorDim(other[m_Rank - 8]) : SymbolicTensorDim.Unknown;
            m_D6 = m_Rank > 6 ? new SymbolicTensorDim(other[m_Rank - 7]) : SymbolicTensorDim.Unknown;
            m_D5 = m_Rank > 5 ? new SymbolicTensorDim(other[m_Rank - 6]) : SymbolicTensorDim.Unknown;
            m_D4 = m_Rank > 4 ? new SymbolicTensorDim(other[m_Rank - 5]) : SymbolicTensorDim.Unknown;
            m_D3 = m_Rank > 3 ? new SymbolicTensorDim(other[m_Rank - 4]) : SymbolicTensorDim.Unknown;
            m_D2 = m_Rank > 2 ? new SymbolicTensorDim(other[m_Rank - 3]) : SymbolicTensorDim.Unknown;
            m_D1 = m_Rank > 1 ? new SymbolicTensorDim(other[m_Rank - 2]) : SymbolicTensorDim.Unknown;
            m_D0 = m_Rank > 0 ? new SymbolicTensorDim(other[m_Rank - 1]) : SymbolicTensorDim.Unknown;
        }

        /// <summary>
        /// Returns a copy of another `SymbolicTensorShape`.
        /// </summary>
        public SymbolicTensorShape(SymbolicTensorShape other)
        {
            m_Rank = other.rank;
            m_IsRankUnknown = other.m_IsRankUnknown;

            m_D7 = other.m_D7;
            m_D6 = other.m_D6;
            m_D5 = other.m_D5;
            m_D4 = other.m_D4;
            m_D3 = other.m_D3;
            m_D2 = other.m_D2;
            m_D1 = other.m_D1;
            m_D0 = other.m_D0;
        }

        /// <summary>
        /// SymbolicTensorShape with given rank and all dimensions unknown
        /// </summary>
        internal static SymbolicTensorShape UnknownOfRank(int rank)
        {
            Logger.AssertIsTrue(rank <= TensorShape.maxRank, "ValueError: SymbolicTensorShape are capped to rank=8, cannot create empty shape of rank {0}", rank);
            var outShape = new SymbolicTensorShape();
            outShape.m_IsRankUnknown = false;
            outShape.m_Rank = rank;
            return outShape;
        }

        /// <summary>
        /// SymbolicTensorShape with same rank as other SymbolicTensorShape and all dimensions unknown
        /// </summary>
        internal static SymbolicTensorShape UnknownOfRankLike(SymbolicTensorShape other)
        {
            if (!other.hasRank)
                return UnknownShape;
            return UnknownOfRank(other.rank);
        }

        /// <summary>
        /// Asserts if this shape has a rank different from the given rank
        /// If tensor is unknown rank then rank is set to given value
        /// </summary>
        internal void DeclareRank(int newRank)
        {
            if (hasRank)
            {
                Logger.AssertAreEqual(m_Rank, newRank, "RankError: incorrect rank, expecting {0}, got {1}", m_Rank, newRank);
                return;
            }

            m_IsRankUnknown = false;
            m_Rank = newRank;
        }

        /// <summary>
        /// SymbolicTensorShape with given rank and all dimensions 1
        /// </summary>
        internal static SymbolicTensorShape Ones(int rank)
        {
            Logger.AssertIsTrue(rank <= TensorShape.maxRank, "ValueError: SymbolicTensorShape are capped to rank=8, cannot create empty shape of rank {0}", rank);
            var outShape = new SymbolicTensorShape();
            outShape.m_IsRankUnknown = false;
            outShape.m_Rank = rank;
            for (var i = 0; i < rank; i++)
            {
                outShape[i] = SymbolicTensorDim.One;
            }

            return outShape;
        }

        /// <summary>
        /// SymbolicTensorShape with rank matching shape and all dimensions 1
        /// </summary>
        internal static SymbolicTensorShape OnesLike(SymbolicTensorShape shape)
        {
            return shape.hasRank ? Ones(shape.rank) : UnknownShape;
        }

        /// <summary>
        /// Returns a string that represents the `SymbolicTensorShape`.
        /// </summary>
        public override string ToString()
        {
            if (!hasRank)
                return "UnknownShape";

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (var i = 0; i < rank; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                var dim = this[i];
                sb.Append(dim.ToString());
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Wraps axis to positive index between 0,rank
        /// (5,2,3,4)
        /// axis = -1 => axis_out = 3
        /// axis = 1 => axis_out = 1
        /// </summary>
        internal int Axis(int axis)
        {
            Logger.AssertIsTrue(axis >= -rank && axis < rank, "IndexError: axis {0} is out of bounds shape of rank, {1}", axis, rank);
            return axis >= 0 ? axis : rank + axis;
        }

        /// <summary>
        /// Removes axes of length 1. For example, if the `SymbolicTensorShape` is (5, 1, 3, 1), the method returns (5, 3).
        /// </summary>
        internal SymbolicTensorShape Squeeze()
        {
            if (!hasRank)
                return UnknownShape;
            Logger.AssertIsTrue(rank != 0, "ValueError: cannot squeeze scalar tensor {0}", this);

            var numAxes = 0;
            for (var i = 0; i < rank; i++)
            {
                if (!this[i].isValue)
                    return UnknownShape;
                if (this[i] == SymbolicTensorDim.One)
                    numAxes += 1;
            }

            var shapeOut = UnknownOfRank(rank - numAxes);
            var index = 0;
            for (var i = 0; i < rank; i++)
            {
                if (this[i] != SymbolicTensorDim.One)
                    shapeOut[index++] = this[i];
            }

            return shapeOut;
        }

        /// <summary>
        /// Removes the axis if its length is 1. For example, if `SymbolicTensorShape` is (5, 1, 3, 1) and `axis` is 1, the method returns (5, 3, 1).
        /// </summary>
        internal SymbolicTensorShape Squeeze(int axis)
        {
            if (!hasRank)
                return UnknownShape;

            axis = Axis(axis);
            var dim = this[axis];

            Logger.AssertIsTrue(!dim.isValue || dim == SymbolicTensorDim.One, "ValueError: cannot squeeze axis with value != 1");

            var shapeOut = UnknownOfRank(rank - 1);
            var index = 0;
            for (var i = 0; i < rank; i++)
            {
                if (i != axis)
                    shapeOut[index++] = this[i];
            }

            return shapeOut;
        }

        /// <summary>
        /// Removes axes if their length is 1. For example, if `SymbolicTensorShape` is (5, 1, 3, 1) and `axes` is {1, -1}, the method returns (5, 3).
        /// </summary>
        internal SymbolicTensorShape Squeeze(int[] axes)
        {
            if (axes == null || axes.Length == 0)
                return Squeeze();

            if (!hasRank)
                return UnknownShape;

            uint axesBitMask = 0;
            for (var i = 0; i < axes.Length; i++)
            {
                var axis = Axis(axes[i]);
                Logger.AssertIsTrue(((axesBitMask >> axis) & 1U) == 0, "ValueError: can't squeeze on same axis multiple times");
                axesBitMask |= 1U << axis;
                var dim = this[axis];
                Logger.AssertIsTrue(!dim.isValue || dim == SymbolicTensorDim.One, "ValueError: cannot squeeze axis with value != 1");
            }

            var shapeOut = UnknownOfRank(rank - axes.Length);
            var index = 0;
            for (var i = 0; i < rank; i++)
            {
                if (((axesBitMask >> i) & 1U) == 0)
                    shapeOut[index++] = this[i];
            }

            return shapeOut;
        }

        /// <summary>
        /// Inserts a new axis at `axis` position. For example if `SymbolicTensorShape` is (2) and the value of `axis` is 0, the method returns (1, 2).
        /// </summary>
        internal SymbolicTensorShape Unsqueeze(int axis)
        {
            if (!hasRank)
                return UnknownShape;

            Logger.AssertIsTrue(rank != TensorShape.maxRank, "ValueError: TensorShape are capped to rank=8, cannot unsqueeze rank 8 SymbolicTensorShape {0}", this);

            var shapeOut = UnknownOfRank(rank + 1);

            axis = shapeOut.Axis(axis);
            var indexIn = 0;
            for (var indexOut = 0; indexOut < shapeOut.rank; indexOut++)
            {
                if (indexOut == axis)
                    shapeOut[indexOut] = SymbolicTensorDim.One;
                else
                    shapeOut[indexOut] = this[indexIn++];
            }

            return shapeOut;
        }

        /// <summary>
        /// Inserts new axes at `axes` positions. For example if `SymbolicTensorShape` is (2) and `axes` is {0, 1}, the method returns (1, 1, 2).
        /// </summary>
        internal SymbolicTensorShape Unsqueeze(int[] axes)
        {
            if (!hasRank)
                return UnknownShape;

            Logger.AssertIsTrue(rank + axes.Length <= TensorShape.maxRank, "ValueError: TensorShape are capped to rank=8, cannot unsqueeze SymbolicTensorShape {0} to rank greater than 8", this);

            var shapeOut = UnknownOfRank(rank + axes.Length);

            uint axesBitMask = 0;
            for (var i = 0; i < axes.Length; i++)
            {
                var axis = shapeOut.Axis(axes[i]);
                Logger.AssertIsTrue(((axesBitMask >> axis) & 1U) == 0, "ValueError: can't unsqueeze on same axis multiple times");
                axesBitMask |= 1U << axis;
            }

            var indexIn = 0;
            for (var indexOut = 0; indexOut < shapeOut.rank; indexOut++)
            {
                if (((axesBitMask >> indexOut) & 1U) == 1)
                    shapeOut[indexOut] = SymbolicTensorDim.One;
                else
                    shapeOut[indexOut] = this[indexIn++];
            }

            return shapeOut;
        }

        /// <summary>
        /// Broadcasts the `SymbolicTensorShape` with another `SymbolicTensorShape`, according to numpy tensor broadcasting rules.
        /// </summary>
        internal SymbolicTensorShape Broadcast(SymbolicTensorShape other)
        {
            if (!hasRank || !other.hasRank)
                return UnknownShape;

            var outRank = Mathf.Max(rank, other.rank);
            var outShape = Ones(outRank);

            SymbolicTensorDim* fixedOther = &other.m_D7;
            SymbolicTensorDim* fixedOut = &outShape.m_D7;
            fixed (SymbolicTensorDim* fixedThis = &m_D7)
            {
                for (var i = 0; i < outRank; i++)
                {
                    if (i < rank)
                        fixedOut[TensorShape.maxRank - i - 1] = fixedThis[TensorShape.maxRank - i - 1];
                    if (i < other.rank)
                        fixedOut[TensorShape.maxRank - i - 1] = SymbolicTensorDim.Broadcast(fixedOut[TensorShape.maxRank - i - 1], fixedOther[TensorShape.maxRank - i - 1]);
                }
            }

            return outShape;
        }

        /// <summary>
        /// Multiplies two `SymbolicTensorShape` objects.
        /// </summary>
        internal SymbolicTensorShape MatMul(SymbolicTensorShape other)
        {
            if (!hasRank || !other.hasRank)
                return UnknownShape;

            Assert.IsTrue(rank >= 2, "MatMul.ValueError: Rank of tensor must be at least 2");
            Assert.IsTrue(other.rank >= 2, "MatMul.ValueError: Rank of tensor must be at least 2");

            // broadcast along the dimensions not used in the matmul
            var outRank = Mathf.Max(rank, other.rank);
            var shapeOut = Ones(outRank);

            SymbolicTensorDim* fixedOther = &other.m_D7;
            SymbolicTensorDim* fixedOut = &shapeOut.m_D7;
            fixed (SymbolicTensorDim* fixedThis = &m_D7)
            {
                for (var i = 2; i < shapeOut.rank; i++)
                {
                    if (i < rank)
                        fixedOut[TensorShape.maxRank - i - 1] = fixedThis[TensorShape.maxRank - i - 1];
                    if (i < other.rank)
                        fixedOut[TensorShape.maxRank - i - 1] = SymbolicTensorDim.Broadcast(fixedOut[TensorShape.maxRank - i - 1], fixedOther[TensorShape.maxRank - i - 1]);
                }

                // Raise an error if the last dimension of a is not the same size as the second-to-last dimension of b.
                var mulThisDim = fixedThis[TensorShape.maxRank - 1];
                var mulOtherDim = fixedOther[TensorShape.maxRank - 2];
                Logger.AssertIsTrue(!mulThisDim.isValue || !mulOtherDim.isValue || mulThisDim == mulOtherDim, "MatMul2D.ValueError: mul dims not equal");

                fixedOut[TensorShape.maxRank - 2] = fixedThis[TensorShape.maxRank - 2];
                fixedOut[TensorShape.maxRank - 1] = fixedOther[TensorShape.maxRank - 1];
            }

            return shapeOut;
        }

        /// <summary>
        /// Compares two `SymbolicTensorShape` objects. Returns `true` if the two objects have the same rank, and all their dimensions are equal.
        /// </summary>
        public static bool operator ==(SymbolicTensorShape a, SymbolicTensorShape b)
        {
            if (a.hasRank != b.hasRank)
                return false;
            if (a.rank != b.rank)
                return false;
            if (a.m_D7 != b.m_D7)
                return false;
            if (a.m_D6 != b.m_D6)
                return false;
            if (a.m_D5 != b.m_D5)
                return false;
            if (a.m_D4 != b.m_D4)
                return false;
            if (a.m_D3 != b.m_D3)
                return false;
            if (a.m_D2 != b.m_D2)
                return false;
            if (a.m_D1 != b.m_D1)
                return false;
            if (a.m_D0 != b.m_D0)
                return false;
            return true;
        }

        public static bool operator !=(SymbolicTensorShape a, SymbolicTensorShape b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current `SymbolicTensorShape`.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (SymbolicTensorShape)obj;
        }

        /// <summary>
        /// Whether symbolic shapes a and b could be referring to the same underlying tensor shape
        /// </summary>
        internal static bool IsCompatible(SymbolicTensorShape a, SymbolicTensorShape b)
        {
            if (!a.hasRank || !b.hasRank)
                return true;
            if (a.rank != b.rank)
                return false;
            for (var i = 0; i < a.rank; i++)
            {
                if (!SymbolicTensorDim.IsCompatible(a[i], b[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            return m_IsRankUnknown.GetHashCode() ^ m_Rank.GetHashCode() ^ m_D7.GetHashCode() ^ m_D6.GetHashCode() ^ m_D5.GetHashCode()
                ^ m_D4.GetHashCode() ^ m_D3.GetHashCode() ^ m_D2.GetHashCode() ^ m_D1.GetHashCode() ^ m_D0.GetHashCode();
        }
    }
}

