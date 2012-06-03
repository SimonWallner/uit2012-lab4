//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace UIT2012.Lab4
{
	using System;
	using System.IO;
	using System.Windows;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using Microsoft.Kinect;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Media;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Width of output drawing
		/// </summary>
		private const float RenderWidth = 640.0f;

		/// <summary>
		/// Height of our output drawing
		/// </summary>
		private const float RenderHeight = 480.0f;

		/// <summary>
		/// Thickness of drawn joint lines
		/// </summary>
		private const double JointThickness = 3;

		/// <summary>
		/// Thickness of body center ellipse
		/// </summary>
		private const double BodyCenterThickness = 10;

		/// <summary>
		/// Thickness of clip edge rectangles
		/// </summary>
		private const double ClipBoundsThickness = 10;

		/// <summary>
		/// Brush used to draw skeleton center point
		/// </summary>
		private readonly Brush centerPointBrush = Brushes.Blue;

		/// <summary>
		/// Brush used for drawing joints that are currently tracked
		/// </summary>
		private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

		/// <summary>
		/// Brush used for drawing joints that are currently inferred
		/// </summary>        
		private readonly Brush inferredJointBrush = Brushes.Yellow;

		/// <summary>
		/// Pen used for drawing bones that are currently tracked
		/// </summary>
		private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

		/// <summary>
		/// Pen used for drawing bones that are currently inferred
		/// </summary>        
		private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

		/// <summary>
		/// Active Kinect sensor
		/// </summary>
		private KinectSensor sensor;

		/// <summary>
		/// Drawing group for skeleton rendering output
		/// </summary>
		private DrawingGroup drawingGroup;

		/// <summary>
		/// Drawing image that we will display
		/// </summary>
		private DrawingImage imageSource;

		/// <summary>
		/// Bitmap that will hold color information
		/// </summary>
		private WriteableBitmap colorBitmap;

		/// <summary>
		/// Intermediate storage for the color data received from the camera
		/// </summary>
		private byte[] colorPixels;

		/// <summary>
		/// Flag to draw debug output or not.
		/// </summary>
		private bool drawDebug;

		/// <summary>
		/// hip precission timer for frame time calculations
		/// </summary>
		private Stopwatch timer;

		/// <summary>
		/// last frame time stamp
		/// </summary>
		private double lastT;

		/// <summary>
		/// last frame time delta
		/// </summary>
		private double deltaT;

		private TouchTarget bongoABC;
		private TouchTarget bongoDEF;
		private TouchTarget bongoGHI;
		private TouchTarget bongoJKL;
		private TouchTarget bongoMNO;
		private TouchTarget bongoPQRS;
		private TouchTarget bongoTUV;
		private TouchTarget bongoWXYZ;
		private TouchTarget bongoSpace;
		private TouchTarget bongoBackSpace;

		private Sprite head;
		private Sprite monkey;
		private Sprite leftHand;
		private Sprite rightHand;
		private Sprite limb;
		private Sprite torso;
		private ImageSource background;

		private List<TouchTarget> targets;

		private InputMachine input;

		private SoundPlayer player;


		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			this.timer = new Stopwatch();
			this.timer.Start();
			this.lastT = timer.ElapsedMilliseconds;
			this.deltaT = 0;

			/* //round layout
			this.bongoABC = new TouchTarget(20, 50, 80, 80, (ImageSource)FindResource("Bongo"), "ABC");
			this.bongoDEF = new TouchTarget(20, 150, 80, 80, (ImageSource)FindResource("Bongo"), "DEF");
			this.bongoGHI = new TouchTarget(50, 250, 80, 80, (ImageSource)FindResource("Bongo"), "GHI");
			this.bongoJKL = new TouchTarget(150, 300, 80, 80, (ImageSource)FindResource("Bongo"), "JKL");
			this.bongoMNO = new TouchTarget(250, 300, 80, 80, (ImageSource)FindResource("Bongo"), "MNO");
			this.bongoPQRS = new TouchTarget(350, 300, 80, 80, (ImageSource)FindResource("Bongo"), "PQRS");
			this.bongoTUV = new TouchTarget(450, 250, 80, 80, (ImageSource)FindResource("Bongo"), "TUV");
			this.bongoWXYZ = new TouchTarget(480, 150, 80, 80, (ImageSource)FindResource("Bongo"), "WXYZ");
			this.bongoSpace = new TouchTarget(480, 50, 80, 80, (ImageSource)FindResource("Bongo"), "_");
			this.bongoBackSpace = new TouchTarget(250, 0, 80, 80, (ImageSource)FindResource("Bongo"), "");
			*/
			
			// mostly straight
			/*
			this.bongoABC = new TouchTarget(0, 270, 80, 80, (ImageSource)FindResource("Bongo"), "ABC");
			this.bongoDEF = new TouchTarget(100, 335, 50, 50, (ImageSource)FindResource("Bongo"), "DEF");
			this.bongoGHI = new TouchTarget(180, 345, 50, 50, (ImageSource)FindResource("Bongo"), "GHI");
			this.bongoJKL = new TouchTarget(260, 350, 50, 50, (ImageSource)FindResource("Bongo"), "JKL");
			this.bongoMNO = new TouchTarget(340, 350, 50, 50, (ImageSource)FindResource("Bongo"), "MNO");
			this.bongoPQRS = new TouchTarget(420, 345, 50, 50, (ImageSource)FindResource("Bongo"), "PQRS");
			this.bongoTUV = new TouchTarget(500, 335, 50, 50, (ImageSource)FindResource("Bongo"), "TUV");
			this.bongoWXYZ = new TouchTarget(570, 270, 80, 80, (ImageSource)FindResource("Bongo"), "WXYZ");
			this.bongoSpace = new TouchTarget(200, -100, 250, 250, (ImageSource)FindResource("Bongo"), "_");
			this.bongoBackSpace = new TouchTarget(550, 50, 80, 80, (ImageSource)FindResource("Bongo"), "");
			 */

			this.bongoABC = new TouchTarget(0, 268, 94, 91, (ImageSource)FindResource("BongoABC"), "ABC", 70, -30);
			this.bongoDEF = new TouchTarget(65, 325, 90, 102, (ImageSource)FindResource("BongoDEF"), "DEF", 60, -50);
			this.bongoGHI = new TouchTarget(155, 352, 82, 98, (ImageSource)FindResource("BongoGHI"), "GHI", 35, -45);
			this.bongoJKL = new TouchTarget(242, 362, 76, 90, (ImageSource)FindResource("BongoJKL"), "JKL", 30, -50);
			this.bongoMNO = new TouchTarget(322, 359, 78, 93, (ImageSource)FindResource("BongoMNO"), "MNO", 27, -50);
			this.bongoPQRS = new TouchTarget(402, 349, 80, 95, (ImageSource)FindResource("BongoPQRS"), "PQRS", 25, -50);
			this.bongoTUV = new TouchTarget(478, 316, 92, 98, (ImageSource)FindResource("BongoTUV"), "TUV", 20, -50);
			this.bongoWXYZ = new TouchTarget(536, 259, 101, 95, (ImageSource)FindResource("BongoWXYZ"), "WXYZ", -15, -25);
			this.bongoSpace = new TouchTarget(4, 6, 129, 123, (ImageSource)FindResource("BongoSpace"), "_", 0, 0);
			this.bongoBackSpace = new TouchTarget(498, 7, 131, 117, (ImageSource)FindResource("BongoBackspace"), "", 0, 0);

			// offset hitboxes a little
			this.bongoABC.CX -= 3;
			this.bongoABC.CY -= 4;
			this.bongoDEF.CX += 5;
			this.bongoDEF.CY -= 6;
			this.bongoGHI.CX += 4;
			this.bongoGHI.CY -= 8;
			this.bongoJKL.CX += 1;
			this.bongoJKL.CY -= 10;
			this.bongoMNO.CX += 1;
			this.bongoMNO.CY -= 10;
			this.bongoPQRS.CX -= 1;
			this.bongoPQRS.CY -= 8;
			this.bongoTUV.CX -= 3;
			this.bongoTUV.CY -= 3;

			this.head = new Sprite((ImageSource)FindResource("Head"));
			this.monkey = new Sprite((ImageSource)FindResource("Monkey"));
			this.leftHand = new Sprite((ImageSource)TryFindResource("Fist"));
			this.rightHand = new Sprite((ImageSource)TryFindResource("Fist"));
			this.limb = new Sprite((ImageSource)TryFindResource("Limb"), 20, 100);
			this.limb.Width = 80;

			this.torso = new Sprite((ImageSource)TryFindResource("Limb"));
			this.torso.Width = 140;
		
			this.background = (ImageSource)TryFindResource("Background");


			this.targets = new List<TouchTarget>();
			targets.Add(this.bongoABC);
			targets.Add(this.bongoDEF);
			targets.Add(this.bongoGHI);
			targets.Add(this.bongoJKL);
			targets.Add(this.bongoMNO);
			targets.Add(this.bongoPQRS);
			targets.Add(this.bongoTUV);
			targets.Add(this.bongoWXYZ);
			targets.Add(this.bongoSpace);
			targets.Add(this.bongoBackSpace);
			
			foreach (TouchTarget target in this.targets)
			{
				target.registerCollisionCallback(this.collisionOccured);
			}

			this.input = new InputMachine();
			this.input.registerAddCharacterCallback(this.addCharacter);
			this.input.registerDeleteCallback(this.deleteCharacter);
			this.input.registerSelectionCallback(this.selectionChanged);

			this.player = new SoundPlayer();
			this.player.Stream = Properties.Resources.boing;

		}

		/// <summary>
		/// Execute startup tasks
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			// Create the drawing group we'll use for drawing
			this.drawingGroup = new DrawingGroup();

			// Create an image source that we can use in our image control
			this.imageSource = new DrawingImage(this.drawingGroup);

			// Display the drawing using our image control
			SkeletonImage.Source = this.imageSource;

			// Look through all sensors and start the first connected one.
			// This requires that a Kinect is connected at the time of app startup.
			// To make your app robust against plug/unplug, 
			// it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
			foreach (var potentialSensor in KinectSensor.KinectSensors)
			{
				if (potentialSensor.Status == KinectStatus.Connected)
				{
					this.sensor = potentialSensor;
					break;
				}
			}
			 
			if (this.sensor != null)
			{
				// Turn on the skeleton stream to receive skeleton frames
				this.sensor.SkeletonStream.Enable();

				// Turn on the color stream to receive color frames
				this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

				// Allocate space to put the pixels we'll receive
				this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

				// This is the bitmap we'll display on-screen
				this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

				// Set the image we display to point to the bitmap where we'll put the image data
				this.ColourImage.Source = this.colorBitmap;


				// Add an event handler to be called whenever there is new skeleton frame data
				this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

				// Add an event handler to be called whenever there is new color frame data
				this.sensor.ColorFrameReady += this.SensorColorFrameReady;


				// Start the sensor!
				try
				{
					this.sensor.Start();
				}
				catch (IOException)
				{
					this.sensor = null;
				}
			}

			if (this.sensor == null)
			{
				this.statusBarText.Text = Properties.Resources.NoKinectReady;
			}
		}

		/// <summary>
		/// Execute shutdown tasks
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.sensor != null)
			{
				this.sensor.Stop();
			}
		}

		/// <summary>
		/// Event handler for Kinect sensor's SkeletonFrameReady event
		/// </summary>
		/// This is where all the fun happens. Kind of our main loop.
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			double currentT = this.timer.ElapsedMilliseconds;
			this.deltaT = currentT - this.lastT;
			this.lastT = currentT;

			this.input.tick(deltaT);

			
			if (this.drawDebug)
				this.refreshRate.Text = "frametime: " + this.deltaT + "ms";
			else
				this.refreshRate.Text = "";
			
			
			Skeleton[] skeletons = new Skeleton[0];

			using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
			{
				if (skeletonFrame != null)
				{
					skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
					skeletonFrame.CopySkeletonDataTo(skeletons);
				}
			}
			
			using (DrawingContext dc = this.drawingGroup.Open())
			{
			   // Draw a transparent background to set the render size
				dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

				List<Point> handPoints = new List<Point>();

				foreach (Skeleton skeleton in skeletons)
				{
					// more limbs
					// legs
					this.drawLimb(dc, skeleton, JointType.HipLeft, JointType.KneeLeft, this.limb);
					this.drawLimb(dc, skeleton, JointType.KneeLeft, JointType.AnkleLeft, this.limb);
					this.drawLimb(dc, skeleton, JointType.HipRight, JointType.KneeRight, this.limb);
					this.drawLimb(dc, skeleton, JointType.KneeRight, JointType.AnkleRight, this.limb);

					// torso
					this.drawLimb(dc, skeleton, JointType.ShoulderCenter, JointType.HipCenter, this.torso);

					if (skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
					{
						Point headPos = this.SkeletonPointToScreen(skeleton.Joints[JointType.Head].Position);
						dc.DrawImage(this.monkey.Image, this.head.centerRect(headPos, 1.0));
					}
				}

				dc.DrawImage(this.background, new Rect(0, 0, 640, 480));

				// draw drums
				foreach (TouchTarget target in this.targets)
				{
					target.draw(dc, deltaT);

					if (this.drawDebug)
						target.drawDebug(dc);
				}

				
				foreach (Skeleton skeleton in skeletons)
				{
					// draw linbs
					// arms
					this.drawLimb(dc, skeleton, JointType.ShoulderLeft, JointType.ElbowLeft, this.limb);
					this.drawLimb(dc, skeleton, JointType.ElbowLeft, JointType.HandLeft, this.limb);
					this.drawLimb(dc, skeleton, JointType.ShoulderRight, JointType.ElbowRight, this.limb);
					this.drawLimb(dc, skeleton, JointType.ElbowRight, JointType.HandRight, this.limb);


					if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
						skeleton.Joints[JointType.WristLeft].TrackingState == JointTrackingState.Tracked)
					{
						Point handPos = this.SkeletonPointToScreen(skeleton.Joints[JointType.HandLeft].Position);
						Point wristPos = this.SkeletonPointToScreen(skeleton.Joints[JointType.WristLeft].Position);
						Point extended = handPos + (handPos - wristPos);

						dc.DrawImage(this.leftHand.Image, this.leftHand.centerRect(handPos, 1.7));

						// dc.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1), extended, 3, 3);
                        handPoints.Add(handPos);
					}

					if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked &&
						skeleton.Joints[JointType.WristRight].TrackingState == JointTrackingState.Tracked)
					{
						Point handPos = this.SkeletonPointToScreen(skeleton.Joints[JointType.HandRight].Position);
						Point wristPos = this.SkeletonPointToScreen(skeleton.Joints[JointType.WristRight].Position);
						Point extended = handPos + (handPos - wristPos);

                        dc.DrawImage(this.leftHand.Image, this.leftHand.centerRect(handPos, 1.7));

						// dc.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1), handPos, 3, 3);
                        handPoints.Add(handPos);
					}
				}

				foreach (TouchTarget target in this.targets)
				{
					target.collide(handPoints);
					target.drawSelection(dc, this.deltaT);
				}

				if (this.drawDebug)
				{
					foreach (Skeleton skeleton in skeletons)
					{
						if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
						{
							this.DrawBonesAndJoints(skeleton, dc);
						}
						else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
						{
							dc.DrawEllipse(
								this.centerPointBrush,
								null,
								this.SkeletonPointToScreen(skeleton.Position),
								BodyCenterThickness,
								BodyCenterThickness);
						}
					}
				}
				// prevent drawing outside of our render area
				this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));           
			}
		}

		private void collisionOccured(TouchTarget.State state, String s)
		{
			if (state == TouchTarget.State.enter)
			{
				this.player.Play();
				this.input.enter(s);
			}
		}

		/// <summary>
		/// Event handler for Kinect sensor's ColorFrameReady event
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
			{
				if (colorFrame != null)
				{
					// Copy the pixel data from the image to a temporary array
					colorFrame.CopyPixelDataTo(this.colorPixels);

					// Write the pixel data into our bitmap
					this.colorBitmap.WritePixels(
						new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
						this.colorPixels,
						this.colorBitmap.PixelWidth * sizeof(int),
						0);
				}
			}
		}

		/// <summary>
		/// Draws a skeleton's bones and joints
		/// </summary>
		/// <param name="skeleton">skeleton to draw</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
		{
			// Render Torso
			this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
			this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

			// Left Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
			this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

			// Right Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
			this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

			// Left Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
			this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

			// Right Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
			this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
 
			// Render Joints
			foreach (Joint joint in skeleton.Joints)
			{
				Brush drawBrush = null;

				if (joint.TrackingState == JointTrackingState.Tracked)
				{
					drawBrush = this.trackedJointBrush;                    
				}
				else if (joint.TrackingState == JointTrackingState.Inferred)
				{
					drawBrush = this.inferredJointBrush;                    
				}

				if (drawBrush != null)
				{
					drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
				}
			}
		}

		/// <summary>
		/// Maps a SkeletonPoint to lie within our render space and converts to Point
		/// </summary>
		/// <param name="skelpoint">point to map</param>
		/// <returns>mapped point</returns>
		private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
		{
			// Convert point to depth space.  
			// We are not using depth directly, but we do want the points in our 640x480 output resolution.
			if (this.sensor != null)
			{
				ColorImagePoint colourPoint = this.sensor.MapSkeletonPointToColor(skelpoint, ColorImageFormat.RgbResolution640x480Fps30);
				return new Point(colourPoint.X, colourPoint.Y);
			}
			else return new Point(0, 0);
		}

		/// <summary>
		/// Draws a bone line between two joints
		/// </summary>
		/// <param name="skeleton">skeleton to draw bones from</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		/// <param name="jointType0">joint to start drawing from</param>
		/// <param name="jointType1">joint to end drawing at</param>
		private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
		{
			Joint joint0 = skeleton.Joints[jointType0];
			Joint joint1 = skeleton.Joints[jointType1];

			// If we can't find either of these joints, exit
			if (joint0.TrackingState == JointTrackingState.NotTracked ||
				joint1.TrackingState == JointTrackingState.NotTracked)
			{
				return;
			}

			// Don't draw if both points are inferred
			if (joint0.TrackingState == JointTrackingState.Inferred &&
				joint1.TrackingState == JointTrackingState.Inferred)
			{
				return;
			}

			// We assume all drawn bones are inferred unless BOTH joints are tracked
			Pen drawPen = this.inferredBonePen;
			if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
			{
				drawPen = this.trackedBonePen;
			}

			drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
		}

		/// <summary>
		/// Handles the checking or unchecking of the seated mode combo box
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void CheckBoxDebugChanged(object sender, RoutedEventArgs e)
		{
			this.drawDebug = this.checkBoxDebug.IsChecked.GetValueOrDefault();
		}

		private void selectionChanged(char character)
		{
			foreach (TouchTarget target in targets)
			{
				target.currentSelection(character);
			}
		}

		private void addCharacter(char character)
		{
			this.TextInputBox.Text += character;
		}

		private void deleteCharacter()
		{
			String text = this.TextInputBox.Text;
			if (text.Length > 0)
				this.TextInputBox.Text = text.Remove(text.Length - 1);
		}

		private void drawLimb(DrawingContext dc, Skeleton skeleton, JointType start, JointType end, Sprite sprite)
		{
			Joint jointStart = skeleton.Joints[start];
			Joint jointEnd = skeleton.Joints[end];

			// Don't draw if both points are inferred
			if ((jointStart.TrackingState == JointTrackingState.Tracked || jointStart.TrackingState == JointTrackingState.Inferred) &&
				jointEnd.TrackingState == JointTrackingState.Tracked || jointEnd.TrackingState == JointTrackingState.Inferred)
			{
				Point startPos = this.SkeletonPointToScreen(jointStart.Position);
				Point endPos = this.SkeletonPointToScreen(jointEnd.Position);

				sprite.drawLimb(dc, startPos, endPos, 10);
			}
		}
	}
}