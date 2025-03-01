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
            return c == '^' || c == '*' || c == '/' || c == '+' || c == '-' || c=='%' || c=='S' || c=='N';
        }

        private static bool IsValidCharacter(char c)
        {
            return char.IsDigit(c) || IsOperator(c) || c == '(' || c == ')' || c == '.';
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
                '^' or 'S' or 'N' => 5,
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
                if (char.IsDigit(c) || c == '.' || c=='-' && number=="")
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
            {
                rpn.Add(number);
            }

            while (operatorStack.Count > 0)
            {
                rpn.Add(operatorStack.Pop().ToString());
            }

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
                '%'=> operand2 != 0 ? operand1 % operand2 : throw new DivideByZeroException("Division(modulo) by zero"),
                _ => 0
            };
        }

        private static double ApplyUnaryOperator(double operand,char op)
        {
            return op switch
            {
                'N' => -operand,
                'S' => Math.Sqrt(operand),
                '^' => Math.Pow(operand, 2),
                _ => 0
            };
        }

        public static double EvaluateRPN(List<string> rpn)
        {
            Stack<double> stack = new Stack<double>();

            foreach (string token in rpn)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                else if (IsOperator(token[0]) && stack.Count >= 1)
                {
                    if (token[0]=='^' || token[0] == 'S' || token[0] == 'N')
                    {
                        double operand = stack.Pop();
                        stack.Push(ApplyUnaryOperator(operand, token[0]));
                        continue;
                    }
                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    stack.Push(ApplyOperator(operand1, operand2, token[0]));
                }
                else
                {
                    throw new InvalidOperationException($"Invalid expression format. Unexpected token: {token}");
                }
            }

            return stack.Peek();
        }
    }
}
