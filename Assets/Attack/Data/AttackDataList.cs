using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AttackDataList : ScriptableObject
{
    [SerializeField]
    private List<AttackData> attackPList;

    public List<AttackData> attackList { get { return attackPList; } set { attackPList = value; } }

    public AttackData findByID(string uniqueID)
    {
        if (attackPList.Count == 0)
            return null;
        foreach (AttackData x in attackPList.ToList<AttackData>())
        {
            if (x.uniqueId == uniqueID)
                return x;
        }
        return null;
    }
}
