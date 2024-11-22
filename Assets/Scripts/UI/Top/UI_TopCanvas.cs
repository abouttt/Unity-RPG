using UnityEngine;

public class UI_TopCanvas : UI_View
{
    protected override void Init()
    {
        UIManager.Register(this);
    }
}
