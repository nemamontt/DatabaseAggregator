using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ViewModels
{
    #region Класс команд - RelayCommand
    /// <summary>Класс реализующий <see cref="ICommand"/>.</summary>
    public partial class RelayCommand : ICommand
    {
        private readonly CanExecuteHandler<object?> canExecute;
        private readonly ExecuteHandler<object?> execute;
        private readonly EventHandler requerySuggested;

        /// <summary>Событие извещающее об изменении состояния команды.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Конструктор команды.</summary>
        /// <param name="execute">Выполняемый метод команды.</param>
        /// <param name="canExecute">Метод, возвращающий состояние команды.</param>
        public RelayCommand(ExecuteHandler<object?> execute, CanExecuteHandler<object?> canExecute)
        {
            dispatcher = Application.Current.Dispatcher;
            this.execute = execute ?? throw Exceptions.executeException;
            this.canExecute = canExecute ?? throw Exceptions.canExecuteException;

            requerySuggested = (o, e) => Invalidate();
            CommandManager.RequerySuggested += requerySuggested;
        }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object?}, CanExecuteHandler{object?})"/>
        public RelayCommand(in ExecuteHandler<object?> execute)
        :this(execute, static obj => true)
        { }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object?}, CanExecuteHandler{object?})"/>
        public RelayCommand(ExecuteHandler execute, CanExecuteHandler canExecute)
                : this
                (
                      execute is not null ? p => execute() : throw Exceptions.executeException,
                      canExecute is not null ? p => canExecute() : throw Exceptions.canExecuteException
                )
        { }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object?}, CanExecuteHandler{object?})"/>
        public RelayCommand(ExecuteHandler execute)
                : this
                (
                      execute is not null ? p => execute() : throw Exceptions.executeException
                )
        { }

        private readonly Dispatcher dispatcher;

        /// <summary>Метод, подымающий событие <see cref="CanExecuteChanged"/>.</summary>
        public void RaiseCanExecuteChanged()
        {
            if (dispatcher.CheckAccess())
                Invalidate();
            else
                dispatcher.BeginInvoke(Invalidate);
        }
        private void Invalidate()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>Вызов метода, возвращающего состояние команды.</summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns><see langword="true"/> - если выполнение команды разрешено.</returns>
        public bool CanExecute(object? parameter) => canExecute(parameter);

        /// <summary>Вызов выполняющего метода команды.</summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object? parameter) => execute(parameter);
    }
    #endregion
}

