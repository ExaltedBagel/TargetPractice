using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class AttackData : IEquatable<AttackData>
{

    public AttackData()
    {
        aName = "New Attack";

        //Create a unique ID for the unit
        uniqueID = System.Guid.NewGuid().ToString();
    }

    [SerializeField]
    private String uniqueID;

    public String uniqueId { get { return uniqueID; } }

    public string aName = "Default";
    public AttackPattern pattern;
    public List<DamageType> dmgTypes = new List<DamageType>();

    public float range = 5f;
    public float radius = 5f;

    public int roll = 50;
    public int dmg = 2;
    public int nDice = 5;

    public int expectedMin;
    public int expectedMax;

    public bool Equals(AttackData other)
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
        return Equals(obj as AttackData);
    }
    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}

