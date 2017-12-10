using System;
using UnityEngine;

public abstract class PopBase : MonoBehaviour
{
    public abstract void SetData(PopUpData data);

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}

public class PopUpData : OverrodeOperator
{
    
}