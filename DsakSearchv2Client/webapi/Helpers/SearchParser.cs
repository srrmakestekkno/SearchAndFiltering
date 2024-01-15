using System.Text;
using webapi.Model;

namespace webapi.Helpers
{
    public static class SearchParser
    {
        public static Search ParseSearchString(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return new Search();
            }

            searchString = RestrictCharactersToIso8859();
            searchString = searchString.Trim();
            if (string.IsNullOrEmpty(searchString))
            {
                return new Search();
            }

            const int MaxCharacters = 1337;
            bool isTruncated = false;
            if (searchString.Length > MaxCharacters)
            {
                searchString = searchString.Substring(0, MaxCharacters);
                isTruncated = true;
            }

            return ParseSearchString();

            Search ParseSearchString()
            {
                var searchTerms = new List<string>();
                var parts = searchString.Split('"', StringSplitOptions.RemoveEmptyEntries);
                const int MaxSearchTerms = 20;
                for (int i = 0; i < parts.Length; i++)
                {
                    var trimmedTerm = parts[i].Trim();
                    if (trimmedTerm == string.Empty)
                    {
                        continue;
                    }

                    searchTerms.Add(trimmedTerm);
                    if (searchTerms.Count == MaxSearchTerms)
                    {
                        isTruncated = true;
                        break;
                    }
                }

                var search = new Search(searchString, searchTerms, isTruncated);
                return search;
            }

            string RestrictCharactersToIso8859()
            {
                var iso8859 = Encoding.GetEncoding("ISO-8859-1");
                var iso8859SearchString = iso8859.GetString(
                    Encoding.Convert(
                        Encoding.UTF8,
                        Encoding.GetEncoding(
                            "ISO-8859-1",
                            new EncoderReplacementFallback(string.Empty),
                            new DecoderExceptionFallback()
                            ),
                        Encoding.UTF8.GetBytes(searchString)
                    )
                );

                return iso8859SearchString;
            }
        }

        public static string ParseSearch(Search search)
        {
            if (search.SearchTerms.Count == 1)
            {
                return search.SearchTerms[0];
            }

            var searchString = new StringBuilder("\"");
            searchString.Append(search.SearchTerms[0]);
            searchString.Append("\"");
            for (int i = 1; i < search.SearchTerms.Count; i++)
            {
                searchString.Append(" \"");
                searchString.Append(search.SearchTerms[i]);
                searchString.Append("\"");
            }

            return searchString.ToString();
        }
    }
}
