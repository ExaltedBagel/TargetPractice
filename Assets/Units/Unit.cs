using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class Unit : UnitBase
{
    /*************************************/
    //Basis
    /************************************/
    public Unit():base()
    {
        
    }   
    //CALL THIS FUNCTION TO CREATE A UNIT FROM DATA
    //MEANT FOR THE CARDS TO CALL
    public override void init(UnitData uData)
    {
        base.init(uData);
    }

    

}
