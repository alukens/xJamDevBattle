using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasiveManeuversTemplate : MonoBehaviour
{
    [Header("Emergency Override Settings")]
    public bool emergencyOverride;
    public float panicDistance;
    public Vector3 destinationWhenPanicked;
    public Vector3 panicDestination;
    private float originalStopDistance;

    public UnityEngine.MonoBehaviour[] scriptsToOverride;

    [Header("Enemy Detection")]
    public List<LittleDude> enemiesNearby;
    public LittleDude closestEnemy;

    private bool scriptsDisabled;
    private LittleDude ld;

    private void Start()
    {
        if (!ld)
            ld = GetComponent<LittleDude>();
    }

    private void Update()
    {
        if (!ld.tagged && !ld.mustReturnToBase)
            EnemyDetection();

        if (emergencyOverride && !scriptsDisabled)
        {
            DisableScripts();
            originalStopDistance = ld.stopDistance;
            destinationWhenPanicked = ld.navTarget;
            ld.stopDistance = 0;
        }
        else if (!emergencyOverride && scriptsDisabled)
        {
            EnableScripts();
            ld.SetDestination(destinationWhenPanicked);
        }

        if (emergencyOverride && !ld.tagged && !ld.mustReturnToBase)
        {
            Vector3 dir = (transform.position - closestEnemy.transform.position);
            dir = dir.normalized;
            panicDestination = (transform.position + dir);
            ld.SetDestination(panicDestination);
        }
    }

    public void EnemyDetection()
    {
        enemiesNearby.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, panicDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (ld.isBlue && ld.isInOpponentArea)
            {
                if (hitCollider.gameObject.CompareTag("Red"))
                {
                    enemiesNearby.Add(hitCollider.GetComponent<LittleDude>());
                }
            }
            else if (ld.isRed && ld.isInOpponentArea)
            {
                if (hitCollider.gameObject.CompareTag("Blue"))
                {
                    enemiesNearby.Add(hitCollider.GetComponent<LittleDude>());
                }
            }
        }

        if (enemiesNearby.Count > 0)
        {
            emergencyOverride = true;
            closestEnemy = GetClosestEnemy();
            ld.SetSprint();
        }
        else
            emergencyOverride = false;
    }

    public LittleDude GetClosestEnemy()
    {
        float closestDist = 1000;
        int closestIndex = 0;

        for (int i = 0; i < enemiesNearby.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, enemiesNearby[i].transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return enemiesNearby[closestIndex];
    }

    public void DisableScripts()
    {
        scriptsDisabled = true;
        for (int i = 0; i < scriptsToOverride.Length; i++)
        {
            scriptsToOverride[i].enabled = false;
        }
    }

    public void EnableScripts()
    {
        scriptsDisabled = false;
        for (int i = 0; i < scriptsToOverride.Length; i++)
        {
            scriptsToOverride[i].enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(panicDestination, 0.5f);
    }
}
