namespace MonumentService.Model
{
    public static class ExtensionMethods
    {
        public static IEnumerable<MonumentBase> ToBase(this IEnumerable<Monument> monuments)
        {
            // we need a proper cast here to the base class, using Cast<MonumentBase> is not enough
            return monuments.Select(m => (MonumentBase)m);
        }
    }
}
