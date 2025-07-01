using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace scientific_calculator_cs
{
    public partial class MainWindow : Window
    {
        // Calculator state variables
        private string currentInput = "0";
        private string previousInput = "";
        private char currentOperator = '\0';
        private bool resetInput = false;
        private bool degreeMode = false; // false=Radians, true=Degrees
        private bool secondFunction = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        // Number button click handler (0-9 and decimal)
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string number = button.Content.ToString();

                if (resetInput || currentInput == "0")
                {
                    currentInput = number;
                    resetInput = false;
                }
                else if (currentInput.Length < 15) // Prevent overflow
                {
                    currentInput += number;
                }

                UpdateDisplay();
            }
        }

        // Basic operation button handler (C, ±, =, etc.)
        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string operation = button.Content.ToString();

                switch (operation)
                {
                    case "C": // Clear all
                        currentInput = "0";
                        previousInput = "";
                        currentOperator = '\0';
                        break;

                    case "±": // Toggle sign
                        if (currentInput != "0")
                        {
                            currentInput = currentInput.StartsWith("-") ?
                                currentInput.Substring(1) : "-" + currentInput;
                        }
                        break;

                    case "(":
                    case ")":
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
        }

        // Scientific function button handler
        private void ScientificButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string function = button.Content.ToString();

                try
                {
                    double inputValue = double.Parse(currentInput);
                    double result = 0;

                    // Handle second functions
                    if (secondFunction)
                    {
                        function = function switch
                        {
                            "sin" => "sin⁻¹",
                            "cos" => "cos⁻¹",
                            "tan" => "tan⁻¹",
                            "log" => "10^x",
                            "ln" => "e^x",
                            "e^x" => "ln",
                            "√" => "x²",
                            "x^y" => "y√x",
                            "x!" => "nPr",
                            "mod" => "nCr",
                            _ => function
                        };
                    }

                    // Perform calculation
                    result = function switch
                    {
                        "sin" => degreeMode ? Math.Sin(inputValue * Math.PI / 180) : Math.Sin(inputValue),
                        "sin⁻¹" => degreeMode ? Math.Asin(inputValue) * 180 / Math.PI : Math.Asin(inputValue),
                        "cos" => degreeMode ? Math.Cos(inputValue * Math.PI / 180) : Math.Cos(inputValue),
                        "cos⁻¹" => degreeMode ? Math.Acos(inputValue) * 180 / Math.PI : Math.Acos(inputValue),
                        "tan" => degreeMode ? Math.Tan(inputValue * Math.PI / 180) : Math.Tan(inputValue),
                        "tan⁻¹" => degreeMode ? Math.Atan(inputValue) * 180 / Math.PI : Math.Atan(inputValue),
                        "π" => Math.PI,
                        "log" => Math.Log10(inputValue),
                        "10^x" => Math.Pow(10, inputValue),
                        "ln" => Math.Log(inputValue),
                        "e^x" => Math.Exp(inputValue),
                        "√" => Math.Sqrt(inputValue),
                        "x²" => Math.Pow(inputValue, 2),
                        "x^y" => HandleBinaryOperation('^', inputValue),
                        "y√x" => HandleBinaryOperation('√', inputValue),
                        "x!" => Factorial((int)inputValue),
                        "mod" => HandleBinaryOperation('%', inputValue),
                        "EE" => HandleBinaryOperation('E', inputValue),
                        _ => inputValue
                    };

                    if (result != double.NaN) // Only update if not a binary operation
                    {
                        currentInput = result.ToString();
                        UpdateDisplay();
                    }
                }
                catch
                {
                    currentInput = "Error";
                    UpdateDisplay();
                }
            }
        }

        // Toggle between Radians and Degrees
        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                degreeMode = !degreeMode;
                button.Content = degreeMode ? "Deg" : "Rad";
            }
        }

        // Toggle second functions
        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                secondFunction = !secondFunction;

                // Update button appearance
                button.Background = secondFunction ?
                    (Brush)FindResource("OrangeBrush") :
                    (Brush)FindResource("LightGrayBrush");

                // Update function button labels
                btnSin.Content = secondFunction ? "sin⁻¹" : "sin";
                btnCos.Content = secondFunction ? "cos⁻¹" : "cos";
                btnTan.Content = secondFunction ? "tan⁻¹" : "tan";
                btnLog.Content = secondFunction ? "10^x" : "log";
                btnLn.Content = secondFunction ? "e^x" : "ln";
                btnExp.Content = secondFunction ? "ln" : "e^x";
                btnSqrt.Content = secondFunction ? "x²" : "√";
                btnPower.Content = secondFunction ? "y√x" : "x^y";
                btnFactorial.Content = secondFunction ? "nPr" : "x!";
                btnMod.Content = secondFunction ? "nCr" : "mod";
            }
        }

        // Handle binary operations (needs two operands)
        private double HandleBinaryOperation(char op, double inputValue)
        {
            previousInput = currentInput;
            currentOperator = op;
            resetInput = true;
            UpdateDisplay();
            return double.NaN; // Special value to indicate binary operation
        }

        // Perform calculation
        private void Calculate()
        {
            if (string.IsNullOrEmpty(previousInput)) return;

            try
            {
                double num1 = double.Parse(previousInput);
                double num2 = double.Parse(currentInput);
                double result = currentOperator switch
                {
                    '+' => num1 + num2,
                    '-' => num1 - num2,
                    '×' => num1 * num2,
                    '÷' => num1 / num2,
                    '^' => Math.Pow(num1, num2),
                    '√' => Math.Pow(num2, 1.0 / num1),
                    '%' => num1 % num2,
                    'E' => num1 * Math.Pow(10, num2),
                    _ => num2
                };

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

        // Calculate factorial
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

        // Update the display
        private void UpdateDisplay()
        {
            txtDisplay.Text = currentInput;
            txtHistory.Text = previousInput + (currentOperator != '\0' ? " " + currentOperator : "");
        }

        #region Window Control Methods

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}