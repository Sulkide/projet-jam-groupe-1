using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] private ItemDatabase database;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (database != null) database.RebuildLookup();
        else Debug.LogWarning("[ItemFactory] Aucune base de données assignée.");
    }

    public ItemInstance Create(string id, int quantity = 1)
    {
        if (database == null)
        {
            Debug.LogWarning("[ItemFactory] Pas de database.");
            return null;
        }

        var def = database.GetById(id);
        if (def == null)
        {
            Debug.LogWarning("[ItemFactory] Id introuvable: " + id);
            return null;
        }

        if (quantity < 1) quantity = 1;
        if (!def.Stackable) quantity = 1;

        return new ItemInstance(def, quantity);
    }
}