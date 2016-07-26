using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CanEditMultipleObjects, CustomEditor(typeof(Star))]
public class StarInspector : Editor {

    private static GUIContent
        insertContent = new GUIContent("+", "duplicate this point"),
        deleteContent = new GUIContent("-", "delete this point"),
        pointContent = GUIContent.none,
        teleportContent = new GUIContent("T");

    private static GUILayoutOption
        buttonWidth = GUILayout.MaxWidth(20f),
        colorWidth = GUILayout.MaxWidth(50f);

    private SerializedObject star;
    private SerializedProperty
        points,
        frequency,
        centerColor;

    private int teleportingElement;
    void OnEnable()
    {
        star = new SerializedObject(targets);
        points = star.FindProperty("points");
        frequency = star.FindProperty("frequency");
        centerColor = star.FindProperty("centerColor");

        teleportingElement = -1;
        teleportContent.tooltip = "start teleporting this point";
    }
    public override void OnInspectorGUI()
    {
        star.Update();

        GUILayout.Label("Points");
        for (int i = 0; i < points.arraySize; i++)
        {
            SerializedProperty
                point = points.GetArrayElementAtIndex(i),
                offset = points.FindPropertyRelative("offset");
            if(offset == null)
            {
                break;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(offset, pointContent);
            EditorGUILayout.PropertyField(point.FindPropertyRelative("color"), pointContent, colorWidth);

            if (GUILayout.Button(teleportContent, EditorStyles.miniButtonLeft, buttonWidth))
            {
                if (teleportingElement >= 0)
                {
                    points.MoveArrayElement(teleportingElement, i);
                    teleportingElement = -1;
                    teleportContent.tooltip = "start teleporting this point";
                }
            }
            if (GUILayout.Button(insertContent, EditorStyles.miniButtonMid, buttonWidth))
            {
                points.InsertArrayElementAtIndex(i);
            }
            if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
            {
                points.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }
        if (teleportingElement >= 0)
        {
            GUILayout.Label("teleporting point " + teleportingElement);
        }

        EditorGUILayout.PropertyField(frequency);
        EditorGUILayout.PropertyField(centerColor);

        if (star.ApplyModifiedProperties() || (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed"))
        {
            foreach (Star s in targets)
            {
                if (PrefabUtility.GetPrefabType(s) != PrefabType.Prefab)
                {
                    s.UpdateStar();
                }
            }
        }
    }
}
