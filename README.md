## Description
I was part of Unity's Sentis Neural Network closed-beta users from April-September 2023. During this time I tested Sentis for different functionality, one of them running well-known MNIST model to recognize hand written digits.

This app runs inference on Unity on a convolutional neural network (CNN) to recognize a user's handwritten digits. 

## Setup
The project includes a jupyter notebook with a CNN I trained to reconize handwritten digits on the Q-MNIST dataset. Training runs on GPU if CUDA is available, else CPU. Using pytorch's onnx module I convert the CNN model to onnx format which Unity3D's Sentis neural network can use for inference. I exported the trained model into Unity3D to predict the user's handwritten digits.

## Capturing the user's hand writing and running model inference on Sentis
The user's hand movements are tracked on world space via a Line Renderer gameobject when the user writes on screen with the human-computer interface of choice. The writing is rendered to camera with a Render Texture. We then read the pixel data from the Render Texture into a Texture2D rendered on screen for the user with a Sprite.

The input Texture2D is converted into a tensor for Sentis to run inference on it.

The model's predicted value is finally displayed to the user.
