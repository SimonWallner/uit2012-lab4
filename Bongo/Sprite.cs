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

		public Sprite(ImageSource image)
		{
			Image = image;
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
			return new Rect(x - (Image.Width * scale) / 2.0,
				y - (Image.Height * scale) / 2.0,
				Image.Width * scale,
				Image.Height * scale);
		}

		public Rect centerRect(Point p, double scale)
		{
			return this.centerRect(p.X, p.Y, scale);
		}
	}
}
