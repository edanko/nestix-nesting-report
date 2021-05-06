using System.Windows;

namespace Report.Forms
{
    /// <summary>
    /// Interaction logic for Launch.xaml
    /// </summary>
    public partial class Launch //: Window
    {
        public string LaunchString;

        public Launch()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            LaunchString = LaunchTextBox.Text;
            Close();
        }
    }
}
