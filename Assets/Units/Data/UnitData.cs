using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitData : IEquatable<UnitData>
{
    public UnitData()
    {
        uName = "New Unit";
        attacksDataID = new List<String>();
        //Create a unique ID for the unit
        uniqueID = System.Guid.NewGuid().ToString();

        typeAtk = new float[(int)DamageType.TOTAL];
        typeDef = new float[(int)DamageType.TOTAL];
    }

    //Needs
    public GameObject model;

    [SerializeField]
    private string uniqueID;

    public String uniqueId { get { return uniqueID; } }

    //Stats
    public string uName;

    public UnitTypes uType;
    public bool isHero;

    public int maxHp;
    public float maxSpeed;

    public int impulse;
    public int memory;

    public float[] typeAtk;
    public float[] typeDef;

    [SerializeField]
    private List<String> attacksDataID;

    public List<String> attacksData { get { return attacksDataID; } set { attacksDataID = value; } }

    /*************************************/
    //Redefined Equalities
    /************************************/
    public bool Equals(UnitData other)
    {
        if (Equals(this, null) && Equals(other, null))
            return true;
        else if (Equals(this, null) | Equals(other, null))
            return false;
        else if (Equals(this.uniqueId == other.uniqueId))
            return true;
        else
            return false;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as UnitData);
    }
    public override int GetHashCode()
    {
        return this.GetHashCode(); 
    }
}

