using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Storyboards;
using fluXis.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Import.osu.Storyboards;

public class OsuStoryboardParser
{
    private const float width = 640;
    private const float widescreen_width = 854;
    private const float x_adjust = (widescreen_width - width) / 2;

    private Storyboard storyboard;
    private StoryboardElement currentElement;
    private OsuStoryboardLoop currentLoop;

    public Storyboard Parse(string data)
    {
        var lines = data.Split("\n");
        storyboard = new Storyboard
        {
            Resolution = new Vector2(widescreen_width, 480)
        };

        var idx = 0;

        foreach (var line in lines)
        {
            idx++;

            try
            {
                if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("["))
                    continue;

                parseLine(line);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to parse storyboard line {idx}: {line}");
            }
        }

        foreach (var element in storyboard.Elements)
        {
            var initialValues = new HashSet<StoryboardAnimationType>();
            var startTime = double.MaxValue;
            var endTime = double.MinValue;

            var toAdd = new List<StoryboardAnimation>();

            foreach (var animation in element.Animations)
            {
                if (initialValues.Add(animation.Type))
                {
                    toAdd.Add(new StoryboardAnimation
                    {
                        Type = animation.Type,
                        ValueStart = animation.ValueStart,
                        ValueEnd = animation.ValueStart
                    });
                }

                startTime = Math.Min(startTime, animation.StartTime);
                endTime = Math.Max(endTime, animation.EndTime);
            }

            foreach (var animation in toAdd)
            {
                animation.StartTime = startTime - 1; // just to make sure it's actually the initial
                element.Animations.Add(animation);
            }

            element.Animations.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

            if (element.Animations.Count <= 0)
            {
                Logger.Log($"Element {element.Type} ({element.Parameters["file"]}) does not have any animations!");
                continue;
            }

            element.StartTime = startTime;
            element.EndTime = endTime;
        }

