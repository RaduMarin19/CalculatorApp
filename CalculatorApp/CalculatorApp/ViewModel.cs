using CalculatorApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CalculatorApp.Properties;
using System.Globalization;
using System.Text.RegularExpressions;


namespace CalculatorApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            m_display = "0";
            m_secondDisplay = string.Empty;
            m_expression = string.Empty;
            m_mode = Settings.Default.Mode.ToString();
            m_isOperatorClick = false;
            m_isEqualClick = false;
            m_operationOrder = false;
            if (m_mode == "Standard")
                m_base = 10;
            else if (int.TryParse(Settings.Default.CalcBase.ToString(), out int savedBase))
                m_base = savedBase;
            else
                Base = 10;
            m_digitGrouping = Settings.Default.DigitGrouping;

            m_calculatorMemory = new CalculatorMemory();
            KeyboardView = new StandardView();

            NumberCommand = new RelayCommand(OnNumberClick, CanExecuteNumber);
            ClearCommand = new RelayCommand(OnClearClick);
            OperatorCommand = new RelayCommand(OnOperatorClick);
            CalculateCommand = new RelayCommand(OnCalculateClick);
            ChangeMode = new RelayCommand(OnClickChangeMode);
            MemoryCommand = new RelayCommand(OnMemoryClick);
            RecallMemoryCommand = new RelayCommand(OnRecallMemory);
            ClipboardCommand = new RelayCommand(OnClipboardClick);
            AboutRadu = new RelayCommand(OnRaduClick);
            ChangeBase = new RelayCommand(OnChangeBaseClick, CanChangeBase);
            OperationOrderCommand = new RelayCommand(OnOperationOrderClick);
            DigitGrouping = new RelayCommand(OnDigitGroupingClick);
            OnClickChangeMode(m_mode);
            OnChangeBaseClick(m_base);
        }
        public ObservableCollection<double> MemoryValues => m_calculatorMemory.MemoryValues;
        public CalculatorKeyboard KeyboardView
        {
            get { return m_keyboardView; }
            set
            {
                m_keyboardView = value;
                OnPropertyChanged(nameof(KeyboardView));
            }
        }
        public bool OperationOrder
        {
            get { return m_operationOrder; }
            set
            {
                m_operationOrder = value;
                OnPropertyChanged(nameof(OperationOrder));
            }
        }
        public double SelectedMemoryValue
        {
            get { return m_selectedMemoryValue; }
            set
            {
                m_selectedMemoryValue = value;
                OnPropertyChanged(nameof(SelectedMemoryValue));
            }
        }
        public int Base
        {
            get { return m_base; }
            set
            {
                 m_base = value;
                 OnPropertyChanged(nameof(Base));
                 Settings.Default.CalcBase = m_base;
                 Settings.Default.Save();
                 ((RelayCommand)NumberCommand).RaiseCanExecuteChanged();
            }
        }
        public string Mode
        {
            get { return m_mode; }
            set { 
                m_mode = value;
                Settings.Default.Mode = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(Mode));
            }
        }
        public string FormattedDisplayText
        {
            get
            {
                if (m_digitGrouping && decimal.TryParse(m_display, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                {
                    return number.ToString("#,0.############################", CultureInfo.CurrentCulture);
                }
                return m_display;
            }
        }

        public string DisplayText
        {
            get { return m_display; }
            set
            {
                m_display = value;
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(FormattedDisplayText)); 
            }
        }
        public string SecondDisplayText
        {
            get { return m_digitGrouping? FormatExpression(m_secondDisplay) : m_secondDisplay; }
            set
            {
                m_secondDisplay = value;
                OnPropertyChanged(nameof(SecondDisplayText));
            }
        }
        private string FormatExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return string.Empty;

            string pattern = @"\d+([.,]\d+)?";

            return Regex.Replace(expression, pattern, match =>
            {
                if (decimal.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                {
                    return number.ToString("#,0.############################", CultureInfo.CurrentCulture); 
                }
                return match.Value;
            });
        }
        private void OnClipboardClick(object obj)
        {
            switch(obj.ToString())
            {
                case "Cut":
                    CustomClipboard.Cut(ref m_display);
                    OnPropertyChanged(nameof(DisplayText));
                    m_expression = string.Empty;
                    SecondDisplayText = string.Empty;
                    DisplayText = "0";
                    break;
                case "Paste":
                    DisplayText = string.Empty;
                    DisplayText += CustomClipboard.Paste();
                    m_expression= DisplayText;
                    m_secondDisplay = string.Empty;
                    break;
                case "Copy":
                    CustomClipboard.Copy(DisplayText);
                    break;
            }
        }

        private void OnDigitGroupingClick(object obj)
        {
            m_digitGrouping = !m_digitGrouping;
            Settings.Default.DigitGrouping = m_digitGrouping;
            Settings.Default.Save();
            OnPropertyChanged(nameof(FormattedDisplayText));
        }
        private void OnRaduClick(object obj)
        {
            MessageBox.Show("Marin Radu-Andrei, grupa 10LF332");
        }
        private void OnOperationOrderClick(object obj)
        {
            OperationOrder = !m_operationOrder;
        }
        private void OnChangeBaseClick(object obj)
        {
            Base = Convert.ToInt32(obj);
            OnClearClick("C");
        }
        private void OnNumberClick(object obj)
        {
            if (m_isEqualClick == true)
            {
                DisplayText = string.Empty;
                m_expression = string.Empty;
                SecondDisplayText = string.Empty;
            }
            if (m_isOperatorClick || (DisplayText == "0" && true))
            {
                DisplayText = string.Empty;
            }

            DisplayText += obj?.ToString();
            m_expression += obj?.ToString();
            m_isOperatorClick = false;
            m_isEqualClick = false;
        }

        private bool CanChangeBase(object parameter)
        {
            return m_mode == "Programmer";
        }
        private bool CanExecuteNumber(object parameter)
        {
            string str = parameter.ToString();

            if (str == ")" || str == "(")
                return true;

            if (Mode == "Programmer")
            {
                if (Base <= 10)
                {
                    return char.IsDigit(str[0]) && (str[0] - '0') < Base;
                }
                else
                {
                    return (char.IsDigit(str[0]) && (str[0] - '0') < Base) ||
                           (char.ToUpper(str[0]) >= 'A' && char.ToUpper(str[0]) < ('A' + (Base - 10)));
                }
            }

            return true;
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
            if (m_expression.Length > 0 && m_expression.Any(c => "+-*/^%~√⅟".Contains(c)) && !m_operationOrder)
            {
                OnCalculateClick(obj);
            }
            SecondDisplayText = m_expression + op;
            m_expression += op;
            m_isOperatorClick = true;
            m_isEqualClick = false;
            if (m_expression.Length > 1 && (op == "√" || op == "⅟" || op == "^" || op == "~") && !m_operationOrder)
            {
                OnCalculateClick(obj);
            }
            
        }
        private void OnClickChangeMode(object obj)
        {
            OnClearClick("C");
            Mode = obj.ToString();
            if (Mode == "Programmer")
            {
                KeyboardView = new ProgrammerView();
            }
            else if (Mode == "Standard")
            {
                KeyboardView = new StandardView();
            }
        }
        private void OnCalculateClick(object obj)
        {
            if (string.IsNullOrEmpty(m_expression) || 
                !m_expression.Any(c => "+-*/^%√~⅟".Contains(c)))
                return;
            
            try
            {
                List<string> rpn = ExpressionEvaluator.ConvertToRPN(m_expression);
                string result = ExpressionEvaluator.EvaluateRPN(rpn,m_base);

                SecondDisplayText = m_expression + " =";
                DisplayText = result;

                m_expression = result;
            }
            catch (Exception ex)
            {
                DisplayText = "Error";
                Console.WriteLine(ex.Message);
            }
            m_isEqualClick  = true;
        }
        private void OnMemoryClick(object obj)
        {
            string action = obj.ToString();

            switch (action)
            {
                case "M+":
                    if (m_calculatorMemory.RecallLast() == 0)
                    {
                        m_calculatorMemory.RemoveLast();
                        m_calculatorMemory.AddToMemory(double.Parse(DisplayText));
                    }
                    else
                    {
                        m_calculatorMemory.ChangeLast(double.Parse(DisplayText));
                    }
                    break;
                case "M-":
                    if (m_calculatorMemory.RecallLast() == 0)
                    {
                        m_calculatorMemory.RemoveLast();
                        m_calculatorMemory.AddToMemory(-double.Parse(DisplayText));
                    }
                    else
                    {
                        m_calculatorMemory.ChangeLast(-double.Parse(DisplayText));
                    }
                    break;
                case "MR":
                    DisplayText = m_calculatorMemory.RecallLast().ToString();
                    m_expression = DisplayText;
                    SecondDisplayText = string.Empty;
                    break;
                case "MC":
                    m_calculatorMemory.ClearMemory();
                    break;
                case "MS":
                    m_calculatorMemory.AddToMemory(double.Parse(DisplayText));
                    break;
                default:
                    break;
            }
        }
        private void OnRecallMemory(object obj)
        {
            string val = obj.ToString();

            DisplayText = val;
            m_expression = val;
            SecondDisplayText = string.Empty;
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
        public ICommand AboutRadu {  get; set; }
        public ICommand MemoryCommand { get; set; }
        public ICommand RecallMemoryCommand { get; set; }
        public ICommand ClipboardCommand { get; set; }
        public ICommand ChangeBase { get; set; }
        public ICommand OperationOrderCommand { get; set; }
        public ICommand DigitGrouping {  get; set; }

        private CalculatorMemory m_calculatorMemory;
        private CalculatorKeyboard m_keyboardView;
        private string m_display;
        private string m_secondDisplay;
        private string m_expression;
        private string m_mode;
        private bool m_isOperatorClick;
        private bool m_isEqualClick;
        private bool m_digitGrouping;
        private bool m_operationOrder;
        private double m_selectedMemoryValue;
        private int m_base;
    }
}
