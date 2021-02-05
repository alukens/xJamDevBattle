using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo : MonoBehaviour
{
    /* BattleInfo contains all imporant real-time match information. Use the following syntax to call it:
     * 
     * ---->    BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();    <----
     * 
     * Best practice: Create a local variable to store the reference, too many GetComponent calls can be processer heavy.
     * 
     * NOTE: A simple "Battle Script" template has been provided in this project.
     * To create a new template, right click in your Project tab and go to Create / C# Dev Battle Script to create a script with BattleInfo already referenced.
     * 
     * 
     * EXAMPLE SETUP:
     * 
     * Declared this at the top of your script that you would like to use BattleInfo:
     * private BattleInfo battleInfo;
     * 
     * Then place this in the Start() function:
     * if (!battleInfo)
     *      battleInfo = BattleInfo.instance.battleInfo.GetComponent<BattleInfo>();
     */

    [Header("Blue Team Trackers")]
    public List<LittleDude> blueTeam;       // Full Team Roster
    public List<LittleDude> blueInvaders;   // Blue players that are in Red's Zone.
    public List<LittleDude> bluePrisoners;  // Blue players in Red's Jail.
    public float bluePercentInJail;         // Percetange of Blue players in Red's Jail.
    public float blueFlagDistanceToRedBase; // If this is reduced to 3 or less, Red Wins.

    [Header("Blue Team Status")]
    public bool blueHasRedFlag;             // True if any Blue team player is carrying Red's flag.
    public bool blueFlagDropped;            // True if Blue's OWN flag is currently dropped (ie, not being carried but not at it's home base)
    public LittleDude redGuyWithBlueFlag;   // The Red teammate that is carrying the flag.

    [Header("Red Team Trackers")]
    public List<LittleDude> redTeam;        // (Same as Blue's descriptions)
    public List<LittleDude> redInvaders;
    public List<LittleDude> redPrisoners;
    public float redPercentInJail;
    public float redFlagDistanceToBlueBase;

    [Header("Red Team Status")]
    public bool redHasBlueFlag;
    public bool redFlagDropped;
    public LittleDude blueGuyWithRedFlag;


    [Header("Setup")]
    public Transform redParent;
    public Transform blueParent;

    [Header("Imporant Battlefield Positions")]
    public Flag redFlag;
    public Transform redJail;
    public Flag blueFlag;
    public Transform blueJail;
    
    

    [Header("Static")]
    public GameObject battleInfo;
    public static BattleInfo instance;

    private void Awake()
    {
        instance = this;
        battleInfo = gameObject;

        PopulateTeamLists();
    }

    private void Update()
    {
        InvadersCheck();
        JailPopulationCheck();
    }

    public void InvadersCheck()
    {
        for (int i = 0; i < redTeam.Count; i++)
        {
            if (redTeam[i].isInOpponentArea && !redTeam[i].tagged)
            {
                if (!redInvaders.Contains(redTeam[i]))
                    redInvaders.Add(redTeam[i]);
            }
            else
            {
                if (redInvaders.Contains(redTeam[i]))
                    redInvaders.Remove(redTeam[i]);
            }

            if (blueTeam[i].isInOpponentArea && !blueTeam[i].tagged)
            {
                if (!blueInvaders.Contains(blueTeam[i]))
                    blueInvaders.Add(blueTeam[i]);
            }
            else
            {
                if (blueInvaders.Contains(blueTeam[i]))
                    blueInvaders.Remove(blueTeam[i]);
            }
        }
    }

    public void JailPopulationCheck()
    {
        for (int i = 0; i < redTeam.Count; i++)
        {
            if (redTeam[i].inJail)
            {
                if (!redPrisoners.Contains(redTeam[i]))
                    redPrisoners.Add(redTeam[i]);
            }
            else
            {
                if (redPrisoners.Contains(redTeam[i]))
                    redPrisoners.Remove(redTeam[i]);
            }

            if (blueTeam[i].inJail)
            {
                if (!bluePrisoners.Contains(blueTeam[i]))
                    bluePrisoners.Add(blueTeam[i]);
            }
            else
            {
                if (bluePrisoners.Contains(blueTeam[i]))
                    bluePrisoners.Remove(blueTeam[i]);
            }
        }
    }

    public void PopulateTeamLists()
    {
        for (int i = 0; i < blueParent.childCount; i++)
        {
            LittleDude ldB = blueParent.GetChild(i).GetComponent<LittleDude>();
            blueTeam.Add(ldB);

            LittleDude ldR = redParent.GetChild(i).GetComponent<LittleDude>();
            redTeam.Add(ldR);
        }
    }

    public LittleDude GetClosest(List<LittleDude> _listToCheck, Vector3 _currentPos)
    {
        float closestDist = 1000;
        int closestIndex = 0;

        for (int i = 0; i < _listToCheck.Count; i++)
        {
            float dist = Vector3.Distance(_currentPos, _listToCheck[i].transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return _listToCheck[closestIndex];
    }

}
