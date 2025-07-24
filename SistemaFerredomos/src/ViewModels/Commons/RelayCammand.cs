using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Commons
{
    public class RelayCommand : ICommand
    {
        // Campos privados
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecuteAction;

        // Constructor para comandos sin restricciones de ejecución
        public RelayCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        // Constructor para comandos con restricciones de ejecución
        public RelayCommand(Action<object> executeAction, Predicate<object> canExecuteAction)
        {
            _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
            _canExecuteAction = canExecuteAction;
        }

        // Evento para notificar cambios en las condiciones de ejecución
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Determina si el comando puede ejecutarse
        public bool CanExecute(object parameter)
        {
            return _canExecuteAction?.Invoke(parameter) ?? true;
        }

        // Ejecuta la acción asociada al comando
        public void Execute(object parameter)
        {
            // Desvincula temporalmente CommandManager.RequerySuggested
            CommandManager.InvalidateRequerySuggested();

            _executeAction(parameter);

            // Vuelve a vincular CommandManager.RequerySuggested
            CommandManager.InvalidateRequerySuggested();
        }

        // Fuerza una actualización de la capacidad de ejecución del comando
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}