using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace fluXis.Map;

public class MapSetResources
{
    public TextureStore BackgroundStore { get; init; }
    public TextureStore CroppedBackgroundStore { get; init; }
    public ITrackStore TrackStore { get; init; }
    public ISampleStore SampleStore { get; init; }
}
