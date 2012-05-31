using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace UIT2012.Lab4
{
	class Sprite
	{
		public ImageSource Image { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public Sprite(ImageSource image)
		{
			Image = image;
			this.Width = (int)image.Width;
			this.Height = (int)image.Height;
		}

		public Sprite(ImageSource image, int width, int height)
		{
			this.Image = image;
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// get a rect that is centerd aroud the given x and y coordinate
		/// </summary>
		/// <param name="x">center x</param>
		/// <param name="y">center y</param>
		/// <param name="scale">scale of the image</param>
		/// <returns>a rect centered</returns>
		public Rect centerRect(double x, double y, double scale)
		{
			return new Rect(x - (this.Width * scale) / 2.0,
				y - (this.Height * scale) / 2.0,
				this.Width * scale,
				this.Height * scale);
		}

		public Rect centerRect(Point p, double scale)
		{
			return this.centerRect(p.X, p.Y, scale);
		}

		public void drawLimb(DrawingContext dc, Point p1, Point p2, int margin)
		{

			dc.PushTransform(new TranslateTransform(p1.X, p1.Y));
			
			Vector diff = p2 - p1;
			diff.Normalize();

			double angle = Math.Acos(diff.Y) * (180.0 / Math.PI);
			if (Double.IsNaN(angle))
				angle = 0;

			dc.PushTransform(new RotateTransform(Math.Sign(diff.X) * (-1) * angle));

			Vector diff2 = p2 - p1;
			double scale = diff2.Length / (this.Height - 2 * margin);
			dc.PushTransform(new ScaleTransform(1, scale));

			dc.PushTransform(new TranslateTransform(-this.Width / 2.0, -margin));

			
			dc.DrawImage(this.Image, new Rect(0, 0, this.Width, this.Height));

			dc.Pop();
			dc.Pop();
			dc.Pop();
			dc.Pop();
		}
	}
}
