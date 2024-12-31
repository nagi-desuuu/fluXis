using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

public partial class NoteEventTag : EditorTag
{
    public override Colour4 TagColour => Colour4.FromHex("#235284");

    private NoteEvent note => (NoteEvent)TimedObject;

    public NoteEventTag(EditorTagContainer parent, NoteEvent timedObject)
        : base(parent, timedObject)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{note.Content}";
    }
}
