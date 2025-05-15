using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase Instance;

    public WeaponItem unarmedWeapon;

    [Header("Weapons")]
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

    [Header("QuickSlotItems")]
    [SerializeField] List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();

    //A list of every item we have in the game
    [Header("Items")]
    private List<Item> items = new List<Item>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Add all of our weapons to our list of items
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }

        //Assin all of our items a unique item ID
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }

        foreach (var item in quickSlotItems)
        {
            items.Add(item);
        }
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }

    public QuickSlotItem GetQuickSlotItemByID(int ID)
    {
        return quickSlotItems.FirstOrDefault(item => item.itemID == ID);
    }
}
