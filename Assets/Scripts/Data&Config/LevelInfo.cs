using System;
using NUnit.Framework;
using System.Collections.Generic;

public class LevelInfo : OverrodeOperator
{
    public float BestTime
    {
        get;
        set;
    }

    public List<RaceSnapshot> BestRace
    {
        get;
        set;
    }
}


