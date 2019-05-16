using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    
    public delegate void SubHpHandler(BaseUnit source, float subHp, DamageType damageType, HpShowType showType);
    public event SubHpHandler onSubHp;

    public void BeAttacked()
    {
        float possibility = UnityEngine.Random.value;
        bool isCritical = UnityEngine.Random.value > 0.5f;
        bool isMiss = UnityEngine.Random.value > 0.5f;
        float harmNumber = 10000f;

        OnBeAttacked(harmNumber, isCritical, isMiss);
    }

    protected virtual void OnBeAttacked(float harmNumber, bool isCritical, bool isMiss)
    {
        DamageType damageType = DamageType.Normal;
        HpShowType hpShowType = HpShowType.Damage;

        if (isCritical)
            damageType = DamageType.Cirtical;
        if (isMiss)
        {
            hpShowType = HpShowType.Miss;
        }

        if (onSubHp != null)
        {
            Debug.Log("BaseUnit");
            onSubHp(this, harmNumber, damageType, hpShowType);
        }
    }

    public bool IsHero
    {
        get { return true; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
