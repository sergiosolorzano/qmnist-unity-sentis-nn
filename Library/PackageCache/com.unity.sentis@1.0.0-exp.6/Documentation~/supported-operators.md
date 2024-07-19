# Supported ONNX operators

The table shows which Open Neural Network Exchange (ONNX) operators Sentis supports, and which data types Sentis supports for each [back end type](create-an-engine.md#back-end-types).

When you import a model, each ONNX operator in the model graph becomes a Sentis layer. A Sentis layer has the same name as the ONNX operator, unless the table shows the operator maps to a different layer. Refer to [How Sentis optimizes a model](models-concept.md#how-sentis-optimizes-a-model) for more information.

If you use a GPU Worker and the model has a layer that isn't supported, or has a layer that uses an input tensor data type that isn't supported, Sentis falls back to using `BackendType.CPU` for the layer, which uses Burst. If `BackendType.CPU` isn't supported, Sentis falls back to using a slower CPU implementation of the layer that doesn't use Burst.

Refer to [the API reference](../api/index.html) for more information on each Sentis layer.

## Supported operators

### ONNX operators

|Name|Supported data types with `BackendType.CPU`|Supported data types with `BackendType.GPUCompute`|Supported data types with `BackendType.GPUPixel`|Notes|
|-|-|-|-|-|
|[Abs](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Abs) | float, int | float, int | float | |
|[Acos](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Acos) | float | float | float | |
|[Acosh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Acosh) | float | float | float | |
|[Add](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Add) | float, int | float, int | float | |
|[And](https://github.com/onnx/onnx/blob/main/docs/Operators.md#And) | int | int | Not supported | |
|[ArgMax](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ArgMax) | float, int | float, int | Not supported | |
|[ArgMin](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ArgMin) | float, int | float, int | Not supported | |
|[Asin](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Asin) | float | float | float | |
|[Asinh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Asinh) | float | float | float | |
|[Atan](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Atan) | float | float | float | |
|[Atanh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Atanh) | float | float | float | |
|[AveragePool](https://github.com/onnx/onnx/blob/main/docs/Operators.md#AveragePool) | float | float (1D and 2D only) | float (2D only) | The `ceil_mode` and `count_include_pad` parameters aren't supported. |
|[BatchNormalization](https://github.com/onnx/onnx/blob/main/docs/Operators.md#BatchNormalization) | float | float | float | The `epsilon`, `momentum`, `spatial` and `training_mode` parameters aren't supported. |
|[Bernoulli](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Bernoulli) | float | float | Not supported | |
|[Cast](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Cast) | float, int | float, int | Not supported | |
|[CastLike](https://github.com/onnx/onnx/blob/main/docs/Operators.md#CastLike) | float, int | float, int | Not supported | |
|[Ceil](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Ceil) | float | float | float | |
|[Celu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Celu) | float | float | float | |
|[Clip](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Clip) | float | float | float | |
|[Compress](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Compress) | float, int | Not supported | Not supported | |
|[Concat](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Concat) | float, int | float, int | float | |
|[Constant](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Constant) | - | - | - | The `sparse_value` parameter isn't supported. |
|[ConstantOfShape](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ConstantOfShape) | float, int | float, int | float | |
|[Conv](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Conv) | float | float (1D, 2D and 3D) | float (2D only) | The `kernel_shape` parameter isn't supported. |
|[ConvTranspose](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ConvTranspose) | float | float (2D only) | float (2D only) | The `dilations`, `group`, `kernel_shape` and `output_shape` parameters aren't supported. |
|[Cos](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Cos) | float | float | float | |
|[Cosh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Cosh) | float | float | float | |
|[CumSum](https://github.com/onnx/onnx/blob/main/docs/Operators.md#CumSum) | float, int | float, int | Not supported | |
|[DepthToSpace](https://github.com/onnx/onnx/blob/main/docs/Operators.md#DepthToSpace) | float | float | float | |
|[Div](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Div) | float, int | float, int | float | |
|[Dropout](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Dropout) | - | - | - | The operator maps to the Sentis layer `Identity`|
|[Einsum](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Einsum) | float | float (1 or 2 inputs only) | Not supported | |
|[Elu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Elu) | float | float | float | |
|[Equal](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Equal) | float, int | float, int | Not supported | |
|[Erf](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Erf) | float | float | float | |
|[Exp](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Exp) | float | float | float | |
|[Expand](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Expand) | float, int | float, int | float | |
|[Flatten](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Flatten) | float, int | float, int | float | |
|[Floor](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Floor) | float | float | float | |
|[Gather](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Gather) | float, int | float, int | Not supported | |
|[GatherElements](https://github.com/onnx/onnx/blob/main/docs/Operators.md#GatherElements) | float, int | float, int | Not supported | |
|[GatherND](https://github.com/onnx/onnx/blob/main/docs/Operators.md#GatherND) | float, int | float, int | Not supported | |
|[Gemm](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Gemm) | float | float | float | |
|[GlobalAveragePool](https://github.com/onnx/onnx/blob/main/docs/Operators.md#GlobalAveragePool) | float | float | float | |
|[GlobalMaxPool](https://github.com/onnx/onnx/blob/main/docs/Operators.md#GlobalMaxPool) | float | float | float | |
|[Greater](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Greater) | float, int | float, int | Not supported | |
|[GreaterOrEqual](https://github.com/onnx/onnx/blob/main/docs/Operators.md#GreaterOrEqual) | float, int | float, int | Not supported | |
|[Hardmax](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Hardmax) | float | float | Not supported | |
|[HardSigmoid](https://github.com/onnx/onnx/blob/main/docs/Operators.md#HardSigmoid) | float | float | float | |
|[HardSwish](https://github.com/onnx/onnx/blob/main/docs/Operators.md#HardSwish) | float | float | float | |
|[Identity](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Identity) | float, int | float, int | float | |
|[InstanceNormalization](https://github.com/onnx/onnx/blob/main/docs/Operators.md#InstanceNormalization) | float | float | Not supported | |
|[IsInf](https://github.com/onnx/onnx/blob/main/docs/Operators.md#IsInf) | float | float | Not supported | |
|[IsNaN](https://github.com/onnx/onnx/blob/main/docs/Operators.md#IsNaN) | float | float | Not supported | |
|[LeakyRelu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#LeakyRelu) | float | float | float | |
|[Less](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Less) | float, int | float, int | Not supported | |
|[LessOrEqual](https://github.com/onnx/onnx/blob/main/docs/Operators.md#LessOrEqual) | float, int | float, int | Not supported | |
|[Log](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Log) | float | float | float | |
|[LogSoftmax](https://github.com/onnx/onnx/blob/main/docs/Operators.md#LogSoftmax) | float | float | float | |
|[LRN](https://github.com/onnx/onnx/blob/main/docs/Operators.md#LRN) | float | Not supported | Not supported | |
|[LSTM](https://github.com/onnx/onnx/blob/main/docs/Operators.md#LSTM) | float | float | Not supported | |
|[MatMul](https://github.com/onnx/onnx/blob/main/docs/Operators.md#MatMul) | float | float | float (2D only) | |
|[Max](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Max) | float, int | float, int | float | |
|[MaxPool](https://github.com/onnx/onnx/blob/main/docs/Operators.md#MaxPool) | float | float (1D and 2D only) | float (2D only) | The `ceil_mode`, `dilations` and `storage_order` parameters aren't supported. |
|[Mean](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Mean) | float | float | float | |
|[Min](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Min) | float, int | float, int | float | |
|[Mod](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Mod) | float, int | float, int | float | |
|[Mul](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Mul) | float, int | float, int | float | |
|[Multinomial](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Multinomial) | float | Not supported | Not supported | |
|[Neg](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Neg) | float, int | float, int | float | |
|[NonMaxSuppression](https://github.com/onnx/onnx/blob/main/docs/Operators.md#NonMaxSuppression) | float | Not supported | Not supported | |
|[NonZero](https://github.com/onnx/onnx/blob/main/docs/Operators.md#NonZero) | float, int | Not supported | Not supported | |
|[Not](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Not) | int | int | Not supported | |
|[OneHot](https://github.com/onnx/onnx/blob/main/docs/Operators.md#OneHot) | (int, int, int) - outputs int | (int, int, int) - outputs int | Not supported | |
|[Or](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Or) | int | int | Not supported | |
|[Pad](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Pad) | float | float | float | |
|[Pow](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Pow) | (float float) or (float, int) | (float float) or (float, int) | float | |
|[PRelu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#PRelu) | float | float | Not supported | |
|[RandomNormal](https://github.com/onnx/onnx/blob/main/docs/Operators.md#RandomNormal) | float | float | Not supported | |
|[RandomNormalLike](https://github.com/onnx/onnx/blob/main/docs/Operators.md#RandomNormalLike) | float | float | Not supported | |
|[RandomUniform](https://github.com/onnx/onnx/blob/main/docs/Operators.md#RandomUniform) | float | float | Not supported | |
|[RandomUniformLike](https://github.com/onnx/onnx/blob/main/docs/Operators.md#RandomUniformLike) | float | float | Not supported | |
|[Range](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Range) | float | float | Not supported | |
|[Reciprocal](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Reciprocal) | float | float | float | |
|[ReduceL1](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceL1) | float, int | float, int | float | |
|[ReduceL2](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceL2) | float | float | float | |
|[ReduceLogSum](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceLogSum) | float | float | float | |
|[ReduceLogSumExp](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceLogSumExp) | float | float | float | |
|[ReduceMax](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceMax) | float, int | float, int | float | |
|[ReduceMean](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceMean) | float | float | float | |
|[ReduceMin](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceMin) | float, int | float, int | float | |
|[ReduceProd](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceProd) | float, int | float, int | float | |
|[ReduceSum](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceSum) | float, int | float, int | float | |
|[ReduceSumSquare](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ReduceSumSquare) | float, int | float, int | float | |
|[Relu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Relu) | float | float | float | |
|[Reshape](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Reshape) | float, int | float, int | float | |
|[Resize](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Resize) | float | float (2D and 3D only) | float (2D only) | The `cubic_coeff_a`, `exclude_outside`, `extrapolation_value` and `roi`  parameters aren't supported. |
|[RoiAlign](https://github.com/onnx/onnx/blob/main/docs/Operators.md#RoiAlign) | float | float | Not supported | The `coordinate_transformation_mode` and `mode` parameters aren't supported. |
|[Round](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Round) | float | float | float | |
|[Scatter (deprecated)](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Scatter) | float, int | float, int | Not supported | The operator maps to the Sentis layer `ScatterElements`. |
|[ScatterElements](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ScatterElements) | float, int | float, int (no ScatterReductionMode) | Not supported | |
|[ScatterND](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ScatterND) | float | float | Not supported | |
|[Selu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Selu) | float | float | float | |
|[Shape](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Shape) | - | - | - | |
|[Shrink](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Shrink) | float | float | Not supported | |
|[Sigmoid](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sigmoid) | float | float | float | |
|[Sign](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sign) | float, int | float, int | Not supported | |
|[Sin](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sin) | float | float | float | |
|[Sinh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sinh) | float | float | float | |
|[Size](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Size) | - | - | - | |
|[Slice](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Slice) | float, int | float, int | float | |
|[Softmax](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Softmax) | float | float | float | |
|[Softplus](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Softplus) | float | float | float | |
|[Softsign](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Softsign) | float | float | float | |
|[SpaceToDepth](https://github.com/onnx/onnx/blob/main/docs/Operators.md#SpaceToDepth) | float | float | float | |
|[Split](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Split) | float, int | float, int | float | |
|[Sqrt](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sqrt) | float | float | float | |
|[Squeeze](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Squeeze) | float, int | float, int | float | |
|[Sub](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sub) | float, int | float, int | float | |
|[Sum](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Sum) | float, int | float, int | Not supported | |
|[Tan](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Tan) | float | float | float | |
|[Tanh](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Tanh) | float | float | float | |
|[ThresholdedRelu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#ThresholdedRelu) | float | float | float | |
|[Tile](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Tile) | float, int | float, int | Not supported | |
|[TopK](https://github.com/onnx/onnx/blob/main/docs/Operators.md#TopK) | float | Not supported | Not supported | |
|[Transpose](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Transpose) | float, int | float, int | float | |
|[Trilu](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Trilu) | float, int | float, int | Not supported | |
|[Unsqueeze](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Unsqueeze) | float, int | float, int | float | |
|[Upsample (deprecated)](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Upsample) | float | float (2D and 3D only) | float (2D only) | The operator maps to the Sentis layer `Resize`. |
|[Where](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Where) | (int, float, float) - outputs float, (int, int, int) - outputs int | (int, float, float)  - outputs float, (int, int, int) - outputs int | Not supported | |
|[Xor](https://github.com/onnx/onnx/blob/main/docs/Operators.md#Xor) | int | int | Not supported | |

### Sentis-only layers

Sentis might create the following layers when it [optimizes the model](models-concept.md).

|Name|Supported data types with `BackendType.CPU`|Supported data types with `BackendType.GPUCompute`|Supported data types with `BackendType.GPUPixel`|
|-|-|-|-|
|Dense | float | float | Not supported |
|MatMul2D | float | float | float |
|Relu6 | float | float | float |
|Square | float | float | float |
|Swish | float | float | float |

## Unsupported operators

The following ONNX operators aren't supported in the current version of Sentis.

- BitShift
- ConcatFromSequence
- ConvInteger
- DequantizeLinear
- Det
- DynamicQuantizeLinear
- EyeLike
- If
- GRU
- Loop
- LpPool
- MatMulInteger
- MaxUnpool
- MeanVarianceNormalization
- NegativeLogLikelihoodLoss
- Optional
- OptionalGetElement
- OptionalHasElement
- QLinearConv
- QLinearMatMul
- QuantizeLinear
- ReverseSequence
- RNN
- Scan
- SequenceAt
- SequenceConstruct
- SequenceEmpty
- SequenceErase
- SequenceInsert
- SequenceLength
- SoftmaxCrossEntropyLoss
- SplitToSequence
- StringNormalizer
- TfIdfVectorizer
- Unique

## Additional resources

- [ONNX operator schemas](https://github.com/onnx/onnx/blob/main/docs/Operators.md)
- [Export an ONNX file from a machine learning framework](export-an-onnx-file.md)
- [Profile a model](profile-a-model.md)

