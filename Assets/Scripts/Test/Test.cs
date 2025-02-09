using UnityEngine;

public class Test : MonoBehaviour
{
    public ItemData itemData;

    public ItemInventory itemInventory;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            itemInventory.AddItem(itemData, 30);
        }
    }
}
