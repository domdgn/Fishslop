using UnityEngine;

namespace DebugPanel.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerNoSpaces(this string input)
        {
            return input.Replace(" ", "").ToLowerInvariant();
        }
    }
}