using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsController : MonoBehaviour
{
    [SerializeField] private FirstCellsPlacementData placement;
    [SerializeField] private SpriteRenderer cellPrefab;
    List<SpriteRenderer> cellsInPool = new List<SpriteRenderer>();
    SpriteRenderer[,] cells;
    int[,] nextCells;
    // Start is called before the first frame update
    void Start()
    {
        bool[,] cellsBool = placement.To2DArray();
        cells = new SpriteRenderer[cellsBool.GetLength(0), cellsBool.GetLength(1)];
        nextCells = new int[cellsBool.GetLength(0), cellsBool.GetLength(1)];
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
        StartCoroutine(GetNextCells());
        InvokeRepeating("CellsUpdate", 0.5f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CellsUpdate();
        }
    }

    void CellsUpdate()
    {
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                if (nextCells[j, i] > 0)
                {
                    if (cellsInPool.Count > 0)
                    {
                        cells[j, i] = cellsInPool[0];
                        cells[j, i].transform.position = new(j - cells.GetLength(1) * 0.5f, -i + cells.GetLength(0) * 0.5f);
                        cells[j, i].gameObject.SetActive(true);
                        cellsInPool.RemoveAt(0);
                    }
                    else
                    {
                        cells[j, i] = Instantiate<SpriteRenderer>(cellPrefab,
                            new(j - cells.GetLength(1) * 0.5f, -i + cells.GetLength(0) * 0.5f), Quaternion.identity);
                    }
                    nextCells[j, i] = 0;
                }
                else if (nextCells[j, i] < 0)
                {
                    cellsInPool.Add(cells[j, i]);
                    cells[j, i].gameObject.SetActive(false);
                    cells[j, i] = null;
                    nextCells[j, i] = 0;
                }
            }
        }
        StartCoroutine(GetNextCells());
    }

    int CellCheck(Vector2Int pos)
    {
        Vector2Int min = -Vector2Int.one;
        Vector2Int max = Vector2Int.one;
        if (pos.x <= 0) min.x = 0;
        else if (pos.x >= cells.GetLength(0) - 1) max.x = 0;
        if (pos.y <= 0) min.y = 0;
        else if (pos.y >= cells.GetLength(1) - 1) max.y = 0;
        int num = 0;
        for (int i = min.y; i <= max.y; i++)
        {
            for (int j = min.x; j <= max.x; j++)
            {
                if ((j != 0 || i != 0) && cells[pos.x + j,pos.y + i] && cells[pos.x + j, pos.y + i].gameObject.activeSelf) num++;
            }
        }
        return num;
    }

    int CellCheck(int posx,int posy)
    {
        Vector2Int min = -Vector2Int.one;
        Vector2Int max = Vector2Int.one;
        if (posx <= 0) min.x = 0;
        else if (posx >= cells.GetLength(0) - 1) max.x = 0;
        if (posy <= 0) min.y = 0;
        else if (posy >= cells.GetLength(1) - 1) max.y = 0;
        int num = 0;
        for (int i = min.y; i <= max.y; i++)
        {
            for (int j = min.x; j <= max.x; j++)
            {
                if ((j != 0 || i != 0) && cells[posx + j, posy + i] && cells[posx + j, posy + i].gameObject.activeSelf) num++;
            }
        }
        return num;
    }

    IEnumerator GetNextCells()
    {
        //nextCells = new int[cells.GetLength(0), cells.GetLength(1)];
        cellPrefab.gameObject.SetActive(false);
        int increaseCellsCount = 0;
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                int count = CellCheck(j, i);
                if (cells[j, i])
                {
                    if (count < 2 || count > 3) nextCells[j, i] = -1;
                }
                else if(count == 3)
                {
                    increaseCellsCount++;
                    nextCells[j, i] = 1;
                    if(cellsInPool.Count < increaseCellsCount) cellsInPool.Add(Instantiate<SpriteRenderer>(cellPrefab));
                }
            }
        }
        yield return null;
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
