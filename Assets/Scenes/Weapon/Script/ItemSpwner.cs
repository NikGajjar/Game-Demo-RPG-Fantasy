
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string itemName;
        public Transform spawnPoint;
        public GameObject pickupPrefab;
        public ItemData itemData;
        public int quantity = 1;
    }

    [SerializeField] private SpawnEntry[] spawnEntries;

    private void Start()
    {
        Debug.Log("ItemSpawner Started!");

        foreach (SpawnEntry entry in spawnEntries)
        {
            if (entry.spawnPoint == null ||entry.pickupPrefab == null || entry.itemData == null)
            {
                Debug.LogError("Missing reference for: " + entry.itemName);
                continue;
            }

            GameObject spawned = Instantiate( entry.pickupPrefab,entry.spawnPoint.position, entry.spawnPoint.rotation);

            ItemPickup pickup = spawned.GetComponent<ItemPickup>();

            if (pickup != null)
            {
                pickup.SetItem(entry.itemData, entry.quantity);
                Debug.Log(entry.itemName + " spawned successfully!");
            }
            else
            {
                Debug.LogError(entry.itemName +
                    " prefab is missing ItemPickup component!");
            }
        }
    }
}
