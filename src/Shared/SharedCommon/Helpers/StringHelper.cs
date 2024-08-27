using System.Text.RegularExpressions;

namespace System;

public static class StringHelper
{
    /// <summary>
    /// Guid/Uuid/unique identifier, but is guaranteed to be both unique and ordered. Calls MassTransit.NewId.Next().ToString()
    /// </summary>
    /// <returns></returns>
    public static string GetStringGuidExt() => MassTransit.NewId.Next().ToString();

    public static string? ReplaceWithStars(this string? text, int visibleCharsCount = 8)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length <= visibleCharsCount)
            return text;
        var result = text.Substring(0, visibleCharsCount);
        var stars = new string('*', text.Length - visibleCharsCount);
        result += stars;
        return result;
    }

	public static string? ShrinkTextExt(this string? text, int visibleCharsCount = 20)
	{
		if (string.IsNullOrWhiteSpace(text) || text.Length <= visibleCharsCount)
			return text;
		return $"{text.Substring(0, visibleCharsCount)}...";
	}

	public static bool IsNullOrEmptyExt(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return true;
        else
            return false;
    }

    public static bool NotNullOrEmptyExt(this string? text) => !IsNullOrEmptyExt(text);
    public static bool IsNotNullOrEmptyExt(this string? text) => !IsNullOrEmptyExt(text);

    public static bool HasValueExt(this string? text)
    {
        return !IsNullOrEmptyExt(text);
    }

    public static List<string> GetSubcriptionIdsFromTextExt(this string textWithSubciptionIds)
    {
        var coll = GetCollectionOfGuidsFromTextExt(textWithSubciptionIds);
        return coll.Count() > 0 ? coll.ToList() : null;
    }

    public static IEnumerable<string> GetCollectionOfGuidsFromTextExt(this string textWithGuids)
    {
        if (!textWithGuids.IsNullOrEmptyExt())
        {
            var spl = textWithGuids.Split(" ").ToList();
            if (spl.Count != 0)
            {
                foreach (var part in spl)
                {
                    if (Guid.TryParse(part, out Guid guid))
                        yield return part;
                }
            }
        }
    }

    public static IEnumerable<string> GetCollectionOfStringsFromTextExt(this string text, string separator = " ")
    {
        if (!text.IsNullOrEmptyExt())
        {
            var spl = text.Split(separator);
            if (spl.Count() != 0)
            {
                foreach (var part in spl)
                {
                    if (!part.Contains(separator))
                        yield return part;
                }
            }
        }
    }

    public static int GetCountCollectionOfStringsExt(this string text, string separator = " ")
    {
        if (!text.IsNullOrEmptyExt())
        {
            var spl = text.Split(separator);
            return spl.Count(a => !string.IsNullOrWhiteSpace(a));
        }
        return 0;
    }

    // Instantiate random number generator.  
    // It is better to keep a single Random instance 
    // and keep using Next on the same instance.  
    private static readonly Random _random = new Random();

    // Generates a random number within a range.      
    public static int RandomNumber(int min, int max)
    {
        return _random.Next(min, max);
    }

    // Generates a random string with a given size.    
    public static string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);

        // Unicode/ASCII Letters are divided into two blocks
        // (Letters 65–90 / 97–122):   
        // The first group containing the uppercase letters and
        // the second group containing the lowercase.  

        // char is a single Unicode character  
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26; // A...Z or a..z: length = 26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    // Generates a random string with a given size.    
    public static string RandomStringDate(int size, bool lowerCase = false, bool addDate = true)
    {
        var builder = new StringBuilder();
        if (addDate)
        {
            var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            builder.Append(date);
        }
        builder.Append(RandomString(size, lowerCase));
        return builder.ToString();
    }


    // Generates a random password.  
    // 4-LowerCase + 4-Digits + 2-UpperCase  
    public static string RandomPassword()
    {
        var passwordBuilder = new StringBuilder();

        // 4-Letters lower case   
        passwordBuilder.Append(RandomString(4, true));

        // 4-Digits between 1000 and 9999  
        passwordBuilder.Append(RandomNumber(1000, 9999));

        // 2-Letters upper case  
        passwordBuilder.Append(RandomString(2));
        return passwordBuilder.ToString();
    }

    public static string GetShortName(string text)
    {
        if (text.IsNullOrEmptyExt())
            return text;

		//live.com#
        if (text.Contains("live.com#"))        
            return text.Replace("live.com#", string.Empty);
		
		if (text.Contains("onmicrosoft.com") && text.Contains("@"))
        {
            var split = text.Split('@');
            if (split.Length > 0 && split[0].Length > 0)
                return split[0];
        }
        return text;
    }

	public static string GetSizeTextInMbExt(this long size) => string.Format("{0:F2} MB", size.ConvertBytesToMegabytesExt());

    public static bool IsEmailExt(this string text) => text is not null &&  Regex.IsMatch(text.Trim(), @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

	public static bool ContainsAnyExt(this string source, StringComparison stringComparison, params string[] values) 
        => values.Any(x => source.Contains(x, stringComparison));

	public static string FindSeparatorExt(this string obj, string defsSeparator = "  ")
    {
        string separator = defsSeparator;
        if (obj == null)
            return separator;
        if (obj.Contains(','))
            separator = ",";
		if (obj.Contains(';'))
			separator = ";";
		return separator;
	}

    public static string AddWhereContainsCondition(List<string> listOfParams, string property, string conditionOrAnd = "or")
    {
        if (listOfParams?.Count == 0 || property == null)
            return null;
        var query = $" | where";
        for (int i = 1;i <= listOfParams.Count; i++)
        {
			query += $" {property} contains '{listOfParams[i - 1]}'";
			if (i < listOfParams.Count)
				query += $" {conditionOrAnd}";
		}
        return query;
    }

	public static string AddWhereEqualsCondition(List<string> listOfParams, string property, string conditionOrAnd = "or")
	{
        if (listOfParams?.Count == 0 || property == null)
			return null;
		var query = $" | where";
		for (int i = 1; i <= listOfParams.Count; i++)
        {
			query += $" {property} == '{listOfParams[i - 1]}'";
			if (i < listOfParams.Count)
				query += $" {conditionOrAnd}";
		}
		return query;
	}

	/// <summary>
	/// Special case for Kusto query, Tags
	/// </summary>
	/// <param name="query"></param>
	/// <param name="listOfParams"></param>
	/// <param name="conditionOrAnd">or and</param>
	/// <param name="isOperatorEquOrCon">true -> contains, false -> == </param>
	/// <returns></returns>
	public static string AddWhereTagsCondition(List<KeyValuePair<string, string>> listOfParams)
	{
		if (listOfParams?.Count == 0)
			return null;
		var query = $" | where";

		//trim and group, 'opEnvironment' or 'opEnvironment ' is a same group
		Dictionary<string, List<KeyValuePair<string, string>>> dictGroup = new();
        foreach (var item in listOfParams)
        {
            var tr = item.Key.Trim();
            if (!dictGroup.ContainsKey(tr))
				dictGroup.Add(tr, new List<KeyValuePair<string, string>> { item });
            else
            {
                if (dictGroup.TryGetValue(tr, out var val))
                    val.Add(item);
            }
		}

        //foreach(var grp in dictGroup)
        for (int g = 1; g <= dictGroup.Count; g++)        
        {
			//add and condition per group, same trimed tag 'opEnvironment' or 'opEnvironment ' with same space
            var grp = dictGroup.ElementAt(g - 1);
            var list = grp.Value;
			query += $" (";
			for (int i = 1; i <= list.Count; i++)
			{
                //| where tags['opEnvironment'] contains "dev"                
				query += $" tags['{list[i - 1].Key}'] contains '{list[i - 1].Value}'";				
				if (i < list.Count)
					query += $" or"; //not last item
                else
					query += $")"; //last item
			}
            if (g < dictGroup.Count)
				query += $" and"; //different tag group
		}
		return query;
    }

	public static string AddWhereSubscriptionsCondition(List<string> subscriptions, string conditionOrAnd = "or")
	{
		if (subscriptions?.Count == 0)
			return null;
		StringBuilder query = new($" | where");
        for (int i = 1; i <= subscriptions.Count; i++)
        {
            var sub = subscriptions[i - 1];
            if (string.IsNullOrWhiteSpace(sub))
                continue;
            if (Guid.TryParse(sub, out Guid subGuid))
                query.Append($" subscriptionId == '{subGuid}'");
            else
                query.Append($" subscriptionName == '{sub}'");
            if (i < subscriptions.Count)				
				query.Append($" {conditionOrAnd}");
		}
        return query.ToString();
	}

    /// <summary>
    /// Create easy to read ID (10000000-0000-0000-0000-000000000000) for debug
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GetSimpleGuidString(this int number)
    {
        if (number <= 0)
            return Guid.Empty.ToString();
        StringBuilder sb = new(Guid.Empty.ToString());
        sb.Remove(0, number.ToString().Length);
        sb.Insert(0, number.ToString());
        return sb.ToString();
    }
}
