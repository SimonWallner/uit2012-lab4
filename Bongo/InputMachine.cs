using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT2012.Lab4
{
	///<summary>
	/// State machine to handle the old school phone style input
	/// </summary>
	class InputMachine
	{
		public delegate void SelectionCallback(char selection);
		public delegate void AddCharacterCallback(char character);
		public delegate void DeleteCallback();

		private enum State { idle, input };

		private State currentState;
		private double timer;

		private String currentCharSet;
		private int currentIndex;

		private SelectionCallback selectionCallback;
		private AddCharacterCallback addCharacterCallback;
		private DeleteCallback deleteCallback;


		public InputMachine()
		{
			this.currentState = State.idle;
			this.currentCharSet = null;
			this.currentIndex = 0;

			this.selectionCallback = null;
			this.addCharacterCallback = null;
			this.deleteCallback = null;
		}

		public void registerSelectionCallback(SelectionCallback cb)
		{
			this.selectionCallback = cb;
		}

		public void registerAddCharacterCallback(AddCharacterCallback cb)
		{
			this.addCharacterCallback = cb;
		}

		public void registerDeleteCallback(DeleteCallback cb)
		{
			this.deleteCallback = cb;
		}

		/// <summary>
		/// Tell the input machine how much time has passed since the last tick.
		/// </summary>
		/// This is used to compute time dependet state transitions like going
		/// input back to idle once a set time has passed.
		/// <param name="deltaT">time passed since the last tick in ms</param>
		public void tick(double deltaT)
		{
			if (this.currentState == State.input)
			{
				this.timer += deltaT;
				if (this.timer > 2000.0)
				{
					currentState = State.idle;
					this.addCharacterCallback(this.currentCharSet[this.currentIndex]);
				}
			}
		}

		public void enter(String charSet)
		{
			if (this.currentState == State.idle)
			{
				this.currentState = State.input;
				this.currentCharSet = charSet;
				this.currentIndex = 0;
				this.timer = 0.0;

				this.selectionCallback(this.currentCharSet[this.currentIndex]);
			}
			else // state == input
			{
				if (this.currentCharSet.Equals(charSet)) // same button hit again
				{
					this.timer = 0.0;
					this.currentIndex = (this.currentIndex + 1) % this.currentCharSet.Length;
					this.selectionCallback(this.currentCharSet[this.currentIndex]);
				}
				else // different button hit
				{
					this.addCharacterCallback(this.currentCharSet[this.currentIndex]);

					this.currentCharSet = charSet;
					this.timer = 0.0;
					this.currentIndex = 0;

					this.selectionCallback(this.currentCharSet[this.currentIndex]);
				}
			}
		}
	}
}
