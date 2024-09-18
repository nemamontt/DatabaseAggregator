using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModels
{

    public abstract partial class ViewModelBase
    {
        private readonly Dictionary<string, RelayCommand> commands = new();

        private bool TryGetCommand(string? commandName, out RelayCommand? command)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw Exceptions.commandNameException;
            return commands.TryGetValue(commandName, out command);
        }

        /// <summary>Конструктор-фабрика команд.</summary>
        /// <typeparam name="T">Тип параметра команды.</typeparam>
        /// <param name="execute">Выполняемый метод команды.</param>
        /// <param name="canExecute">Метод, возвращающий состояние команды.</param>
        /// <param name="converter">Делегат метода преобразующего <see cref="object"/> в тип <typeparamref name="T"/>.</param>
        /// <param name="commandName">Имя комманды - обычно имя свойства.</param>
        /// <returns>Возвращает экземпляр <see cref="RelayCommand"/>.
        /// Все команды содержатся в общем словаре по ключу <paramref name="commandName"/>.
        /// Если команды в словаре нет, то она создаётся и записывается в словарь.
        /// Создаваемые команды подписываются на <see cref="CommandManager.RequerySuggested"/>.</returns>
        /// <exception cref="ArgumentException">Если один из параметров <see langword="null"/>
        /// или если <see cref="string.IsNullOrWhiteSpace(string?)">string.IsNullOrWhiteSpace(<paramref name="commandName"/>)</see>.</exception>
        protected RelayCommand<T> GetCommand<T>(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute, ConverterFromObjectHandler<T> converter, [CallerMemberName] string? commandName = null)
        {

            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand<T>(execute, canExecute, converter);
                commands.Add(commandName!, command);
                RaisePropertyChanged(commandName!);
            }
            return (RelayCommand<T>)command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand<T> GetCommand<T>(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute, [CallerMemberName] string? commandName = null)
        {

            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand<T>(execute, canExecute);
                commands.Add(commandName!, command);
            }
            return (RelayCommand<T>)command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand<T> GetCommand<T>(ExecuteHandler<T> execute, ConverterFromObjectHandler<T> converter, [CallerMemberName] string? commandName = null)
        {

            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand<T>(execute, converter);
                commands.Add(commandName!, command);
            }
            return (RelayCommand<T>)command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand<T> GetCommand<T>(ExecuteHandler<T> execute, [CallerMemberName] string? commandName = null)
        {

            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand<T>(execute);
                commands.Add(commandName!, command);
            }
            return (RelayCommand<T>)command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand GetCommand(ExecuteHandler<object?> execute, CanExecuteHandler<object?> canExecute, [CallerMemberName] string? commandName = null)
        {
            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand(execute, canExecute);
                commands.Add(commandName!, command);
            }
            return command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand GetCommand(ExecuteHandler<object?> execute, [CallerMemberName] string? commandName = null)
        {
            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand(execute);
                commands.Add(commandName!, command);
            }
            return command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand GetCommand(ExecuteHandler execute, CanExecuteHandler canExecute, [CallerMemberName] string? commandName = null)
        {
            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand(execute, canExecute);
                commands.Add(commandName!, command);
            }
            return command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand GetCommand(ExecuteHandler execute, [CallerMemberName] string? commandName = null)
        {
            if (!TryGetCommand(commandName, out RelayCommand? command))
            {
                command = new RelayCommand(execute);
                commands.Add(commandName!, command);
            }
            return command!;
        }

        /// <inheritdoc cref="GetCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T}, string?)"/>
        protected RelayCommand? GetCommand([CallerMemberName] string? commandName = null)
        {
            TryGetCommand(commandName, out RelayCommand? command);
            return command;
        }


        /// <summary>Удаляет команду.</summary>
        /// <param name="commandName">Имя команды.</param>
        /// <returns><see langword="true"/>, если команда удалена.</returns>
        protected bool RemoveCommand(string commandName)
        {
            bool remove = commands.Remove(commandName);
            RaisePropertyChanged(commandName);
            return remove ;
        }

        public static class Exceptions
        {
            public static readonly ArgumentException commandNameException = new("null, Empty и только пробелы не разрешёны", "commandName");
        }

    }
}
