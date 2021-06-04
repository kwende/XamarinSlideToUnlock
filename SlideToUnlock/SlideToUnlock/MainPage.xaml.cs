using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SlideToUnlock
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Handle_SlideCompleted(object sender, EventArgs e)
        {
            //HangupButton.Text = "Calling...";
        }

        private void SlideToUnlockControl_SlideStarted(object sender, EventArgs e)
        {
        }

        private void HangupButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
