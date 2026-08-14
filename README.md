# Limbo Fetch

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/limbo-works/Limbo.Umbraco.Fetch/blob/v17/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.Fetch.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Fetch)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.Fetch.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Fetch)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.fetch)

`Limbo.Umbraco.Fetch` is a package for Umbraco for downloaded external files to disk to be used in your Umbraco installation.

For instance, as shown in [Configuration](#configuration), the package can be used to download the RSS feed of my blog to disk, which can then be used to display my recent blog posts on the website.



<br /><br /><br />

## Installation

Install for Umbraco 17 via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.Fetch/17.0.0-alpha001).

.NET CLI:

```
dotnet add package Limbo.Umbraco.Fetch --version 17.0.0-alpha001
```

NuGet Package Manager:

```
Install-Package Limbo.Umbraco.Fetch -Version 17.0.0-alpha001
```

For other versions of Umbraco, see the following branches:

- [**`v13/main`**](https://github.com/limbo-works/Limbo.Umbraco.Fetch/tree/v13/main) Umbraco 13
- ~~[**`v10/main`**](https://github.com/limbo-works/Limbo.Umbraco.Fetch/tree/v10/main) Umbraco 10, 11 and 12~~ <sub title="Umbraco 10, 11 and 12 have reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v9/main`**](https://github.com/limbo-works/Limbo.Umbraco.Fetch/tree/v9/main) Umbraco 9~~ <sub title="Umbraco 9 has reached end-of-life"><sup>(EOL)</sup>



<br /><br /><br />

## Configuration

To configure the package, add a `Limbo:Fetch` section to your `appsettings.json` like in the example below:

```json
{
  "Limbo": {
    "Fetch": {
      "Feeds": [
        {
          "Alias": "MyBlog",
          "Url": "http://www.bjerner.dk/blog/rss",
          "Path": "~/umbraco/Data/TEMP/Limbo/Fetch/BjernerRss.xml",
          "Interval": 30
        }
      ]
    }
  }
}
```

If the interval is specified as a number, it is treated as the amount of minutes between each time the file will be downloaded. Alternatively, the interval may be specified using the ISO 8601 duration format - eg. `PT1H30M` for one and a half hours.
