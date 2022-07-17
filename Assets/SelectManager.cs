using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using DG.Tweening;

public class SelectManager : MonoBehaviour
{
    public class StageInfo
    {
        public string EnglishName, JapaneseName;
        public int stageNumber;
    }
    [SerializeField] SpriteShapeRenderer[] lineRend = new SpriteShapeRenderer[1];
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] StageInfo[] stageInfomation = new StageInfo[1];
    Material[] lineMaterial;
    // Start is called before the first frame update
    void Start()
    {
        lineMaterial = new Material[lineRend.Length];
        for (int i = 0; i < lineRend.Length; i++)
        {
            lineMaterial[i] = lineRend[i].materials[1];
            lineMaterial[i].SetFloat("_Value", 0);
            lineMaterial[i].DOFloat(1f, "_Value", 3f)
                .SetEase(Ease.InOutQuart);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
