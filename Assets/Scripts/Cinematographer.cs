using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Cinematographer : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject skyCam;
    public GameObject blueTeamCam;
    public GameObject blueInvadersCam;
    public GameObject blueJailCam;
    public GameObject redTeamCam;
    public GameObject redInvadersCam;
    public GameObject redJailCam;

    [Header("Target Groups")]
    public CinemachineTargetGroup redTeamTg;
    public CinemachineTargetGroup redInvadersTg;
    public CinemachineTargetGroup blueTeamTg;
    public CinemachineTargetGroup blueInvadersTg;
    public CinemachineTargetGroup skyCamTg;

    [Header("Setup")]
    public Transform redTeamParent;
    public Transform blueTeamParent;
    private BattleInfo battleInfo;

    private void Start()
    {
        battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();
        StartCoroutine(CheckInvadersRoutine());
    }

    IEnumerator CheckInvadersRoutine()
    {
        // Red
        for (int i = 0; i < battleInfo.redInvaders.Count; i++)
        {
            if (redInvadersTg.FindMember(battleInfo.redInvaders[i].transform) == -1)
            {
                redInvadersTg.AddMember(battleInfo.redInvaders[i].transform, 0, 0);
            }
        }

        for (int i = 0; i < redInvadersTg.m_Targets.Length; i++)
        {
            if (!battleInfo.redInvaders.Contains(redInvadersTg.m_Targets[i].target.GetComponent<LittleDude>()))
            {
                redInvadersTg.RemoveMember(redInvadersTg.m_Targets[i].target);
            }
        }

        // Blue
        for (int i = 0; i < battleInfo.blueInvaders.Count; i++)
        {
            if (blueInvadersTg.FindMember(battleInfo.blueInvaders[i].transform) == -1)
            {
                blueInvadersTg.AddMember(battleInfo.blueInvaders[i].transform, 0, 0);
            }
        }

        for (int i = 0; i < blueInvadersTg.m_Targets.Length; i++)
        {
            if (!battleInfo.blueInvaders.Contains(blueInvadersTg.m_Targets[i].target.GetComponent<LittleDude>()))
            {
                blueInvadersTg.RemoveMember(blueInvadersTg.m_Targets[i].target);
            }
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(CheckInvadersRoutine());
    }


    public void PopulateRedTeamTargetGroups()
    {
        for (int i = 0; i < redTeamParent.childCount; i++)
        {
            redTeamTg.AddMember(redTeamParent.GetChild(i), 0, 0);
        }
    }

    public void ClearRedTeamTargetGroups()
    {
        redTeamTg.m_Targets = new CinemachineTargetGroup.Target[0];
        redInvadersTg.m_Targets = new CinemachineTargetGroup.Target[0];
    }

    public void PopulateBlueTeamTargetGroups()
    {
        for (int i = 0; i < blueTeamParent.childCount; i++)
        {
            blueTeamTg.AddMember(blueTeamParent.GetChild(i), 0, 0);
        }
    }

    public void ClearBlueTeamTargetGroups()
    {
        blueTeamTg.m_Targets = new CinemachineTargetGroup.Target[0];
        blueInvadersTg.m_Targets = new CinemachineTargetGroup.Target[0];
    }

    public void PopulateMainSkyCamTargetGroup()
    {
        for (int i = 0; i < redTeamParent.childCount; i++)
        {
            skyCamTg.AddMember(redTeamParent.GetChild(i), 0, 0);
        }

        for (int i = 0; i < blueTeamParent.childCount; i++)
        {
            skyCamTg.AddMember(blueTeamParent.GetChild(i), 0, 0);
        }
    }

    public void ClearMainSkyCamTargetGroup()
    {
        skyCamTg.m_Targets = new CinemachineTargetGroup.Target[0];
    }
}
