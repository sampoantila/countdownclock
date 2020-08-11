using System.Windows;

namespace CountDownClock
{
    public class UserPreferences
    {
        private double _windowTop;
        private double _windowLeft;
        private double _windowHeight;
        private double _windowWidth;
        private WindowState _windowState;

        public UserPreferences()
        {
            //Load the settings
            Load();

            //Size it to fit the current screen
            SizeToFit();

            //Move the window at least partially into view
            MoveIntoView();
        }

        public double WindowTop
        {
            get { return _windowTop; }
            set { _windowTop = value; }
        }

        public double WindowLeft
        {
            get { return _windowLeft; }
            set { _windowLeft = value; }
        }

        public double WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; }
        }

        public double WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

        public WindowState WindowState
        {
            get { return _windowState; }
            set { _windowState = value; }
        }

        private void Load()
        {
            _windowTop =Settings.Default.WindowTop;
            _windowLeft = Settings.Default.WindowLeft;
            _windowHeight = Settings.Default.WindowHeight;
            _windowWidth = Settings.Default.WindowWidth;
            _windowState = Settings.Default.WindowState;
        }

        public void Save()
        {
            if (_windowState != WindowState.Minimized)
            {
                Settings.Default.WindowTop = _windowTop;
                Settings.Default.WindowLeft = _windowLeft;
                Settings.Default.WindowHeight = _windowHeight;
                Settings.Default.WindowWidth = _windowWidth;
                Settings.Default.WindowState = _windowState;

                Settings.Default.Save();
            }
        }

        public void SizeToFit()
        {
            if (_windowHeight > SystemParameters.VirtualScreenHeight)
            {
                _windowHeight = SystemParameters.VirtualScreenHeight;
            }

            if (_windowWidth > SystemParameters.VirtualScreenWidth)
            {
                _windowWidth = SystemParameters.VirtualScreenWidth;
            }
        }

        public void MoveIntoView()
        {
            if (_windowTop + _windowHeight / 2 > SystemParameters.VirtualScreenHeight)
            {
                _windowTop = SystemParameters.VirtualScreenHeight - _windowHeight;
            }

            if (_windowLeft + _windowWidth / 2 > SystemParameters.VirtualScreenWidth)
            {
                _windowLeft = SystemParameters.VirtualScreenWidth - _windowWidth;
            }

            if (_windowTop < 0)
            {
                _windowTop = 0;
            }

            if (_windowLeft < 0)
            {
                _windowLeft = 0;
            }
        }
    }
}
