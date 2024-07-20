In this Jupyter notebook I train a simple CNN using Q-MNIST dataset to predict hand written digits. Training runs on GPU if CUDA is available, else CPU. 

Using pytorch's onnx module the the CNN model is converted to ONNX format which Unity3D's Sentis neural network can use for inference in an app.

Unity's Sentis neural network runs inference on the model.