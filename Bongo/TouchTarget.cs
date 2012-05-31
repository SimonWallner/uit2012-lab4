using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

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

		public double CX
		{
			get
			{
				return this.X + this.Width / 2.0;
			}
		}

		public double CY
		{
			get
			{
				return this.Y + this.Height / 2.0;
			}
		}

		/// <summary>
		/// Radious that is used for colission detection if state was outside.
		/// </summary>
		public double ROutside { get; set; }
		public double RInside { get; set; }

		private CollisionCallback callback;
		private State lastState;

		private ImageSource image;

		private String characters;

		private readonly Brush insideBrush = new SolidColorBrush(Color.FromArgb(192, 0, 255, 0));
		private readonly Brush outsideBrush = new SolidColorBrush(Color.FromArgb(192, 255, 0, 0));
		private readonly Brush rOutsideBrush = new SolidColorBrush(Color.FromArgb(192, 192, 192, 192));
		private readonly Pen pen = new Pen(Brushes.Black, 1);

		private double timeInside;
		private double selectionTime;
		private char selection;

		/// <summary>
		/// Delegate callback for collisions
		/// </summary>
		/// <param name="s"></param>
		public delegate void CollisionCallback(State state, String s);

		public enum State {enter, inside, exit, outside};

		public TouchTarget(double x, double y, double width, double height,
			ImageSource image, String characters)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			this.image = image;

			ROutside = width / 2.0;
			RInside = ROutside * 1.2;

			this.callback = null;
			this.lastState = State.outside;

			this.characters = characters;

			this.timeInside = 0.0;
			this.selection = ' ';
			this.selectionTime = 0.0;
		}
	
		public void draw(DrawingContext dc, double deltaT)
		{
			double scale = 1.0;
			if (this.lastState == State.inside)
			{
				timeInside += deltaT;
				scale = 1 - (1 / (timeInside/100.0 + 1));
			}

			
			dc.DrawImage(image, new Rect(X, Y, Width * scale, Height * scale));
			dc.DrawText(new FormattedText(characters, 
							CultureInfo.GetCultureInfo("en-us"),
							FlowDirection.LeftToRight,
							new Typeface("Impact"),
							24,
							Brushes.Red),
								new Point(X, Y + 55));
		}

		public void drawSelection(DrawingContext dc, double deltaT)
		{
			this.selectionTime += deltaT;

			if (characters.Contains(selection))
			{
				dc.DrawText(new FormattedText(selection.ToString(),
							CultureInfo.GetCultureInfo("en-us"),
							FlowDirection.LeftToRight,
							new Typeface("Impact"),
							48,
							new SolidColorBrush(Color.FromArgb((byte)(Math.Max(1 - Math.Pow(selectionTime / 2000, 2), 0) * 255), 255, 0, 0))),
								new Point(X + 10, (Y - 60) + (selectionTime / 2000) * 40 ));
			}
		}

		public void drawDebug(DrawingContext dc)
		{
			if (this.lastState == State.outside)
				dc.DrawEllipse(outsideBrush, pen, new Point(this.CX, this.CY), ROutside, ROutside);
			else
				dc.DrawEllipse(insideBrush, pen, new Point(this.CX, this.CY), RInside, RInside);
		}

		public void registerCollisionCallback(CollisionCallback cs)
		{
			this.callback = cs;
		}

		public bool collide(List<Point> points)
		{
			bool collision = false;

			foreach (Point p in points)
			{ 
				double dx = this.CX - p.X;
				double dy = this.CY - p.Y;
				double distance = Math.Sqrt(dx * dx + dy * dy);

				collision = collision || ((this.lastState == State.outside && distance < this.ROutside) ||
					(this.lastState == State.inside && distance < this.RInside));
			}

			if (collision) // something's inside
			{
				if (this.callback != null)
				{
					if (this.lastState == State.outside)
					{
						this.timeInside = 0.0;
						this.callback(State.enter, characters);
					}
					else
						this.callback(State.inside, characters);
				}
				this.lastState = State.inside;
				return true;
			}
			else // all are outside
			{
				if (this.lastState == State.inside)
				{
					if (this.callback != null)
						this.callback(State.exit, characters);
				}

				this.lastState = State.outside;
				return false;
			}
		}

		/// <summary>
		/// Set the current selection from the input machine
		/// </summary>
		/// This function is to be called by the selectionChanged callback in the
		/// main program.
		/// <param name="selection"></param>
		public void currentSelection(char selection)
		{
			this.selection = selection;
			this.selectionTime = 0.0;
		}
	}
}
