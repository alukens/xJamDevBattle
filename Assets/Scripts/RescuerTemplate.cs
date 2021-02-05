using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuerTemplate : MonoBehaviour
{

    // Outside scripts needed.
    private LittleDude ld;
    private BattleInfo battleInfo;

    // Settings / Important info
    private Vector3 startPosition;
    private bool rescueOperationInProgress;
    private List<LittleDude> prisoners;

    private void Start()
    {
        // Outside scripts setup
        if (!battleInfo)
            battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();

        if (!ld)
            ld = GetComponent<LittleDude>();

        // This template will use it's starting position as it's "return to base" position.
        startPosition = transform.position;
    }

    private void Update()
    {
        RescueOperationCheck();
        FindNearestTeammate();
    }


    /********************************************************************************************************************
     * This function will:
     *      1) Determine if this unit is on the Red or Blue team.
     *      2) Check to see if any of it's teammates are in jail.
     *      3) If there are, it will set it's destination to the jail.
     *      4) If there are no more units to be rescued, it will return to it's starting position.
     *      
     ********************************************************************************************************************/
    public void RescueOperationCheck()
    {
        if (ld.isBlue) // Blue
        {
            // If this unit has teammates in jail and has not already started a rescue operation, set it's navTarget to the opponent's jail.
            if (battleInfo.bluePrisoners.Count > 0 && !rescueOperationInProgress)
            {
                if (ld.navTarget != battleInfo.redJail.position)    // Checks to see if the navTarget has been set.
                {
                    if (!rescueOperationInProgress)                 
                        rescueOperationInProgress = true;
                }
            }
            else if (battleInfo.bluePrisoners.Count == 0 && rescueOperationInProgress)
            {
                if (ld.navTarget != startPosition)
                    ld.navTarget = startPosition;

                if (rescueOperationInProgress)
                    rescueOperationInProgress = false;
            }

        }
        else // Red 
        {
            if (battleInfo.redPrisoners.Count > 0 && !rescueOperationInProgress)
            {
                if (!rescueOperationInProgress)
                    rescueOperationInProgress = true;
            }
            else if (battleInfo.redPrisoners.Count == 0 && rescueOperationInProgress)
            {
                if (ld.navTarget != startPosition)
                    ld.navTarget = startPosition;

                if (rescueOperationInProgress)
                    rescueOperationInProgress = false;
            }
        }
    }

    /*
     * If rescueOperationInProgess is true, this script will detect the closest teammate that is in Jail
     */
    public void FindNearestTeammate()
    {
        if (rescueOperationInProgress)
        {
            if (ld.isBlue)
            {
                ld.navTarget = battleInfo.GetClosest(battleInfo.bluePrisoners, transform.position).transform.position;
            }
            else
            {
                ld.navTarget = battleInfo.GetClosest(battleInfo.redPrisoners, transform.position).transform.position;
            }
        }
    }
}
