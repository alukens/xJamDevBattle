using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public bool isRed;
    public bool isBlue;
    public bool pickedUp;
    public bool dropped;
    public bool atHomeBase;

    [Header("Setup")]
    public Transform homeBase;

    // Important Outside Scripts
    private BattleInfo battleInfo;

    void Start()
    {
        if (!battleInfo)
            battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();
    }

    public void PickUp(Transform _flagSlot, LittleDude _ld)
    {
        atHomeBase = false;
        pickedUp = true;
        dropped = false;

        if (isBlue)
        {
            battleInfo.redHasBlueFlag = true;
            battleInfo.blueFlagDropped = false;
            battleInfo.redGuyWithBlueFlag = _ld;
        }
        else
        {
            battleInfo.blueHasRedFlag = true;
            battleInfo.redFlagDropped = false;
            battleInfo.blueGuyWithRedFlag = _ld;
        }

        transform.position = _flagSlot.position;
        transform.parent = _flagSlot;
    }

    public void Drop()
    {
        atHomeBase = false;
        pickedUp = false;
        dropped = true;

        if (isBlue)
        {
            battleInfo.blueFlagDropped = true;
            battleInfo.redHasBlueFlag = false;

        }
        else
        {
            battleInfo.redFlagDropped = true;
            battleInfo.blueHasRedFlag = false;
        }

        transform.parent = homeBase;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void Return()
    {
        atHomeBase = true;
        pickedUp = false;
        dropped = false;

        if (isBlue)
        {
            battleInfo.blueFlagDropped = false;
        }
        else
        {
            battleInfo.redFlagDropped = false;
        }

        transform.position = homeBase.position;
        transform.parent = homeBase;
    }
}