using System;
using System.Windows;
using System.Windows.Controls;

namespace scientific_calculator_cs
{
    public partial class MainWindow : Window
    {
        private string currentInput = "0";
        private string previousInput = "";
        private char currentOperator = '\0';
        private bool resetInput = false;
        private bool degreeMode = true; // true for degrees, false for radians

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string number = button.Content.ToString();

            if (resetInput || currentInput == "0")
            {
                currentInput = number;
                resetInput = false;
            }
            else
            {
                currentInput += number;
            }

            UpdateDisplay();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string operation = button.Content.ToString();

            switch (operation)
            {
                case "C": // Clear all
                    currentInput = "0";
                    previousInput = "";
                    currentOperator = '\0';
                    break;

                case "CE": // Clear entry
                    currentInput = "0";
                    break;

                case "⌫": // Backspace
                    if (currentInput.Length > 1)
                    {
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    }
                    else
                    {
                        currentInput = "0";
                    }
                    break;

                case "±": // Plus/minus
                    if (currentInput != "0")
                    {
                        if (currentInput.StartsWith("-"))
                        {
                            currentInput = currentInput.Substring(1);
                        }
                        else
                        {
                            currentInput = "-" + currentInput;
                        }
                    }
                    break;

                case "%": // Percentage
                    try
                    {
                        double value = double.Parse(currentInput) / 100;
                        currentInput = value.ToString();
                    }
                    catch
                    {
                        currentInput = "Error";
                    }
                    break;

                case "(": // Open parenthesis
                case ")": // Close parenthesis
                    currentInput += operation;
                    break;

                case "+":
                case "-":
                case "×":
                case "÷":
                    if (currentOperator != '\0')
                    {
                        Calculate();
                    }
                    previousInput = currentInput;
                    currentOperator = operation[0];
                    resetInput = true;
                    break;

                case "=":
                    Calculate();
                    currentOperator = '\0';
                    break;
            }

            UpdateDisplay();
        }

        private void ScientificButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string function = button.Content.ToString();

            try
            {
                double inputValue = double.Parse(currentInput);
                double result = 0;

                switch (function)
                {
                    case "sin":
                        result = degreeMode ? Math.Sin(inputValue * Math.PI / 180) : Math.Sin(inputValue);
                        break;

                    case "cos":
                        result = degreeMode ? Math.Cos(inputValue * Math.PI / 180) : Math.Cos(inputValue);
                        break;

                    case "tan":
                        result = degreeMode ? Math.Tan(inputValue * Math.PI / 180) : Math.Tan(inputValue);
                        break;

                    case "π":
                        result = Math.PI;
                        break;

                    case "log":
                        result = Math.Log10(inputValue);
                        break;

                    case "ln":
                        result = Math.Log(inputValue);
                        break;

                    case "e^x":
                        result = Math.Exp(inputValue);
                        break;

                    case "√":
                        result = Math.Sqrt(inputValue);
                        break;

                    case "x^y":
                        previousInput = currentInput;
                        currentOperator = '^';
                        resetInput = true;
                        UpdateDisplay();
                        return;

                    case "n!":
                        result = Factorial((int)inputValue);
                        break;

                    case "mod":
                        previousInput = currentInput;
                        currentOperator = '%';
                        resetInput = true;
                        UpdateDisplay();
                        return;

                    case "EE":
                        previousInput = currentInput;
                        currentOperator = 'E';
                        resetInput = true;
                        UpdateDisplay();
                        return;
                }

                currentInput = result.ToString();
                UpdateDisplay();
            }
            catch
            {
                currentInput = "Error";
                UpdateDisplay();
            }
        }

        private void Calculate()
        {
            if (string.IsNullOrEmpty(previousInput) || currentOperator == '\0') return;

            try
            {
                double num1 = double.Parse(previousInput);
                double num2 = double.Parse(currentInput);
                double result = 0;

                switch (currentOperator)
                {
                    case '+':
                        result = num1 + num2;
                        break;
                    case '-':
                        result = num1 - num2;
                        break;
                    case '×':
                        result = num1 * num2;
                        break;
                    case '÷':
                        result = num1 / num2;
                        break;
                    case '^':
                        result = Math.Pow(num1, num2);
                        break;
                    case '%':
                        result = num1 % num2;
                        break;
                    case 'E':
                        result = num1 * Math.Pow(10, num2);
                        break;
                }

                currentInput = result.ToString();
                previousInput = "";
                resetInput = true;
                UpdateDisplay();
            }
            catch
            {
                currentInput = "Error";
                UpdateDisplay();
            }
        }

        private double Factorial(int n)
        {
            if (n < 0) return double.NaN;
            if (n == 0) return 1;

            double result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        private void UpdateDisplay()
        {
            txtDisplay.Text = currentInput;
            txtHistory.Text = previousInput + " " + (currentOperator != '\0' ? currentOperator.ToString() : "");
        }
    }
}