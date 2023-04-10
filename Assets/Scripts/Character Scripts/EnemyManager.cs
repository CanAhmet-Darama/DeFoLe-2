using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static bool[] campsAlerted = new bool[GameManager.numberOfCamps];
    public static bool[][] enemiesCanSee;
    public static bool[][] enemiesDead;
    public static Vector3[] lastSeenPosOfPlayer = new Vector3[GameManager.numberOfCamps];
    public static EnemyScript[][] enemies;
    public static float sortCoversCooldown = 6;
    static EnemyManager enemyManagerIns;
    static Coroutine dealertCampCoroutine;

    public void EnemyManagerStart()
    {
        enemiesCanSee = new bool[GameManager.numberOfCamps][];
        enemiesDead = new bool[GameManager.numberOfCamps][];
        enemies = new EnemyScript[GameManager.numberOfCamps][];
        enemyManagerIns = this;
        StartCoroutine(AreEnemiesSeeingTarget());
    }

    // Adds enemy to the given camp list
    public static void AddEnemyToList(byte campNumber, EnemyScript newEnemy)
    {
        if (enemies[campNumber - 1] == null)
        {
            enemies[campNumber - 1] = new EnemyScript[0];
        }
        if (enemiesCanSee[campNumber - 1] == null)
        {
            enemiesCanSee[campNumber - 1] = new bool[0];
            enemiesDead[campNumber - 1] = new bool[0];
        }
        EnemyScript[] holderArray = enemies[campNumber - 1];
        enemies[campNumber - 1] = new EnemyScript[holderArray.Length + 1];
        enemiesDead[campNumber - 1] = new bool[holderArray.Length + 1];
        GameManager.CopyArray(holderArray, enemies[campNumber - 1]);
        enemies[campNumber - 1][holderArray.Length] = newEnemy;
        newEnemy.enemyNumCode = (byte)holderArray.Length;

        bool[] visionHolderArray = enemiesCanSee[campNumber- 1];
        enemiesCanSee[campNumber - 1] = new bool[visionHolderArray.Length + 1];
        GameManager.CopyArray(visionHolderArray, enemiesCanSee[campNumber - 1]);
    }
    public static void AlertWholeCamp(byte campNumber)
    {
        for (short index = (short)(enemies[campNumber - 1].Length - 1); index >= 0;index--)
        {
            if(enemies[campNumber - 1][index].enemyState != EnemyScript.EnemyAIState.Alerted)
            enemies[campNumber - 1][index].ChangeEnemyAIState(EnemyScript.EnemyAIState.Alerted);
        }
        campsAlerted[campNumber - 1] = true;
        //Debug.Log("Camp " + (campNumber) + " is alerted");
        enemyManagerIns.StartCoroutine(enemyManagerIns.SortCoversByDistance(campNumber));
    }
    public static void DealertWholeCamp(byte campNumber)
    {
        for (short index = (short)(enemies[campNumber].Length - 1); index >= 0; index--)
        {
            if (enemies[campNumber][index].enemyState == EnemyScript.EnemyAIState.Alerted)
                enemies[campNumber][index].ChangeEnemyAIState(EnemyScript.EnemyAIState.Searching);
        }
        campsAlerted[campNumber] = false;
        //Debug.Log("Camp " + (campNumber + 1) + " is not alerted anymore");

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
        for (short campIndex = (short)(enemies.Length - 1) ; campIndex >= 0 ; campIndex--)
        {
            if (campsAlerted[campIndex])
            {
                bool anyoneSawTarget = false;
                for (short enemyIndex = (short)(enemies[campIndex].Length-1); enemyIndex >= 0; enemyIndex--)
                {
                    if (enemiesCanSee[campIndex][enemyIndex])
                    {
                        lastSeenPosOfPlayer[campIndex] = enemies[campIndex][enemyIndex].lastSeenPos;
                        anyoneSawTarget = true;
                        break;
                    }
                }
                if(!anyoneSawTarget)
                {
                    if(dealertCampCoroutine == null)
                    {
                        dealertCampCoroutine = enemyManagerIns.StartCoroutine(enemyManagerIns.DealertingCampCoroutine((byte)campIndex));
                    }
                }
                else
                {
                    if(dealertCampCoroutine != null)
                    {
                        enemyManagerIns.StopCoroutine(dealertCampCoroutine);
                    }
                }
            }
        }
    }
    IEnumerator DealertingCampCoroutine(byte campIndex)
    {
        yield return new WaitForSeconds(3);
        DealertWholeCamp(campIndex);
    }
    IEnumerator AreEnemiesSeeingTarget()
    {
        CheckAnyoneInCampCanSeeTarget();
        yield return new WaitForSeconds(2);
        StartCoroutine(AreEnemiesSeeingTarget());
    }

    public static void ActivateEnemy(EnemyScript enemyScr, bool activate)
    {

        enemyScr.enabled = activate;
        enemyScr.animator.enabled = activate;
        enemyScr.enemyAimer.enabled = activate;
        enemyScr.navAgent.enabled = activate;
        if(enemyScr.health > 0)
        {
        enemyScr.enemyHitboxes.SetActive(activate);
        }
        enemyScr.stairSlopeChecker.enabled = activate;
        enemyScr.groundChecker.SetActive(activate);
        if(enemyScr.weaponState == GeneralCharacter.WeaponState.ranged)
        {
            for(int i = enemyScr.mainWeapon.meshedPartOfWeapon.Length - 1; i>= 0; i--)
            {
                enemyScr.mainWeapon.meshedPartOfWeapon[i].SetActive(activate);
            }
        }
    }
    public static void UndetailEnemy(EnemyScript enemyScr, bool undetailing)
    {
        if(enemyScr.health > 0)
        {
            enemyScr.animator.enabled = !undetailing;
            enemyScr.stairSlopeChecker.enabled = !undetailing;
            enemyScr.groundChecker.SetActive(!undetailing);
        }
        if (enemyScr.weaponState == GeneralCharacter.WeaponState.ranged)
        {
            for (int i = enemyScr.mainWeapon.meshedPartOfWeapon.Length - 1; i >= 0; i--)
            {
                enemyScr.mainWeapon.meshedPartOfWeapon[i].SetActive(!undetailing);
            }
        }
        else if(enemyScr.weaponState == GeneralCharacter.WeaponState.melee)
        {
            enemyScr.mainMelee.meshPart.SetActive(!undetailing);
        }
    }
}
