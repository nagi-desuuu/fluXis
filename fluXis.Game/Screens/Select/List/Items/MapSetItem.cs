﻿using System;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.List.Drawables.MapSet;
using fluXis.Game.UI;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Select.List.Items;

public class MapSetItem : IListItem, IComparable<MapSetItem>
{
    public Bindable<MapUtils.SortingMode> Sorting { get; set; }
    public Bindable<SelectedState> State { get; } = new();

    public SelectScreen Screen { get; set; }
    public MapStore Store { get; set; }

    public RealmMapMetadata Metadata => set.Metadata;

    public Drawable Drawable { get; set; }

    private RealmMapSet set { get; }

    public MapSetItem(RealmMapSet set)
    {
        this.set = set;
    }

    public Drawable CreateDrawable()
    {
        return Drawable = new DrawableMapSetItem(set)
        {
            SelectAction = Screen.Accept,
            EditAction = Screen.EditMap,
            DeleteAction = Screen.DeleteMapSet,
            ExportAction = Screen.ExportMapSet
        };
    }

    public void Bind()
    {
        Store.MapSetBindable.BindValueChanged(mapSetChanged, true);
    }

    public void Unbind()
    {
        Store.MapSetBindable.ValueChanged -= mapSetChanged;
    }

    public void Select(bool last = false)
    {
        var map = last ? set.HighestDifficulty : set.LowestDifficulty;
        Store.Select(map, true);
    }

    public bool Matches(object obj)
    {
        if (obj is RealmMapSet)
            return ReferenceEquals(obj, set);

        if (obj is RealmMap map)
            return set.Maps.Any(m => ReferenceEquals(m, map));

        return false;
    }

    public bool MatchesFilter(SearchFilters filters)
    {
        var first = set.Maps.FirstOrDefault(filters.Matches);
        return first is not null;
    }

    public bool ChangeChild(int by)
    {
        var maps = set.MapsSorted;

        int current = maps.IndexOf(Store.CurrentMap);
        current += by;

        if (current < 0)
            return true;

        if (current >= maps.Count)
            return true;

        Store.Select(maps[current], true);
        return false;
    }

    private void mapSetChanged(ValueChangedEvent<RealmMapSet> e)
        => State.Value = ReferenceEquals(e.NewValue, set) ? SelectedState.Selected : SelectedState.Deselected;

    public int CompareTo(MapSetItem other) => MapUtils.CompareSets(set, other.set, Sorting.Value, Screen.SortInverse);

    public int CompareTo(IListItem other)
    {
        if (other is not MapSetItem o)
            return -1;

        return CompareTo(o);
    }
}
