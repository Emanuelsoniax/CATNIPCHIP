
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : ScriptableObject
{
    [HideInInspector]
    public Cat cat;

    public virtual void OnStart()
    {
       cat  = (Cat)FindAnyObjectByType<StateMachine>();
    }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() {
#if UNITY_EDITOR
        Debug.Log(this +  ": OnEnd");
#endif

    }
}
