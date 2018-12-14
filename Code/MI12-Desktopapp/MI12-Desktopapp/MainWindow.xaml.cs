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
        KinectSensor sensor;
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
     

        public void FramesReady(object sender, AllFramesReadyEventArgs e)
        {
            ColorImageFrame Vframe = e.OpenColorImageFrame();
            if(Vframe == null) {return;}
            SkeletonFrame sframe = e.OpenSkeletonFrame();
             if(sframe == null) { return; }
            Skeleton[] skeletons = new Skeleton[sframe.SkeletonArrayLength];
            sframe.CopySkeletonDataTo(skeletons);
            foreach (Skeleton item in skeletons)
            {
                if(item.TrackingState == SkeletonTrackingState.Tracked)
                {
                    SkeletonPoint sloc = item.Joints[JointType.Head].Position;
                }
            }
            BitmapSource bmp = ImagetoBitMap(Vframe);
            img_sensor.Source = bmp;
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
    }
}
