using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public class NoLnMod : IMod
{
    public string Name => "No LN";
    public string Acronym => "NLN";
    public string Description => "Removes all long notes and replaces them with single notes.";
    public IconUsage Icon => FontAwesome.Solid.ArrowsAltV;
    public float ScoreMultiplier => .8f;
    public bool Rankable => true;
    public IEnumerable<string> IncompatibleMods => Array.Empty<string>();
}