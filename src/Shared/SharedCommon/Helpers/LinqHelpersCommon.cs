namespace SharedCommon.Helpers;

public static class LinqHelpersCommon
{
    /// <summary>
    /// Is Collection null or empty.
    /// Jan's Extension method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrEmptyCollection<T>(this IEnumerable<T> source)
    {
        //return source == null || source.Any() == false || source.FirstOrDefault() == null;
        if (source == null)
            return true;

        //Enumerable.TryGetNonEnumeratedCount(source, out int result);
        return source.Count() <= 0 || source.FirstOrDefault() == null;
    }

    /// <summary>
    /// Collection NOT null or NOT empty.
    /// Jan's Extension method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool HasAnyInCollection<T>(this IEnumerable<T> source)
    {
        return !source.IsNullOrEmptyCollection();
    }

    public static List<List<T>> BuildChunksWithRange<T>(List<T> fullList, int batchSize)
    {
        List<List<T>> chunkedList = new List<List<T>>();
        int index = 0;

        while (index < fullList.Count)
        {
            int rest = fullList.Count - index;
            if (rest >= batchSize)
                chunkedList.Add(fullList.GetRange(index, batchSize));
            else
                chunkedList.Add(fullList.GetRange(index, rest));
            index += batchSize;
        }

        return chunkedList;
    }

    public static string GetStringBySeparator<T>(this IEnumerable<T> source, char separator = ' ', string space = " ")
    {
        if (source.IsNullOrEmptyCollection())
            return null;

        StringBuilder sb = new();
        foreach (T item in source)
            sb.Append($"{item}{separator}{space}");
        return sb.ToString().TrimEnd().TrimEnd(separator);
    }
}
