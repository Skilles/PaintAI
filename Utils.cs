using TensorFlow;

namespace PaintAI
{
    public class Utils
    {
        public static TFTensor BitmapToTensorGrayScale(Bitmap bitmap)
        {
            // Resize to 28x28
            bitmap = new Bitmap(bitmap, new Size(28, 28));
            using (bitmap)
            {
                var matrix = new float[1, bitmap.Size.Height, bitmap.Size.Width, 1];
                for (var iy = 0; iy < bitmap.Size.Height; iy++)
                {
                    for (int ix = 0, index = iy * bitmap.Size.Width; ix < bitmap.Size.Width; ix++, index++)
                    {
                        Color pixel = bitmap.GetPixel(ix, iy);
                        matrix[0, iy, ix, 0] = pixel.B / 255.0f;
                        //matrix[0, iy, ix, 1] = pixel.Green() / 255.0f;
                        //matrix[0, iy, ix, 2] = pixel.Red() / 255.0f;
                    }
                }
                TFTensor tensor = matrix;
                return tensor;
            }
            
        }

        internal static int[] Quantized(float[,] results)
        {
            int[] q = new int[]
            {
                results[0,0]>0.5?1:0,
                results[0,1]>0.5?1:0,
                results[0,2]>0.5?1:0,
                results[0,3]>0.5?1:0,
                results[0,4]>0.5?1:0,
                results[0,5]>0.5?1:0,
                results[0,6]>0.5?1:0,
                results[0,7]>0.5?1:0,
                results[0,8]>0.5?1:0,
                results[0,9]>0.5?1:0,
            };
            return q;
        }

        internal static int[] GetSortedResults(float[,] results)
        {
            var mapped = new Dictionary<int, float>();
            for (int i = 0; i < 10; i++)
            {
                mapped.Add(i, results[0, i]);
            }
            var sorted = mapped.OrderByDescending(x => x.Value);
            var output = new int[10];
            var j = 0;
            foreach (var pair in sorted)
            {
                output[j] = pair.Key;
                j++;
            }

            return output;

        }

        internal static int GetIntegerFromQuantized(int[] quantized)
        {
            var index = Array.IndexOf(quantized, 1);
            if (index == -1)
            {
                throw new ArgumentException("Quantized data contained no valid outputs");
            }
            return index;
        }
    }
}