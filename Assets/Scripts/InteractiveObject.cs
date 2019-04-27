using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public string[] RequiredItems;

    public bool CanInteract(List<GameObject> inventory)
    {
        bool can_interact = true;
        foreach(var item in RequiredItems)
        {
            can_interact =
                inventory.Exists(i => i.name.Contains(item));
        }

        return can_interact;
    }

    public void Interact()
    {
        Debug.Log("YOU HAVE EVERYTHING I NEEED");
    }
}
