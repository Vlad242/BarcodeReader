using AForge.Video;
using AForge.Video.DirectShow;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZXing;

namespace BarcodeReader
{
    public partial class Form1 : Form
    {

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private ZXing.BarcodeReader reader;

        public Form1()
        {
            InitializeComponent();
        }

        delegate void SetStringDelegate(string parameter);

        void SetResult(string result)
        {
            if (!InvokeRequired)
            {
                resultBox.Text = result;
            }
            else
            {
                Invoke(new SetStringDelegate(SetResult), new object[] { result });
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            reader = new ZXing.BarcodeReader();
           // reader.Options.PossibleFormats = new List<BarcodeFormat>();
           // reader.Options.PossibleFormats.Add(BarcodeFormat.QR_CODE);

            if (videoDevices.Count > 0)
            {
                foreach(FilterInfo device in videoDevices)
                {
                    devices.Items.Add(device.Name);
                }
                devices.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if(videoSource == null)
            {
                videoSource = new VideoCaptureDevice(videoDevices[devices.SelectedIndex].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("You have runnable scanning proccess!");
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            window.Image = bitmap;
            ZXing.Result result = reader.Decode((Bitmap)eventArgs.Frame.Clone());
            if(result != null)
            {
                SetResult(result.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(videoSource != null)
            {
                videoSource.Stop();
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (videoSource != null)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                window.Image = null;
                videoSource = null;
            }
            else
            {
                MessageBox.Show("Opened reader stream no detected!");
            }
        }
    }
}
