using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace Sitecore.Feature.RSS.Syndication
{
    public class CustomSearchResultItem : SearchResultItem
    {
        [IndexField("primarytag")]
        public IEnumerable<ID> PrimaryTag { get; set; }
    }
}