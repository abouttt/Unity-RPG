using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviourSingleton<CooldownManager>
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _completedCooldownQueue = new();

    private void LateUpdate()
    {
        UpdateCooldowns();
    }

    public static void UpdateCooldowns()
    {
        var instance = Instance;
        float deltaTime = Time.deltaTime;

        foreach (var cooldown in instance._cooldowns)
        {
            cooldown.UpdateCooldown(deltaTime);
            if (cooldown.RemainingTime <= 0f)
            {
                instance._completedCooldownQueue.Enqueue(cooldown);
            }
        }

        while (instance._completedCooldownQueue.Count > 0)
        {
            var cooldown = instance._completedCooldownQueue.Dequeue();
            instance._cooldowns.Remove(cooldown);
        }
    }

    public static void AddCooldown(Cooldown cooldown)
    {
        if (cooldown == null)
        {
            return;
        }

        if (cooldown.RemainingTime <= 0f)
        {
            return;
        }

        Instance._cooldowns.Add(cooldown);
    }

    public static void Clear()
    {
        var instance = Instance;

        foreach (var cooldown in instance._cooldowns)
        {
            cooldown.Clear();
        }

        instance._cooldowns.Clear();
        instance._completedCooldownQueue.Clear();
    }
}
