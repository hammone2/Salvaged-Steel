using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [Header("Item Screens")]
    [SerializeField] private GameObject gunInfo;
    [SerializeField] private GameObject propulsionInfo;
    [SerializeField] private GameObject turretInfo;

    [Header("Gun Stats")]
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI fireRate;
    [SerializeField] private TextMeshProUGUI ammo;

    [Header("Propulsion Stats")]
    [SerializeField] private TextMeshProUGUI propHealth;
    [SerializeField] private TextMeshProUGUI moveSpeed;

    [Header("Turret Stats")]
    [SerializeField] private TextMeshProUGUI turretHealth;
    [SerializeField] private TextMeshProUGUI rotateSpeed;

    [Header("Other Stuff")]
    [SerializeField] private Image banner;
    [SerializeField] private TextMeshProUGUI nameText;
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
        var health = item.GetComponent<HealthComponent>();
        var gun = item.GetComponent<Gun>();
        var propulsion = item.GetComponent<Propulsion>();
        var turret = item.GetComponent<Turret>();

        // Switch statement based on which component is found
        switch (true)
        {
            case bool _ when gun != null:
                //itemType = ItemType.Gun;
                gunInfo.SetActive(true);
                damage.text = "Damage: " + gun.damage;
                fireRate.text = "Fire Rate: " + gun.fireRate;
                ammo.text = "Ammo: " + gun.ammo;
                break;

            case bool _ when propulsion != null:
                //itemType = ItemType.Propulsion;
                propulsionInfo.SetActive(true);
                propHealth.text = "Health: " + health.health;
                moveSpeed.text = "Speed: " + propulsion.moveSpeed;
                break;

            case bool _ when turret != null:
                //itemType = ItemType.Turret;
                turretInfo.SetActive(true);
                turretHealth.text = "Health: " + health.health;
                rotateSpeed.text = "Rotation Speed: " + turret.rotationSpeed;
                break;

            default:
                Debug.LogWarning("Unknown item type");
                break;
        }

        banner.color = rarityColor;
        nameText.text = item.name;
        selected = true;
    }
}
