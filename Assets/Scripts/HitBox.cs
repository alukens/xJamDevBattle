using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public LittleDude littleDude;
    public List<LittleDude> friendlyTargets;
    public List<LittleDude> enemyTargets;

    private bool isRed;
    private bool isBlue;

    private void Start()
    {
        isRed = littleDude.isRed;
        isBlue = littleDude.isBlue;
    }

    private void Update()
    {
        if (friendlyTargets.Count > 0)
        {
            TargetsRefreshRoutine();
        }
            //ListCleanup();
    }

    private void TargetsRefreshRoutine()
    {
        for (int i = 0; i < enemyTargets.Count; i++)
        {
            if (enemyTargets[i].tagged)
                enemyTargets.Remove(enemyTargets[i]);
        }

        for (int i = 0; i < friendlyTargets.Count; i++)
        {
            if (!friendlyTargets[i].inJail || !friendlyTargets[i].tagged)
                friendlyTargets.Remove(friendlyTargets[i]);
        }
    }

    public LittleDude ClosestEnemyTarget()
    {
        float closestDist = 1000;
        int closestIndex = 0;
        if (enemyTargets.Count > 1)
        {
            for (int i = 0; i < enemyTargets.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, enemyTargets[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }
            return enemyTargets[closestIndex];
        }
        else if (enemyTargets.Count == 1)
            return enemyTargets[0];
        else
            return null;

    }

    public LittleDude ClosestFriendlyTarget()
    {
        float closestDist = 1000;
        int closestIndex = 0;
        if (friendlyTargets.Count > 1)
        {
            for (int i = 0; i < friendlyTargets.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, friendlyTargets[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }
            return friendlyTargets[closestIndex];
        }
        else if (friendlyTargets.Count == 1)
            return friendlyTargets[0];
        else
            return null;

    }

    private void OnTriggerEnter(Collider other)
    {
        LittleDude oLd = other.gameObject.GetComponent<LittleDude>();
        if (isRed)
        {
            if (other.gameObject.CompareTag("Blue") && !oLd.tagged)
            {
                if (!enemyTargets.Contains(oLd))
                {
                    enemyTargets.Add(oLd);
                }
            }

            //Friendly
            if (other.gameObject.CompareTag("Red") && oLd.tagged && oLd.inJail)
            {
                if (oLd.transform != GetComponent<LittleDude>())
                {
                    if (!friendlyTargets.Contains(oLd))
                    {
                        friendlyTargets.Add(oLd);
                    }
                }
            }

        }
        if (isBlue)
        {
            if (other.gameObject.CompareTag("Red") && !oLd.tagged)
            {
                if (!enemyTargets.Contains(oLd))
                {
                    enemyTargets.Add(oLd);
                }
            }

            //Friendly
            if (other.gameObject.CompareTag("Blue") && oLd.tagged && oLd.inJail)
            {
                if (!friendlyTargets.Contains(oLd))
                {
                    friendlyTargets.Add(oLd);
                }
            }
        }
    }

    public void ListCleanup()
    {
        for (int i = 0; i < friendlyTargets.Count; i++)
        {
            if (!friendlyTargets[i].tagged)
                friendlyTargets.Remove(friendlyTargets[i]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LittleDude oLd = other.gameObject.GetComponent<LittleDude>();
        if (isRed)
        {
            if (other.gameObject.CompareTag("Blue"))
            {
                if (enemyTargets.Contains(oLd))
                {
                    enemyTargets.Remove(oLd);
                }
            }
        }
        if (isBlue)
        {
            if (other.gameObject.CompareTag("Red"))
            {
                if (enemyTargets.Contains(oLd))
                {
                    enemyTargets.Remove(oLd);
                }
            }
        }
    }

}
