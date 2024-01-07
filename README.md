# Limbo Fetch

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.Fetch.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Fetch)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.Fetch.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Fetch)
[![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-fetch/)

`Limbo.Umbraco.Fetch` is a package for Umbraco 9 for downloaded external files to disk to be used in your Umbraco installation.

For instance, as shown in [Configuration](#configuration), the package can be used to download the RSS feed of my blog to disk, which can then be used to display my recent blog posts on the website.

## Installation

Install for Umbraco 9 via [NuGet](https://www.nuget.org/packages/Limbo.Umbraco.Fetch/9.0.0):

```
dotnet add package Limbo.Umbraco.Fetch --version 9.0.0
```

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
          "Path": "~/App_Data/Fetch/BjernerRss.xml",
          "Interval": 30
        }
      ]
    }
  }
}
```

If the interval is specified as a number, it is treated as the amount of minutes between each time the file will be downloaded. Alternatively, the interval may be specified using the ISO 8601 duration format - eg. `PT1H30M` for one and a half hours.
