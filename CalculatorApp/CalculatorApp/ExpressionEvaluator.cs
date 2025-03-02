using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalculatorApp
{
    public class ExpressionEvaluator
    {
        private static bool IsOperator(char c)
        {
            return c == '^' || c == '*' || c == '/' || c == '+' || c == '-' || c == '%' || c == '√' || c == '~' || c == '⅟';
        }

        private static bool IsValidCharacter(char c)
        {
            return char.IsDigit(c) || IsOperator(c) || c == '(' || c == ')' || c == '.' || (char.IsLetter(c) && c >= 'A' && c <= 'F');
        }

        public static bool ValidateExpression(string expression)
        {
            Stack<char> parenthesesStack = new Stack<char>();

            foreach (char c in expression)
            {
                if (!IsValidCharacter(c))
                    return false;
                if (c == '(')
                    parenthesesStack.Push(c);
                else if (c == ')')
                {
                    if (parenthesesStack.Count == 0)
                        return false;
                    parenthesesStack.Pop();
                }
            }
            return parenthesesStack.Count == 0;
        }

        private static int GetPrecedence(char c)
        {
            return c switch
            {
                '^' or '√' or '⅟' => 5,
                '*' or '/' or '%' => 3,
                '+' or '-' => 2,
                '(' => 0,
                _ => -1
            };
        }

        public static List<string> ConvertToRPN(string expression)
        {
            List<string> rpn = new List<string>();
            Stack<char> operatorStack = new Stack<char>();
            string number = "";

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.' || (c == '-' && (number == "" || (rpn.Count > 0 && IsOperator(rpn.Last()[0])))))
                {
                    number += c;
                }
                else if (IsValidCharacter(c))
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        rpn.Add(number);
                        number = "";
                    }
                    if (c == ')')
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                        {
                            rpn.Add(operatorStack.Pop().ToString());
                        }
                        if (operatorStack.Count > 0)
                            operatorStack.Pop();
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && GetPrecedence(c) <= GetPrecedence(operatorStack.Peek()))
                        {
                            rpn.Add(operatorStack.Pop().ToString());
                        }
                        operatorStack.Push(c);
                    }
                }
            }
            if (!string.IsNullOrEmpty(number))
                rpn.Add(number);
            while (operatorStack.Count > 0)
                rpn.Add(operatorStack.Pop().ToString());

            return rpn;
        }

        private static double ApplyOperator(double operand1, double operand2, char op)
        {
            return op switch
            {
                '-' => operand1 - operand2,
                '+' => operand1 + operand2,
                '*' => operand1 * operand2,
                '/' => operand2 != 0 ? operand1 / operand2 : throw new DivideByZeroException("Division by zero"),
                '^' => Math.Pow(operand1, operand2),
                '%' => operand2 != 0 ? operand1 % operand2 : throw new DivideByZeroException("Modulo by zero"),
                _ => throw new InvalidOperationException($"Unknown operator: {op}")
            };
        }

        private static double ApplyUnaryOperator(double operand, char op)
        {
            return op switch
            {
                '⅟' => 1 / operand,
                '~' => -operand,
                '√' => Math.Sqrt(operand),
                _ => throw new InvalidOperationException($"Unknown unary operator: {op}")
            };
        }

        private static int BaseKToBase10(string number, int baseK)
        {
            int result = 0, power = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                int digit = char.IsDigit(number[i]) ? number[i] - '0' : char.ToUpper(number[i]) - 'A' + 10;
                if (digit >= baseK) throw new ArgumentException($"Invalid digit '{number[i]}' for base {baseK}");
                result += digit * power;
                power *= baseK;
            }
            return result;
        }

        private static string Base10ToBaseK(int number, int baseK)
        {
            if (baseK < 2 || baseK > 36) throw new ArgumentException("Base must be between 2 and 36");
            if (number == 0) return "0";
            string result = "";
            while (number > 0)
            {
                int remainder = number % baseK;
                char digit = remainder < 10 ? (char)('0' + remainder) : (char)('A' + remainder - 10);
                result = digit + result;
                number /= baseK;
            }
            return result;
        }

        public static double EvaluateRPN(List<string> rpn, int b)
        {
            Stack<double> stack = new Stack<double>();
            foreach (string token in rpn)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                else if (IsOperator(token[0]) && stack.Count >= 1 && "^√~⅟".Contains(token[0]))
                {
                    stack.Push(ApplyUnaryOperator(stack.Pop(), token[0]));
                }
                else if (IsOperator(token[0]) && stack.Count >= 2)
                {
                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    if (b != 10)
                    {
                        operand1 = BaseKToBase10(operand1.ToString(), b);
                        operand2 = BaseKToBase10(operand2.ToString(), b);
                    }
                    stack.Push(ApplyOperator(operand1, operand2, token[0]));
                }
                else
                {
                    throw new InvalidOperationException($"Invalid token: {token}");
                }
            }
            if (b == 10)
            {
                return stack.Pop();
            }
            else
            {
                if (double.TryParse(Base10ToBaseK((int)stack.Pop(), b), NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                    return number;
                return 0;
            }
        }
    }
}