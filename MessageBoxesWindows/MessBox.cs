using System.Windows;

namespace MessageBoxesWindows
{
    public class MessBox
    {
        public static void showError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
