using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
//public class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
	public GameObject itemGameObject;

    public abstract void Use();
	public abstract bool getUltiStatus();
    public abstract void timerEnded();
}
