// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using Hackman.Router.ART;
using System.Collections.Specialized;
using System.Text;
using System.Text.Unicode;

public class AdaptiveRadixTreeTest
{
    #region public static GenerateTestKeys()
    /// <param name="max_random_entries_per_character">ex: 3, key='ABCD', result='ABBCCCD'</param>
    public static IEnumerable<string> GenerateTestKeys(long count, int max_random_entries_per_character = 3, uint seed = 0xBADC0FFE)
    {
        var random = new Random(unchecked((int)seed));
        for (long i = 0; i < count; i++)
        {
            var key = ChangeBase(i);

            if (max_random_entries_per_character <= 1)
                yield return "/" + key;
            else
            {
                var sb = new StringBuilder();
                for (int j = 0; j < key.Length; j++)
                {
                    var c = key[j];
                    var dupes = j + 1 < key.Length && c == key[j + 1] ?
                        // consecutive character repeats have special rules in order to ensure they are unique
                        max_random_entries_per_character :
                        random.Next(max_random_entries_per_character) + 1;
                    sb.Append(c, dupes);
                }
                yield return "/" + sb.ToString();
            }
        }
    }
    #endregion
    #region private static ChangeBase()
    private static string ChangeBase(long value, string new_base = "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
    {
        var current = Math.Abs(value);

        if (current >= 0 && current < new_base.Length)
        {
            return value >= 0 ?
                new string(new_base[unchecked((int)value)], 1) :
                new string(new char[2] { '-', new_base[unchecked((int)value)] });
        }

        char[] res;
        int new_base_size = new_base.Length;
        var size = unchecked((int)Math.Ceiling(Math.Log(current + 1, new_base_size)));

        if (value > 0)
            res = new char[size];
        else
        {
            res = new char[size + 1];
            res[0] = '-';
        }

        int index = res.Length;

        do
        {
            res[--index] = new_base[unchecked((int)(current % new_base_size))];
            current /= new_base_size;
        } while (current > 0);

        return new string(res);
    }
    #endregion
}

[AllStatisticsColumn]
public class ARTTest
{
    ArtTree art1 = new ArtTree();
    AdaptiveRadixTree<string, int> art = new AdaptiveRadixTree<string, int>();
    Dictionary<string, int> dict = new Dictionary<string, int>();
    string key;
    byte[] key2;
    public ARTTest()
    {
        int aa = 0;

        //var items = AdaptiveRadixTreeTest.GenerateTestKeys(100000).ToArray();
        var items = new string[] { "/aad/dsd/fsdd", "/aad/dsds/fsd", "/aaddd/dsds/fsd" };
        key = items.Last();
        key2 = System.Text.UTF8Encoding.UTF8.GetBytes(key);

        foreach (var item in items)
        {
            art.Add(item, aa);
            art1.Insert(System.Text.UTF8Encoding.UTF8.GetBytes(item), aa);
            dict.Add(item, aa);
            aa++;
        }
    }

    [Benchmark]
    public object ArtTreeSearch() => art1.Search(key2);

    [Benchmark]
    public object AdaptiveRadixTreeTest()
    {
        art.TryGetValue(key, out var a);
        return a;
    }


    [Benchmark]
    public object DictionarySearch()
    {
        dict.TryGetValue(key, out var a);
        return a;
    }
}