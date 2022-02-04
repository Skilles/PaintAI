using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleEngine;

#if USE_WINML_NUGET
using Microsoft.AI.MachineLearning;
#else
using Windows.AI.MachineLearning;
#endif

namespace PaintAI
{
    internal class Recognizer
    {
        private mnistModel modelGen;
        private mnistInput mnistInput = new mnistInput();
        private mnistOutput mnistOutput;
        private Helper helper = new Helper();
        public async Task LoadModelAsync()
        {
            //Load a machine learning model
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///mnist.onnx"));
            modelGen = await mnistModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);
        }

        public async void recognizeImage(PictureBoxRenderer boxRenderer, Label outputLabel)
        {
            //Bind model input with contents from InkCanvas
            VideoFrame vf = await helper.GetHandWrittenImage(boxRenderer);
            mnistInput.Input3 = ImageFeatureValue.CreateFromVideoFrame(vf);

            //Evaluate the model
            mnistOutput = await modelGen.EvaluateAsync(mnistInput);

            //Convert output to datatype
            IReadOnlyList<float> vectorImage = mnistOutput.Plus214_Output_0.GetAsVectorView();
            IList<float> imageList = vectorImage.ToList();

            //LINQ query to check for highest probability digit
            var maxIndex = imageList.IndexOf(imageList.Max());

            //Display the results
            outputLabel.Text = maxIndex.ToString();
        }
    }
}
