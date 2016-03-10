using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int attackNum = 0;
    
    void OnEnable()
    {
        attackNum = AtkHUD.getNActive();
    }    

    public void OnPointerEnter(PointerEventData pointer)
    {        
        if(!UnitHandler.unit1.moving && !UnitAttackHandler.atkReady)
            UnitAttackHandler.showCircle(attackNum);
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        if(!UnitAttackHandler.atkReady)
            UnitHandler.unit1.toggleAtkCircle(false);
    }
}
