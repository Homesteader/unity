using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BaseObj
{
    public virtual bool isHero
    {
        get { return false; }
    }
}

public class Solider : BaseObj
{
    public override bool isHero
    {
        get { return false; }
    }
}

public class Hero : BaseObj
{
    public int id;
    public float currentHp;
    public float maxHp;
    public float attack;
    public float defence;
    
    public Hero()
    {}

    public Hero(int id, float maxHp, float attack, float defence)
    {
        this.id = id;
        this.attack = attack;
        this.currentHp = maxHp;
        this.maxHp = maxHp;
        this.defence = defence;
    }

    public float PowerRank
    {
        get { return 0.5f * maxHp + 0.2f * attack + 0.3f * defence; }
    }

    public override bool isHero
    {
        get { return true; }
    }
}


public class Test : MonoBehaviour
{
    private void Start()
    {
        List<Hero> vHeroList = new List<Hero>();
        vHeroList.Add(new Hero(1,1000f,50f,500f));
        vHeroList.Add(new Hero(2,200f,20f,512f));
        vHeroList.Add(new Hero(2,100f,100f,4090f));
        vHeroList.Add(new Hero(4,460f,60f,500f));
        vHeroList.Add(new Hero(5,50f,10f,511f));
        vHeroList.Add(new Hero(6,210f,50f,50f));

        SortHeros(vHeroList, delegate(Hero hero, Hero hero1)
        {
            int comp = hero.id.CompareTo(hero1.id);
            if (comp == 0)
            {
                return hero.maxHp.CompareTo(hero1.maxHp);
            }
            return comp;
        }, "按英雄Id排序：");
      
      
    }

    public void SortHeros(List<Hero> vHeroes, Comparison<Hero> sortOrder,
        string orderTitle)
    {
        vHeroes.Sort(sortOrder);
        Debug.Log(orderTitle);
        foreach (Hero hero in vHeroes)
        {
            Debug.Log(hero.id + " , " + hero.maxHp + " , " + hero.attack + " , " + hero.defence + " , " + hero.PowerRank);
        }
    }
}
