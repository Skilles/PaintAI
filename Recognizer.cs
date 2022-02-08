using Newtonsoft.Json;
using SimpleEngine;
using TensorFlow;


namespace PaintAI
{
    internal class Recognizer
    {
        private TFGraph graph;
        private TFSession session;

        private Label[] outputLabels;
        private PictureBoxRenderer boxRenderer;
        private Task<int[]> thread;

        private bool loaded;
        private bool recognizing;

        public Recognizer(PictureBoxRenderer boxRenderer, Label[] outputLabels)
        {
            this.outputLabels = outputLabels;
            this.boxRenderer = boxRenderer;
            LoadModel();
        }

        ~Recognizer()
        {
            Dispose();
        }
        public void Dispose()
        {
            session.Dispose();
            graph.Dispose();
        }
        private void LoadModel()
        {
            if (loaded)
            {
                return;
            }
            // Load model
            loaded = true;
            byte[] buffer = File.ReadAllBytes("C:\\Users\\super\\Desktop\\Code\\C# Projects\\SimpleGFX\\PaintAI\\Mnist_model_98.5success.pb");
            graph = new TFGraph();
            graph.Import(buffer);
            session = new TFSession(graph);
            
            outputLabels[0].Text = "Model loaded";
        }

        private int[] RecognizeImage()
        {
            var runner = session.GetRunner();
            var bitmap = boxRenderer.GetBitmap().ToTensorFormat();
            var tensor = Utils.BitmapToTensorGrayScale(bitmap);

            runner.AddInput(graph["conv1_input"][0], tensor);
            runner.Fetch(graph["activation_4/Softmax"][0]);

            var output = runner.Run();
            var vecResults = output[0].GetValue();
            float[,] results = (float[,])vecResults;

            // Evaluate the results
            // int max_result = Utils.GetIntegerFromQuantized(quantized);
            int[] int_results = Utils.GetSortedResults(results);

            tensor.Dispose();
            bitmap.Dispose();

            return int_results;
        }

        public void RecognizeImageAsync()
        {
            if (thread != null)
            {
                if (!thread.IsCompleted)
                {
                    return;
                }
                DisplayResults(thread.Result);
            }
            thread = new Task<int[]>(RecognizeImage);
            thread.Start();
        }

        public void DisplayResults(int[] results)
        {
            int minIndex = Math.Min(results.Length, outputLabels.Length);
            for (int i = 0; i < minIndex; i++)
            {
                string result = results[i].ToString();
                outputLabels[i].Text = result;
            }
        }
    }
}
