using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Items/Weapon", fileName = "NewWeapon")]
public class WeaponDefinition : ItemDefinition
{
    public EquipmentSlot Slot = EquipmentSlot.Weapon;

    [Header("Weapon Stats (exactement celles demandées)")]
    public int Damage;                
    public int maxMagazin;            
    public int currentMagazin;        
    public float maxPressureLevel;    
    public float currentPressureLevel;
    public int reloadTime;            
    public float ergonomy;            

    private void OnValidate()
    {
        Type = ItemType.Weapon;
        Stackable = false;
        MaxStack = 1;
        
        if (maxMagazin < 0) maxMagazin = 0;
        if (currentMagazin < 0) currentMagazin = 0;
        if (currentMagazin > maxMagazin) currentMagazin = maxMagazin;
        if (maxPressureLevel < 0f) maxPressureLevel = 0f;
        if (currentPressureLevel < 0f) currentPressureLevel = 0f;
        if (currentPressureLevel > maxPressureLevel) currentPressureLevel = maxPressureLevel;
        if (reloadTime < 0) reloadTime = 0;
        if (ergonomy < 0f) ergonomy = 0f;
    }
}