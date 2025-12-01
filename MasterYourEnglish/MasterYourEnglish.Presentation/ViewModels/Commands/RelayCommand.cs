namespace MasterYourEnglish.Presentation.ViewModels.Commands
{
    using System;
    using System.Threading.Tasks; // <-- ДОДАНО: Для роботи з Task
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        // Поля для синхронної команди
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        // Поля для асинхронної команди
        private readonly Func<object, Task> executeAsync; // <-- НОВЕ ПОЛЕ
        private readonly Predicate<object> canExecuteAsync; // <-- НОВЕ ПОЛЕ
        private readonly bool isAsync; // <-- НОВЕ ПОЛЕ
        private bool isExecuting; // <-- НОВЕ ПОЛЕ

        // === СИНХРОННІ КОНСТРУКТОРИ ===

        // 1. Конструктор для команд з параметрами (старий)
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
            this.isAsync = false; // Позначаємо як синхронну
        }

        // 2. Конструктор для команд без параметрів (старий)
        public RelayCommand(Action execute)
            : this(p => execute(), null)
        {
        }

        // === АСИНХРОННИЙ КОНСТРУКТОР ===

        // 3. Конструктор для асинхронних команд
        public RelayCommand(Func<object, Task> executeAsync, Predicate<object> canExecuteAsync = null)
        {
            this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecuteAsync = canExecuteAsync;
            this.isAsync = true; // Позначаємо як асинхронну
        }

        // Примітка: Додайте конструктор для асинхронної команди без параметрів
        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecuteAsync = null)
            : this(
                p => executeAsync(),
                p => canExecuteAsync == null || canExecuteAsync())
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (this.isAsync)
            {
                // Для асинхронної: перевіряємо чи не виконується і чи дозволено Predicate
                return !this.isExecuting && (this.canExecuteAsync == null || this.canExecuteAsync(parameter));
            }
            else
            {
                // Для синхронної: перевіряємо Predicate
                return this.canExecute == null || this.canExecute(parameter);
            }
        }

        public async void Execute(object parameter)
        {
            if (this.isAsync)
            {
                if (this.isExecuting)
                {
                    return;
                }

                this.isExecuting = true;
                try
                {
                    await this.executeAsync(parameter);
                }
                finally
                {
                    this.isExecuting = false;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
            else
            {
                this.execute(parameter);
            }
        }
    }
}