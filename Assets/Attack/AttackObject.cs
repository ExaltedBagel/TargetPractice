using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {

    public AttackObject() { }

    Attack linkedAttack;
    GameObject targetShape;

    private SpellCard linkedCard;
    public SpellCard LinkedCard { get { return linkedCard; } set { linkedCard = value; } }
    public Attack LinkedAttack { get { return linkedAttack; } set { linkedAttack = value; } }

    public void init()
    {
        GameObject instance;
        switch (linkedAttack.attackPatt)
        {
            case AttackPattern.SINGLE:
                //Load sphere shape collider, attach to object, scale accordingly
                instance = (Resources.Load("Attack/TargetSphere") as GameObject);
                targetShape = (GameObject)Instantiate(instance, this.transform.position, Quaternion.identity);
                targetShape.transform.SetParent(this.gameObject.transform);
                targetShape.transform.localScale *= 0.1f;
                break;
            case AttackPattern.BLAST:
                //Load sphere shape collider, attach to object, scale accordingly
                instance = (Resources.Load("Attack/TargetSphere") as GameObject);
                targetShape = (GameObject)Instantiate(instance, this.transform.position, Quaternion.identity);
                targetShape.transform.SetParent(this.gameObject.transform);
                targetShape.transform.localScale *= (linkedAttack as AttackBlastCircle).radius;
                break;
        }
        //Set attack as offensive
        Effector.setLayer(true);
    }
    
    public void drawSpellRange()
    {          
        UnitHandler.currentHero.toggleAtkCircle(linkedAttack.range*CMath.spellRange(UnitHandler.currentHero.impulse));
    }
}
