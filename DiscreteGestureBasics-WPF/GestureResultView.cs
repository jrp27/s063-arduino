//------------------------------------------------------------------------------
// <copyright file="GestureResultView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Stores discrete gesture results for the GestureDetector.
    /// Properties are stored/updated for display in the UI.
    /// </summary>
    public sealed class GestureResultView : INotifyPropertyChanged
    {


        static HttpClient client1 = new HttpClient();
        //static HttpClient client2 = new HttpClient();
        static String light1Ip = "http://18.111.116.43/";
        //static String light2Ip = "http://18.111.122.20/";

        private bool isStanding = false;
        private bool isSitting = false;
        private bool isTyping = false;
        private bool isWriting = false;
        private bool isUsingPhone = false;
        private bool isNapping = false;

        private DateTime lastSentStanding = DateTime.MinValue;
        private DateTime lastSentSitting = DateTime.MinValue;
        private DateTime lastSentTyping = DateTime.MinValue;
        private DateTime lastSentWriting = DateTime.MinValue;
        private DateTime lastSentUsingPhone = DateTime.MinValue;
        private DateTime lastSentNapping = DateTime.MinValue;

        private DateTime lastDetectedStanding = DateTime.MinValue;
        private DateTime lastDetectedSitting = DateTime.MinValue;
        private DateTime lastDetectedTyping = DateTime.MinValue;
        private DateTime lastDetectedWriting = DateTime.MinValue;
        private DateTime lastDetectedUsingPhone = DateTime.MinValue;
        private DateTime lastDetectedNapping = DateTime.MinValue;


        /// <summary> Image to show when the 'detected' property is true for a tracked body </summary>
        private readonly ImageSource seatedImage = new BitmapImage(new Uri(@"Images\Seated.png", UriKind.Relative));

        /// <summary> Image to show when the 'detected' property is false for a tracked body </summary>
        private readonly ImageSource notSeatedImage = new BitmapImage(new Uri(@"Images\NotSeated.png", UriKind.Relative));

        /// <summary> Image to show when the body associated with the GestureResultView object is not being tracked </summary>
        private readonly ImageSource notTrackedImage = new BitmapImage(new Uri(@"Images\NotTracked.png", UriKind.Relative));

        /// <summary> Array of brush colors to use for a tracked body; array position corresponds to the body colors used in the KinectBodyView class </summary>
        private readonly Brush[] trackedColors = new Brush[] { Brushes.Red, Brushes.Orange, Brushes.Green, Brushes.Blue, Brushes.Indigo, Brushes.Violet };

        /// <summary> Brush color to use as background in the UI </summary>
        private Brush bodyColor = Brushes.Gray;

        /// <summary> The body index (0-5) associated with the current gesture detector </summary>
        private int bodyIndex = 0;

        /// <summary> Image to display in UI which corresponds to tracking/detection state </summary>
        private ImageSource imageSource = null;
        
        /// <summary> True, if the body is currently being tracked </summary>
        private bool isTracked = false;

        /// <summary>
        /// Initializes a new instance of the GestureResultView class and sets initial property values
        /// </summary>
        /// <param name="bodyIndex">Body Index associated with the current gesture detector</param>
        /// <param name="isTracked">True, if the body is currently tracked</param>
        /// <param name="detected">True, if the gesture is currently detected for the associated body</param>
        /// <param name="confidence">Confidence value for detection of the 'Seated' gesture</param>
        public GestureResultView(int bodyIndex, bool isTracked, bool standing, bool sitting, bool typing, bool writing, bool usingPhone, bool napping)
        {
            this.BodyIndex = bodyIndex;
            this.IsTracked = isTracked;
            this.isStanding = standing;
            this.isSitting = sitting;
            this.isTyping = typing;
            this.isWriting = writing;
            this.isUsingPhone = usingPhone;
            this.isNapping = napping;
            this.ImageSource = this.notTrackedImage;

            client1.BaseAddress = new Uri(light1Ip);
            client1.DefaultRequestHeaders.Accept.Clear();
            client1.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client1.DefaultRequestHeaders.Add("Connection", "close");

            //client2.BaseAddress = new Uri(light2Ip);
            //client2.DefaultRequestHeaders.Accept.Clear();
            //client2.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
            //client2.DefaultRequestHeaders.Add("Connection", "close");
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 
        /// Gets the body index associated with the current gesture detector result 
        /// </summary>
        public int BodyIndex
        {
            get
            {
                return this.bodyIndex;
            }

            private set
            {
                if (this.bodyIndex != value)
                {
                    this.bodyIndex = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets the body color corresponding to the body index for the result
        /// </summary>
        public Brush BodyColor
        {
            get
            {
                return this.bodyColor;
            }

            private set
            {
                if (this.bodyColor != value)
                {
                    this.bodyColor = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the body associated with the gesture detector is currently being tracked 
        /// </summary>
        public bool IsTracked 
        {
            get
            {
                return this.isTracked;
            }

            private set
            {
                if (this.IsTracked != value)
                {
                    this.isTracked = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture standing has been detected
        /// </summary>
        public bool IsStanding 
        {
            get
            {
                return this.isStanding;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedStanding = DateTime.Now;
                }
                if (this.isStanding != value)
                {
                    this.isStanding = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture sitting has been detected
        /// </summary>
        public bool IsSitting
        {
            get
            {
                return this.isSitting;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedSitting = DateTime.Now;
                }
                if (this.isSitting != value)
                {
                    this.isSitting = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture typing has been detected
        /// </summary>
        public bool IsTyping
        {
            get
            {
                return this.isTyping;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedTyping = DateTime.Now;
                }
                if (this.isTyping != value)
                {
                    this.isTyping = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture writing has been detected
        /// </summary>
        public bool IsWriting
        {
            get
            {
                return this.isWriting;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedWriting = DateTime.Now;
                }
                if (this.isWriting != value)
                {
                    this.isWriting = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture using phone has been detected
        /// </summary>
        public bool IsUsingPhone
        {
            get
            {
                return this.isUsingPhone;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedUsingPhone = DateTime.Now;
                }
                if (this.isUsingPhone != value)
                {
                    this.isUsingPhone = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture napping has been detected
        /// </summary>
        public bool IsNapping
        {
            get
            {
                return this.isNapping;
            }

            private set
            {
                if (value == true)
                {
                    this.lastDetectedNapping = DateTime.Now;
                }
                if (this.isNapping != value)
                {
                    this.isNapping = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets an image for display in the UI which represents the current gesture result for the associated body 
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }

            private set
            {
                if (this.ImageSource != value)
                {
                    this.imageSource = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates the values associated with the discrete gesture detection result
        /// </summary>
        /// <param name="isBodyTrackingIdValid">True, if the body associated with the GestureResultView object is still being tracked</param>
        /// <param name="isGestureDetected">True, if the discrete gesture is currently detected for the associated body</param>
        /// <param name="detectionConfidence">Confidence value for detection of the discrete gesture</param>
        public void UpdateGestureResult(bool isBodyTrackingIdValid, bool standing, bool sitting, bool typing, bool writing, bool usingPhone, bool napping)
        {
            this.IsTracked = isBodyTrackingIdValid;

            if (!this.IsTracked)
            {
                this.ImageSource = this.notTrackedImage;
                this.IsStanding = false;
                this.IsSitting = false;
                this.IsTyping = false;
                this.IsWriting = false;
                this.IsUsingPhone = false;
                this.IsNapping = false;
                this.BodyColor = Brushes.Gray;
            }
            else
            {
                this.IsStanding = standing;
                this.IsSitting = sitting;
                this.IsTyping = typing;
                this.IsWriting = writing;
                this.IsUsingPhone = usingPhone;
                this.IsNapping = napping;
                this.BodyColor = this.trackedColors[this.BodyIndex];

                DateTime mostRecentGesture = this.lastDetectedNapping;
                if (this.lastDetectedUsingPhone.CompareTo(mostRecentGesture) > 0)
                {
                    mostRecentGesture = this.lastDetectedUsingPhone;
                }

                if (this.lastDetectedWriting.CompareTo(mostRecentGesture) > 0)
                {
                    mostRecentGesture = this.lastDetectedWriting;
                }

                if (this.lastDetectedTyping.CompareTo(mostRecentGesture) > 0)
                {
                    mostRecentGesture = this.lastDetectedTyping;
                }

                
                if (this.IsStanding && (DateTime.Now -  this.lastSentStanding).TotalSeconds >  5 && (DateTime.Now - mostRecentGesture).TotalSeconds > 2)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightStanding(client1);
                    //SetLightStanding(client2);
                    this.lastSentStanding = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }

                if (this.IsSitting && (DateTime.Now - this.lastSentSitting).TotalSeconds > 5 && (DateTime.Now - mostRecentGesture).TotalSeconds > 2)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightSitting(client1);
                    //SetLightSitting(client2);
                    this.lastSentSitting = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }

                if (this.IsTyping && (DateTime.Now - this.lastSentTyping).TotalSeconds > 5)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightTyping(client1);
                    //SetLightTyping(client2);
                    this.lastSentTyping = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }

                if (this.IsWriting && (DateTime.Now - this.lastSentWriting).TotalSeconds > 5)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightWriting(client1);
                    //SetLightWriting(client2);
                    this.lastSentWriting = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }

                if (this.IsUsingPhone && (DateTime.Now - this.lastSentUsingPhone).TotalSeconds > 5)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightUsingPhone(client1);
                    //SetLightUsingPhone(client2);
                    this.lastSentUsingPhone = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }

                if (this.IsNapping && (DateTime.Now - this.lastSentNapping).TotalSeconds > 5)
                {
                    this.ImageSource = this.seatedImage;
                    SetLightNapping(client1);
                    //SetLightNapping(client2);
                    this.lastSentNapping = DateTime.Now;
                }
                else
                {
                    this.ImageSource = this.notSeatedImage;
                }
            }
        }

        /// <summary>
        /// Notifies UI that a property has changed
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param> 
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        static async void SetLightStanding(HttpClient client)
        {
            try
            {
                Console.WriteLine("Setting lights to standing");
                HttpResponseMessage response = await client.GetAsync("standing");

                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                }
                else
                {
                    Console.WriteLine("Fail " + content);
                }
            } catch
            {
                Console.WriteLine("Something bad");
                SetLightStanding(client);
            }
        }

        static async void SetLightSitting(HttpClient client)
        {
            try
            { 
                Console.WriteLine("Setting lights to sitting");
                HttpResponseMessage response = await client.GetAsync("sitting");

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                }
                else
                {
                    Console.WriteLine("Fail " + content);
                }
            }
            catch
            {
                Console.WriteLine("Something bad");
                SetLightSitting(client);
            }
        }

        static async void SetLightTyping(HttpClient client)
        {
            //try
            //{
                Console.WriteLine("Setting lights to typing");
                HttpResponseMessage response = await client.GetAsync("typing");

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                }
                else
                {
                    Console.WriteLine("Fail " + content);
                }
            //}
            //catch
            //{
              //  Console.WriteLine("Something bad");
              //  SetLightTyping(client);
            //}
        }

        static async void SetLightWriting(HttpClient client)
        {
            try {
                Console.WriteLine("Setting lights to writing");
                HttpResponseMessage response = await client.GetAsync("writing");

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                }
                else
                {
                    Console.WriteLine("Fail " + content);
                }
            }
            catch
            {
                Console.WriteLine("Something bad");
                SetLightWriting(client);
            }
        }

        static async void SetLightUsingPhone(HttpClient client)
        {
            try {
                Console.WriteLine("Setting lights to using phone");
                HttpResponseMessage response = await client.GetAsync("usingPhone");

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                }
                else
                {
                    Console.WriteLine("Fail " + content);
                }
            }
            catch
            {
                Console.WriteLine("Something bad");
                SetLightUsingPhone(client);
            }
        }

        static async void SetLightNapping(HttpClient client)
        {
            try {
                Console.WriteLine("Setting lights to napping");
                HttpResponseMessage response = await client.GetAsync(requestUri: "napping");

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success " + content);
                } else
                {
                    Console.WriteLine("Fail " + content);
                }
            }
            catch
            {
                Console.WriteLine("Something bad");
                SetLightNapping(client);
            }
        }
    }
}
