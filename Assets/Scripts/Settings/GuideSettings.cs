using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Guide Settings", fileName = "GuideSettings")]
public class GuideSettings : ScriptableObject
{
    [field: SerializeField]
    public string ItemSpliteText { get; private set; }

    [field: SerializeField]
    public string DestroyItemText { get; private set; }
}
