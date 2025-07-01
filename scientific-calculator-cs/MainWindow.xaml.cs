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
        private string currentInput = "0";       // Current number being entered
        private string previousInput = "";      // Previous number in memory
        private char currentOperator = '\0';     // Current operation (+, -, ×, ÷, etc.)
        private bool resetInput = false;        // Flag to reset input on next entry
        private bool degreeMode = false;        // false=Radians, true=Degrees
        private bool secondFunction = false;     // Whether 2nd functions are active

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay(); // Initialize display
        }

        #region Button Handlers

        // Handles number button clicks (0-9 and decimal)
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string number = button.Content.ToString();

            // Reset input if needed or if current display is "0"
            if (resetInput || currentInput == "0")
            {
                currentInput = number;
                resetInput = false;
            }
            else
            {
                // Limit input length to prevent overflow
                if (currentInput.Length < 15)
                {
                    currentInput += number;
                }
            }

            UpdateDisplay();
        }

        // Handles basic operation buttons (C, ±, =, etc.)
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

                case "±": // Toggle positive/negative
                    if (currentInput != "0")
                    {
                        currentInput = currentInput.StartsWith("-") ?
                            currentInput.Substring(1) : "-" + currentInput;
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
                    // If there's a pending operation, calculate it first
                    if (currentOperator != '\0')
                    {
                        Calculate();
                    }
                    // Store current input and operator
                    previousInput = currentInput;
                    currentOperator = operation[0];
                    resetInput = true;
                    break;

                case "=": // Perform calculation
                    Calculate();
                    currentOperator = '\0';
                    break;
            }

            UpdateDisplay();
        }

        // Handles scientific function buttons (sin, cos, log, etc.)
        private void ScientificButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string function = button.Content.ToString();

            try
            {
                double inputValue = double.Parse(currentInput);
                double result = 0;

                // Handle second functions (inverse operations)
                if (secondFunction)
                {
                    switch (function)
                    {
                        case "sin": function = "sin⁻¹"; break;
                        case "cos": function = "cos⁻¹"; break;
                        case "tan": function = "tan⁻¹"; break;
                        case "log": function = "10^x"; break;
                        case "ln": function = "e^x"; break;
                        case "e^x": function = "ln"; break;
                        case "√": function = "x²"; break;
                        case "x^y": function = "y√x"; break;
                        case "x!": function = "nPr"; break;
                        case "mod": function = "nCr"; break;
                    }
                }

                // Perform the appropriate calculation
                switch (function)
                {
                    case "sin":
                        result = degreeMode ? Math.Sin(inputValue * Math.PI / 180) : Math.Sin(inputValue);
                        break;
                    case "sin⁻¹":
                        result = degreeMode ? Math.Asin(inputValue) * 180 / Math.PI : Math.Asin(inputValue);
                        break;
                    case "cos":
                        result = degreeMode ? Math.Cos(inputValue * Math.PI / 180) : Math.Cos(inputValue);
                        break;
                    case "cos⁻¹":
                        result = degreeMode ? Math.Acos(inputValue) * 180 / Math.PI : Math.Acos(inputValue);
                        break;
                    case "tan":
                        result = degreeMode ? Math.Tan(inputValue * Math.PI / 180) : Math.Tan(inputValue);
                        break;
                    case "tan⁻¹":
                        result = degreeMode ? Math.Atan(inputValue) * 180 / Math.PI : Math.Atan(inputValue);
                        break;
                    case "π":
                        result = Math.PI;
                        break;
                    case "log":
                        result = Math.Log10(inputValue);
                        break;
                    case "10^x":
                        result = Math.Pow(10, inputValue);
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
                    case "x²":
                        result = Math.Pow(inputValue, 2);
                        break;
                    case "x^y":
                        previousInput = currentInput;
                        currentOperator = '^';
                        resetInput = true;
                        UpdateDisplay();
                        return;
                    case "y√x":
                        previousInput = currentInput;
                        currentOperator = '√';
                        resetInput = true;
                        UpdateDisplay();
                        return;
                    case "x!":
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

        // Toggles between Radians and Degrees mode
        private void RadDegButton_Click(object sender, RoutedEventArgs e)
        {
            degreeMode = !degreeMode;
            btnRadDeg.Content = degreeMode ? "Deg" : "Rad";
        }

        // Toggles secondary functions
        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            secondFunction = !secondFunction;

            // Change button appearance when in 2nd function mode
            btnSecond.Background = secondFunction ?
                (Brush)FindResource("OrangeButton") :
                (Brush)FindResource("LightGrayButton");

            // Update all function button labels
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

        #endregion

        #region Calculation Methods

        // Performs the current calculation
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
                    case '√': // y√x operation (x^(1/y))
                        result = Math.Pow(num2, 1.0 / num1);
                        break;
                    case '%':
                        result = num1 % num2;
                        break;
                    case 'E': // Scientific notation
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

        // Calculates factorial (n!)
        private double Factorial(int n)
        {
            if (n < 0) return double.NaN; // Error for negative numbers
            if (n == 0) return 1;         // 0! is 1

            double result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        #endregion

        #region UI Update Methods

        // Updates the display with current values
        private void UpdateDisplay()
        {
            txtDisplay.Text = currentInput;
            // Show previous input and current operator in history
            txtHistory.Text = previousInput + " " + (currentOperator != '\0' ? currentOperator.ToString() : "");
        }

        #endregion

        #region Window Control Methods

        // Allows dragging the window by title bar
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // Minimize window
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Toggle maximize/restore
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // Close window
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}