using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeathPotion : Item
{
    [SerializeField]
    private int healAmount = 5;
    public int HealAmount { 
        get { return healAmount;}
    }
}
