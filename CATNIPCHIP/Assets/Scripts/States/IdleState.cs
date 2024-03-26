using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "States/Idle", order = 1)]
public class IdleState : BaseState
{
    public int idleAnimationIndex;
    public event Action onWaitedForEvent;
    private float timer;

    [SerializeField]
    [Tooltip("Time needed to complete interaction with the cat")]
    private float interactionTime;

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void OnStart()
    {
        base.OnStart();
        cat.controller.animator.SetFloat("IdleIndex", idleAnimationIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (cat.IsInteracting())
        {
            timer -= Time.deltaTime;
        }
    }

    public IEnumerator InteractionTimer()
    {
        timer = interactionTime;
        yield return new WaitUntil(() => timer <= 0);

        onWaitedForEvent.Invoke();
        yield return null;
    }

}
