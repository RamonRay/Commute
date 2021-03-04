using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SubjectNerd.Utilities;

[Serializable]
public struct Order
{
    public int index;
    public float timeInterval;
}

public class MonsterCarController : MonoBehaviour {

    [SerializeField] MonsterController[] monsterControllers;
    [Reorderable]
    [SerializeField] Order[] order;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SequencialSummon()
    {
        StartCoroutine(StartSequencialSummon());
    }

    IEnumerator StartSequencialSummon()
    {
        int index = 0;
        while(index<order.Length)
        {
            yield return new WaitForSeconds(order[index].timeInterval);
            monsterControllers[order[index].index].StartSummon();
            index += 1;
        }
        yield break;
    }

    public void SummonAllAtOnce()
    {
        foreach(var monsterController in monsterControllers)
        {
            monsterController.SummonAll();
        }
    }


    public void StopSummon()
    {
        StopAllCoroutines();
    }

    public void StopSummonAndKill()
    {
        StopAllCoroutines();

        foreach (var monsterController in monsterControllers)
        {
            monsterController.StopSummon();
        }
    }
}
