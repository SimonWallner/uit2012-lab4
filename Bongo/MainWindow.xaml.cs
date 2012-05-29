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

		private TouchTarget bongoA;

		private Sprite head;
		private Sprite leftHand;
		private Sprite rightHand;


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

			this.bongoA = new TouchTarget(100, 100, 100, 100, (ImageSource)FindResource("Bongo"));
			this.head = new Sprite((ImageSource)FindResource("Head"));
			this.leftHand = new Sprite((ImageSource)TryFindResource("Fist"));
			this.rightHand = new Sprite((ImageSource)TryFindResource("Fist"));
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

				this.bongoA.draw(dc);

				if (skeletons.Length != 0)
				{
					if (skeletons[0].Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
					{
						Point headPos = this.SkeletonPointToScreen(skeletons[0].Joints[JointType.Head].Position);
						dc.DrawImage(this.head.Image, this.head.centerRect(headPos, 0.2));
					}

					if (skeletons[0].Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
					{
						Point headPos = this.SkeletonPointToScreen(skeletons[0].Joints[JointType.HandLeft].Position);
						dc.DrawImage(this.leftHand.Image, this.leftHand.centerRect(headPos, 0.5));
					}

					if (skeletons[0].Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
					{
						Point headPos = this.SkeletonPointToScreen(skeletons[0].Joints[JointType.HandRight].Position);
						dc.DrawImage(this.rightHand.Image, this.rightHand.centerRect(headPos, 0.5));
					}


					foreach (Skeleton skel in skeletons)
					{
						if (skel.TrackingState == SkeletonTrackingState.Tracked)
						{
							this.DrawBonesAndJoints(skel, dc);
						}
						else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
						{
							dc.DrawEllipse(
								this.centerPointBrush,
								null,
								this.SkeletonPointToScreen(skel.Position),
								BodyCenterThickness,
								BodyCenterThickness);
						}
					}
				}
				// prevent drawing outside of our render area
				this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));           
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
			ColorImagePoint colourPoint = this.sensor.MapSkeletonPointToColor(skelpoint, ColorImageFormat.RgbResolution640x480Fps30);
			return new Point(colourPoint.X, colourPoint.Y);
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
	}
}