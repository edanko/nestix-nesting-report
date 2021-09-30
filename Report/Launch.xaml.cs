using System.Windows;

namespace Report
{
    public partial class Launch
    {
        public string LaunchString;
        //public bool SaveDxf;

        public Launch()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            LaunchString = LaunchTextBox.Text;
            // if (SaveDxfCheckBox.IsChecked != null)
            // {
            //     SaveDxf = SaveDxfCheckBox.IsChecked.Value;
            // }

            Close();
        }
    }
}