using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class InventoryEntry
{
    public string itemName;
    public int quantity;
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryEntry> entries = new List<InventoryEntry>();
}

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    public Text inventoryText;
    public Button saveButton;
    public Button loadButton;
    public Button addRandomButton;

    [Header("Items disponibles")]
    public List<Item> availableItems;

    private Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/inventory.json";

        if (saveButton) saveButton.onClick.AddListener(SaveInventory);
        if (loadButton) loadButton.onClick.AddListener(LoadInventory);
        if (addRandomButton) addRandomButton.onClick.AddListener(AddRandomItem);

        LoadInventory();
        UpdateUI();
    }

    public void AddItem(Item item, int amount)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += amount;
        else
            inventory[item] = amount;

        Debug.Log($"Ajouté : {item.itemName} x{amount}");
        UpdateUI();
    }

    public void RemoveItem(Item item, int amount)
    {
        if (!inventory.ContainsKey(item)) return;

        inventory[item] -= amount;
        if (inventory[item] <= 0)
            inventory.Remove(item);

        Debug.Log($"Retiré : {item.itemName} x{amount}");
        UpdateUI();
    }

    void AddRandomItem()
    {
        if (availableItems.Count == 0) return;

        Item randomItem = availableItems[Random.Range(0, availableItems.Count)];
        AddItem(randomItem, 1);
    }

    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        foreach (var kvp in inventory)
        {
            saveData.entries.Add(new InventoryEntry
            {
                itemName = kvp.Key.itemName,
                quantity = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Inventaire sauvegardé !");
    }

    public void LoadInventory()
    {
        inventory.Clear();

        if (!File.Exists(savePath))
        {
            Debug.Log("Aucun fichier d’inventaire trouvé.");
            UpdateUI();
            return;
        }

        string json = File.ReadAllText(savePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        foreach (var entry in data.entries)
        {
            Item foundItem = availableItems.Find(i => i.itemName == entry.itemName);
            if (foundItem != null)
                inventory[foundItem] = entry.quantity;
        }

        Debug.Log("Inventaire chargé !");
        UpdateUI();
    }

    void UpdateUI()
    {
        if (!inventoryText) return;

        if (inventory.Count == 0)
        {
            inventoryText.text = "Inventaire vide";
            return;
        }

        string text = "Inventaire :\n";
        foreach (var kvp in inventory)
            text += $"- {kvp.Key.itemName} x{kvp.Value}\n";

        inventoryText.text = text;
    }
}
