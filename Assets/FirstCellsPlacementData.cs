using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(FirstCellsPlacementData))]
public class CellsPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        FirstCellsPlacementData data = target as FirstCellsPlacementData;
        int sqrt = (int)Mathf.Sqrt(data.cellsArray.Length);
        bool[,] array2D = new bool[sqrt,sqrt];
        for (int i = 0; i < data.cellsArray.Length; i++)
        {
            array2D[Mathf.FloorToInt(i / array2D.GetLength(0)), i % array2D.GetLength(0)] = data.cellsArray[i];
        }
        EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                int originLength = array2D.GetLength(0);
                int arrayLength = EditorGUILayout.DelayedIntField("size", originLength);
                bool[,] copiedArray = new bool[arrayLength, arrayLength];
                if(arrayLength > originLength)
                {
                    for (int i = 0; i < originLength; i++)
                    {
                        for (int j = 0; j < originLength; j++)
                        {
                            copiedArray[j, i] = array2D[j, i];
                        }
                    }
                    array2D = copiedArray;
                    data.cellsArray = new bool[arrayLength * arrayLength];
                }
                else if (arrayLength < originLength)
                {
                    for (int i = 0; i < arrayLength; i++)
                    {
                        for (int j = 0; j < arrayLength; j++)
                        {
                            copiedArray[j, i] = array2D[j, i];
                        }
                    }
                    array2D = copiedArray;
                    data.cellsArray = new bool[arrayLength * arrayLength];
                }
            }
            for (int i = 0; i < array2D.GetLength(1); i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = 0; j < array2D.GetLength(0); j++)
                    {
                        array2D[j, i] = EditorGUILayout.Toggle(array2D[j, i],GUILayout.MaxWidth(16),GUILayout.MaxHeight(16),GUILayout.MinWidth(4), GUILayout.MinWidth(4));
                    }
                }
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < data.cellsArray.Length; i++)
            {
                data.cellsArray[i] = array2D[Mathf.FloorToInt(i / array2D.GetLength(0)), i % array2D.GetLength(0)];
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif

[CreateAssetMenu(menuName ="CellsPlacement")]
public class FirstCellsPlacementData : ScriptableObject
{
    public bool[] cellsArray = new bool[36];

    public bool[,] To2DArray()
    {
        int sqrt = (int)Mathf.Sqrt(cellsArray.Length);
        bool[,] array2D = new bool[sqrt, sqrt];
        for (int i = 0; i < cellsArray.Length; i++)
        {
            array2D[Mathf.FloorToInt(i / array2D.GetLength(0)), i % array2D.GetLength(0)] = cellsArray[i];
        }
        return array2D;
    }
}
