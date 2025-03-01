using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculatorApp
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> m_execute;
        private readonly Func<object,bool> m_canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.m_execute = execute;
            this.m_canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return m_canExecute == null || m_canExecute(parameter);
        }


        public void Execute(object? parameter)
        {
            m_execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
