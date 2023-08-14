using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace VRCOSC
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string vrcAddress = "127.0.0.1";
        private int vrcPort = 9000;
        private string currentText = string.Empty;

        public string CurrentText
        {
            get { return currentText; }
            set
            {
                currentText = value;
                OnPropertyChanged(nameof(CurrentText));
            }
        }

        public string VrcAddress
        {
            get { return vrcAddress; }
            set
            {
                vrcAddress = value;
                OnPropertyChanged(nameof(VrcAddress));
            }
        }

        public int VrcPort
        {
            get { return vrcPort; }
            set
            {
                vrcPort = value;
                OnPropertyChanged(nameof(VrcPort));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateCharacterCount();
        }

        public void SendOSCMessage()
        {
            var message = new SharpOSC.OscMessage("/chatbox/input", CurrentText, true);
            var sender = new SharpOSC.UDPSender(VrcAddress, VrcPort);
            sender.Send(message);
        }

        public void SendOSCTypingSignal(bool typing)
        {
            var message = new SharpOSC.OscMessage("/chatbox/typing", typing);
            var sender = new SharpOSC.UDPSender(VrcAddress, VrcPort);
            sender.Send(message);
        }

        private void ClearMessage()
        {
            ChatBox.Text = string.Empty;
        }

        private void UpdateCharacterCount()
        {
            NumberLetter.Text = $"({ChatBox.Text.Length}/144)";
            SendOSCTypingSignal(ChatBox.Text.Length > 0);
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            SendOSCMessage();
            ClearMessage();
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendOSCMessage();
                ClearMessage();
            }
        }

        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateCharacterCount();
        }
    }
}
