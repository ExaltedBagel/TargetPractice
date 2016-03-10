using System;
using System.Collections.Generic;
using UnityEngine;

//Will be used to calculate results of operations
static public class CMath
{
    //Calculate damage when damage is not known
    public static int calculateDamage(UnitBase attacker, UnitBase defender, Attack attack)
    {
        float attackFactor = 1f;
        float defenceFactor = 1f;

        float damage = rollDamage(attack.roll, attack.dmg, attack.nDice);

        foreach(DamageType x in attack.getTypes())
        {
            attackFactor *= attacker.typeAtk[(int)x];
            defenceFactor *= defender.typeDef[(int)x];
        }

        return (int)(damage *= (attackFactor/defenceFactor));
    }

    //Calculate damage when damage is already known
    public static int calculateDamage(Unit attacker, Unit defender, Attack attack, int dmg)
    {
        float attackFactor = 1f;
        float defenceFactor = 1f;

        foreach (DamageType x in attack.getTypes())
        {
            attackFactor *= attacker.typeAtk[(int)x];
            defenceFactor *= defender.typeDef[(int)x];
        }

        return (int)((float)dmg *(attackFactor / defenceFactor));
    }

    public static float rollDamage(int rate, int dmg, int nDices)
    {
        float total = 0f;
        for(int i = 0; i < nDices; i++)
        {
            int success = UnityEngine.Random.Range(0, 100);

            if(success < rate)
            {
                total += (float)dmg;
            }
        }
        return total;
    }

    public static float spellRange(int impulse)
    {
        float result = (float)impulse;
        result = Mathf.Sqrt(result);
        return result;
    }
}

