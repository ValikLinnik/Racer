using System;
using UnityEngine;

public class RaceSnapshot:OverrodeOperator
{
    public float Time
    {
        get;
        set;
    }

    public Vector3 Position
    {
        get;
        set;
    }

    public Quaternion Rotation
    {
        get;
        set;
    }

    public override string ToString()
    {
        return string.Format("[RaceSnapshot: Time={0}, Position={1}, Rotation={2}]", Time, Position, Rotation);
    }
}


