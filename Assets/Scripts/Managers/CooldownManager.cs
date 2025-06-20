using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CooldownManager : MonoSingleton<CooldownManager>, IManager
{
    private readonly Dictionary<string, float> _cooldowns = new();

    public void Initialize()
    {
    }

    private void LateUpdate()
    {
        Cooling();
    }

    public void ApplyCooldown(ICooldownable cooldown)
    {
        _cooldowns[cooldown.CooldownKey] = cooldown.CooldownDuration;
    }

    public bool IsOnCooldown(ICooldownable cooldown)
    {
        return _cooldowns.TryGetValue(cooldown.CooldownKey, out float remaining) && remaining > 0f;
    }

    public float GetRemainingCooldown(ICooldownable cooldown)
    {
        _cooldowns.TryGetValue(cooldown.CooldownKey, out float remaining);
        return remaining;
    }

    public void Clear()
    {
        _cooldowns.Clear();
    }

    private void Cooling()
    {
        var keys = _cooldowns.Keys.ToList();
        foreach (var key in keys)
        {
            _cooldowns[key] -= Time.deltaTime;
            if (_cooldowns[key] <= 0f)
            {
                _cooldowns.Remove(key);
            }
        }
    }
}
