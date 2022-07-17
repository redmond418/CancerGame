using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    public static PlayerController player;
    [SerializeField] PlayerController playerCharactor;
    // Start is called before the first frame update
    void Start()
    {
        player = playerCharactor;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/
}
