using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "UISettings", menuName = "Settings/UI Settings")]
public class UISettings : ScriptableSingleton<UISettings>
{
    [Serializable]
    public class Settings
    {
        [field: SerializeField]
        public bool GraphicRaycaster { get; private set; } = true;

        [field: SerializeField]
        public List<GameObject> Prefabs { get; private set; } = new();
    }

    public Settings this[UIType uiType] => _settings[uiType];

    [SerializeField, SerializedDictionary("UI Type", "Settings")]
    private SerializedDictionary<UIType, Settings> _settings;

#if UNITY_EDITOR
    public void RefreshUIPrefabs()
    {
        if (_settings == null)
        {
            _settings = new();
        }

        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            if (!_settings.ContainsKey(uiType))
            {
                _settings.Add(uiType, new());
            }
        }

        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/UI" }))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab.TryGetComponent<UI_View>(out var view))
            {
                if (view.IsValidForUISettings)
                {
                    _settings[view.UIType].Prefabs.Add(prefab);
                }
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
