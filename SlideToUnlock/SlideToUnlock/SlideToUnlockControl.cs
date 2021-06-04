using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SlideToUnlock
{
    //https://stackoverflow.com/questions/45936224/xamarin-forms-slide-button
    public class SlideToUnlockControl : AbsoluteLayout
    {
        public static readonly BindableProperty ThumbProperty =
        BindableProperty.Create(
            "Thumb", typeof(View), typeof(SlideToUnlockControl),
            defaultValue: default(View), propertyChanged: OnThumbChanged);

        public View Thumb
        {
            get { return (View)GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        private static void OnThumbChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SlideToUnlockControl)bindable).OnThumbChangedImpl((View)oldValue, (View)newValue);
        }

        protected virtual void OnThumbChangedImpl(View oldValue, View newValue)
        {
            OnSizeChanged(this, EventArgs.Empty);
        }

        public static readonly BindableProperty TrackBarProperty =
            BindableProperty.Create(
                "TrackBar", typeof(View), typeof(SlideToUnlockControl),
                defaultValue: default(View), propertyChanged: OnTrackBarChanged);

        public View TrackBar
        {
            get { return (View)GetValue(TrackBarProperty); }
            set { SetValue(TrackBarProperty, value); }
        }

        public Button HangupButton { get; private set; }

        private static void OnTrackBarChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((SlideToUnlockControl)bindable).OnTrackBarChangedImpl((View)oldValue, (View)newValue);
        }

        protected virtual void OnTrackBarChangedImpl(View oldValue, View newValue)
        {
            OnSizeChanged(this, EventArgs.Empty);
        }

        private PanGestureRecognizer _panGesture = new PanGestureRecognizer();
        private View _gestureListener;
        public SlideToUnlockControl()
        {
            _panGesture.PanUpdated += OnPanGestureUpdated;
            SizeChanged += OnSizeChanged;

            _gestureListener = new ContentView { BackgroundColor = Color.White, Opacity = 0.05 };
            _gestureListener.GestureRecognizers.Add(_panGesture);
            HangupButton = new Button();
            HangupButton.Text = "";
            HangupButton.TextColor = Color.Black; 
            HangupButton.BackgroundColor = Color.FromHex("#7b9dbb");
            HangupButton.CornerRadius = 10; 
            HangupButton.Clicked += HangupButton_Clicked;
        }

        private void HangupButton_Clicked(object sender, EventArgs e)
        {
            TrackBar.IsVisible = true;
            Thumb.IsVisible = true;
            HangupButton.Text = "";
            LowerChild(HangupButton);
        }

        public event EventHandler SlideCompleted;
        public event EventHandler SlideStarted; 

        private const double _fadeEffect = 0.5;
        private const uint _animLength = 50;
        async void OnPanGestureUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Thumb == null | TrackBar == null)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    await TrackBar.FadeTo(_fadeEffect, _animLength);
                    SlideStarted?.Invoke(this, EventArgs.Empty);
                    break;

                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    var x = Math.Max(0, e.TotalX);
                    if (x > (Width - Thumb.Width))
                        x = (Width - Thumb.Width);

                    if (e.TotalX < Thumb.TranslationX)
                        return;
                    Thumb.TranslationX = x;
                    break;

                case GestureStatus.Completed:
                    var posX = Thumb.TranslationX;

                    if (posX >= (Width - Thumb.Width - 10/* keep some margin for error*/))
                    {
                        SlideCompleted?.Invoke(this, EventArgs.Empty);
                        HangupButton.Text = "Hang Up";
                        TrackBar.IsVisible = false;
                        Thumb.IsVisible = false;
                        RaiseChild(HangupButton);
                    }

                    // Reset translation applied during the pan (snap effect)
                    await TrackBar.FadeTo(1, _animLength);
                    await Thumb.TranslateTo(0, 0, _animLength * 2, Easing.CubicIn);
                    break;
            }
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;
            if (Thumb == null || TrackBar == null)
                return;


            Children.Clear();

            SetLayoutFlags(HangupButton, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(HangupButton, new Rectangle(0, 0, 1, 1));
            Children.Add(HangupButton);

            SetLayoutFlags(TrackBar, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(TrackBar, new Rectangle(0, 0, 1, 1));
            Children.Add(TrackBar);

            SetLayoutFlags(Thumb, AbsoluteLayoutFlags.None);
            SetLayoutBounds(Thumb, new Rectangle(0, 0, this.Width / 5, this.Height));
            Children.Add(Thumb);

            SetLayoutFlags(_gestureListener, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(_gestureListener, new Rectangle(0, 0, 1, 1));
            Children.Add(_gestureListener);
        }
    }
}
