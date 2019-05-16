using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggToolKit : MonoBehaviour
{

    public BaseUnit unit;
    
    // Start is called before the first frame update
    void Start()
    {
        this.unit = gameObject.GetComponent<BaseUnit>();
        this.AddListener();
        this.unit.BeAttacked();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddListener()
    {
        this.unit.onSubHp += this.onSubHp;
    }

    void RemoveListener()
    {
        this.unit.onSubHp -= new BaseUnit.SubHpHandler(this.onSubHp);
    }
    
    void onSubHp(BaseUnit source, float harmNumber, DamageType damageType, HpShowType hpShowType)
    {
        Debug.Log("EggToolKit:" + source.IsHero); 
    }
        
}