        storyboard.Elements.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        return storyboard;
    }

    private void parseLine(string line)
    {
        var depth = 0;

        foreach (char c in line)
        {
            if (c is ' ' or '_')
                depth++;
            else
                break;
        }

        line = line[depth..];

        var split = line.Split(',');

        if (depth == 0)
        {
            if (currentLoop is not null)
            {
                currentLoop.PushTo(currentElement.Animations);
                currentLoop = null;
            }

            currentElement = null;

            switch (split[0])
            {
                case "Sprite" or "Animation":
                {
                    var origin = parseOrigin(split[2]);
                    var path = cleanFilename(split[3]);
                    var x = split[4].ToFloatInvariant() + x_adjust;
                    var y = split[5].ToFloatInvariant();

                    if (split[0] == "Animation")
                    {
                        var pathExt = path.Split('.').Last();
                        path = path.Replace($".{pathExt}", $"0.{pathExt}");
                    }

                    currentElement = new StoryboardElement
                    {
                        Type = StoryboardElementType.Sprite,
                        Layer = StoryboardLayer.Background,
                        Anchor = Anchor.TopLeft,
                        Origin = origin,
                        StartX = x,
                        StartY = y,
                        Parameters = new Dictionary<string, JToken>
                        {
                            { "file", path }
                        }
                    };
                    break;
                }
            }

            storyboard.Elements.Add(currentElement);
        }
        else
        {
            var commandType = split[0];

            if (depth == 1 && currentLoop is not null)
            {
                currentLoop.PushTo(currentElement.Animations);
                currentLoop = null;
            }

            switch (commandType)
            {
                case "L":
                {
                    var startTime = split[1].ToDoubleInvariant();
                    var count = split[2].ToIntInvariant();

                    currentLoop = new OsuStoryboardLoop
                    {
                        StartTime = startTime,
                        LoopCount = count
                    };
                    break;
                }

                default:
                {
                    if (string.IsNullOrEmpty(split[3]))
                        split[3] = split[2];

                    var easing = (Easing)split[1].ToIntInvariant();
                    var startTime = split[2].ToDoubleInvariant();
                    var endTime = split[3].ToDoubleInvariant();
                    var duration = endTime - startTime;

                    StoryboardAnimation[] buffer = null;

                    switch (commandType)
                    {
                        case "F": // fade
                        {
                            var start = split[4].ToFloatInvariant();
                            var end = split.Length > 5 ? split[5].ToFloatInvariant() : start;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.Fade,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = start.ToStringInvariant(),
                                    ValueEnd = end.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "S": // scale
                        {
                            var start = split[4].ToFloatInvariant();
                            var end = split.Length > 5 ? split[5].ToFloatInvariant() : start;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.Scale,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = start.ToStringInvariant(),
                                    ValueEnd = end.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "V": // vector scale
                        {
                            var startX = split[4].ToFloatInvariant();
                            var startY = split[5].ToFloatInvariant();
                            var endX = split.Length > 6 ? split[6].ToFloatInvariant() : startX;
                            var endY = split.Length > 7 ? split[7].ToFloatInvariant() : startY;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.ScaleVector,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = $"{startX},{startY}",
                                    ValueEnd = $"{endX},{endY}"
                                }
                            };

                            break;
                        }

                        case "R": // rotate
                        {
                            var start = split[4].ToFloatInvariant();
                            var end = split.Length > 5 ? split[5].ToFloatInvariant() : start;

                            var startDeg = float.RadiansToDegrees(start);
                            var endDeg = float.RadiansToDegrees(end);

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.Rotate,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = startDeg.ToStringInvariant(),
                                    ValueEnd = endDeg.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "M": // move
                        {
                            var startX = split[4].ToFloatInvariant() + x_adjust;
                            var startY = split[5].ToFloatInvariant();
                            var endX = split.Length > 6 ? split[6].ToFloatInvariant() + x_adjust : startX;
                            var endY = split.Length > 7 ? split[7].ToFloatInvariant() : startY;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.MoveX,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = startX.ToStringInvariant(),
                                    ValueEnd = endX.ToStringInvariant()
                                },
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.MoveY,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = startY.ToStringInvariant(),
                                    ValueEnd = endY.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "MX": // move x
                        {
                            var start = split[4].ToFloatInvariant() + x_adjust;
                            var end = split.Length > 5 ? split[5].ToFloatInvariant() + x_adjust : start;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.MoveX,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = start.ToStringInvariant(),
                                    ValueEnd = end.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "MY": // move y
                        {
                            var start = split[4].ToFloatInvariant();
                            var end = split.Length > 5 ? split[5].ToFloatInvariant() : start;

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.MoveY,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = start.ToStringInvariant(),
                                    ValueEnd = end.ToStringInvariant()
                                }
                            };

                            break;
                        }

                        case "C":
                        {
                            var startRed = split[4].ToFloatInvariant();
                            var startGreen = split[5].ToFloatInvariant();
                            var startBlue = split[6].ToFloatInvariant();
                            var endRed = split.Length > 7 ? split[7].ToFloatInvariant() : startRed;
                            var endGreen = split.Length > 8 ? split[8].ToFloatInvariant() : startGreen;
                            var endBlue = split.Length > 9 ? split[9].ToFloatInvariant() : startBlue;

                            var hexStart = $"#{(int)startRed:X2}{(int)startGreen:X2}{(int)startBlue:X2}";
                            var hexEnd = $"#{(int)endRed:X2}{(int)endGreen:X2}{(int)endBlue:X2}";

                            buffer = new[]
                            {
                                new StoryboardAnimation
                                {
                                    Type = StoryboardAnimationType.Color,
                                    StartTime = startTime,
                                    Duration = duration,
                                    Easing = easing,
                                    ValueStart = hexStart,
                                    ValueEnd = hexEnd
                                }
                            };
                            break;
                        }
                    }

                    if (buffer is null || buffer.Length == 0)
                        break;

                    if (depth == 2 && currentLoop is not null)
                        buffer.ForEach(currentLoop.Animations.Add);
                    else if (depth == 1)
                        buffer.ForEach(currentElement.Animations.Add);

                    break;
                }
            }
        }
    }

    private Anchor parseOrigin(string val)
    {
        switch (val)
        {
            case "TopLeft":
                return Anchor.TopLeft;

            case "TopCentre":
                return Anchor.TopCentre;

            case "TopRight":
                return Anchor.TopRight;

            case "CentreLeft":
                return Anchor.CentreLeft;

            case "Centre":
                return Anchor.Centre;

            case "CentreRight":
                return Anchor.CentreRight;

            case "BottomLeft":
                return Anchor.BottomLeft;

            case "BottomCentre":
                return Anchor.BottomCentre;

            case "BottomRight":
                return Anchor.BottomRight;

            default:
                return Anchor.TopLeft;
        }
    }

    private string cleanFilename(string filename)
    {
        return filename.Replace(@"\\", @"\")
                       .Trim('"')
                       .ToStandardisedPath();
    }
}
