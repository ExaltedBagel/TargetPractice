using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


[CustomEditor(typeof(AttackDataList))]
public class AttackDatabaseInspector : Editor
{
    public override void OnInspectorGUI()
    {
        AttackDataList myDatabase = (AttackDataList)target;
        int i = 0;
        foreach (AttackData x in myDatabase.attackList.ToList<AttackData>())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString() + ") " + x.aName);
            
            EditorGUILayout.LabelField(x.expectedMin.ToString() + " - " + x.expectedMax.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(x.uniqueId);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type: "+ x.pattern.ToString());
            EditorGUILayout.LabelField("Range: " + x.range.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            i++;
        }
    }
}

