# Scientific Calculator (WPF, C# 7.0)

A feature-rich scientific calculator application built with WPF and C# 7.0, designed with an Apple-inspired UI.

## Features

- **Basic Operations**: Addition, subtraction, multiplication, division
- **Scientific Functions**:
  - Trigonometric (sin, cos, tan) with inverse functions
  - Logarithmic (log, ln) and exponential functions
  - Square root and power functions
  - Factorial and modulus operations
- **Special Modes**:
  - Radian/Degree toggle
  - Second function layer (access inverse functions)
- **Error Handling**: Clear error messages for invalid operations

## Installation

### Prerequisites
- .NET Framework 4.7.2 or later
- Visual Studio 2017 or later (for development)

### Running the Application
1. Clone the repository:
   ```bash
   git clone https://github.com/Naoyuki-Christopher-H/scientific-calculator-cs.git
   ```
2. Open the solution in Visual Studio
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## Usage

### Basic Operations
1. Enter numbers using the number buttons (0-9)
2. Use operation buttons (+, -, ×, ÷) for calculations
3. Press "=" to see the result
4. Use "C" to clear all or "CE" to clear current entry

### Scientific Functions
1. Use the top row of buttons for scientific functions:
   - Trigonometric: sin, cos, tan
   - Logarithmic: log, ln
   - Constants: π, e
2. Press "2nd" to access inverse functions:
   - sin⁻¹, cos⁻¹, tan⁻¹
   - 10^x, e^x

### Modes
- Toggle between Radians and Degrees using the "Rad"/"Deg" button
- Use parentheses "(" and ")" for complex expressions

## Code Structure

```
scientific-calculator-cs/
├── MainWindow.xaml        # Main UI definition
├── MainWindow.xaml.cs     # Calculator logic
├── App.xaml               # Application resources
└── Properties/            # Assembly properties
```

### Key Components

1. **UI Layer**:
   - Custom styles for all buttons
   - Responsive layout with Grid panels

2. **Business Logic**:
   - State management for calculator operations
   - Scientific function implementations
   - Error handling

3. **Window Management**:
   - Custom title bar with minimize/maximize/close
   - Window dragging functionality

## Development

### Building from Source
1. Clone the repository
2. Open in Visual Studio
3. Restore NuGet packages if needed
4. Build and run

### Dependencies
- .NET Framework 4.7.2
- WPF (included with .NET Framework)

### Coding Style
- C# 7.0 features
- MVVM-inspired separation of concerns
- Consistent naming conventions
- XML documentation for major methods

## Contributing

Contributions are welcome! Please follow these guidelines:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

### Reporting Issues
Please use the GitHub issue tracker to report any bugs or suggest features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

- [ ] Add memory functions (M+, M-, MR, MC)
- [ ] Implement unit conversions
- [ ] Add graphing capabilities
- [ ] Support for complex numbers

## Acknowledgments

- WPF documentation
- .NET community resources
