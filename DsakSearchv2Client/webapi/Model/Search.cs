namespace webapi.Model
{
    public struct Search
    {
        public Search(string searchString, List<string> searchTerms, bool isTruncated)
        {
            SearchString = searchString;
            SearchTerms = searchTerms;
            IsTruncated = isTruncated;
            IsValid = searchTerms.Count > 0;
        }

        public string SearchString { get; }
        public List<string> SearchTerms { get; }
        public bool IsTruncated { get; }
        public bool IsValid { get; }
    }
}
