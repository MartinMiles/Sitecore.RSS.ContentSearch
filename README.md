# Sitecore RSS ContentSearch

The adequate extension for an outdated Sitecore built-in legacy RSS feed.
## Description
New `Content search` section is added to built-in `RSS Feed` template upon installation enabling your feed to benefit from using index:

![Sifon](https://raw.githubusercontent.com/wiki/MartinMiles/Sitecore.RSS.ContentSearch/images/ContentSearchSection.png "New Content Search section")


## Instructions

Clone the code and rename as a Helix feature module, or use on its own. The code contains class library for a feature module alond with TDS project with the serialization.
You may alternatively prefer using traditional sitecore package from SitecorePackage folder instead of using TDS.

## Serilization / TDS contains the following nodes with children:
- `/sitecore/templates/Feature/RSS` - RSS folder and standard values with insert options for RSS Feed.
- `/sitecore/templates/System/Feeds/RSS Feed/Content search` - An additional template section with fields to control **Content Search API** consumption

This extends built-in RSS feed with a Content Search section. By using this section you intercept default (Sitecore query-based) behaviour and return feed directly from you utilized context index. If `Type` field is empty - the default out-of-the-box RSS Feed behaviour is used. The feed is configured by the following criterias:
- `Root Path`: parent-level node under which content is being searched
- `Included Content Types`: one or more page templates to be returned
- `Tags`: it is assumed that you used some kind of taxonomy, so that works as an additional filtering criteria to thoses two above
- `Recency` limits the above resultset to only numbered amont of months to date (24 months is default value). It date is taken from item's Created field, not the Updates (but you may change if wanted).

## Code

`CustomSearchResultItem` class maps context search to a specified index field, that containg tags. Replace `IndexField` attribute barameter with the index field you're using.

`ContentSearchFeed` inherites from PublicFeed and carries on the whole business logic. Feel free to change its defaults and behaviour, if needed.


## Credits
Douglas Couto (his [GitHub](https://github.com/dcouto "GitHub") page).