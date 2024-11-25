using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Guide Settings", fileName = "GuideSettings")]
public class GuideSettings : ScriptableSingleton<GuideSettings>
{
    [field: SerializeField]
    public string ItemSpliteText { get; private set; }

    [field: SerializeField]
    public string DestroyText { get; private set; }
}
