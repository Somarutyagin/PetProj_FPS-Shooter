using System;

public static class NumberFormatter
{
    /// <summary>
    /// Converts a percentage increase to a scale multiplier and formats it as a string.
    /// For scale >= 1, formats the percentage increase with suffixes if large (e.g., "+1.5Kx").
    /// For scale < 1, returns the scale value as "0.XX" (e.g., "0.23").
    /// </summary>
    /// <param name="percentIncrease">The percentage increase (e.g., 10 for +10%).</param>
    /// <param name="baseValue">The base value to compare against (default 1).</param>
    /// <returns>The formatted string.</returns>
    public static string ConvertPercentToScaleAndFormat(float percentIncrease, float baseValue = 1f)
    {
        float scale = baseValue * (1f + percentIncrease / 100f);

        if (scale >= 1f)
        {
            return "+" + FormatNumber(scale) + "x";
        }
        else
        {
            return scale.ToString("F2");
        }
    }

    /// <summary>
    /// Formats a number into a shortened string with suffixes (K, M, B, etc.) for large values.
    /// For example, 500000000 becomes "500M".
    /// </summary>
    /// <param name="number">The number to format.</param>
    /// <returns>The formatted string.</returns>
    private static string FormatNumber(float number)
    {
        if (number < 10)
        {
            return number.ToString("F2"); // 2 decimals for small numbers
        }
        if (number < 100)
        {
            return number.ToString("F1"); // 1 decimal for small numbers
        }
        if (number < 1000)
        {
            return number.ToString("F0"); // No decimals for small numbers
        }

        string[] suffixes = { "", "K", "M", "B", "T", "q", "Q", "s", "S" }; // Add more if needed
        int suffixIndex = 0;
        float tempNumber = number;

        while (tempNumber >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            tempNumber /= 1000;
            suffixIndex++;
        }

        // Round to 1 decimal place for readability
        return tempNumber.ToString("F1") + suffixes[suffixIndex];
    }
}