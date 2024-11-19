using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviourSingleton<CooldownManager>
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _completedCooldownQueue = new();

    public void Clear()
    {
        foreach (var cooldown in _cooldowns)
        {
            cooldown.Clear();
        }

        _cooldowns.Clear();
        _completedCooldownQueue.Clear();
    }

    public void UpdateCooldowns()
    {
        foreach (var cooldown in _cooldowns)
        {
            cooldown.Update();
            if (cooldown.RemainingTime <= 0f)
            {
                _completedCooldownQueue.Enqueue(cooldown);
            }
        }

        while (_completedCooldownQueue.Count > 0)
        {
            var cooldown = _completedCooldownQueue.Dequeue();
            _cooldowns.Remove(cooldown);
        }
    }

    public void AddCooldown(Cooldown cooldown)
    {
        if (cooldown == null)
        {
            return;
        }

        if (cooldown.RemainingTime <= 0f)
        {
            return;
        }

        _cooldowns.Add(cooldown);
    }
}
