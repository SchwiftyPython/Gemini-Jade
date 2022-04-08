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
    }
}
