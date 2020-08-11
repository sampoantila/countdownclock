using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CountDownClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer broadcastTimer = new DispatcherTimer();
        private DateTime? BroadCastTime = null;
        private readonly DispatcherTimer speechTimer = new DispatcherTimer();
        private DateTime? SpeechTime = null;
        private readonly DispatcherTimer runningTimer = new DispatcherTimer();
        private DateTime? RunningTime = null;
        private readonly DispatcherTimer clockTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;

            TargetTime.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            SpeechLength.KeyDown += new KeyEventHandler(SpeechTextBox_KeyDown);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var userPrefs = new UserPreferences
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowTop = Top,
                WindowLeft = Left,
                WindowState = WindowState
            };

            userPrefs.Save();

            base.OnClosing(e);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void UpdateBroadcastTimerLabel()
        {
            var now = DateTime.Now;
            if (BroadCastTime.HasValue)
            {
                var target = new DateTime(now.Year, now.Month, now.Day)
                    .AddHours(BroadCastTime.Value.Hour)
                    .AddMinutes(BroadCastTime.Value.Minute)
                    .AddSeconds(BroadCastTime.Value.Second);

                var diff = target.Subtract(DateTime.Now);
                if (diff.Hours < 0 || diff.Minutes < 0 || diff.Seconds < 0)
                {
                    BroadcastTimer.Content = "-.--";
                    broadcastTimer.Stop();
                }
                else
                {
                    var left = new DateTime(now.Year, now.Month, now.Day, diff.Hours, diff.Minutes, diff.Seconds);
                    BroadcastTimer.Content = diff.Hours > 0 ? left.ToString("H:mm:ss") : left.ToString("m:ss");
                }
            }
        }

        void BroadcastTimer_Tick(object sender, EventArgs e)
        {
            UpdateBroadcastTimerLabel();
        }

        private void BroadcastStopBtn_Click(object sender, RoutedEventArgs e)
        {
            broadcastTimer.Stop();
        }

        private void BroadcastStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TargetTime.Text)) return;

            BroadCastTime = DateTime.Parse(TargetTime.Text);

            if (BroadCastTime.Value < DateTime.Now)
            {
                BroadcastTimer.Content = "Target passed";
            }
            else
            {
                UpdateBroadcastTimerLabel();
                broadcastTimer.Interval = TimeSpan.FromMilliseconds(50);
                broadcastTimer.Tick += BroadcastTimer_Tick;
                broadcastTimer.Start();
            }
        }

        void SpeechTimer_Tick(object sender, EventArgs e)
        {
            UpdateSpeechTimerLabel();
        }

        private void UpdateSpeechTimerLabel()
        {
            var now = DateTime.Now;
            if (SpeechTime.HasValue)
            {
                var target = SpeechTime - DateTime.Now;

                if (target.Value.TotalSeconds <= 0)
                {
                    SpeechTimer.Content = "0.00";
                    if (SpeechTimer.Foreground == Brushes.Black)
                    {
                        SpeechTimer.Foreground = Brushes.Red;
                    }
                    else
                    {
                        SpeechTimer.Foreground = Brushes.Black;
                    }

                    // Slow down blinking time
                    speechTimer.Interval = TimeSpan.FromSeconds(1);
                    speechTimer.Start();
                }
                else
                {
                    var left = new DateTime(now.Year, now.Month, now.Day, target.Value.Hours, target.Value.Minutes, target.Value.Seconds);
                    SpeechTimer.Content = target.Value.Hours > 0 ? left.ToString("H:mm:ss") : left.ToString("m:ss");
                }
            }
        }

        private void StopSpeechBtn_Click(object sender, RoutedEventArgs e)
        {
            speechTimer.Stop();
        }

        private void StartSpeechBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SpeechLength.Text)) return;

            if (DateTime.TryParse(SpeechLength.Text, out DateTime speechLength))
            {
                SpeechTime = DateTime.Now
                    .AddMinutes(speechLength.Hour)
                    .AddSeconds(speechLength.Minute);
            }
            else
            {
                int minutes = Int32.Parse(SpeechLength.Text);
                SpeechTime = DateTime.Now
                    .AddHours(minutes/60)
                    .AddMinutes(minutes%60);
            }

            UpdateSpeechTimerLabel();
            speechTimer.Interval = TimeSpan.FromMilliseconds(300);
            speechTimer.Tick += SpeechTimer_Tick;
            speechTimer.Start();
        }

        private void SpeechTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSpeechBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void StopRunningBtn_Click(object sender, RoutedEventArgs e)
        {
            runningTimer.Stop();
        }

        private void StartRunningBtn_Click(object sender, RoutedEventArgs e)
        {
            RunningTime = DateTime.Now;

            UpdateRunningTimerLabel();
            runningTimer.Interval = TimeSpan.FromMilliseconds(300);
            runningTimer.Tick += RunningTimer_Tick;
            runningTimer.Start();
        }

        void RunningTimer_Tick(object sender, EventArgs e)
        {
            UpdateRunningTimerLabel();
        }

        private void UpdateRunningTimerLabel()
        {
            var now = DateTime.Now;
            var diff = now.Subtract(RunningTime.Value);

            var gone = new DateTime(now.Year, now.Month, now.Day, diff.Hours, diff.Minutes, diff.Seconds);
            RunningTimer.Content = diff.Hours > 0 ? gone.ToString("H:mm:ss") : gone.ToString("m:ss");
        }

        private void StartClockBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateClockTimerLabel();
            clockTimer.Interval = TimeSpan.FromMilliseconds(300);
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        private void UpdateClockTimerLabel()
        {
            ClockTimer.Content = DateTime.Now.ToString("H:mm:ss");
        }

        void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateClockTimerLabel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CountDownClockTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BroadcastTab.IsSelected)
            {
                this.Title = "Lähetys";
            }

            if (SpeechTab.IsSelected)
            {
                this.Title = "Puheen kesto";
            }

            if (RunningTab.IsSelected)
            {
                this.Title = "Kuluva aika";
            }

            if (ClockTab.IsSelected)
            {
                this.Title = "Kello";
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
