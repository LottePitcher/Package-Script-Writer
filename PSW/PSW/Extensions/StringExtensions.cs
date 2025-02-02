﻿using System.Reflection;

using PSW.Configuration;

namespace PSW.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) { return value; }

        if (value.Length > maxLength) { return value.Substring(0, maxLength) + "..."; }

        return value.Substring(0, value.Length);
    }

    public static string GetVersionText(this string version, string templateName, PSWConfig pswConfig)
    {
        if (string.IsNullOrWhiteSpace(version)) { return version; }

        if (templateName != "Umbraco.Templates") { return version; }

        var majorVersionNumberAsString = version.Split('.').FirstOrDefault();

        if (string.IsNullOrWhiteSpace(majorVersionNumberAsString)) { return version; }

        _ = int.TryParse(majorVersionNumberAsString, out var majorVersionNumber);

        if (pswConfig.UmbracoVersions == null || !pswConfig.UmbracoVersions.Any()) return version;

        if (!pswConfig.UmbracoVersions.Select(x => x.Version).Contains(majorVersionNumber)) { return version; }

        var versionInUse = pswConfig.UmbracoVersions.FirstOrDefault(x => x.Version == majorVersionNumber);

        var oneYearFromNow = DateTime.UtcNow.AddYears(1);
        var isLTS = versionInUse.ReleaseType == "LTS";
        var isEndOfLife = DateTime.UtcNow >= versionInUse.EndOfLife;
        var isSTS = !isLTS;
        var willEOLInLessThanAYear = !isEndOfLife && oneYearFromNow > versionInUse.EndOfLife;
        var isFutureRelease = versionInUse.ReleaseDate > DateTime.UtcNow;

        var suffix = "";

        if (isEndOfLife)
        {
            suffix = "💀";
        }
        else if ((isSTS && !isFutureRelease) || willEOLInLessThanAYear)
        {
            suffix = "⚠️";
        }
        else if (isFutureRelease)
        {
            suffix = "🔮";
        }
        else
        {
            suffix = "✅";
        }

        return suffix + " " + version;
    }
}