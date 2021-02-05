using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rescuer : MonoBehaviour
{
    public Transform redJail;
    private LittleDude ld;
    private BattleInfo battleInfo;


    private void Start()
    {
        battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();

        if (!ld)
            ld = GetComponent<LittleDude>();
    }

    private void Update()
    {
        if (ld.isBlue)
        {
            if (battleInfo.bluePrisoners.Count > 0 && ld.navTarget != redJail.position)
            {
                ld.navTarget = redJail.position;
            }
              
        }
        else
        {
            if (battleInfo.redPrisoners.Count > 0 && ld.navTarget != redJail.position)
            {
                ld.navTarget = redJail.position;
            }
        }
    }

}
