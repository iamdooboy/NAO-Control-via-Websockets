/**
 * Last Modified by Duy Le
 * Last Modified: 2018-4-5
 * 
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Drawing;

using Aldebaran;
using NAO_Camera_WPF;
using SuperWebSocket;
using SuperSocket.SocketBase;
using Aldebaran.Proxies;
using System.Collections;

namespace NAOserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Classes
        private static Camera naoCam = null;
        private static Motion naoMotion = null;
        private static DispatcherTimer sendFrameTimer = new DispatcherTimer();
        //private DispatcherTimer recordFrameTimer = new DispatcherTimer();
        private WebSocketServer appServer = null;
        private NotifyIcon naoNotifyIcon;
        
        // Variables
        private const int COLOR_SPACE = 13;
        private const int FPS = 30;
        private static string ip = "192.168.0.102";
        private static int port = 443;
        private bool isCamInitialized;
        private bool isPictureUpdating = false;
        private bool saveImage = false;
        private static NaoCamImageFormat currentFormat;
        private BitmapSource imageBitmap;
        private BitmapFrame frame;
        private static String imageString = "";
        private static List<WebSocketSession> sessionList = new List<WebSocketSession>();
        private static float yaw;
        private static float pitch;
        private static float roll;
        private static float hand;
        private bool failed = false;
        private long frameCount = 0;

        /// <summary>
        /// constuctor for MainWindow
        /// </summary>
        public MainWindow()
        {
            naoNotifyIcon = new System.Windows.Forms.NotifyIcon();
            naoNotifyIcon.Icon = new System.Drawing.Icon("logo_NAO.ico");
            naoNotifyIcon.Text = "NAO Websocket Server";
            naoNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(naoNotifyIcon_MouseDoubleClick);

            InitializeComponent();

            // call the Camera constuctor, and set the image format to 640x480
            naoCam = new Camera();

            // call the Motion constructor
            naoMotion = new Motion();

            currentFormat = naoCam.NaoCamImageFormats[2];

            // Make sure the standard output directory exists
            //if (!System.IO.Directory.Exists("C:\\NAOserver\\"))
            //{
            //    System.IO.Directory.CreateDirectory("C:\\NAOserver\\");
            //}

            connect();
        }

        /// <summary>
        /// // Restores the window when the icon is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void naoNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Remember needs event in xaml!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                //naoNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                //naoNotifyIcon.BalloonTipText = "Minimized the app ";
                //naoNotifyIcon.ShowBalloonTip(400);
                naoNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                naoNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// called when the window closes
        /// </summary>
        /// <param name="sender"> object that created the event </param>
        /// <param name="e"> any addtional arguments </param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                appServer.Stop(); // stop the websocket

                // disconnect from camera and stop the timer
                naoCam.Disconnect();
                sendFrameTimer.Stop();
                //recordFrameTimer.Stop();
            }
            catch (Exception)
            { }

            naoNotifyIcon.Dispose();
        }

        /// <summary>
        /// connect to the NAO robot
        /// </summary>
        /// <param name="sender"> object that created event </param>
        /// <param name="e"> any addional methods </param>
        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            connect();
        }

        /// <summary>
        /// disconnects from the NAO robot
        /// </summary>
        /// <param name="sender"> object that created the event </param>
        /// <param name="e"> any additional arguments </param>
        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {

            // disconnect from camera and stop the timer
            sendFrameTimer.Stop();
            //recordFrameTimer.Stop();
            naoCam.Disconnect();
            appServer.Stop();

            connectButton.IsEnabled = true;
            disconnectButton.IsEnabled = false;

        }

        private void connect()
        {
            failed = false;

            appServer = new WebSocketServer();

            //Setup the websocket
            if (!appServer.Setup(port)) //Setup with listening port
            {
                failed = true;
            }

            //Try to start the websocket
            if (!appServer.Start() && !failed)
            {
                failed = true;
            }

            if (!failed)
            {
                // event handlers for websocket events
                appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);
                appServer.SessionClosed += new SessionHandler<WebSocketSession, SuperSocket.SocketBase.CloseReason>(appServer_sessionClosed);

                try // attempt to connect to the camera and motion system
                {
                    // connect to the NAO Motion API
                    naoMotion.connect(ip);

                    naoCam.connect(ip, currentFormat, COLOR_SPACE, FPS);

                    // Create a timer for event based frame acquisition. 
                    // Program will attempt to get new frame 30 times a second
                    sendFrameTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Math.Ceiling(1000.0 / 30));
                    sendFrameTimer.Start();

                    // whenever the timer ticks the bitmapReady event is called
                    sendFrameTimer.Tick += new EventHandler(bitmapReady);

                    // timer to store images to file every 10 seconds
                    //recordFrameTimer.Interval = new TimeSpan(0, 0, 10);
                    //recordFrameTimer.Start();

                    //recordFrameTimer.Tick += new EventHandler(storeFrame);

                    // let rest of program know that camera is ready
                    isCamInitialized = true;

                }
                catch (Exception ex)
                {
                    isCamInitialized = false;

                    failed = true;
                }
            }

            if (!failed)
            {
                connectButton.IsEnabled = false;
                disconnectButton.IsEnabled = true;
            }
        }
       
        /// <summary>
        /// Sends a new jpg through the websocket
        /// </summary>
        /// <param name="sender"> object that called the method </param>
        /// <param name="e"> any additional arguments </param>

        private void bitmapReady(object sender, EventArgs e)
        {
            // check for websocket sessions if none exist nothing needs to be done
            if (sessionList.Count > 0 || saveImage) 
            {
                if (isCamInitialized && !isPictureUpdating)
                {
                    isPictureUpdating = true; // picture is being updated

                    try // try to get a new image
                    {
                        byte[] imageBytes = naoCam.getImage(); // store an image in imageBytes

                        if (imageBytes != null) // if the image isnt empty create a bitmap and send via websocket
                        {
                            imageBitmap = BitmapSource.Create(currentFormat.width, currentFormat.height, 96, 96,
                                PixelFormats.Bgr24, BitmapPalettes.WebPalette, imageBytes, currentFormat.width * 3);

                            frame = BitmapFrame.Create(imageBitmap);

                            // converts bitmap frames to jpg
                            JpegBitmapEncoder converter = new JpegBitmapEncoder();

                            converter.Frames.Add(frame);

                            // memory stream to save jpg to byte array
                            MemoryStream ms = new MemoryStream();

                            converter.Save(ms);

                            ms.Close();

                            byte[] bytes = ms.ToArray();
             
                            // since html can convert base64strings to images, convert the image to a base64 string
                            imageString = Convert.ToBase64String(bytes);

                            // send it to all connected sessions
                            for (int x = 0; x < sessionList.Count; x++)
                            {
                                sessionList[x].Send(imageString);
                            }
                        }
                    }
                    catch (Exception e1)
                    {

                    }
                }
            }
            isPictureUpdating = false; // picture is updated
        }

        /// <summary>
        /// Event handler for new messages recieved via websocket
        /// </summary>
        /// <param name="session"> the session that sent a message </param>
        /// <param name="message"> the message sent </param>
        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            // if start was sent, add the session to the session list
            if (message == "start")
            {
                sessionList.Add(session);
            }
           
           
            // move the robots head in the desired direction
            if (message == "left")
            {
               yaw = naoMotion.getAngle("HeadYaw");
               naoMotion.moveJoint(yaw + .25f, "HeadYaw");
            }
            if (message == "right")
            {
                yaw = naoMotion.getAngle("HeadYaw");
                naoMotion.moveJoint(yaw - .25f, "HeadYaw");
            }
            if (message == "up")
            {
                pitch = naoMotion.getAngle("HeadPitch");
                naoMotion.moveJoint(pitch - .1f, "HeadPitch");
            }
            if (message == "down")
            {
                pitch = naoMotion.getAngle("HeadPitch");
                naoMotion.moveJoint(pitch + .1f, "HeadPitch");
            }
           
            if(message == "Reset NAO")
            {
                naoMotion.connect(ip);
                naoCam.Disconnect();
                naoCam.connect(ip, currentFormat, COLOR_SPACE, FPS);
            }
            if (message == "Disconnect")
            {
                naoCam.Disconnect();
            }
        }

        
 

        /// <summary>
        /// event handler for sessions disconnecting
        /// </summary>
        /// <param name="session"> the session that is disconnecting </param>
        /// <param name="close"> the reason why the session was closed </param>
        static void appServer_sessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason close)
        {
            sessionList.Remove(session); // remove the session from the session list
        }
    }
}
