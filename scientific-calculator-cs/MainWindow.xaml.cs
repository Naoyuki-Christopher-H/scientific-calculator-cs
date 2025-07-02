using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Media; // For system sounds
using System.Windows.Media.Imaging; // For bitmap handling
using System.Windows.Interop; // For icon conversion
using System.Runtime.InteropServices; // For DllImport

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

            // Play beep sound on startup
            SystemSounds.Beep.Play();

            // Set window icon with fallback
            try
            {
                // Try to load custom icon from resources
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/icon/calculator.png"));
            }
            catch
            {
                // Fallback to default application icon
                // Using Win32 API to get default icon for C# 7.0 compatibility
                this.Icon = Imaging.CreateBitmapSourceFromHIcon(
                    GetDefaultIcon(),
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }

        // Win32 API method to get default application icon
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr iconName);

        private static IntPtr GetDefaultIcon()
        {
            // IDI_APPLICATION is the default application icon
            IntPtr IDI_APPLICATION = new IntPtr(32512);
            return LoadIcon(IntPtr.Zero, IDI_APPLICATION);
        }

        // Number button click handler (0-9 and decimal)
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string number = button.Content.ToString();

                // Handle reset condition or initial zero state
                if (resetInput || currentInput == "0")
                {
                    currentInput = number;
                    resetInput = false;
                }
                // Prevent display overflow
                else if (currentInput.Length < 15)
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
                    case ")": // Parentheses handling
                        currentInput += operation;
                        break;

                    case "+":
                    case "-":
                    case "×":
                    case "÷": // Basic operations
                        if (currentOperator != '\0')
                        {
                            Calculate();
                        }
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

                    // Handle second functions (inverse operations)
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

                    // Perform the scientific calculation
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

                    if (!double.IsNaN(result)) // Only update if not a binary operation
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
                double result = 0;

                // Perform the appropriate calculation based on operator
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
                    case '√':
                        result = Math.Pow(num2, 1.0 / num1);
                        break;
                    case '%':
                        result = num1 % num2;
                        break;
                    case 'E':
                        result = num1 * Math.Pow(10, num2);
                        break;
                    default:
                        result = num2;
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

        // Calculate factorial (iterative implementation)
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

        // Handle window dragging
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        // Minimize window
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Toggle maximize/restore
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // Close application
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}