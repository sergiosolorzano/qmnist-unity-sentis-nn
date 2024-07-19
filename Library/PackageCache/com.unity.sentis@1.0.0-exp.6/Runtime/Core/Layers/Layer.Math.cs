using System;

namespace Unity.Sentis.Layers
{
    /// <summary>
    /// Represents an element-wise `Abs` math layer: f(x) = |x|.
    /// </summary>
    [Serializable]
    public class Abs : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Abs` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Abs(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Abs(inputTensors[0] as TensorInt);
            else
                return ctx.ops.Abs(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Abs";
    }

    /// <summary>
    /// Represents an element-wise `Add` math operation layer: f(a, b) = a + b.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Add : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Add` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the first input tensor of the layer.</param>
        /// <param name="b">The name to use for the second input tensor of the layer.</param>
        public Add(string name, string a, string b)
            : base(name, a, b) { }

        internal override PartialTensor InferPartialTensor(PartialTensor[] partialTensors, ShapeInferenceContext ctx)
        {
            return PartialTensor.BroadcastWithOp(partialTensors[0], partialTensors[1], (a, b) => a + b);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Add(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else
                return ctx.ops.Add(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "Add";
    }

    /// <summary>
    /// Represents an element-wise `Ceil` math layer: f(x) = ceil(x).
    /// </summary>
    [Serializable]
    public class Ceil : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Ceil` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Ceil(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Ceil(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Ceil";
    }

    /// <summary>
    /// Represents an element-wise `Clip` math layer: f(x, xmin, xmax) = min(max(x, xmin), xmax)
    /// </summary>
    [Serializable]
    [Optimization.CPUFallback.CPUReadInputs(1, 2)]
    public class Clip : Layer
    {
        /// <summary>
        /// Initializes and returns an instance of `Clip` math layer with no min or max tensors.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Clip(string name, string input)
        {
            this.name = name;
            this.inputs = new[] { input };
        }

        /// <summary>
        /// Initializes and returns an instance of `Clip` math layer with no max tensor.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="min">The name to use for the min scalar tensor of the layer.</param>
        public Clip(string name, string input, string min)
        {
            this.name = name;
            this.inputs = new[] { input, min };
        }

        /// <summary>
        /// Initializes and returns an instance of `Clip` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="min">The name to use for the min scalar tensor of the layer.</param>
        /// <param name="max">The name to use for the max scalar tensor of the layer.</param>
        public Clip(string name, string input, string min, string max)
        {
            this.name = name;
            this.inputs = new[] { input, min, max };
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return inputShapes[0];
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputs, ExecutionContext ctx)
        {
            var min = inputs.Length > 1 && inputs[1] != null ? (inputs[1] as TensorFloat)[0] : float.MinValue;
            var max = inputs.Length > 2 && inputs[2] != null ? (inputs[2] as TensorFloat)[0] : float.MaxValue;
            return ctx.ops.Clip(inputs[0] as TensorFloat, min, max);
        }

        internal override string profilerTag => "Clip";
    }

    /// <summary>
    /// Represents a `CumSum` math layer that performs the cumulative sum along a given axis.
    /// </summary>
    [Serializable]
    [Optimization.CPUFallback.CPUReadInputs(1)]
    public class CumSum : Layer
    {
        /// <summary>
        /// Whether to perform the cumulative sum from the end of the axis.
        /// </summary>
        public bool reverse;
        /// <summary>
        /// Whether to include the respective input element in the cumulative sum.
        /// </summary>
        public bool exclusive;

        /// <summary>
        /// Initializes and returns an instance of `CumSum` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="axis">The name to use for the axis scalar tensor along which to perform the cumulative sum.</param>
        /// <param name="reverse">Whether to perform the cumulative sum from the end of the axis.</param>
        /// <param name="exclusive">Whether to include the respective input element in the cumulative sum.</param>
        public CumSum(string name, string input, string axis, bool reverse, bool exclusive)
        {
            this.name = name;
            this.inputs = new[] { input, axis };
            this.reverse = reverse;
            this.exclusive = exclusive;
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return inputShapes[0];
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.CumSum(inputTensors[0] as TensorInt, (inputTensors[1] as TensorInt)[0], reverse, exclusive);
            else
                return ctx.ops.CumSum(inputTensors[0] as TensorFloat, (inputTensors[1] as TensorInt)[0], reverse, exclusive);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, reverse: {reverse}, exclusive: {exclusive}";
        }

        internal override string profilerTag => "Hardmax";
    }

    /// <summary>
    /// Represents a `Dense` math operation layer which performs a matrix multiplication operation: f(x, w, b) = X x W + B.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Dense : FusedActivation
    {
        /// <summary>
        /// Initializes and returns an instance of `Dense` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the first input tensor of the layer.</param>
        /// <param name="weights">The name to use for the weights input tensor of the layer.</param>
        /// <param name="bias">The name to use for the bias input tensor of the layer.</param>
        /// <param name="fusedActivation">The fusable activation to apply to the output tensor of the layer.</param>
        public Dense(string name, string input, string weights, string bias, FusableActivation fusedActivation = FusableActivation.None)
        {
            this.name = name;
            inputs = new[] { input, weights, bias };
            this.fusedActivation = fusedActivation;
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return SymbolicInference.Dense(inputShapes[0], inputShapes[1], inputShapes[2]);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Dense(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat, inputTensors[2] as TensorFloat, fusedActivation);
        }

        internal override string profilerTag => "Dense";
    }

    /// <summary>
    /// Represents an element-wise `Div` math operation layer: f(a, b) = a / b.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Div : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Div` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the numerator input tensor of the layer.</param>
        /// <param name="b">The name to use for the denominator input tensor of the layer.</param>
        public Div(string name, string a, string b)
            : base(name, a, b) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Div(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else
                return ctx.ops.Div(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "Div";
    }

    /// <summary>
    /// Represents an `Einsum` math operation layer.
    /// </summary>
    /// <description>
    /// The Einsum operator evaluates algebraic tensor operations on a sequence of tensors, using the Einstein summation convention. The equation string contains a comma-separated sequence of lower case letters. Each term corresponds to an operand tensor, and the characters within the terms correspond to operands dimensions.
    /// This sequence may be followed by "->" to separate the left and right hand side of the equation. If the equation contains "->" followed by the right-hand side, the explicit (not classical) form of the Einstein summation is performed, and the right-hand side indices indicate output tensor dimensions. In other cases, output indices are (implicitly) set to the alphabetically sorted sequence of indices appearing exactly once in the equation.
    /// When a dimension character is repeated in the left-hand side, it represents summation along the dimension.
    /// The equation may contain ellipsis ("...") to enable broadcasting. Ellipsis must indicate a fixed number of dimensions. Specifically, every occurrence of ellipsis in the equation must represent the same number of dimensions. The right-hand side may contain exactly one ellipsis. In implicit mode, the ellipsis dimensions are set to the beginning of the output. The equation string may contain space (U+0020) character.
    /// </description>
    [Serializable]
    public class Einsum : Layer
    {
        /// <summary>
        /// The equation of the Einstein summation as a comma-separated list of subscript labels.
        /// </summary>
        public string equation;

        /// <summary>
        /// Initializes and returns an instance of `Einsum` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="inputs">The names to use for the input tensors of the layer.</param>
        /// <param name="equation">The equation of the Einstein summation as a comma-separated list of subscript labels.</param>
        public Einsum(string name, string[] inputs, string equation)
        {
            this.name = name;
            this.inputs = inputs;
            this.equation = equation;
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            var operandIndices = new TensorIndex[inputShapes.Length];
            return EinsumHelper.ParseEquationStringShape(equation, inputShapes, ref operandIndices, out _, out _);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Einsum(equation, Array.ConvertAll(inputTensors, i => i as TensorFloat));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, equation: {equation}";
        }

        internal override string profilerTag => "Einsum";
    }

    /// <summary>
    /// Represents an element-wise `Exp` math layer: f(x) = e^{x}.
    /// </summary>
    [Serializable]
    public class Exp : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Exp` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Exp(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Exp(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Exp";
    }

    /// <summary>
    /// Represents an element-wise `Floor` math layer: f(x) = floor(x).
    /// </summary>
    [Serializable]
    public class Floor : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Floor` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Floor(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Floor(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Floor";
    }

    /// <summary>
    /// Represents an element-wise `Log` math layer: f(x) = log(x).
    /// </summary>
    [Serializable]
    public class Log : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Log` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Log(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Log(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Log";
    }

    /// <summary>
    /// Represents a `MatMul` math operation layer which performs a matrix multiplication operation: f(a, b) = a x b.
    /// </summary>
    [Serializable]
    public class MatMul : Layer
    {
        /// <summary>
        /// Initializes and returns an instance of `MatMul` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input0">The name to use for the first input tensor of the layer.</param>
        /// <param name="input1">The name to use for the second input tensor of the layer.</param>
        public MatMul(string name, string input0, string input1)
        {
            this.name = name;
            this.inputs = new[] { input0, input1 };
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return SymbolicInference.MatMul(inputShapes[0], inputShapes[1]);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.MatMul(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "MatMul";
    }

    /// <summary>
    /// Represents a `MatMul2D` math operation layer which performs a matrix multiplication operation with optional transposes: f(a, b) = a' x b'.
    /// </summary>
    [Serializable]
    public class MatMul2D : Layer
    {
        /// <summary>
        /// Whether to transpose the first input before performing the matrix multiplication.
        /// </summary>
        public bool transposeA;
        /// <summary>
        /// Whether to transpose the second input before performing the matrix multiplication.
        /// </summary>
        public bool transposeB;

        /// <summary>
        /// Initializes and returns an instance of `MatMul2D` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input0">The name to use for the first input tensor of the layer.</param>
        /// <param name="transpose0">Whether to transpose the first input before performing the matrix multiplication.</param>
        /// <param name="input1">The name to use for the second input tensor of the layer.</param>
        /// <param name="transpose1">Whether to transpose the second input before performing the matrix multiplication.</param>
        public MatMul2D(string name, string input0, bool transpose0, string input1, bool transpose1)
        {
            this.name = name;
            this.inputs = new[] { input0, input1 };
            this.transposeA = transpose0;
            this.transposeB = transpose1;
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return SymbolicInference.MatMul2D(inputShapes[0], transposeA, inputShapes[1], transposeB);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.MatMul2D(inputTensors[0] as TensorFloat, transposeA, inputTensors[1] as TensorFloat, transposeB);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, transposeA: {transposeA}, transposeB: {transposeB}";
        }

        internal override string profilerTag => "MatMul2D";
    }

    /// <summary>
    /// Represents an element-wise `Max` math operation layer: f(x1, x2 ... xn) = max(x1, x2 ... xn).
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Max : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Max` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="inputs">The array of names to use for the input tensors of the layer.</param>
        public Max(string name, string[] inputs)
            : base(name, inputs) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Max(Array.ConvertAll(inputTensors, i => i as TensorInt));
            else
                return ctx.ops.Max(Array.ConvertAll(inputTensors, i => i as TensorFloat));
        }

        internal override string profilerTag => "Max";
    }

    /// <summary>
    /// Represents an element-wise `Mean` math operation layer: f(x1, x2 ... xn) = (x1 + x2 ... xn) / n.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Mean : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Mean` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="inputs">The array of names to use for the input tensors of the layer.</param>
        public Mean(string name, string[] inputs)
            : base(name, inputs) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Mean(Array.ConvertAll(inputTensors, i => i as TensorFloat));
        }

        internal override string profilerTag => "Mean";
    }

