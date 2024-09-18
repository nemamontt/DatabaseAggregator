using System.Windows.Input;

namespace ViewModels
{
    public static class CommandHelper
    {
        public static bool TryExecute(this ICommand command, object? parameter)
        {
            bool can = command.CanExecute(parameter);
            if (can)
                command.Execute(parameter);
            return can;
        }
        public static bool TryExecute(this ICommand command)
            => command.TryExecute(null);
    }
}

