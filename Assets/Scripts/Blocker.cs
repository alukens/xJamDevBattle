using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    public bool hasTarget;
    public LittleDude target;

    private BattleInfo battleInfo;
    private LittleDude ld;


    private void Start()
    {
        if (!battleInfo)
            battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();

        if (!ld)
            ld = GetComponent<LittleDude>();

        StartCoroutine(InvadersCheckRoutine());
    }

    private void Update()
    {

        if (target != null)
        {
            if (target.tagged || !target.isInOpponentArea)
            {
                target = null;
                hasTarget = false;
            }
        }

        if (!ld.tagged)
        {
            if (hasTarget)
            {
                if (ld.isBlue)
                {
                    if (battleInfo.redHasBlueFlag && target != battleInfo.redGuyWithBlueFlag)
                    {
                        target = battleInfo.redGuyWithBlueFlag;
                    }
                }
                else
                {
                    if (battleInfo.blueHasRedFlag && target != battleInfo.blueGuyWithRedFlag)
                    {
                        target = battleInfo.blueGuyWithRedFlag;
                    }
                }
                ld.SetDestination(target.transform.position);
                ld.SetSprint();

            }
            else
            {
                ld.SetDestination(transform.position);
            }
        }
    }

    IEnumerator InvadersCheckRoutine()
    {
        GetClosestInvader();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(InvadersCheckRoutine());
    }

    public void GetClosestInvader()
    {
        if (ld.isBlue)
        {
            if (battleInfo.redInvaders.Count > 0)
            {
                target = battleInfo.GetClosest(battleInfo.redInvaders, transform.position);
                if (target != null)
                    hasTarget = true;
                else
                    hasTarget = false;
            }
            else
                hasTarget = false;
        }
        else
        {
            if (battleInfo.blueInvaders.Count > 0)
            {
                target = battleInfo.GetClosest(battleInfo.blueInvaders, transform.position);
                if (target != null)
                    hasTarget = true;
                else
                    hasTarget = false;
            }
            else
                hasTarget = false;
        }

    }

}
