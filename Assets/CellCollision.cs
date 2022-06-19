using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCollision : MonoBehaviour
{
    Collider2D colid2D;
    Vector2 direction;
    bool canPush;
    // Start is called before the first frame update
    void Start()
    {
        if (!colid2D) colid2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }
    */
    public void Generating(int surroundings, float time)
    {
        direction = Vector2.zero;
        canPush = true;
        if (surroundings % 2 == 1) direction.x = -1;
        if(surroundings % 4 >= 2)
        {
            if (direction.x == 0) direction.x = 1;
            else canPush = false;
        }
        if (surroundings % 8 >= 4) direction.y = -1;
        if(surroundings >= 8)
        {
            if (direction.y == 0) direction.y = 1;
            else canPush = false;
        }
        else if (direction == Vector2.zero) canPush = false;
        StartCoroutine(ColliderOn(time));
    }

    public void Deleting(int surroundings, float time)
    {
        StartCoroutine(ColliderOff(time));
    }

    IEnumerator ColliderOn(float time)
    {
        if (!colid2D) colid2D = GetComponent<Collider2D>();
        colid2D.enabled = false;
        colid2D.isTrigger = true;
        yield return new WaitForSeconds(0.1f * time);
        colid2D.enabled = true;
        yield return new WaitForSeconds(0.9f * time);
        colid2D.isTrigger = false;
    }

    IEnumerator ColliderOff(float time)
    {
        yield return new WaitForSeconds(0.1f * time);
        colid2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canPush) StageManager.player.Push(direction.normalized);
    }
}
