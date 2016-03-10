using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCard : Card
{
    public EquipmentCard() : base() { }

    public override void init(CardData card)
    {
        base.init(card);
        this.type = card.type;
        this.eType = card.eType;
        this.durability = card.durability;
    }
    public override void playCard() { }

    public UnitTypes Type { get { return type; } }
    public EquipType EType { get { return eType; } }
    public int Durablity { get { return durability; } set { durability = Durablity; } }

    public UnitTypes type;
    public EquipType eType;
    public int durability;
}
