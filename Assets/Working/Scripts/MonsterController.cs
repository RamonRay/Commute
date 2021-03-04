using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    [SerializeField] float defaultReviveTime=5f;
    //[SerializeField] Vector2 initialTimeInterval = new Vector2(3f,5f);
    [SerializeField] float minTimeInterval=0.5f;
    [SerializeField] float multiplier=0.95f;
    [SerializeField] bool summonAll;



    public bool isOn { get; private set; }

    private Vector2 realtimeTimeInterval;

    private Coroutine summoning;

    private Monster[] monsters;

    private float lastKillTime=0f;

    private MonsterDifficultyControl monsterDifficulty;

    // Use this for initialization
    void Start () {
        isOn = false;
        monsters = transform.parent.gameObject.GetComponentsInChildren<Monster>();
        monsterDifficulty = gameObject.AddComponent<MonsterDifficultyControl>();
        monsterDifficulty.SetDefaultTime(defaultReviveTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartSummon()
    {
        monsterDifficulty.ResetDifficulty(1f);
        lastKillTime = Time.time;
        if (!summonAll)
        {
            isOn = true;
            summoning = StartCoroutine(SummonMonsters());
        }
        else
        {
            foreach (var monster in monsters)
            {
                monster.isAlive = true;
            }
        }
    }

    public void SummonAll()
    {
        foreach(var monster in monsters)
        {
            monster.isAlive = true;
        }
    }


    public void StopSummon()
    {
        isOn = false;
        if (summoning != null)
        {
            StopCoroutine(summoning);
        }

        foreach(var monster in monsters)
        {
            monster.isAlive = false;
        }
    }


    //Summon random monsters
    IEnumerator SummonMonsters()
    {

        //realtimeTimeInterval = initialTimeInterval;
        //RandomAlive();

        while(true)
        {
            bool isAllAlive = RandomAlive();
            yield return new WaitForSeconds(monsterDifficulty.reviveTime*Random.Range(0.9f,1.1f));
            //if (isAllAlive&&realtimeTimeInterval.x > minTimeInterval)
            //{
                //realtimeTimeInterval = realtimeTimeInterval * multiplier;
            //}
        }
    }


    //Set a random monster in the array alive, and return bool value if it set alive any monster
    private bool RandomAlive()
    {
        int _aliveNum = 0;
        _aliveNum = AliveMonsters();

        if(_aliveNum==monsters.Length)
        {
            Debug.Log("Maximum monster count reached");
            return false;
        }

        int random = Random.Range(0, monsters.Length - 1);
        while(monsters[random].isAlive)
        {
            random += 1;
            if(random>=monsters.Length)
            {
                random = 0;
            }
        }

        monsters[random].isAlive = true;
        return true;
    }

    //Number of monsters alive
    private int AliveMonsters()
    {
        int _count = 0;
        foreach(var monster in monsters)
        {
            if(monster.isAlive)
            {
                _count++;
            }
        }
        return _count;
    }

    public void ChildMonsterDead()
    {
        monsterDifficulty.UpdateDifficulty(Time.time - lastKillTime);
        lastKillTime = Time.time;
    }
}
