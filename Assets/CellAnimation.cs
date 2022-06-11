using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellAnimation : MonoBehaviour
{
    [System.NonSerialized] public int surroundingsMode;
    Material material;

    private void OnEnable()
    {
        if (!material) material = GetComponent<SpriteRenderer>().material;
        if (surroundingsMode % 2 == 1) material.SetInt("_Right", 1);
        if (surroundingsMode % 4 >= 2) material.SetInt("_Left", 1);
        if (surroundingsMode % 8 >= 4) material.SetInt("_Up", 1);
        if (surroundingsMode >= 8) material.SetInt("_Down", 1);
        material.SetFloat("_Value", 1);
        material.DOFloat(0, "_Value", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
