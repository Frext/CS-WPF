using System;
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
        public SongController()
        {
            InitializeComponent();
            this.DataContext = this;

            EnablePlayButton_DisablePauseButton();

            volumeSlider.Value = 50;    // The volume element of MediaElement is set to 0.5 (50) by default. So, we should set the value of the slider to 50 by default.
        }

        public string SongName { get; set; }

        public Uri MusicSource { get; set; }

        #region Helper Methods

        private void PlaySong()
        {
            myMusic.Play();
        }

        private void PauseSong()
        {
            myMusic.Pause();
        }

        private void EnablePlayButton_DisablePauseButton()
        {
            playSongButton.IsEnabled = true;
            pauseSongButton.IsEnabled = false;
        }

        private void EnablePauseButton_DisablePlayButton()
        {
            playSongButton.IsEnabled = false;
            pauseSongButton.IsEnabled = true;
        }

        private void SetBorderColorTo(Brush color)
        {
            myBorder.BorderBrush = color;
        }
        #endregion

        private void playSongButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySong();
            SetBorderColorTo(Brushes.Red);
            EnablePauseButton_DisablePlayButton();
        }

        private void pauseSongButton_Click(object sender, RoutedEventArgs e)
        {
            PauseSong();
            SetBorderColorTo(Brushes.White);
            EnablePlayButton_DisablePauseButton();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeTextBlock.Text = ((int)volumeSlider.Value).ToString(); // Update the text inside volume text block next to the volume slider.

            myMusic.Volume = volumeSlider.Value / 100; // It's divided by 100 because the volume element uses a linear scale between 0 and 1.
        }

        private void restartSongButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Do you REALLY want to restart \"{songNameTextBlock.Text}\"?"  , "Application", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                myMusic.Stop();
                SetBorderColorTo(Brushes.White);
                EnablePlayButton_DisablePauseButton();
            }

        }
    }
}