    /// <summary>
    /// Represents an element-wise `Min` math operation layer: f(x1, x2 ... xn) = min(x1, x2 ... xn).
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Min : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Min` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="inputs">The array of names to use for the input tensors of the layer.</param>
        public Min(string name, string[] inputs)
            : base(name, inputs) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Min(Array.ConvertAll(inputTensors, i => i as TensorInt));
            else
                return ctx.ops.Min(Array.ConvertAll(inputTensors, i => i as TensorFloat));
        }

        internal override string profilerTag => "Min";
    }

    /// <summary>
    /// Represents an element-wise `Max` math operation layer: f(a, b) = a % b.
    ///
    /// If fmod is false the sign of the remainder is the same as that of the divisor as in Python.
    ///
    /// If fmod is true the sign of the remainder is the same as that of the dividend as in C#.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Mod : Broadcast
    {
        /// <summary>
        /// Whether to have the sign of the remainder the same as that of the dividend rather than that of the divisor.
        /// </summary>
        public bool fmod;

        /// <summary>
        /// Initializes and returns an instance of `Mod` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the divisor input tensor of the layer.</param>
        /// <param name="b">The name to use for the dividend input tensor of the layer.</param>
        /// <param name="fmod">Whether to have the sign of the remainder the same as that of the dividend rather than that of the divisor.</param>
        public Mod(string name, string a, string b, bool fmod = false)
            : base(name, a, b)
        {
            this.fmod = fmod;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (!fmod)
                return ctx.ops.Mod(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else if (inputTensors[0] is TensorInt)
                return ctx.ops.FMod(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else
                return ctx.ops.FMod(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, fmod: {fmod}";
        }

        internal override string profilerTag => "Mod";
    }

    /// <summary>
    /// Represents an element-wise `Mul` math operation layer: f(a, b) = a * b.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Mul : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Mul` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the first input tensor of the layer.</param>
        /// <param name="b">The name to use for the second input tensor of the layer.</param>
        public Mul(string name, string a, string b)
            : base(name, a, b) { }

