## Description
This Unity app runs inference on Unity on a convolutional neural network (CNN) that we train in a jupyter notebook to recognize a user's handwritten digits. 

<p align="center">
<img width="150" alt="star" src="https://github.com/sergiosolorzano/ai_gallery/assets/24430655/3c0b02ea-9b11-401a-b6f5-c61b69ad651b">
</p>

---------------------------------------------

## Setup
The project includes a jupyter notebook in [the Train-MNIST directory](https://github.com/sergiosolorzano/qmnist-unity-sentis-nn/tree/main/Train-MNIST) with a CNN trained to reconize handwritten digits on the Q-MNIST dataset. Training runs on GPU if CUDA is available, else CPU. Using pytorch's onnx module the the CNN model is converted to [ONNX format](https://docs.unity3d.com/Packages/com.unity.sentis@1.3/manual/convert-a-file-to-onnx.html#:~:text=Converting%20PyTorch%20files%20to%20ONNX&text=You%20will%20need%20to%20first,not%20contain%20the%20model%20graph.) which Unity3D's Sentis neural network can use for inference. I exported the trained model into Unity3D to predict the user's handwritten digits.

## Capturing the user's hand writing and running model inference on Sentis
The user's hand movements are tracked on world space with a Line Renderer gameobject when the user writes on screen. The writing is rendered to camera with a Render Texture gameobject. We read the pixel data from the Render Texture into a Texture2D. The Texture2D is converted into a tensor, which is then used by Sentis as input to perform inference with the model.

The model's predicted value is finally displayed to the user.

<video src="https://github.com/user-attachments/assets/1bc558f5-4936-41f7-8301-1e964bed4fa6" controls="controls" muted="muted" playsinline="playsinline">
      </video>

## Inference to Frame speed considerations

The app is setup for the user to press the button to run inference while game play pauses. This is because the running execution in a single frame would cause low or stuttering framerates in gameplay. Unity Sentis provides a a method to peek on the network's layers, keeping a balance game play required frames and inference use of resources. You can read more [here](https://docs.unity3d.com/Packages/com.unity.sentis@1.3/manual/run-a-model-a-layer-at-a-time.html).

## Acknowledgements
Thanks to Unity for the Sentis quick start samples.

If you find this helpful you can buy me a coffee :)
   
<a href="https://www.buymeacoffee.com/sergiosolorzano" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>
      
