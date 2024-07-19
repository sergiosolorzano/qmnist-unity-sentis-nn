## Description
This Unity app runs inference on Unity on a convolutional neural network (CNN) that we train in a jupyter notebook to recognize a user's handwritten digits. 

## Setup
The project includes a jupyter notebook in [the Train-MNIST directory](https://github.com/sergiosolorzano/qmnist-unity-sentis-nn/tree/main/Train-MNIST) with a CNN trained to reconize handwritten digits on the Q-MNIST dataset. Training runs on GPU if CUDA is available, else CPU. Using pytorch's onnx module the the CNN model is converted to [ONNX format](https://docs.unity3d.com/Packages/com.unity.sentis@1.3/manual/convert-a-file-to-onnx.html#:~:text=Converting%20PyTorch%20files%20to%20ONNX&text=You%20will%20need%20to%20first,not%20contain%20the%20model%20graph.) which Unity3D's Sentis neural network can use for inference. I exported the trained model into Unity3D to predict the user's handwritten digits.

## Capturing the user's hand writing and running model inference on Sentis
The user's hand movements are tracked on world space with a Line Renderer gameobject when the user writes on screen. The writing is rendered to camera with a Render Texture gameobject. We read the pixel data from the Render Texture into a Texture2D. The Texture2D is converted into a tensor, which is then used by Sentis as input to perform inference with the model.

The model's predicted value is finally displayed to the user.

<video src="https://github.com/user-attachments/assets/1bc558f5-4936-41f7-8301-1e964bed4fa6" controls="controls" muted="muted" playsinline="playsinline">
      </video>


## Acknowledgements
Thanks to Unity for the Sentis quick start samples.

