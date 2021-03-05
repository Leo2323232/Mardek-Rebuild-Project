﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateSOWindow : EditorWindow
{
    static List<Type> types;
    static SerializedProperty property;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (types != null)
        {
            foreach (Type t in types)
            {

                // TODO: check abstract class
                if (GUILayout.Button(new GUIContent(t.Name)))
                {
                    ScriptableObject newSO = CreateInstance(t);
                    newSO.name = "(Instance)" + newSO.name;
                    property.objectReferenceValue = newSO;
                    if (property.objectReferenceValue == null)
                    {
                        // couldn't hold SO instance, save it to a .asset
                        Debug.LogWarning("An asset can't hold a reference to a non-asset instance (Type Mismatch), saving the created object as asset first");
                        SaveSOToAsset(newSO);
                        property.objectReferenceValue = newSO;
                    }
                    property.serializedObject.ApplyModifiedProperties();
                    this.Close();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    public static void SaveSOToAsset(ScriptableObject so)
    {
        string path = EditorUtility.SaveFilePanelInProject(so.name, $"New {so.GetType().Name}", "asset", "save scriptable object as asset");
        if (path.Length != 0)
        {
            AssetDatabase.CreateAsset(so, path);
        }
    }

    public static void Open(Type type, ref SerializedProperty serializedProperty)
    {
        if (type == typeof(ScriptableObject))
        {
            Debug.LogError("Creation of type 'ScriptableObject' not implemented");
        }
        else
        {
            CreateSOWindow window = ScriptableObject.CreateInstance<CreateSOWindow>();
            window.ShowUtility();
            property = serializedProperty;
            types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(type);
            types.Add(type);
        }
    }
}

public static class ReflectionHelpers
{
    public static List<Type> GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        var assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);                
            }
        }
        return result;
    }
}