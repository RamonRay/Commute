using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonsterWindow : Monster {
    [SerializeField] HandOnWindow handOnWindow;
    [SerializeField] Vector2 timeInterval = new Vector2(5f, 8f);
    private Coroutine slap;

    private Action onSlapEvent;

    protected override void Start()
    {
        base.Start();

    }


    protected override void Alive()
    {
        base.Alive();
        slap = StartCoroutine(KeepSlapping());

    }

    protected override void Dead()
    {
        base.Dead();
        StopCoroutine(slap);
    }

    IEnumerator KeepSlapping()
    {
        yield return new WaitForSeconds(2f);
        while(true)
        {
            Slap();
            yield return new WaitForSeconds(UnityEngine.Random.Range(timeInterval.x, timeInterval.y));
        }
        yield break;
    }

    private void Slap()
    {
        onSlapEvent();
        handOnWindow.Slap();
    }

    public void RegisterOnSlapEvent(Action _onSlap)
    {
        onSlapEvent += _onSlap;
    }
}
