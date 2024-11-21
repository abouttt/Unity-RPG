using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public event Action CooldownStarted;

    [field: SerializeField]
    public float MaxTime { get; private set; }

    [field: SerializeField]
    public float RemainingTime { get; private set; }

    public void Start()
    {
        RemainingTime = MaxTime;
        CooldownManager.AddCooldown(this);
        CooldownStarted?.Invoke();
    }

    public void Update(float deltaTime)
    {
        RemainingTime = Mathf.Max(RemainingTime - deltaTime, 0);
    }

    public void Clear()
    {
        RemainingTime = 0f;
        CooldownStarted = null;
    }
}
