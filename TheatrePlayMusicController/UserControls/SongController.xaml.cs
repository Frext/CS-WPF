using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheatrePlayMusicController.UserControls
{
    /// <summary>
    /// Interaction logic for SongController.xaml
    /// </summary>
    public partial class SongController : UserControl
    {
        private static class MyBorderColors
        {
            public static Brush InactiveColor = Brushes.White;
            public static Brush ActiveColor = Brushes.Red;
        }

        public SongController()
        {
            InitializeComponent();
            DataContext = this;

            EnablePlayButton_DisablePauseButton();
           
            sldVolumeValue.Value = 50;    // The volume element of MediaElement is set to 0.5 (50) by default. So, we should set the value of the slider to 50 by default.

            // Pass the method that will be called in every 1000 milliseconds.
            // Pass null for the state because we won't need it.
            // The timer won'T start because it's delayed for an infinite time.
            myTimer = new Timer(new TimerCallback(myTimer_Tick),null,Timeout.Infinite,1000);
        }

        public string SongName { get; set; }

        public Uri MusicSource { get; set; }

        private readonly Timer myTimer;

        #region Helper Methods

        private void StartSong() => meSongPlayer.Play();

        private void PauseSong() => meSongPlayer.Pause();

        private void ResetSong() => meSongPlayer.Stop();

        private void EnablePlayButton_DisablePauseButton()
        {
            btnStartSong.IsEnabled = true;
            btnPauseSong.IsEnabled = false;
        }

        private void EnablePauseButton_DisablePlayButton()
        {
            btnStartSong.IsEnabled = false;
            btnPauseSong.IsEnabled = true;
        }

        private void SetBorderColorTo(Brush color) => myBorder.BorderBrush = color;

        private void StartTimer()
        {
            myTimer.Change(1, 1000);
            // Start the timer with a little delay and set the interval to 1 second.

            // The little delay is needed to obtain the position of the media player correctly. 
            // If you set it to 0, and click on the buttons with this sequence : Start-Pause-Start, the text block shows 00:00 for one second, and shows the real position after one second.
        }

        private void PauseTimer() => myTimer.Change(Timeout.Infinite, Timeout.Infinite);// Start the timer with an Infinite delay. In other words, don't start it.

        private void UpdateTBTimePassed() => tbTimePassed.Text = meSongPlayer.Position.ToString(@"mm\ \:\ ss");
        #endregion

        private void sldVolumeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbVolumeValue.Text = ((int)sldVolumeValue.Value).ToString(); // Update the text inside volume text block next to the volume slider.

            meSongPlayer.Volume = sldVolumeValue.Value / 100; // It's divided by 100 because the volume element uses a linear scale between 0 and 1.
        }

        private void btnStartSong_Click(object sender, RoutedEventArgs e)
        {
            StartSong();
            StartTimer();

            SetBorderColorTo(MyBorderColors.ActiveColor);
            EnablePauseButton_DisablePlayButton();
        }

        private void btnPauseSong_Click(object sender, RoutedEventArgs e)
        {
            PauseSong();
            PauseTimer();

            SetBorderColorTo(MyBorderColors.InactiveColor);
            EnablePlayButton_DisablePauseButton();
        }
        private void btnResetSong_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Do you REALLY want to reset \"{SongName}\"?", "Application", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ResetSong();
                UpdateTBTimePassed();

                SetBorderColorTo(MyBorderColors.InactiveColor);
                EnablePlayButton_DisablePauseButton();
            }
        }

        private void meSongPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            ResetSong();
            UpdateTBTimePassed();

            SetBorderColorTo(MyBorderColors.InactiveColor);
            EnablePlayButton_DisablePauseButton();
        }

        private void myTimer_Tick(object state)
        {
            // Use Invoke() to access the UI thread outside of it.
            // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it/9732853#9732853

            Dispatcher.Invoke(() =>
            {
                if (meSongPlayer.Source != null)
                {
                    UpdateTBTimePassed();
                }

                else
                {
                    PauseTimer();   // If the timer is not paused, this message comes out every second.
                    MessageBox.Show($"The song file for \"{SongName}\". cannot be loaded.\nThe application will terminate.", "Error", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
            });

        }

        private void meSongPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e) => MessageBox.Show($"An error occured, error message:\n\"{e.ErrorException}\"");
    }
}
