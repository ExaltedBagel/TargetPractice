using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/*
[CustomEditor(typeof(Attack))]
public class AttackScriptEditor : Editor
{

protected Attack myAttack;
public override void OnInspectorGUI()
{
    myAttack.aName = EditorGUILayout.TextField("Attack Name", myAttack.aName);

    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Stats:");

    myAttack.dmg = EditorGUILayout.IntField("Number of Rolls", myAttack.dmg);
    myAttack.nDice = EditorGUILayout.IntField("Number of Rolls", myAttack.nDice);
    myAttack.roll = EditorGUILayout.IntField("Success Rate", myAttack.roll);
    myAttack.range = EditorGUILayout.Slider("Range", myAttack.range, 2f, 30f);

}
}

[CustomEditor(typeof(AttackSingleTarget))]
public class AttackSingleTargetScriptEditor : AttackScriptEditor
{
public override void OnInspectorGUI()
{
    //myAttack = (AttackSingleTarget)target;

    base.OnInspectorGUI();
}

}
*/
