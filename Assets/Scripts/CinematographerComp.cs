using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cinematographer))]
public class CinematographerComp : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cinematographer myScript = (Cinematographer)target;
        if (GUILayout.Button("Build Red Team Target Groups"))
        {
            myScript.PopulateRedTeamTargetGroups();
        }

        if (GUILayout.Button("Clear Red Team Target Groups"))
        {
            myScript.ClearRedTeamTargetGroups();
        }

        if (GUILayout.Button("Build Blue Team Target Groups"))
        {
            myScript.PopulateBlueTeamTargetGroups();
        }

        if (GUILayout.Button("Clear Blue Team Target Groups"))
        {
            myScript.ClearBlueTeamTargetGroups();
        }

        if (GUILayout.Button("Build Sky Cam Target Groups"))
        {
            myScript.PopulateMainSkyCamTargetGroup();
        }

        if (GUILayout.Button("Clear Sky Cam Target Groups"))
        {
            myScript.ClearMainSkyCamTargetGroup();
        }
    }
}
