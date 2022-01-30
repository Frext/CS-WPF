using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PomodoroApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private int PomodoroPhase;
        private int PomodoroCount;
 
        private static class TimerValue{
            public static int seconds;
            public static int minutes;
        }

        private static class PomodoroPhases
        {
            public static int Work = 2;
            public static int ShortBreak = 1;
            public static int LongBreak = 0;
        }

        private static class PomodoroPhaseDurations
        {
            // In minutes
            public static int WorkDuration = 25;
            public static int ShortBreakDuration = 5;
            public static int LongBreakDuration = 15;
        }

        private static class PomodoroPhaseMessages
        {
            // Visible on the top-left part of the app.
            public static string WorkMessage = "Work";
            public static string ShortBreakMessage = "Break";
            public static string LongBreakMessage = "Long Break";
        }
        public MainWindow()
        {
            // Set up the dispatcherTimer settings.
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            InitializeComponent();

            btnStop.IsEnabled = false;

            // First app screen
            ResetApp();
        }
        private void ResetApp()
        {
            UpdatePomodoroPhaseTo(PomodoroPhases.Work);

            UpdateTimeLeftTo(PomodoroPhaseDurations.WorkDuration);

            UpdatePomodoroCountTo(0);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DecreaseTimerValue();

            UpdateTimeLeftText();
        }

        #region Button Click Events
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartTimerAndManageButtons();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopTimerAndManageButtons();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            StopTimerAndManageButtons();

            ResetApp();
        }
        #endregion

        #region Timer Methods
        private void SetTimerValueTo(int minutes)
        {
            TimerValue.minutes = minutes;
            TimerValue.seconds = 0;
        }
        
        private void StartTimerAndManageButtons()
        {
            dispatcherTimer.Start();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void StopTimerAndManageButtons()
        {
            dispatcherTimer.Stop();

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void DecreaseTimerValue()
        {
            if (TimerValue.seconds == 0)
            {
                if (TimerValue.minutes > 0)
                {
                    TimerValue.minutes--;
                    TimerValue.seconds = 59;
                }
                else
                {
                    dispatcherTimer.Stop();
                }
            }
            else
            {
                TimerValue.seconds--;
            }
        }
        private void PlayTimerFinishedSound()
        {
            using (var soundPlayer = new SoundPlayer(@"C:\Users\furka\source\repos\PomodoroApp\Resources\finish_sound_effect.wav"))
            {
                soundPlayer.Play();
            }
        }
        #endregion

        #region TextBlock Update Methods - Helper Methods
        private void UpdateTimeLeftText()
        {
            // From https://stackoverflow.com/questions/5972949/number-formatting-how-to-convert-1-to-01-2-to-02-etc/5972961
            tbTimeLeft.Text = $"{TimerValue.minutes}:{TimerValue.seconds.ToString("D2")}";
            
            if(tbTimeLeft.Text == "0:00")   // If the time has ended
            {
                StopTimerAndManageButtons();

                SwitchToNextPomodoroPhase();

                PlayTimerFinishedSound();
            }
        }

        private void UpdatePomodoroCountText()
        {
            tbPomodoroCount.Text = $"Pomodoro Count : {PomodoroCount}";
        }

        private void UpdatePomodoroPhaseText()
        {
            if(PomodoroPhase == PomodoroPhases.Work)
            {
                tbPomodoroPhase.Text = PomodoroPhaseMessages.WorkMessage;
                tbPomodoroPhase.Background = System.Windows.Media.Brushes.PaleVioletRed;
            }
            else if (PomodoroPhase == PomodoroPhases.ShortBreak)
            {
                tbPomodoroPhase.Text = PomodoroPhaseMessages.ShortBreakMessage;
                tbPomodoroPhase.Background = System.Windows.Media.Brushes.SlateGray;
            }
            else if (PomodoroPhase == PomodoroPhases.LongBreak)
            {
                tbPomodoroPhase.Text = PomodoroPhaseMessages.LongBreakMessage;
                tbPomodoroPhase.Background = System.Windows.Media.Brushes.Yellow;
            }
        }
        #endregion

        #region Pomodoro Phase Method
        private void SwitchToNextPomodoroPhase()
        {
            if (PomodoroPhase == PomodoroPhases.Work)
            {
                UpdatePomodoroPhaseTo(PomodoroPhases.ShortBreak);

                UpdateTimeLeftTo(PomodoroPhaseDurations.ShortBreakDuration);
                
            }
            else if(PomodoroPhase == PomodoroPhases.ShortBreak)
            {
                UpdatePomodoroCountTo(++PomodoroCount);
                UpdatePomodoroCountText();
                
                if (PomodoroCount % 4 == 0) // For every 4 pomodoro, take a long break
                {
                    UpdatePomodoroPhaseTo(PomodoroPhases.LongBreak);

                    UpdateTimeLeftTo(PomodoroPhaseDurations.LongBreakDuration);
                }
                else
                {
                    UpdatePomodoroPhaseTo(PomodoroPhases.Work);

                    UpdateTimeLeftTo(PomodoroPhaseDurations.WorkDuration);
                }
            }
            else if(PomodoroPhase == PomodoroPhases.LongBreak)
            {
                UpdatePomodoroPhaseTo(PomodoroPhases.Work);

                UpdateTimeLeftTo(PomodoroPhaseDurations.WorkDuration);
            }
        }
        #endregion

        #region Update Visually and Programatically

        private void UpdatePomodoroPhaseTo(int pomodoroPhaseToSet)
        {
            PomodoroPhase = pomodoroPhaseToSet;

            UpdatePomodoroPhaseText();
        }

        private void UpdateTimeLeftTo(int minutesToSet)
        {
            SetTimerValueTo(minutesToSet);

            UpdateTimeLeftText();
        }

        private void UpdatePomodoroCountTo(int pomodoroCountToSet)
        {
            PomodoroCount = pomodoroCountToSet;
            UpdatePomodoroCountText();
        }
        #endregion
    }
}
