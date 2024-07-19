using System;

namespace Unity.Sentis.Layers
{
    /// <summary>
    /// Represents an element-wise activation layer.
    /// </summary>
    [Serializable]
    public abstract class Activation : Layer
    {
        protected Activation(string name, string input)
        {
            this.name = name;
            this.inputs = new[] { input };
        }

        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return inputShapes[0];
        }
    }

    /// <summary>
    /// Represents an element-wise `Celu` activation layer: f(x) = max(0, x) + min(0, alpha * (exp(x / alpha) - 1)).
    /// </summary>
    [Serializable]
    public class Celu : Activation
    {
        /// <summary>
        /// The alpha value to use for the `Celu` activation function.
        /// </summary>
        public float alpha;

        /// <summary>
        /// Initializes and returns an instance of `Celu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `Celu` activation function.</param>
        public Celu(string name, string input, float alpha)
            : base(name, input)
        {
            this.alpha = alpha;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Celu(inputTensors[0] as TensorFloat, alpha);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}";
        }

        internal override string profilerTag => "Celu";
    }

    /// <summary>
    /// Represents an element-wise `Elu` activation layer: f(x) = x if x >= 0, otherwise f(x) = alpha * (e^x - 1).
    /// </summary>
    [Serializable]
    public class Elu : Activation
    {
        /// <summary>
        /// The alpha value to use for the `Elu` activation function.
        /// </summary>
        public float alpha;

        /// <summary>
        /// Initializes and returns an instance of `Elu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `Elu` activation function.</param>
        public Elu(string name, string input, float alpha = 1.0f)
            : base(name, input)
        {
            this.alpha = alpha;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Elu(inputTensors[0] as TensorFloat, alpha);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}";
        }

        internal override string profilerTag => "Elu";
    }

    /// <summary>
    /// Represents an element-wise `Gelu` activation layer: f(x) = x / 2 * (1 + erf(x / sqrt(2))).
    /// </summary>
    [Serializable]
    public class Gelu : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Gelu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Gelu(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Gelu(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Gelu";
    }

    /// <summary>
    /// Represents an element-wise `Erf` activation layer: f(x) = erf(x).
    /// </summary>
    [Serializable]
    public class Erf : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Erf` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Erf(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Erf(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Erf";
    }

    /// <summary>
    /// Represents a `Hardmax` activation layer along an axis: f(x, axis) = 1 if x is the first maximum value along the specified axis, otherwise f(x) = 0.
    /// </summary>
    [Serializable]
    public class Hardmax : Activation
    {
        /// <summary>
        /// The axis along which to apply the `Hardmax` activation function.
        /// </summary>
        public int axis;

        /// <summary>
        /// Initializes and returns an instance of `Hardmax` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="axis">The axis along which to apply the `Hardmax` activation function.</param>
        public Hardmax(string name, string input, int axis = -1)
            : base(name, input)
        {
            this.axis = axis;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Hardmax(inputTensors[0] as TensorFloat, axis);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, axis: {axis}";
        }

        internal override string profilerTag => "Hardmax";
    }

    /// <summary>
    /// Represents an element-wise `HardSigmoid` activation layer: f(x) = clamp(alpha * x + beta, 0, 1).
    /// </summary>
    [Serializable]
    public class HardSigmoid : Activation
    {
        /// <summary>
        /// The alpha value to use for the `HardSigmoid` activation function.
        /// </summary>
        public float alpha;
        /// <summary>
        /// The beta value to use for the `HardSigmoid` activation function.
        /// </summary>
        public float beta;

        /// <summary>
        /// Initializes and returns an instance of `HardSigmoid` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `HardSigmoid` activation function. The default value is 0.1.</param>
        /// <param name="beta">The beta value to use for the `HardSigmoid` activation function. The default value is 0.5.</param>
        public HardSigmoid(string name, string input, float alpha = 0.2f, float beta = 0.5f)
            : base(name, input)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.HardSigmoid(inputTensors[0] as TensorFloat, alpha, beta);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}, beta: {beta}";
        }

        internal override string profilerTag => "HardSigmoid";
    }

    /// <summary>
    /// Represents an element-wise `HardSwish` activation layer: f(x) = x * max(0, min(1, alpha * x + beta)) = x * HardSigmoid(x, alpha, beta), where alpha = 1/6 and beta = 0.5.
    /// </summary>
    [Serializable]
    public class HardSwish : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `HardSwish` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public HardSwish(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.HardSwish(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "HardSwish";
    }

    /// <summary>
    /// Represents an element-wise `LeakyRelu` activation layer: f(x) = x if x >= 0, otherwise f(x) = alpha * x.
    /// </summary>
    [Serializable]
    public class LeakyRelu : Activation
    {
        /// <summary>
        /// The alpha value to use for the `LeakyRelu` activation function.
        /// </summary>
        public float alpha;

        /// <summary>
        /// Initializes and returns an instance of `LeakyRelu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `LeakyRelu` activation function. The default value is 0.01.</param>
        public LeakyRelu(string name, string input, float alpha = 0.01f)
            : base(name, input)
        {
            this.alpha = alpha;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.LeakyRelu(inputTensors[0] as TensorFloat, alpha);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}";
        }

        internal override string profilerTag => "LeakyRelu";
    }

    /// <summary>
    /// Represents an element-wise `PRelu` activation layer: f(x) = x if x >= 0, otherwise f(x) = slope * x.
    ///
    /// The slope tensor must be unidirectional broadcastable to x.
    /// </summary>
    [Serializable]
    public class PRelu : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `PRelu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the first input tensor of the layer.</param>
        /// <param name="slope">The name to use for the slope input tensor of the layer.</param>
        public PRelu(string name, string input, string slope)
            : base(name, input)
        {
            this.name = name;
            this.inputs = new[] { input, slope };
        }

        /// <inheritdoc/>
        internal override SymbolicTensorShape InferOutputShape(SymbolicTensorShape[] inputShapes, ShapeInferenceContext ctx)
        {
            return SymbolicInference.PRelu(inputShapes[0], inputShapes[1]);
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.PRelu(inputTensors[0] as TensorFloat, inputTensors[1] as TensorFloat);
        }

        internal override string profilerTag => "PRelu";
    }

    /// <summary>
    /// Represents an element-wise `Relu` activation layer: f(x) = max(0, x).
    /// </summary>
    [Serializable]
    public class Relu : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Relu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Relu(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Relu(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Relu";
    }

    /// <summary>
    /// Represents an element-wise `Relu6` activation layer: f(x) = clamp(x, 0, 6).
    /// </summary>
    [Serializable]
    public class Relu6 : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Relu6` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Relu6(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Relu6(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Relu6";
    }

    /// <summary>
    /// Represents an element-wise `Selu` activation layer: f(x) = gamma * x if x >= 0, otherwise f(x) = (alpha * e^x - alpha).
    /// </summary>
    [Serializable]
    public class Selu : Activation
    {
        /// <summary>
        /// The alpha value to use for the `Selu` activation function.
        /// </summary>
        public float alpha;
        /// <summary>
        /// The gamma value to use for the `Selu` activation function.
        /// </summary>
        public float gamma;

        /// <summary>
        /// Initializes and returns an instance of `Selu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `Selu` activation function. The default value is 1.67326.</param>
        /// <param name="gamma">The gamma value to use for the `Selu` activation function. The default value is 1.0507.</param>
        public Selu(string name, string input, float alpha = 1.67326f, float gamma = 1.0507f)
            : base(name, input)
        {
            this.alpha = alpha;
            this.gamma = gamma;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Selu(inputTensors[0] as TensorFloat, alpha, gamma);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}, gamma: {gamma}";
        }

        internal override string profilerTag => "Selu";
    }

    /// <summary>
    /// Represents an element-wise `Sigmoid` activation layer: f(x) = 1/(1 + e^(-x)).
    /// </summary>
    [Serializable]
    public class Sigmoid : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Sigmoid` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Sigmoid(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Sigmoid(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Sigmoid";
    }

    /// <summary>
    /// Represents an element-wise `Softplus` activation layer: f(x) = ln(e^x + 1).
    /// </summary>
    [Serializable]
    public class Softplus : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Softplus` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Softplus(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Softplus(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Softplus";
    }

    /// <summary>
    /// Represents an element-wise `Softsign` activation layer: f(x) = x/(|x| + 1).
    /// </summary>
    [Serializable]
    public class Softsign : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Softsign` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Softsign(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Softsign(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Softsign";
    }

    /// <summary>
    /// Represents an element-wise `Swish` activation layer. f(x) = sigmoid(x) * x = x / (1 + e^{-x}).
    /// </summary>
    [Serializable]
    public class Swish : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Swish` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Swish(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Swish(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Swish";
    }

    /// <summary>
    /// Represents an element-wise `Tanh` activation layer: f(x) = tanh(x).
    /// </summary>
    [Serializable]
    public class Tanh : Activation
    {
        /// <summary>
        /// Initializes and returns an instance of `Tanh` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        public Tanh(string name, string input)
            : base(name, input) { }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.Tanh(inputTensors[0] as TensorFloat);
        }

        internal override string profilerTag => "Tanh";
    }

    /// <summary>
    /// Represents an element-wise `ThresholdedRelu` activation layer: f(x) = x if x > alpha, otherwise f(x) = 0.
    /// </summary>
    [Serializable]
    public class ThresholdedRelu : Activation
    {
        /// <summary>
        /// The alpha value to use for the `ThresholdedRelu` activation function.
        /// </summary>
        public float alpha;

        /// <summary>
        /// Initializes and returns an instance of `ThresholdedRelu` activation layer.
        /// </summary>
        /// <param name="name">The name to use for the output tensor of the layer.</param>
        /// <param name="input">The name to use for the input tensor of the layer.</param>
        /// <param name="alpha">The alpha value to use for the `ThresholdedRelu` activation function.</param>
        public ThresholdedRelu(string name, string input, float alpha)
            : base(name, input)
        {
            this.alpha = alpha;
        }

        /// <inheritdoc/>
        public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
        {
            return ctx.ops.ThresholdedRelu(inputTensors[0] as TensorFloat, alpha);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{base.ToString()}, alpha: {alpha}";
        }

        internal override string profilerTag => "ThresholdedRelu";
    }
}
