using System;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class TextUtils
    {
        public static string CapitalizeFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string ToStringPercent(this float f)
        {
            return (f * 100f).ToStringDecimalIfSmall() + "%";
        }

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
