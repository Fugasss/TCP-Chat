namespace Common.Messages
{
    internal static class StringExtension
    {

        public static string AlignCenter(this string str, int maxLength)
        {
            int left = (maxLength - str.Length) / 2;
            int right = (maxLength - str.Length) - left;

            if (left > 0) str = str.Insert(0, new string(' ', left));
            if (right > 0) str = str.Insert(str.Length + left - 1, new string(' ', right));
            str = str.Replace("\0", " ");

            return str;
        }
    }
}