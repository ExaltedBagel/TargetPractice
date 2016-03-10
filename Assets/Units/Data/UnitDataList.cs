using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UnitDataList : ScriptableObject
{
    [SerializeField]
    private List<UnitData> unitPList;

    public List<UnitData> unitList { get { return unitPList; } set { unitPList = value; } }

    public UnitData findByID(string uniqueID)
    {
        foreach(UnitData x in unitPList.ToList<UnitData>())
        {
            if (x.uniqueId == uniqueID)
                return x;
        }
        return null;
    }
}

