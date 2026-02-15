using System.Text.RegularExpressions;
using SmartController.Shared.Enums;

namespace SmartController.Api.Services;

public class ParsedCounter
{
    public CounterType Channel { get; set; }
    public long Count { get; set; }
}

public static class RawDataParser
{
    // Format: {KKTSayı} örn: {00T1250}, {01T320}, {02T166}
    private static readonly Regex Pattern = new(@"\{(\d{2})T(\d+)\}", RegexOptions.Compiled);

    public static List<ParsedCounter> Parse(string rawData)
    {
        var results = new Dictionary<CounterType, long>();
        var matches = Pattern.Matches(rawData);

        foreach (Match match in matches)
        {
            var channelCode = int.Parse(match.Groups[1].Value);
            var count = long.Parse(match.Groups[2].Value);

            if (channelCode >= 0 && channelCode <= 4)
            {
                var channel = (CounterType)channelCode;
                // Aynı kanal birden fazla gelirse son değeri al (kümülatif)
                results[channel] = count;
            }
        }

        return results.Select(r => new ParsedCounter { Channel = r.Key, Count = r.Value }).ToList();
    }
}
