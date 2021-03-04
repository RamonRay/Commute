using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDifficultyControl : MonoBehaviour {
    
    //public static MonsterDifficultyControl instance;
    [SerializeField] float defaultReviveTime=5f;
    [SerializeField] int maxQueueLength = 12;
    [SerializeField] float difficultyChangeSpeed = 0.5f;


    public float reviveTime { get; private set; }
    

    private Queue<float> killTimes;

    private float averageTime;



	// Use this for initialization
	void Start ()
    {
        ResetDifficulty(1f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    //Reset the difficulty of the game. Larger multiplier the easier.
    public void ResetDifficulty(float multiplier)
    {
        reviveTime = defaultReviveTime;
        killTimes = new Queue<float>();
        for (int i = 0; i < 5; i++)
        {
            killTimes.Enqueue(defaultReviveTime*multiplier);
        }
        averageTime = GetAverage();
    }

    public void SetDefaultTime(float _defaultTime)
    {
        defaultReviveTime = _defaultTime;
    }

    //Return the average killing time of the monster
    private float GetAverage()
    {
        if(killTimes.Count<1)
        {
            return defaultReviveTime;
        }
        float tmp = 0f;
        foreach(float time in killTimes)
        {
            tmp += time;
        }
        tmp = tmp / killTimes.Count;

        return tmp;
    }
    //Called every time a monster is killed with its survival time
    public void UpdateDifficulty(float killTime)
    {
        if(killTimes.Count>maxQueueLength)
        {
            killTimes.Dequeue();
        }

        killTimes.Enqueue(killTime);

        averageTime = GetAverage();

        //Update the revive time
        reviveTime = difficultyChangeSpeed * averageTime + (1f - difficultyChangeSpeed) * reviveTime;

    }


}
