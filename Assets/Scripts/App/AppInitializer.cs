using UnityEngine;

public static class AppInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Settings.Initialize();
        Databases.Initialize();
        Managers.Initialize();
    }
}
