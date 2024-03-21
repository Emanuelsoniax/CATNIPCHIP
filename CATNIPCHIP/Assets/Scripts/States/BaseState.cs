
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState : ScriptableObject
{
    [HideInInspector]
    public Cat cat;
    public NavMeshAgent agent;

    public virtual void OnStart()
    {
       cat  = FindAnyObjectByType<Cat>();
       agent = cat.GetComponent<NavMeshAgent>();
    }

    public virtual void OnUpdate() { }
    public virtual void OnEnd() {
#if UNITY_EDITOR
        Debug.Log(this +  ": OnEnd");
#endif

    }
}
