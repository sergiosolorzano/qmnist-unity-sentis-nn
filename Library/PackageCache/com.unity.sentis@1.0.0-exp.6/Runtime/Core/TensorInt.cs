using System;
using UnityEngine;

namespace Unity.Sentis {

/// <summary>
/// Represents data in a multidimensional array-like structure of ints.
/// </summary>
public class TensorInt : Tensor
{
    int[] m_Cache;

    /// <inheritdoc/>
    public override DataType dataType => DataType.Int;

    /// <inheritdoc/>
    protected override bool isCacheNull => (m_Cache == null);

    /// <inheritdoc/>
    internal TensorInt(TensorShape shape, ITensorData data = null, ITensorAllocator allocator = null) : base(shape, data, allocator) { }

    /// <summary>
    /// Initializes and returns a tensor with the specified `shape` and an int[] array of `srcData` data.
    /// </summary>
    public TensorInt(TensorShape shape, int[] srcData) : this(shape, srcData, 0) { }

    /// <summary>
    /// Initializes and returns a tensor with specified `shape` and an int[] array of `srcData` data. Sentis reads `srcData` from `dataStartIndex`. You can provide an optional debug `name`.
    ///
    /// `srcData.Length` - `dataStartIndex` must be bigger than or equal to `shape.length`.
    /// </summary>
    public TensorInt(TensorShape shape, int[] srcData, int dataStartIndex = 0) : base(shape)
    {
        tensorOnDevice = new ArrayTensorData(shape);
        Logger.AssertIsTrue((srcData.Length - dataStartIndex) >= shape.length, "RangeError: array length {0} is too small compared to shape length {1}", srcData.Length, shape);
        m_TensorOnDevice.Upload(srcData, shape.length, dataStartIndex);
        m_TensorAllocator = null;
        ClearCache();
    }

    /// <summary>
    /// Initializes and returns a scalar tensor with the value of `srcData`.
    /// </summary>
    public TensorInt(int srcData) : this(new TensorShape(), new[] { srcData }) { }

    /// <summary>
    /// Initializes and returns a tensor with the specified `shape` and filled with `0`.
    /// </summary>
    public static TensorInt Zeros(TensorShape shape)
    {
        return new TensorInt(shape, new int[shape.length]);
    }

    /// <inheritdoc/>
    protected override void UploadIfDirty()
    {
        if (m_CacheIsDirty && m_TensorOnDevice != null)
            m_TensorOnDevice.Upload(m_Cache, shape.length);
        m_CacheIsDirty = false;
    }

    /// <inheritdoc/>
    public override bool PrepareCacheForAccess(bool blocking = true)
    {
        // non-blocking, schedule download for later
        if (!blocking && m_TensorOnDevice != null && m_Cache == null)
            if (!m_TensorOnDevice.ScheduleAsyncDownload())
                return false;

        // blocking, have to get data now!
        if (m_Cache == null)
        {
            if (m_TensorOnDevice != null)
                m_Cache = m_TensorOnDevice.Download<int>(shape.length);
            else
                m_Cache = new int[shape.length];
            m_CacheIsDirty = false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override Tensor ShallowReshape(TensorShape newShape)
    {
        TensorInt copy;
        if (m_TensorAllocator != null)
            copy = m_TensorAllocator.Alloc(newShape, DataType.Int, m_TensorOnDevice, AllocScope.LayerOutput) as TensorInt;
        else
            copy = new TensorInt(newShape, m_TensorOnDevice, null);

        copy.m_Cache = m_Cache;
        copy.m_CacheIsDirty = m_CacheIsDirty;

        return copy;
    }

    /// <inheritdoc/>
    public override Tensor DeepCopy()
    {
        // @TODO: use Tensor allocator
        var copy = new TensorInt(shape);
        if (m_TensorOnDevice is ICloneable)
        {
            UploadIfDirty();
            var copyOfTensorData = (m_TensorOnDevice as ICloneable).Clone() as ITensorData;
            copy.AttachToDevice(copyOfTensorData);
        }
        else
        {
            PrepareCacheForAccess();
            copy.PrepareCacheForAccess();
            Array.Copy(m_Cache, 0, copy.m_Cache, 0, shape.length);
        }

        return copy;
    }

    /// <inheritdoc/>
    protected override void ClearCache()
    {
        m_Cache = null;
        m_CacheIsDirty = false;
    }

    /// <summary>
    /// Returns the tensor element at offset `(d7, d6, d5, d4, d3, d2, d1, d0)`, which is position `d7 * stride6 + d6 * stride5 + d5 * stride4 + d4 * stride3 + d3 * stride2 + d2 * stride1 + d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d7, int d6, int d5, int d4, int d3, int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d7, d6, d5, d4, d3, d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d7, d6, d5, d4, d3, d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d6, d5, d4, d3, d2, d1, d0)`, which is position `d6 * stride5 + d5 * stride4 + d4 * stride3 + d3 * stride2 + d2 * stride1 + d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d6, int d5, int d4, int d3, int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d6, d5, d4, d3, d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d6, d5, d4, d3, d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d5, d4, d3, d2, d1, d0)`, which is position `d5 * stride4 + d4 * stride3 + d3 * stride2 + d2 * stride1 + d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d5, int d4, int d3, int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d5, d4, d3, d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d5, d4, d3, d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d4, d3, d2, d1, d0)`, which is position `d4 * stride3 + d3 * stride2 + d2 * stride1 + d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d4, int d3, int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d4, d3, d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d4, d3, d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d3, d2, d1, d0)`, which is position `d3 * stride2 + d2 * stride1 + d1 * stride0 + d0` in this tensor.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d3, int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d3, d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d3, d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d2, d1, d0)`, which is position `d2 * stride1 + d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d2, int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d2, d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d2, d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `(d1, d0)`, which is position `d1 * stride0 + d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d1, int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[shape.RavelIndex(d1, d0)]; }
        set { PrepareCacheForAccess(); m_Cache[shape.RavelIndex(d1, d0)] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the tensor element at offset `d0`.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    /// </summary>
    public int this[int d0]
    {
        get { PrepareCacheForAccess(); return m_Cache[d0]; }
        set { PrepareCacheForAccess(); m_Cache[d0] = value; m_CacheIsDirty = true; }
    }

    /// <summary>
    /// Returns the cached linear memory representation of this tensor data.
    ///
    /// If the tensor is the result of computation on a different device, the method creates a blocking read.
    ///
    /// If you modify the contents of the returned array, the behaviour is undefined.
    /// </summary>
    public int[] ToReadOnlyArray()
    {
        // @TODO: implement via ITensorData.SharedAccess(), public int[] ToReadOnlyArray(ref int arrayOffset)
        PrepareCacheForAccess();
        return m_Cache;
    }
}

} // namespace Unity.Sentis
