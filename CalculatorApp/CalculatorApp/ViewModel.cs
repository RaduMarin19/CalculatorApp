using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CalculatorApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            m_display = "0";
            m_secondDisplay = string.Empty;
            m_expression = string.Empty;
            m_mode = "Standard";
            m_isOperatorClick = false;
            m_isEqualClick = false;

            NumberCommand = new RelayCommand(OnNumberClick);
            ClearCommand = new RelayCommand(OnClearClick);
            OperatorCommand = new RelayCommand(OnOperatorClick);
            CalculateCommand = new RelayCommand(OnCalculateClick);
            ChangeMode = new RelayCommand(OnClickChangeMode);
        }

        public string Mode
        {
            get { return m_mode; }
            set { 
                m_mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        public string DisplayText
        {
            get { return m_display; }
            set
            {
                m_display = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string SecondDisplayText
        {
            get { return m_secondDisplay; }
            set
            {
                m_secondDisplay = value;
                OnPropertyChanged(nameof(SecondDisplayText));
            }
        }

        private void OnNumberClick(object obj)
        {
            if (m_isEqualClick == true)
            {
                DisplayText = string.Empty;
                m_expression = string.Empty;
                SecondDisplayText = string.Empty;
            }
            if (m_isOperatorClick || DisplayText == "0")
            {
                DisplayText = string.Empty;
            }

            DisplayText += obj?.ToString();
            m_expression += obj?.ToString();
            m_isOperatorClick = false;
            m_isEqualClick = false;
        }

        private void OnClearClick(object obj)
        {
            string clearOption = obj.ToString();

            switch (clearOption)
            {
                case "C":
                    DisplayText = "0";
                    SecondDisplayText = string.Empty;
                    m_expression = string.Empty;
                    break;
                case "CE":
                    DisplayText = "0";
                    if (m_expression.Length > 0)
                    {
                        m_expression = string.Empty;
                    }
                    break;
                case "BS":
                    if (DisplayText.Length > 1)
                    {
                        DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
                        m_expression = m_expression.Substring(0, m_expression.Length - 1);
                    }
                    else
                    {
                        DisplayText = "0";
                        m_expression = string.Empty;
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnOperatorClick(object obj)
        {
            string op = obj.ToString();

            if (m_expression.Length > 0 && "+-*/^%~√⅟".Contains(m_expression[^1]))
            {
                m_expression = m_expression.Substring(0, m_expression.Length-1);
            }
            if(m_expression.Length > 0 && m_expression.Any(c => "+-*/^%~√⅟".Contains(c)))
            {
                OnCalculateClick(obj);
            }
            SecondDisplayText = m_expression + op;
            m_expression += op;
            m_isOperatorClick = true;
            m_isEqualClick = false;
        }
        private void OnClickChangeMode(object obj)
        {
            Mode = obj.ToString();
        }

        private void OnCalculateClick(object obj)
        {
            if (string.IsNullOrEmpty(m_expression) || 
                !m_expression.Any(c => "+-*/^%√~⅟".Contains(c)))
                return;
            
            try
            {
                List<string> rpn = ExpressionEvaluator.ConvertToRPN(m_expression);
                double result = ExpressionEvaluator.EvaluateRPN(rpn);

                SecondDisplayText = m_expression + " =";
                DisplayText = result.ToString();

                m_expression = result.ToString(); 
            }
            catch (Exception ex)
            {
                DisplayText = "Error";
                Console.WriteLine(ex.Message);
            }
            m_isEqualClick  = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand NumberCommand { get; set; }
        public ICommand OperatorCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CalculateCommand { get; set; }
        public ICommand ChangeMode { get; set; }

        private string m_display;
        private string m_secondDisplay;
        private string m_expression;
        private string m_mode;
        private bool m_isOperatorClick;
        private bool m_isEqualClick;
    }
}
