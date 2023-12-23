using System.Windows;

namespace WPC_Editor
{
    /// <summary>
    /// Логика взаимодействия для HoldOnWindow.xaml
    /// </summary>
    public partial class HoldOnWindow : Window
    {
        public HoldOnWindow(string processName)
        {
            InitializeComponent();
            titleTB.Text = this.Title.Replace("procname", processName);
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