        internal override PartialTensor InferPartialTensor(PartialTensor[] partialTensors, ShapeInferenceContext ctx)
        {
            return PartialTensor.BroadcastWithOp(partialTensors[0], partialTensors[1], (a, b) => a * b);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Mul(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else
                return ctx.ops.Mul(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "Mul";
    }

    /// <summary>
    /// Represents an element-wise `Neg` math layer: f(x) = -x.
    /// </summary>
    [Serializable]
    public class Neg : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Neg` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Neg(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Neg(inputTensors[0] as TensorInt);
            else
                return ctx.ops.Neg(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Neg";
    }

    /// <summary>
    /// Represents an element-wise `Pow` math operation layer: f(a, b) = pow(a, b).
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Pow : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Pow` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the first input tensor of the layer.</param>
        /// <param name="b">The name to use for the second input tensor of the layer.</param>
        public Pow(string name, string a, string b)
            : base(name, a, b) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[1] is TensorFloat)
                return ctx.ops.Pow(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
            else
                return ctx.ops.Pow(inputTensors[0] as TensorFloat, inputTensors[1] as TensorInt);
        }

        internal override string profilerTag => "Pow";
    }

    /// <summary>
    /// Represents an element-wise `Reciprocal` math layer: f(x) = 1 / x.
    /// </summary>
    [Serializable]
    public class Reciprocal : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Reciprocal` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Reciprocal(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Reciprocal(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Reciprocal";
    }

    /// <summary>
    /// Represents an element-wise `Round` math layer: f(x) = round(x).
    /// </summary>
    [Serializable]
    public class Round : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Round` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Round(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Round(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Round";
    }

    /// <summary>
    /// Represents an element-wise `Shrink` math layer: f(x) = x + bias if x &lt; lambd. f(x) = x - bias if x &gt; lambd. Otherwise f(x) = 0.
    /// </summary>
    [Serializable]
    public class Shrink : Layer
    {
        /// <summary>
        /// The value of the bias for the shrink function.
        /// </summary>
        public float bias;
        /// <summary>
        /// The value of lambda for the shrink function.
        /// </summary>
        public float lambd;

        /// <summary>
        /// Initializes and returns an instance of `Shrink` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="bias">The value of the bias for the shrink function.</param>
        /// <param name="lambd">The value of lambda for the shrink function.</param>
        public Shrink(string name, string input, float bias, float lambd)
        {
            this.name = name;
            inputs = new[] { input };
            this.bias = bias;
            this.lambd = lambd;
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return inputShapes[0];
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Shrink(inputTensors[0] as TensorFloat, bias, lambd);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, bias: {bias}, lambd: {lambd}";
        }

        internal override string profilerTag => "Shrink";
    }

    /// <summary>
    /// Represents an element-wise `Sign` math layer: f(x) = 1 if x > 0. f(x) = -1 if x &lt; 0. Otherwise f(x) = 0.
    /// </summary>
    [Serializable]
    public class Sign : Layer
    {
        /// <summary>
        /// Initializes and returns an instance of `Sign` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Sign(string name, string input)
        {
            this.name = name;
            this.inputs = new[] { input };
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return inputShapes[0];
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Sign(inputTensors[0] as TensorInt);
            else
                return ctx.ops.Sign(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Sign";
    }

    /// <summary>
    /// Represents an element-wise `Sqrt` math layer: f(x) = sqrt(x).
    /// </summary>
    [Serializable]
    public class Sqrt : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Sqrt` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Sqrt(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Sqrt(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Sqrt";
    }

    /// <summary>
    /// Represents an element-wise `Square` math layer: f(x) = x * x.
    /// </summary>
    [Serializable]
    public class Square : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Square` math layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Square(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Square(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Square";
    }

    /// <summary>
    /// Represents an element-wise `Sub` math operation layer: f(a, b) = a - b.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Sub : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Sub` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="a">The name to use for the first input tensor of the layer.</param>
        /// <param name="b">The name to use for the second input tensor of the layer.</param>
        public Sub(string name, string a, string b)
            : base(name, a, b) { }

        internal override PartialTensor InferPartialTensor(PartialTensor[] partialTensors, ShapeInferenceContext ctx)
        {
            return PartialTensor.BroadcastWithOp(partialTensors[0], partialTensors[1], (a, b) => a - b);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            if (inputTensors[0] is TensorInt)
                return ctx.ops.Sub(inputTensors[0] as TensorInt, inputTensors[1] as TensorInt);
            else
                return ctx.ops.Sub(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "Sub";
    }

    /// <summary>
    /// Represents an element-wise `Sum` math operation layer: f(x1, x2 ... xn) = x1 + x2 ... xn.
    ///
    /// This supports numpy-style broadcasting of input tensors.
    /// </summary>
    [Serializable]
    public class Sum : Broadcast
    {
        /// <summary>
        /// Initializes and returns an instance of `Sum` math operation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="inputs">The array of names to use for the input tensors of the layer.</param>
        public Sum(string name, string[] inputs)
            : base(name, inputs) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Sum(Array.ConvertAll(inputTensors, i => i as TensorFloat));
        }

        internal override string profilerTag => "Sum";
    }
}
