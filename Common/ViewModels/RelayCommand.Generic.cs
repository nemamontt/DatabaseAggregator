namespace ViewModels
{
    /// <summary>Реализация RelayCommand для методов с обобщённым параметром.</summary>
    /// <typeparam name="T">Тип параметра методов.</typeparam>
    public class RelayCommand<T> : RelayCommand
    {
        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object?}, CanExecuteHandler{object?})"/>
        /// <param name="converter">Делегат метода преобразующего <see cref="object"/>-параметр,
        /// в тип параметра команды <typeparamref name="T"/>. Применяется,
        /// если не получается привести <see cref="object"/> к <typeparamref name="T"/> оператором "is".</param>
        public RelayCommand(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute, ConverterFromObjectHandler<T> converter)
            : base
            (
                  execute is null ? throw Exceptions.executeException
                  : converter is null ? throw Exceptions.converterException
                  : p =>
                  {
                      if (p is T t || converter(p, out t))
                          execute(t);
                  },

                  canExecute is null ? throw Exceptions.canExecuteException
                  : converter is null ? throw Exceptions.converterException
                  : p => (p is T t || converter(p, out t)) && canExecute(t)
            )
        { }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T})"/>
        public RelayCommand(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute)
            : base
            (
                  execute is null ? throw Exceptions.executeException
                  : p =>
                  {
                      if (p is T t)
                          execute(t);
                  },

                  canExecute is null ? throw Exceptions.canExecuteException
                  : p => p is T t && canExecute(t)
            )
        { }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T})"/>
        public RelayCommand(ExecuteHandler<T> execute, ConverterFromObjectHandler<T> converter)
            : base
            (
                  execute is null ? throw Exceptions.executeException
                  : converter is null ? throw Exceptions.converterException
                  : p =>
                  {
                      if (p is T t || converter(p, out t))
                          execute(t);
                  }
            )
        { }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T})"/>
        public RelayCommand(ExecuteHandler<T> execute)
            : base
            (
                  execute is null ? throw Exceptions.executeException
                  : p =>
                  {
                      if (p is T t)
                          execute(t);
                  }
            )
        { }
    }
}

