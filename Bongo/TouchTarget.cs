using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace UIT2012.Lab4
{
	/// <summary>
	/// Touch Target to be used for Bongos and such
	/// </summary>
	/// The Touch target has a hitbox and also an associated image sprite.
	/// All coordinates are in screen coordinates.
	class TouchTarget
	{
		private double X { get; set; }
		private double Y { get; set; }
		private double Width { get; set; }
		private double Height { get; set; }

		private ImageSource image;

		public TouchTarget(double x, double y, double width, double height,
			ImageSource image)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			this.image = image;
		}
	
		public void draw(DrawingContext dc)
		{
			dc.DrawImage(image, new Rect(X, Y, Width, Height));
		}
	}
}
