using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class SceneReferenceAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // **型チェック**
        if (property.propertyType != SerializedPropertyType.String)
        {
            // エラーを表示
            EditorGUI.LabelField(position, label.text, "[SceneReference] は string にのみ適用可能！");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // 現在のシーンアセットを取得
        SceneAsset sceneAsset = null;
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        }

        // シーンアセットの選択フィールドを表示
        SceneAsset newScene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false);

        // 選択されたシーンのパスを保存
        if (newScene != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(newScene);
            if (scenePath.EndsWith(".unity"))
            {
                property.stringValue = scenePath;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
[AttributeUsage(AttributeTargets.Class)]
public class AutoSingletonAttribute : Attribute { }

public static class AutoSingletonInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSingletons()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.GetCustomAttribute<AutoSingletonAttribute>() != null && typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                if (type.GetField("Instance", BindingFlags.Static | BindingFlags.Public) != null)
                {
                    GameObject singletonObject = new GameObject(type.Name);
                    MonoBehaviour instance = singletonObject.AddComponent(type) as MonoBehaviour;
                    type.GetField("Instance", BindingFlags.Static | BindingFlags.Public).SetValue(null, instance);
                    UnityEngine.Object.DontDestroyOnLoad(singletonObject);
                }
            }
        }
    }
}

