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
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }

		/// <summary>
		/// Radious that is used for colission detection.
		/// </summary>
		public double R { get; set; }

		private CollisionCallback callback;
		private State lastState;

		private ImageSource image;

		/// <summary>
		/// Delegate callback for collisions
		/// </summary>
		/// <param name="s"></param>
		public delegate void CollisionCallback(State state, String s);

		public enum State {enter, inside, exit, outside};

		public TouchTarget(double x, double y, double width, double height,
			ImageSource image)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			this.image = image;

			R = width / 2.0;

			this.callback = null;
			this.lastState = State.outside;
		}
	
		public void draw(DrawingContext dc)
		{
			dc.DrawImage(image, new Rect(X, Y, Width, Height));
		}

		public void registerCollisionCallback(CollisionCallback cs)
		{
			this.callback = cs;
		}

		public bool collide(double x, double y)
		{
			double dx = (this.X + this.Width / 2.0) - x;
			double dy = (this.Y + this.Height / 2.0) - y;

			if (Math.Sqrt(dx * dx + dy * dy) < this.R) // collide
			{
				if (this.callback != null)
				{
					if (this.lastState == State.outside)
						this.callback(State.enter, "enter");
					else
						this.callback(State.inside, "inside");

					this.lastState = State.inside;
					return true;
				}
			}

			if (this.lastState == State.inside)
			{
				if (this.callback != null)
					this.callback(State.exit, "exit");
			}

			this.lastState = State.outside;
			return false;
		}

		public bool collide (Point p)
		{
			return this.collide(p.X, p.Y);
		}
	}
}