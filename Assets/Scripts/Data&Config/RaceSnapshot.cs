using UnityEngine;
using System;

[Serializable]
public class RaceSnapshot:OverrodeOperator
{
    public float Time;

    private Vector3Serialize _position;

    public Vector3 Position
    {
        get
        {
            return _position == null ? Vector3.zero : (Vector3)_position;
        }
        set
        {
            _position = (Vector3)value;
        }
    }

    private QuaternionSerialize _rotation;

    public Quaternion Rotation
    {
        get
        {
            return _rotation == null ? Quaternion.identity : (Quaternion)_rotation;
        }
        set
        {
            _rotation = (Quaternion)value;
        }
    }
}

[Serializable]
class QuaternionSerialize
{
    private float x;
    private float y;
    private float z;
    private float w;

    public QuaternionSerialize()
    {

    }

    public QuaternionSerialize(Quaternion quat)
    {
        x = quat.x;
        y = quat.y;
        z = quat.z;
        w = quat.w;
    }

    public static implicit operator QuaternionSerialize(Quaternion qua) 
    {
        return new QuaternionSerialize(qua);
    }

    public static explicit operator Quaternion(QuaternionSerialize qua) 
    {
        return new Quaternion(qua.x, qua.y, qua.z, qua.w);
    }
}

[Serializable]
class Vector3Serialize
{
    private float x;
    private float y;
    private float z;

    public Vector3Serialize()
    {
        
    }

    public Vector3Serialize(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public static implicit operator Vector3Serialize(Vector3 vec) 
    {
        return new Vector3Serialize(vec);
    }

    public static explicit operator Vector3(Vector3Serialize vec) 
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }
}


