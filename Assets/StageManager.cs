using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] PlayerController playerCharactor;
    public static PlayerController player;
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
