using UnityEngine;

public abstract class UI_View : UI_Base
{
    [field: SerializeField]
    public UIType UIType { get; protected set; }

    [field: SerializeField]
    public bool IsValidForUISettings { get; private set; } = true;
}
