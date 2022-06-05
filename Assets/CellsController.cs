using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsController : MonoBehaviour
{
    [SerializeField] private FirstCellsPlacementData placement;
    [SerializeField] private SpriteRenderer cellPrefab;
    SpriteRenderer[,] cells;
    // Start is called before the first frame update
    void Start()
    {
        bool[,] cellsBool = placement.To2DArray();
        cells = new SpriteRenderer[cellsBool.GetLength(0), cellsBool.GetLength(1)];
        cellPrefab.gameObject.SetActive(true);
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                if(cellsBool[j,i]) cells[j, i] = Instantiate<SpriteRenderer>(cellPrefab,
                     new(j - cells.GetLength(1) * 0.5f, -i + cells.GetLength(0) * 0.5f), Quaternion.identity);
            }
        }
        //StartCoroutine(SetOtherSells());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int CellCheck(Vector2Int pos)
    {
        Vector2Int min = -Vector2Int.one;
        Vector2Int max = Vector2Int.one;
        if (pos.x <= 0) min.x = 0;
        else if (pos.x >= cells.GetLength(0)) max.x = 0;
        if (pos.y <= 0) min.y = 0;
        else if (pos.y >= cells.GetLength(1)) max.y = 0;
        int num = 0;
        for (int i = min.y; i < max.y; i++)
        {
            for (int j = min.x; j < max.x; j++)
            {
                if ((j != 0 || i != 0) && cells[pos.x + j,pos.y + i] && cells[pos.x + j, pos.y + i].gameObject.activeSelf) num++;
            }
        }
        return num;
    }

    /*IEnumerator SetOtherSells()
    {
        cellPrefab.gameObject.SetActive(false);
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                if (!cells[j, i]) cells[j, i] = Instantiate<SpriteRenderer>(cellPrefab,
                       new(j - cells.GetLength(1) * 0.5f, -i + cells.GetLength(0) * 0.5f), Quaternion.identity);
            }
        }
        yield return null;
    }*/
}