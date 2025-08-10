using Microsoft.Playwright;

namespace AStar.Dev.Test.Helpers.EndToEnd;

/// <summary>
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="additionalDictionaries"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<Dictionary<TKey, TValue>?>? additionalDictionaries)
        where TKey : class
    {
        var result = new Dictionary<TKey, TValue>();

        foreach(var dictionaryEntry in dictionary)
        {
            result[dictionaryEntry.Key] = dictionaryEntry.Value;
        }

        if(additionalDictionaries == null)
        {
            return result;
        }

        foreach(var dictionaryEntry in additionalDictionaries.Where(additionalDictionary => additionalDictionary is not null).SelectMany(additionalDictionary => additionalDictionary!))
        {
            result[dictionaryEntry.Key] = dictionaryEntry.Value;
        }

        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="browser"></param>
    /// <returns></returns>
    public static IPage SetExtraHttpHeaders(this Dictionary<string, string> dictionary, IBrowser browser)
    {
        var page = browser.NewPageAsync().Result;
        page.SetExtraHTTPHeadersAsync(dictionary);

        return page;
    }
}
