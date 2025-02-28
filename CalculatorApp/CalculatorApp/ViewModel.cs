using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculatorApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel() {
            m_model = new Model();
            m_display = "0";
            m_secondDisplay = string.Empty;
            m_isOperatorClick = false;

            NumberCommand = new RelayCommand(OnNumberClick);
            ClearCommand = new RelayCommand(OnClearClick);
            OperatorCommand = new RelayCommand(OnOperatorClick);
            CalculateCommand = new RelayCommand(OnCalculateClick);
        }

        public string DisplayText
        {
            get { return m_display; }
            set { 
                m_display = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }
        public string ClearOption
        {
            get { return m_clearOption; }
            set {
                m_clearOption = value;
                OnPropertyChanged(nameof(ClearOption));
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
        public bool IsOperatorClick
        {
            get { return m_isOperatorClick; }
            set { m_isOperatorClick = value;}
        }

        private void OnNumberClick(object obj)
        {
            if(m_isOperatorClick || DisplayText == "0") 
            {   DisplayText = string.Empty;}

            DisplayText += obj?.ToString();
            m_isOperatorClick = false;
            if(m_operator==null)
            { m_model.FirstNumber = double.Parse(DisplayText); }
            else { m_model.SecondNumber = double.Parse(DisplayText); }
        }
        private void OnClearClick(object obj)
        {
            m_clearOption = obj.ToString();
            switch (m_clearOption)
            {
                case "C":
                    DisplayText = "0";
                    SecondDisplayText = string.Empty;
                    m_model.FirstNumber = 0;
                    m_model.SecondNumber = 0;
                    m_operator = null;
                    break;
                case "CE":
                    DisplayText = "0";
                    if(m_model.SecondNumber != 0)
                    {
                        m_model.SecondNumber = 0;
                    }
                    else
                    {
                        m_model.FirstNumber = 0;
                        m_operator = null;
                        SecondDisplayText= string.Empty;
                    }
                    break;
                case "R":
                    break;
                default:
                    break;
            }
        }
        private void OnOperatorClick(object obj)
        {
            m_operator = obj.ToString();
            m_model.Operator = m_operator;
            if (m_operator == "S") 
            { SecondDisplayText = "sqrt(" + m_model.FirstNumber.ToString() + ")"; }
            else 
            { SecondDisplayText = m_model.FirstNumber.ToString() + m_operator; }
            m_isOperatorClick = true;
        }
        private void OnCalculateClick(object obj)
        {
            if(m_operator == null)
            { return; }
            if (m_operator == "^")
            { SecondDisplayText += "2="; }
            else if(m_operator == "S")
            { SecondDisplayText += "="; }
            else 
            { SecondDisplayText += m_model.SecondNumber.ToString() + '='; }
            DisplayText = m_model.Calculate().ToString();
            m_model.FirstNumber=double.Parse(DisplayText);
            m_model.SecondNumber = 0;
            m_operator = null;
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

        private Model m_model;
        private String m_display;
        private String m_operator;
        private String m_secondDisplay;
        private String m_clearOption;
        private bool m_isOperatorClick;


    }
}
