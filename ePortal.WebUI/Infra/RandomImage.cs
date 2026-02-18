using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ePortal.WebUI.Infra
{
    public class RandomImage
    {
        //Default Constructor 
        public RandomImage() { }
        //property
        public string Text
        {
            get { return this.text; }
        }
        public Bitmap Image
        {
            get { return this.image; }
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        //Private variable
        private string text;
        private int width;
        private int height;
        private Bitmap image;
        private Random random = new Random();
        //Methods declaration
        public RandomImage(string s, int width, int height)
        {
            this.text = s;
            this.SetDimensions(width, height);
            this.GenerateImage();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.image.Dispose();
        }
        private void SetDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", width,
                    "Argument out of range, must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", height,
                    "Argument out of range, must be greater than zero.");
            this.width = width;
            this.height = height;
        }
        private void GenerateImage()
        {
            Bitmap objBitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.White);
            Random objRandom = new Random();
            objGraphics.DrawLine(Pens.Black, objRandom.Next(0, 50), objRandom.Next(10, 30), objRandom.Next(0, 200), objRandom.Next(0, 50));
            objGraphics.DrawRectangle(Pens.Blue, objRandom.Next(0, 20), objRandom.Next(0, 20), objRandom.Next(50, 80), objRandom.Next(0, 20));
            objGraphics.DrawLine(Pens.Blue, objRandom.Next(0, 20), objRandom.Next(10, 50), objRandom.Next(100, 200), objRandom.Next(0, 80));
            Brush objBrush = default(Brush);
            //create background style  
            HatchStyle[] aHatchStyles = new HatchStyle[]
            {
                HatchStyle.BackwardDiagonal, HatchStyle.Cross, HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal, HatchStyle.DashedUpwardDiagonal,
                HatchStyle.DashedVertical, HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross, HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid,
                HatchStyle.ForwardDiagonal, HatchStyle.Horizontal, HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, HatchStyle.LargeConfetti,
                HatchStyle.LargeGrid, HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal
            };
            //create rectangular area  
            RectangleF oRectangleF = new RectangleF(0, 0, 300, 300);
            objBrush = new HatchBrush(aHatchStyles[objRandom.Next(aHatchStyles.Length - 3)], Color.FromArgb((objRandom.Next(100, 255)), (objRandom.Next(100, 255)), (objRandom.Next(100, 255))), Color.White);
            objGraphics.FillRectangle(objBrush, oRectangleF);
            //Generate the image for captcha  
            string captchaText = this.text; //string.Format("{0:X}", objRandom.Next(1000000, 9999999));
            //add the captcha value in session  
            System.Drawing.Font objFont = new System.Drawing.Font("Times New Roman", 40, FontStyle.Bold);
            //Draw the image for captcha  
            objGraphics.DrawString(captchaText, objFont, Brushes.Black, 50, 5);
            this.image = objBitmap;


            //  Bitmap bitmap = new Bitmap
            //    (this.width, this.height, PixelFormat.Format32bppArgb);
            //  Graphics g = Graphics.FromImage(bitmap);
            //  g.SmoothingMode = SmoothingMode.AntiAlias;
            //  Rectangle rect = new Rectangle(0, 0, this.width, this.height);
            //  HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti,
            //      Color.LightGray, Color.White);
            //  g.FillRectangle(hatchBrush, rect);
            //  SizeF size;
            //  float fontSize = rect.Height + 1;
            //  Font font;

            //  do
            //  {
            //      fontSize--;
            //      font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
            //      size = g.MeasureString(this.text, font);
            //  } while (size.Width > rect.Width);
            //  StringFormat format = new StringFormat();
            //  format.Alignment = StringAlignment.Center;
            //  format.LineAlignment = StringAlignment.Center;
            //  GraphicsPath path = new GraphicsPath();
            //  //path.AddString(this.text, font.FontFamily, (int) font.Style, 
            //  //    font.Size, rect, format);
            //  path.AddString(this.text, font.FontFamily, (int)font.Style, 75, rect, format);
            //  float v = 4F;
            //  PointF[] points =
            //  {
            //      new PointF(this.random.Next(rect.Width) / v, this.random.Next(
            //         rect.Height) / v),
            //      new PointF(rect.Width - this.random.Next(rect.Width) / v,
            //          this.random.Next(rect.Height) / v),
            //      new PointF(this.random.Next(rect.Width) / v,
            //          rect.Height - this.random.Next(rect.Height) / v),
            //      new PointF(rect.Width - this.random.Next(rect.Width) / v,
            //          rect.Height - this.random.Next(rect.Height) / v)
            //};
            //  Matrix matrix = new Matrix();
            //  matrix.Translate(0F, 0F);
            //  path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            //  hatchBrush = new HatchBrush(HatchStyle.Percent10, Color.Black, Color.SkyBlue);
            //  g.FillPath(hatchBrush, path);
            //  int m = Math.Max(rect.Width, rect.Height);
            //  for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            //  {
            //      int x = this.random.Next(rect.Width);
            //      int y = this.random.Next(rect.Height);
            //      int w = this.random.Next(m / 50);
            //      int h = this.random.Next(m / 50);
            //      g.FillEllipse(hatchBrush, x, y, w, h);
            //  }
            //  font.Dispose();
            //  hatchBrush.Dispose();
            //  g.Dispose();
            //  this.image = bitmap;
        }
    }
}
