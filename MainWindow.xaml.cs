using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PomodoroApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private ViewModel viewModel1 = new ViewModel();

        private int PomodoroCount = 0;
        private const string POMODORO_COUNT_IS = "Pomodoro Count : ";

        private static class TimerValue{
            public static int seconds;
            public static int minutes;
        }
        private static class PomodoroPhaseDurationsInMinutes
        {
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

            DataContext = viewModel1;

            InitializeComponent();

            // First app screen
            ResetApp();
        }
        private void ResetApp()
        {
            // Set the pomodor phase to work phase

            viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.WorkMessage;

            SetTimerValueTo(PomodoroPhaseDurationsInMinutes.WorkDuration);
            UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.WorkDuration, 0);

            SetPomodoroCountTo(0);

            StopTimerAndDisableStopButton();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DecreaseTimerValue();

            UpdateTimeLeftString(TimerValue.minutes,TimerValue.seconds);
        }

        #region Button Click Events
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartTimerAndDisableStartButton();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopTimerAndDisableStopButton();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetApp();
        }
        #endregion

        #region Timer Methods
        private void SetTimerValueTo(int minutes)
        {
            TimerValue.minutes = minutes;
            TimerValue.seconds = 0;
        }
        
        private void StartTimerAndDisableStartButton()
        {
            dispatcherTimer.Start();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void StopTimerAndDisableStopButton()
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
                    StopTimerAndDisableStopButton();
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

        #region Text Update Method
        private void UpdateTimeLeftString(int minutes, int seconds)
        {
            // From https://stackoverflow.com/questions/5972949/number-formatting-how-to-convert-1-to-01-2-to-02-etc/5972961
            viewModel1.TimeLeftString = $"{minutes}:{seconds.ToString("D2")}";    // D2 = 2 Digits
            
            if(TimerValue.minutes.Equals(0) && TimerValue.seconds.Equals(0))   // If the timer has ended
            {
                SwitchToNextPomodoroPhase();

                PlayTimerFinishedSound();
            }
        }

        #endregion

        private void SwitchToNextPomodoroPhase()
        {
            if (viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.WorkMessage)                    // Work phase -> Short Break phase
            {
                viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.ShortBreakMessage;

                SetTimerValueTo(PomodoroPhaseDurationsInMinutes.ShortBreakDuration);
                UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.ShortBreakDuration, 0);
            }
            else if(viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.ShortBreakMessage)          // Short Break phase -> Long Break  OR  Work phase
            {
                // A work phase and a short break phase is considered a pomodoro
                SetPomodoroCountTo(PomodoroCount + 1);


                if (PomodoroCount % 4 == 0)     // For every 4 pomodoro, take a long break                 Short Break phase -> Long Break phase
                {
                    viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.LongBreakMessage;

                    SetTimerValueTo(PomodoroPhaseDurationsInMinutes.LongBreakDuration);
                    UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.LongBreakDuration, 0);
                }

                else                                                                                    // Short Break phase -> Work phase
                {
                    viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.WorkMessage;

                    SetTimerValueTo(PomodoroPhaseDurationsInMinutes.WorkDuration);
                    UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.WorkDuration, 0);
                }
            }
            else if(viewModel1.PomodoroPhaseString == PomodoroPhaseMessages.LongBreakMessage)           // Long Break phase -> Work phase
            {
                viewModel1.PomodoroPhaseString = PomodoroPhaseMessages.WorkMessage;

                SetTimerValueTo(PomodoroPhaseDurationsInMinutes.WorkDuration);
                UpdateTimeLeftString(PomodoroPhaseDurationsInMinutes.WorkDuration, 0);
            }
        }

        private void SetPomodoroCountTo(int pomodoroCountToSet)
        {
            PomodoroCount = pomodoroCountToSet;

            viewModel1.PomodoroCountString = POMODORO_COUNT_IS + PomodoroCount;
        }
    }
}
