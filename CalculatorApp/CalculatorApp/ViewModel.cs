using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                OnPropertyChanged("DisplayText");
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
            DisplayText = "0";
            m_model.FirstNumber = 0;
            m_model.SecondNumber = 0;
            m_operator = null;
        }
        private void OnOperatorClick(object obj)
        {
            m_operator = obj.ToString();
            m_model.Operator = m_operator;
            m_isOperatorClick = true;
        }
        private void OnCalculateClick(object obj)
        {
            DisplayText = m_model.Calculate().ToString();
            m_model.FirstNumber=double.Parse(DisplayText);
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
        private bool m_isOperatorClick;


    }
}
