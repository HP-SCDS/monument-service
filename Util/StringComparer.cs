namespace MonumentService.Util
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public static class StringComparer
    {
        public static bool CompareInvariantIgnoreCase(string? a, string? b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsInvariantIgnoreCase(string? a, string? b)
        {
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
            {
                return true;
            }
            else if (string.IsNullOrWhiteSpace(a))
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(b))
            {
                return false;
            }

            return a.Contains(b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsInvariantIgnoreCase(IEnumerable<string>? enumerable, string? a)
        {
            return enumerable != null && enumerable.Any(i => CompareInvariantIgnoreCase(i, a));
        }
    }
}