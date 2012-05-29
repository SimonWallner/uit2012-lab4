using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT2012.Lab4
{
	///<summary>
	/// State machine to handle the old school phone style input
	/// </summary>
	class Input
	{
		private enum State { idle, input };

		private State currentState;
		private double timer;

		private String currentCharSet;
		private int currentIndex;

		public String Text { get; set; }

		public Input()
		{
			this.currentState = State.idle;
			this.currentCharSet = null;
			this.currentIndex = 0;
			this.Text = "";
		}

		public void tick(double deltaT)
		{
			if (this.currentState == State.input)
			{
				this.timer += deltaT;
				if (this.timer > 1000.0)
				{
					this.Text += currentCharSet[currentIndex];

					currentState = State.idle;
					this.timer = 0.0;
					this.currentCharSet = null;
					this.currentIndex = 0;
				}
			}
		}

		public void enter(String charSet)
		{
			if (currentState == State.input)
			{
				if (currentCharSet.Equals(charSet))
				{
					currentIndex = (currentIndex + 1) % currentCharSet.Length;
					this.timer = 0.0;
				}
				else
				{
					this.Text += currentCharSet[currentIndex];

					this.currentCharSet = charSet;
					this.currentIndex = 0;
					this.timer = 0.0;
				}
			}
			else
			{
				this.currentState = State.input;
				this.currentCharSet = charSet;
			}
		}
	}
}
