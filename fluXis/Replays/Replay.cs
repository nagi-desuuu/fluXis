﻿using System.Collections.Generic;

namespace fluXis.Replays;

public class Replay
{
    public long PlayerID { get; set; } = -1;
    public List<ReplayFrame> Frames { get; set; } = new();
}
