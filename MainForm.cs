using SimpleEngine;


namespace PaintAI
{
    public partial class MainForm : Form
    {
        private readonly PictureBoxRenderer boxRenderer;
        private bool drawing;
        private int brushSize = 1;


        public MainForm()
        {
            InitializeComponent();
            this.draw_box.MouseWheel += new MouseEventHandler(draw_box_MouseWheel);

            this.boxRenderer = new PictureBoxRenderer(draw_box);
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
            }
        }

        private void draw_box_MouseLeave(object sender, EventArgs e)
        {
            drawing = false;
        }

        private void draw_box_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void color_button_Click(object sender, EventArgs e)
        {
            color_dialog.ShowDialog();
        }
    }
}