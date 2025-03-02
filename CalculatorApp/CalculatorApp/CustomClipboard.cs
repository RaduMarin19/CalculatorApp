using System.Windows;

namespace CalculatorApp
{
    public static class CustomClipboard
    {
        private static string m_buffer = string.Empty;

        public static void Copy(string text)
        {
            m_buffer = text;
            Clipboard.SetText(text);
        }

        public static string Paste()
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : m_buffer;
        }

        public static void Cut(ref string source)
        {
            m_buffer = source;
            Clipboard.SetText(source);
            source = string.Empty;
        }
    }
}