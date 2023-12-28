using System.Globalization;

namespace ADroneSimulator.Bases;

internal class CountryFlags
{
    private static readonly List<RegionInfo> regionInfos = [];

    public static string GetFlagName(string country)
    {
        if (regionInfos.Count == 0) throw new InvalidProgramException("CountryFlags constructeur doesn't called before");

        RegionInfo? regionInfo = regionInfos.FirstOrDefault(region => region.EnglishName.Contains(country[(country.LastIndexOf(',') + 2)..]));
        return regionInfo == null ? string.Empty : $"{System.IO.Path.GetFullPath(".")}/Images/Flags/{regionInfo.TwoLetterISORegionName.ToLower()}.png";
    }

    public CountryFlags()
    {
        foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures))
        {
            try
            {
                RegionInfo region = new(cultureInfo.LCID);
                regionInfos.Add(region);
            }
            catch { } // The culture does not have an associated region.
        }
    }
}
