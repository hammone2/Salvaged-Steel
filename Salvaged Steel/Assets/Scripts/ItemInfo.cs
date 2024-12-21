using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenCover.Framework.Model;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private Image banner;
    public bool selected = false;
    public enum ItemType
    {
        Gun,
        Propulsion,
        Turret
    }
    private ItemType itemType;

    private void Update()
    {
        if (selected == false)
            return;
        transform.rotation = Quaternion.Euler(30, 0, 0);

        Transform parentObject = transform.parent;
        transform.position = parentObject.position + Vector3.up * 4;
    }

    public void Initialize(Color rarityColor, GameObject item)
    {
        // Try to get the components from the GameObject
        var partObject = item.GetComponent<PartObject>();
        var gun = item.GetComponent<Gun>();
        var propulsion = item.GetComponent<Propulsion>();
        var turret = item.GetComponent<Turret>();

        // Switch statement based on which component is found
        switch (true)
        {
            case bool _ when gun != null:
                itemType = ItemType.Gun;
                break;

            case bool _ when propulsion != null:
                itemType = ItemType.Propulsion;
                break;

            case bool _ when turret != null:
                itemType = ItemType.Turret;
                break;

            default:
                Debug.LogWarning("Unknown item type");
                break;
        }

        banner.color = rarityColor;
        selected = true;
    }
}
