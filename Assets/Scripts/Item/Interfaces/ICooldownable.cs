using UnityEngine;

public interface ICooldownable
{
    string CooldownKey { get; }
    float CooldownDuration { get; }
}
