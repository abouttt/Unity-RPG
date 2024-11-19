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
        Managers.Cooldown.AddCooldown(this);
        CooldownStarted?.Invoke();
    }

    public void Update()
    {
        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
        }
    }

    public void Clear()
    {
        RemainingTime = 0f;
        CooldownStarted = null;
    }
}
