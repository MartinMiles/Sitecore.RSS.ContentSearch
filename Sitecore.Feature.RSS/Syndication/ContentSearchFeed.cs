using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Syndication;

namespace Sitecore.Feature.RSS.Syndication
{
    public class ContentSearchFeed : PublicFeed
    {
        const string INDEX = "sitecore_{0}_index";
        const int defaultMonthsRecency = 24;

        private ID _rootPath = ID.Null;
        private IEnumerable<ID> _tags;
        private IEnumerable<ID> _includedContentTypes;

        #region Properties

        int MaximumItemsInFeed => int.Parse(Settings.GetSetting("Feeds.MaximumItemsInFeed", "250"));
        private ID RootPathField => new ID("{BCCD393E-F6C0-4D43-B419-33C9F2D86BF6}");
        private ID IncludedContentTypesField => new ID("{3FB39450-49EC-4A20-A319-D478B06D05B9}");
        private ID TagsField => new ID("{D0600F55-7B10-4252-8E90-2B5AF047867E}");
        private ID RecencyField => new ID("{38C13277-195A-40EF-9D67-357CF4280B75}");

        private ID RootPath
        {
            get
            {
                if (_rootPath.IsNull)
                {
                    _rootPath = ((LookupField)this.FeedItem.Fields[RootPathField]).TargetID;
                }

                return _rootPath;
            }
        }

        private IEnumerable<ID> IncludedContentTypes
        {
            get
            {
                if (_includedContentTypes == null)
                {
                    _includedContentTypes = ((MultilistField)this.FeedItem.Fields[IncludedContentTypesField]).Items.Select(ID.Parse);
                }

                return _includedContentTypes;
            }
        }

        private IEnumerable<ID> Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = ((MultilistField)this.FeedItem.Fields[TagsField]).Items.Select(ID.Parse);
                }

                return _tags;
            }
        }

        private int RecencyMonths
        {
            get
            {
                var field = (MultilistField)this.FeedItem.Fields[RecencyField];
                int intValue = int.TryParse(field.Value, out intValue) ? intValue : defaultMonthsRecency;

                return intValue;
            }
        }
        
        #endregion

        public override IEnumerable<Item> GetSourceItems()
        {
            if (ID.IsNullOrEmpty(RootPath) || IncludedContentTypes.Any() == false)
            {
                return Enumerable.Empty<Item>();
            }

            string databaseName = Sitecore.Context.Database.Name;

            using (var searcher = ContentSearchManager.GetIndex(string.Format(INDEX, databaseName)).CreateSearchContext())
            {
                var query = PredicateBuilder.True<CustomSearchResultItem>();
                query = query.And(i => i.Paths.Contains(RootPath));
                var contentTypesQuery = PredicateBuilder.False<CustomSearchResultItem>();

                foreach (var ct in IncludedContentTypes)
                {
                    var id = ct;
                    contentTypesQuery = contentTypesQuery.Or(i => i.TemplateId == id);
                }

                query = query.And(contentTypesQuery);

                var from = DateTime.Today.AddMonths(-RecencyMonths);
                query = query.And(x => x.CreatedDate.Between(from, DateTime.Today, Inclusion.Both));

                var tagsQuery = PredicateBuilder.True<CustomSearchResultItem>();

                foreach (var tag in Tags)
                {
                    var id = tag;
                    tagsQuery = tagsQuery.And(i => i.PrimaryTag.Contains(id));
                }

                query = query.And(tagsQuery);

                var results = searcher.GetQueryable<CustomSearchResultItem>()
                    .Where(query)
                    .Take(MaximumItemsInFeed)
                    .GetResults()
                    .Hits
                    .Select(i => i.Document.GetItem())
                    .ToList();

                return results;
            }
        }
    }
}