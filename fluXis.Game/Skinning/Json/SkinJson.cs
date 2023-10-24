using System;
using System.Collections.Generic;
using fluXis.Game.Scoring.Enums;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Skinning.Json;

public class SkinJson
{
    [JsonProperty("1k")]
    public SkinKeymode OneKey { get; set; } = new() { ColumnWidth = 132 };

    [JsonProperty("2k")]
    public SkinKeymode TwoKey { get; set; } = new() { ColumnWidth = 126 };

    [JsonProperty("3k")]
    public SkinKeymode ThreeKey { get; set; } = new() { ColumnWidth = 120 };

    [JsonProperty("4k")]
    public SkinKeymode FourKey { get; set; } = new() { ColumnWidth = 114 };

    [JsonProperty("5k")]
    public SkinKeymode FiveKey { get; set; } = new() { ColumnWidth = 108 };

    [JsonProperty("6k")]
    public SkinKeymode SixKey { get; set; } = new() { ColumnWidth = 102 };

    [JsonProperty("7k")]
    public SkinKeymode SevenKey { get; set; } = new() { ColumnWidth = 96 };

    [JsonProperty("8k")]
    public SkinKeymode EightKey { get; set; } = new() { ColumnWidth = 90 };

    [JsonProperty("9k")]
    public SkinKeymode NineKey { get; set; } = new() { ColumnWidth = 84 };

    [JsonProperty("10k")]
    public SkinKeymode TenKey { get; set; } = new() { ColumnWidth = 78 };

    [JsonProperty("judgements")]
    public SkinJudgements Judgements { get; set; } = new();

    [JsonProperty("overrides")]
    public Dictionary<string, string> Overrides { get; set; } = new();

    public SkinKeymode GetKeymode(int keyCount)
    {
        return keyCount switch
        {
            1 => OneKey,
            2 => TwoKey,
            3 => ThreeKey,
            4 => FourKey,
            5 => FiveKey,
            6 => SixKey,
            7 => SevenKey,
            8 => EightKey,
            9 => NineKey,
            10 => TenKey,
            _ => throw new ArgumentOutOfRangeException(nameof(keyCount), keyCount, null)
        };
    }

    public Colour4 GetColorForJudgement(Judgement judgement)
    {
        var hex = judgement switch
        {
            Judgement.Flawless => Judgements.Flawless,
            Judgement.Perfect => Judgements.Perfect,
            Judgement.Great => Judgements.Great,
            Judgement.Alright => Judgements.Alright,
            Judgement.Okay => Judgements.Okay,
            Judgement.Miss => Judgements.Miss,
            _ => throw new ArgumentOutOfRangeException(nameof(judgement), judgement, null)
        };

        try { return Colour4.FromHex(hex); }
        catch { return Colour4.White; }
    }

    [CanBeNull]
    public string GetOverride(string key) => Overrides.TryGetValue(key, out var value) ? value : null;

    public string GetOverriderOrDefault(string key) => GetOverride(key) ?? key;

    public SkinJson Copy() => (SkinJson)MemberwiseClone();
}