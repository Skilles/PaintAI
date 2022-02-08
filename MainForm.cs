using SimpleEngine;


namespace PaintAI
{
    public partial class MainForm : Form
    {
        private readonly PictureBoxRenderer boxRenderer;
        private readonly Recognizer recognizer;
        private bool drawing;
        private int brushSize = 1;
        private int recognizeTimer;

        private const int REC_RATE = 100;


        public MainForm()
        {
            InitializeComponent();
            this.draw_box.MouseWheel += new MouseEventHandler(draw_box_MouseWheel);

            this.boxRenderer = new PictureBoxRenderer(draw_box);
            this.recognizer = new Recognizer(this.boxRenderer, new Label[8] { pred_1, pred_2, pred_3, pred_4, pred_5, pred_6, pred_7, pred_8 });
            boxRenderer.Clear(Color.White);
        }
        

        // Mouse events

        private void draw_box_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                boxRenderer.DrawPixelAndUpdate(e.X, e.Y, brushSize, color_dialog.Color);
            }
            else if (e.Button == MouseButtons.Right)
            {
                drawing = false;
                boxRenderer.Clear(Color.White);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                pred_1.Text = boxRenderer.GetBitmap().GetPixel(e.X, e.Y).Name;
            }
        }

        private void draw_box_MouseWheel(object sender, MouseEventArgs e)
        {
            brushSize = Math.Max(1, brushSize + e.Delta / 120);
            brush_label.Text = "Brush Size: " + brushSize;
        }

        private void draw_box_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                boxRenderer.DrawPixelAndUpdate(e.X, e.Y, brushSize, color_dialog.Color);
                if (recognizeTimer++ == REC_RATE)
                {
                    recognizer.RecognizeImageAsync();
                    recognizeTimer = 0;
                }
            }
        }

        private void draw_box_MouseLeave(object sender, EventArgs e)
        {
            drawing = false;
        }

        private void draw_box_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
            recognizer.RecognizeImageAsync();
        }

        private void color_button_Click(object sender, EventArgs e)
        {
            color_dialog.ShowDialog();
        }

        private void recognize_button_Click(object sender, EventArgs e)
        {
            recognizer.RecognizeImageAsync();
        }
    }
}