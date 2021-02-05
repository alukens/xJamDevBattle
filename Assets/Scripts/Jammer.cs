using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammer : MonoBehaviour
{
    public LittleDude ld;
    public Flag enemyFlag;
    private BattleInfo battleInfo;

    private void Start()
    {
        if (ld == null)
            ld = GetComponent<LittleDude>();

        if (!battleInfo)
            battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();

        if (ld.isBlue)
            enemyFlag = battleInfo.redFlag;
        else
            enemyFlag = battleInfo.blueFlag;


        if (ld.navTarget != enemyFlag.transform.position)
            ld.SetDestination(enemyFlag.transform.position);
    }

    private void Update()
    {
        if (ld.isBlue && !ld.tagged)
        {
            if (ld.hasFlag || battleInfo.blueHasRedFlag)
            {
                Vector3 runHomePos = new Vector3(-5, transform.position.y, transform.position.z);
                if (ld.navTarget != runHomePos)
                ld.navTarget = runHomePos;
            }
            else
            {
                
                if (ld.navTarget != enemyFlag.transform.position)
                    ld.SetDestination(enemyFlag.transform.position);
            }
        }
        
        if (ld.isRed && !ld.tagged)
        {
            if (ld.hasFlag || battleInfo.redHasBlueFlag)
            {
                ld.navTarget = new Vector3(5, transform.position.y, transform.position.z);
            }
            else
            {
                if (ld.navTarget != enemyFlag.transform.position)
                    ld.SetDestination(enemyFlag.transform.position);
            }
        }
    }
}
