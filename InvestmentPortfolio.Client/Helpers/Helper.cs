namespace InvestmentPortfolio.Client.Helpers;

public static class Helper
{
    /// <summary>
    /// Gets the CSS style color based on the provided numerical value.
    /// </summary>
    /// <param name="value">The numerical value to determine the color for.</param>
    /// <returns>A CSS style string representing the color based on the value.</returns>
    public static string GetStyleColorByValue(float value)
    {
        string style = string.Empty;

        if (value > 0)
        {
            style = "color:forestgreen";
        }
        else if (value < 0)
        {
            style = "color:red";
        }

        return style;
    }
}