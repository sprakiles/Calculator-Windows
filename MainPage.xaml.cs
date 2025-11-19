using System.Data;

namespace Calculator;

public partial class MainPage : ContentPage
{
    private string expression = "";
    private bool isResultDisplayed = false;

    public MainPage()
    {
        InitializeComponent();
        UpdateDisplay();
        if (Application.Current != null)
        {
             // Theme handled by bindings
        }
    }

    private void UpdateDisplay()
    {
        ResultText.Text = string.IsNullOrEmpty(expression) ? "0" : expression;
        CalculationText.Text = ""; // Clear the secondary display until an evaluation is made
    }
    
    private async void AnimateButton(Button button)
    {
        await button.ScaleTo(0.9, 50, Easing.Linear);
        await button.ScaleTo(1.0, 50, Easing.Linear);
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            expression = "";
            isResultDisplayed = false;
        }

        expression += button.Text;
        UpdateDisplay();
    }

    private void OnOperatorClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            isResultDisplayed = false;
        }
        
        if (!string.IsNullOrEmpty(expression) && !" ".Equals(expression.Substring(expression.Length-1)))
        {
            string op = button.Text == "x" ? "*" : button.Text;
            expression += $" {op} ";
            UpdateDisplay();
        }
    }

    private void OnEqualsClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        try
        {
            var result = new DataTable().Compute(expression, null);
            CalculationText.Text = $"{expression} =";
            expression = result.ToString();
            ResultText.Text = expression;
            isResultDisplayed = true;
        }
        catch
        {
            ResultText.Text = "Error";
            expression = "";
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        expression = "";
        isResultDisplayed = false;
        UpdateDisplay();
    }

    private void OnDecimalClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            expression = "0.";
            isResultDisplayed = false;
        }
        else
        {
            var lastNumberMatch = System.Text.RegularExpressions.Regex.Match(expression, @"[\d\.]+$");
            if (lastNumberMatch.Success && !lastNumberMatch.Value.Contains("."))
            {
                expression += ".";
            }
            else if (!lastNumberMatch.Success)
            {
                expression += "0.";
            }
        }
        UpdateDisplay();
    }

    private void OnSignClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            if (expression.StartsWith("-"))
            {
                expression = expression.Substring(1);
            }
            else
            {
                expression = "-" + expression;
            }
        }
        else
        {
            var lastNumberMatch = System.Text.RegularExpressions.Regex.Match(expression, @"[\d\.]+$");
            if (lastNumberMatch.Success)
            {
                string lastNumber = lastNumberMatch.Value;
                int lastNumberIndex = lastNumberMatch.Index;
                if (lastNumberIndex > 0 && expression[lastNumberIndex - 1] == '-')
                {
                    // It's a negative number, make it positive
                    expression = expression.Substring(0, lastNumberIndex - 1) + lastNumber;
                }
                else
                {
                    // It's a positive number, make it negative
                    expression = expression.Substring(0, lastNumberIndex) + "-" + lastNumber;
                }
            }
        }
        UpdateDisplay();
    }

    private void OnPercentageClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            try
            {
                double number = double.Parse(expression);
                number /= 100;
                expression = number.ToString();
            }
            catch
            {
                expression = "";
            }
        }
        else
        {
            var lastNumberMatch = System.Text.RegularExpressions.Regex.Match(expression, @"[\d\.]+$");
            if (lastNumberMatch.Success)
            {
                string lastNumberStr = lastNumberMatch.Value;
                try
                {
                    double number = double.Parse(lastNumberStr);
                    number /= 100;
                    expression = expression.Substring(0, lastNumberMatch.Index) + number.ToString();
                }
                catch
                {
                    // Error parsing, do nothing.
                }
            }
        }
        UpdateDisplay();
    }
    
    private async void OnThemeSwitchClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
        {
            // Animate knob
            if (Application.Current.UserAppTheme == AppTheme.Dark)
            {
                // Switch to Light
                await ThemeToggleKnob.TranslateTo(0, 0, 250, Easing.CubicOut);
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                // Switch to Dark
                await ThemeToggleKnob.TranslateTo(28, 0, 250, Easing.CubicOut);
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }
    }

    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            expression = "";
            isResultDisplayed = false;
            UpdateDisplay();
            return;
        }
        
        if (!string.IsNullOrEmpty(expression))
        {
            expression = expression.Substring(0, expression.Length - 1).Trim();
            UpdateDisplay();
        }
    }

    private void OnBracketClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        AnimateButton(button);

        if (isResultDisplayed)
        {
            expression = "";
            isResultDisplayed = false;
        }

        int openCount = expression.Count(c => c == '(');
        int closeCount = expression.Count(c => c == ')');

        if (string.IsNullOrEmpty(expression))
        {
            expression += "(";
        }
        else
        {
            char lastChar = expression[expression.Length - 1];

            if (char.IsDigit(lastChar) || lastChar == ')')
            {
                if (openCount > closeCount)
                {
                    expression += ")";
                }
                else
                {
                    expression += " * (";
                }
            }
            else
            {
                expression += "(";
            }
        }
        UpdateDisplay();
    }
}
