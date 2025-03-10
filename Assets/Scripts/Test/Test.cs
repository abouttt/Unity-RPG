using UnityEngine;

public class Test : MonoBehaviour
{
    public ItemData itemData;
    public ItemInventory itemInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            itemInventory.Add(itemData, 30);
        }
    }
}
