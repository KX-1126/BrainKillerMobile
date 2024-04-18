namespace Utilities
{
    public class removeZWSP
    {
        public static string RemoveZeroWidthSpaces(string s) // handle on server side
        {
            return s.Replace("\u200B", "");
        }
    }
}