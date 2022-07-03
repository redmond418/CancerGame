using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    [SerializeField] ParticleSystem parti;
    [SerializeField] Collider2D colid2D;
    [SerializeField] Renderer rend0;
    [SerializeField] Renderer rend1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            rend0.enabled = false;
            rend1.enabled = false;
            colid2D.enabled = false;
            parti.Play();
            stageManager.Goal();
        }
    }
}
