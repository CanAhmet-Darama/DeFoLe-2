using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static bool[] campsAlerted = new bool[GameManager.numberOfCamps];
    public static EnemyScript[][] enemies;

    void Start()
    {
        enemies = new EnemyScript[GameManager.numberOfCamps][];
    }

    void Update()
    {
        
    }
    public static void AddEnemyToList(byte campNumber, EnemyScript newEnemy)
    {
        if (enemies[campNumber - 1] == null)
        {
            enemies[campNumber - 1] = new EnemyScript[0];
        }
        EnemyScript[] holderArray = enemies[campNumber - 1];
        enemies[campNumber - 1] = new EnemyScript[holderArray.Length + 1];
        for (short i = (short)(holderArray.Length - 1); i >= 0; i--)
        {
            enemies[campNumber - 1][i] = holderArray[i];
        }
        enemies[campNumber - 1][holderArray.Length] = newEnemy;
    }
    public static void AlertWholeCamp(byte campNumber)
    {
        for (short index = (short)(enemies[campNumber - 1].Length - 1); index >= 0;index--)
        {
            if(enemies[campNumber - 1][index].enemyState != EnemyScript.EnemyAIState.Alerted)
            enemies[campNumber - 1][index].ChangeEnemyAIState(EnemyScript.EnemyAIState.Alerted);
        }
    }

}
