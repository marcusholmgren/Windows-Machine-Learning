﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.AI.MachineLearning;
using Windows.Media;

namespace MNIST_Demo
{
    public sealed partial class MainPage : Page
    {
        private modelModel              modelGen = new modelModel();
        private modelInput              modelInput = new modelInput();
        private modelOutput             modelOutput = new modelOutput();
        //private LearningModelSession    session;
        private Helper                  helper = new Helper();
        RenderTargetBitmap              renderBitmap = new RenderTargetBitmap();

        public MainPage()
        {
            this.InitializeComponent();
            
            // Set supported inking device types.
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(
                new Windows.UI.Input.Inking.InkDrawingAttributes()
                {
                    Color = Windows.UI.Colors.White,
                    Size = new Size(22, 22),
                    IgnorePressure = true,
                    IgnoreTilt = true,
                }
            );
            LoadModel();
        }

        private async void LoadModel()
        {
            //Load a machine learning model
            //StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/model.onnx"));
            Uri uri = new Uri("ms-appx:///Assets/model.onnx");
            StorageFile stf = await StorageFile.GetFileFromApplicationUriAsync(uri);
            modelGen = await modelModel.CreateFromFilePathAsync(stf.Path);
        }

        private async void recognizeButton_Click(object sender, RoutedEventArgs e)
        {
            //Bind model input with contents from InkCanvas
            VideoFrame vf = await helper.GetHandWrittenImage(inkGrid);
            modelInput.Input3 = ImageFeatureValue.CreateFromVideoFrame(vf);
            //Evaluate the model
            modelOutput = await modelGen.Evaluate(modelInput);

            //Convert output to datatype
            IReadOnlyList<float> VectorImage = modelOutput.Plus214_Output_0.GetAsVectorView();
            IList<float> ImageList = VectorImage.ToList();

            //LINQ query to check for highest probability digit
            var maxIndex = ImageList.IndexOf(ImageList.Max());

            //Display the results
            numberLabel.Text = maxIndex.ToString();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            numberLabel.Text = "";
        }
    }
}
