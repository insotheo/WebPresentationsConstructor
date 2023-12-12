using System.Windows;

namespace MessageBoxesWindows
{
    public class MessBox
    {
        public static void showError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult showQuestionWithThreeOptions(string message)
        {
            return MessageBox.Show(message, "Вопрос", MessageBoxButton.YesNoCancel, MessageBoxImage.Question); ;
        }

        public static MessageBoxResult showQuestionWithTwoOptions(string message)
        {
            return MessageBox.Show(message, "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question); ;
        }

        public static void showInfo(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void showWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
