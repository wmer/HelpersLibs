namespace HelpersLibs.Collection; 
public static class IEnumerableExtension {
    private static Random rand = new Random();
    public static List<List<T>> SplitList<T>(this IEnumerable<T> locations, int nSize = 30) {
        return locations
                    .Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / nSize)
                    .Select(x => x.Select(v => v.Value).ToList())
                    .ToList();
    }

    public static string TostringList(this IEnumerable<string> list, string separator = ", ", string surrounders = "") {
        var strArray = list.Select(x => $"{surrounders}{x}{surrounders}").ToArray();
        var str = string.Join(separator, strArray);

        return str;
    }

    public static void Shuffle<T>(this IList<T> values) {
        for (int i = values.Count - 1; i > 0; i--) {
            int k = rand.Next(i + 1);
            T value = values[k];
            values[k] = values[i];
            values[i] = value;
        }
    }
}