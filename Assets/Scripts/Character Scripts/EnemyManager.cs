using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static bool[] campsAlerted = new bool[GameManager.numberOfCamps];
    public static bool[][] enemiesCanSee = new bool[GameManager.numberOfCamps][];
    public static Vector3[] lastSeenPosOfPlayer = new Vector3[GameManager.numberOfCamps];
    public static EnemyScript[][] enemies;
    public static float sortCoversCooldown = 6;
    static EnemyManager enemyManagerIns;

    public void EnemyManagerStart()
    {
        enemies = new EnemyScript[GameManager.numberOfCamps][];
        enemyManagerIns = this;
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
        if (enemiesCanSee[campNumber - 1] == null)
        {
            enemiesCanSee[campNumber - 1] = new bool[0];
        }
        EnemyScript[] holderArray = enemies[campNumber - 1];
        enemies[campNumber - 1] = new EnemyScript[holderArray.Length + 1];
        Array.Copy(holderArray, enemies[campNumber - 1], holderArray.Length);
        enemies[campNumber - 1][holderArray.Length] = newEnemy;
        newEnemy.enemyNumCode = (byte)holderArray.Length;

        bool[] visionHolderArray = enemiesCanSee[campNumber- 1];
        enemiesCanSee[campNumber - 1] = new bool[visionHolderArray.Length + 1];
        Array.Copy(visionHolderArray, enemiesCanSee[campNumber - 1], visionHolderArray.Length);
    }
    public static void AlertWholeCamp(byte campNumber)
    {
        for (short index = (short)(enemies[campNumber - 1].Length - 1); index >= 0;index--)
        {
            if(enemies[campNumber - 1][index].enemyState != EnemyScript.EnemyAIState.Alerted)
            enemies[campNumber - 1][index].ChangeEnemyAIState(EnemyScript.EnemyAIState.Alerted);
        }
        Debug.Log("Camp " + campNumber + " is alerted");
        enemyManagerIns.StartCoroutine(enemyManagerIns.SortCoversByDistance(campNumber));
    }
    public static void DealertWholeCamp(byte campNumber)
    {
        for (short index = (short)(enemies[campNumber - 1].Length - 1); index >= 0; index--)
        {
            if (enemies[campNumber - 1][index].enemyState == EnemyScript.EnemyAIState.Alerted)
                enemies[campNumber - 1][index].ChangeEnemyAIState(EnemyScript.EnemyAIState.Searching);
        }
        Debug.Log("Camp " + campNumber + " is not alerted anymore");

    }
    IEnumerator SortCoversByDistance(byte campNumber)
    {
        CoverObjectsManager.SortCoverObjectsByDistance(campNumber, GameManager.mainChar.position);
        yield return new WaitForSeconds(sortCoversCooldown);
        if(!campsAlerted[campNumber - 1])
        {
            enemyManagerIns.StartCoroutine(SortCoversByDistance(campNumber));
        }
    }

    public static void CheckAnyoneInCampCanSeeTarget()
    {
        for (byte campIndex = (byte)(enemies.Length - 1) ; campIndex >= 0 ; campIndex--)
        {
            if (campsAlerted[campIndex])
            {
                bool anyoneSawTarget = false;
                for (byte enemyIndex = (byte)(enemies[campIndex].Length-1); enemyIndex >= 0; enemyIndex--)
                {
                    if (enemiesCanSee[campIndex][enemyIndex])
                    {
                        lastSeenPosOfPlayer[campIndex] = enemies[campIndex][enemyIndex].lastSeenPos;
                        anyoneSawTarget = true;
                    }
                }
                if(!anyoneSawTarget)
                {
                    DealertWholeCamp(campIndex);
                }
            }
        }
    }
}
