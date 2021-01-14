using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GInventory 
{
    public List<GameObject> items = new List<GameObject>(); //new list of gameobject items

    //adds a gameobject to the agents inventory 
    public void AddItem(GameObject i)
    {
        items.Add(i);

    }

    //search inventory for an item
    public GameObject FindItemWithTag( string tag)
    {
        foreach(GameObject i in items)
        {
            if (i == null)
                break;

            if(i.tag == tag)
            {
                return i;
            }
        }
        return null;
    }

    //remove item from inventory
    public void RemoveItem(GameObject i)
    {
        int indexToRemove = -1;
        foreach(GameObject g in items)
        {
            indexToRemove++;
                if (g == i)
                break;
        }
        if (indexToRemove >= -1)
            items.RemoveAt(indexToRemove);
    }

    public GameObject RemoveandDestoryItem(GameObject i)
    {
        GameObject remove = null;
        int indexToRemove = -1;
        foreach (GameObject g in items)
        {
            indexToRemove++;
            if (g == i)
                remove = g;
                break;
        }
        if (indexToRemove >= -1)
            items.RemoveAt(indexToRemove);
        return remove;
    }
}
