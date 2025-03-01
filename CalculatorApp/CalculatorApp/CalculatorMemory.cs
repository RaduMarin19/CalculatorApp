using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CalculatorApp
{
    public class CalculatorMemory : INotifyPropertyChanged
    {
        private ObservableCollection<double> m_memoryValues;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CalculatorMemory()
        {
            m_memoryValues = new ObservableCollection<double>();
        }
        public ObservableCollection<double> MemoryValues
        {
            get { return m_memoryValues; }
        }
        public void RemoveElement(double value) 
        {
            m_memoryValues.Remove(value);
        }
        public void RemoveLast()
        {
            if (m_memoryValues.Count == 0)
                return;
            m_memoryValues.RemoveAt(m_memoryValues.Count - 1);
        }
        public void ChangeLast(double value)
        {
            m_memoryValues[^1] += value;
        }

        public void AddToMemory(double value)
        {
            m_memoryValues.Add(value);
            OnPropertyChanged(nameof(MemoryValues));
        }
        public double RecallLast()
        {
            return m_memoryValues.Count > 0 ? m_memoryValues[^1] : 0;
        }

        public void ClearMemory()
        {
            m_memoryValues.Clear();
            OnPropertyChanged(nameof(MemoryValues));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
