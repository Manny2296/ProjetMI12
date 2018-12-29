using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
namespace MI12_Desktopapp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;
        KinectSensor sensor;
        private const double BodyCenterThickness = 1;
        private const double ClipBoundsThickness = 1;
        private Brush centerPointBrush = Brushes.Blue;
        private Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private Pen trackedBonPen = new Pen(Brushes.Green, 6);
        private Pen inferredBonPen = new Pen(Brushes.Gray, 1);
        private DrawingImage imageSource;
        private double jointThickness = 3;
        private DrawingGroup drawingGroup;
        

        public MainWindow()
        {
            InitializeComponent();
            sensor = KinectSensor.KinectSensors[0];
            if (sensor != null)
            {
                //MessageBox.Show("Sensor conectado exitosamente");
                //  sensor.ColorFrameReady += FrameReady;
                lbl_state.Content = "Conectado";
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                sensor.SkeletonStream.Enable();
                sensor.AllFramesReady += FramesReady;
               // sensor.DepthFrameReady += DepthFrameReady;

                sensor.Start();

            }
            else {

                lbl_state.Content = "Desconectado";
                MessageBox.Show("No se encontro dospositivo KINECT");

            }
        }

        //public void FrameReady(object sender, ColorImageFrameReadyEventArgs e)
        //{
        //    ColorImageFrame imageFrame = e.OpenColorImageFrame();
        //    BitmapSource bmp = ImagetoBitMap(imageFrame);
        //    img_sensor.Source = bmp;
        //}

        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        public void FramesReady(object sender, AllFramesReadyEventArgs e)
        {
            ColorImageFrame Vframe = e.OpenColorImageFrame();
            if(Vframe == null) {return;}
            SkeletonFrame sframe = e.OpenSkeletonFrame();
             if(sframe == null) { return; }
            Skeleton[] skeletons = new Skeleton[sframe.SkeletonArrayLength];
            sframe.CopySkeletonDataTo(skeletons);
            BitmapSource bmp = ImagetoBitMap(Vframe);
           // CroppedBitmap croppedBitmap = new CroppedBitmap(bmp, new Int32Rect(20, 20, 100, 100)); 
            using (DrawingContext dc = this.drawingGroup.Open())
            {
               // dc.DrawImage(bmp, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                if(skeletons != null) { 
                foreach (Skeleton item in skeletons)
            {
                        Random r = new Random();
                        switch (r.Next(1,4))
                        {
                            case 1:
                                trackedBonPen = new Pen(Brushes.White, 5);
                                break;
                            case 2:
                                trackedBonPen = new Pen(Brushes.Yellow, 5);
                                break;
                           
                        }
                  
                       
                        RenderClippedEdges(item, dc);

                    if (item.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.DrawBonesAndJoints(item, dc);
                    }
                    else if (item.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        this.SkeletonPointToScreen(item.Position),
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }
                }

            }
            }

                 img_sensor.Source = bmp;


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
            Pen drawPen = this.inferredBonPen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonPen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
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
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), jointThickness, jointThickness);
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
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        //public void  DepthFrameReady(object sender,DepthImageFrameReadyEventArgs e)
        //{
        //    DepthImageFrame imageFrame = e.OpenDepthImageFrame();
        //    if(imageFrame != null)
        //    {
        //        img_sensor.Source = ImagetoBitMap(imageFrame);
        //    }
        //}

        BitmapSource ImagetoBitMap(ColorImageFrame Image) {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            BitmapSource bmap = BitmapSource.Create(Image.Width,Image.Height,96,96,PixelFormats.Gray16,null,pixeldata,Image.Width*Image.BytesPerPixel);
            return bmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { 
            // get elevation
            if (sensor.ElevationAngle != sensor.MaxElevationAngle)
                sensor.ElevationAngle += 4;
           
            }
            catch(Exception ex){
                Console.WriteLine("CException :" + ex.ToString());
            }
        }

     

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sensor.ElevationAngle != sensor.MinElevationAngle)
                    sensor.ElevationAngle -= 4;
             
            }
            catch(Exception ex){
                Console.WriteLine("CException :" + ex.ToString());
            }
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            // Display the drawing using our image control
           Imagen.Source = this.imageSource;
            //img_sensor.Source = this.imageSource;,


        }
    }
}
