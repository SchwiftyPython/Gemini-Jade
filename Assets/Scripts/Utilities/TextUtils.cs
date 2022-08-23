using System;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// The text utils class
    /// </summary>
    public static class TextUtils
    {
        /// <summary>
        /// Capitalizes the first using the specified str
        /// </summary>
        /// <param name="str">The str</param>
        /// <returns>The string</returns>
        public static string CapitalizeFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Returns the string percent using the specified f
        /// </summary>
        /// <param name="f">The </param>
        /// <returns>The string</returns>
        public static string ToStringPercent(this float f)
        {
            return (f * 100f).ToStringDecimalIfSmall() + "%";
        }

        /// <summary>
        /// Returns the string decimal if small using the specified f
        /// </summary>
        /// <param name="f">The </param>
        /// <returns>The string</returns>
        public static string ToStringDecimalIfSmall(this float f)
        {
            if (Mathf.Abs(f) < 1f)
            {
                return Math.Round(f, 2).ToString("0.##");
            }
            if (Mathf.Abs(f) < 10f)
            {
                return Math.Round(f, 1).ToString("0.#");
            }

            return Mathf.RoundToInt(f).ToString();
        }
    }
}
