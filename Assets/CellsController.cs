using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CellsController : MonoBehaviour
{
    class CellComponents
    {
        public SpriteRenderer spriteRenderer;
        public CellCollision cellCollision;
        public CellComponents(SpriteRenderer spri,CellCollision ceco)
        {
            spriteRenderer = spri;
            cellCollision = ceco;
        }
    }
    [SerializeField] private FirstCellsPlacementData placement;
    [SerializeField] private SpriteRenderer cellPrefab;
    [SerializeField] private float firstTime = 0.5f;
    [SerializeField] private float span = 0.3f;
    List<CellComponents> cellsInPool = new List<CellComponents>();
    SpriteRenderer[,] cells;
    CellCollision[,] cellsCollisionSystem;
    int[,] nextCells;
#if UNITY_EDITOR
    readonly Color gizmoColor = new Color(0, 1f, 0.4f, 0.3f);
    bool[] gizmoToCell;
    /*[System.NonSerialized] public */bool[,] gizmo2DArray;

    private void OnDrawGizmos()
    {
        if(gizmo2DArray == null || !gizmoToCell.SequenceEqual(placement.cellsArray))
        {
            gizmoToCell = new bool[placement.cellsArray.Length];
            placement.cellsArray.CopyTo(gizmoToCell,0);
            bool[,] got2D = placement.To2DArray();
            gizmo2DArray = new bool[got2D.GetLength(0),got2D.GetLength(1)];
            Array.Copy(got2D, 0, gizmo2DArray, 0, gizmo2DArray.Length);
        }
        if(gizmo2DArray != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(Vector2.zero, Vector2.one * gizmo2DArray.GetLength(0));
            if (!EditorApplication.isPlaying)
            {
                for (int i = 0; i < gizmo2DArray.GetLength(1); i++)
                {
                    for (int j = 0; j < gizmo2DArray.GetLength(0); j++)
                    {
                        if (gizmo2DArray[j, i]) Gizmos.DrawCube(new(j - gizmo2DArray.GetLength(1) * 0.5f + 0.5f, -i + gizmo2DArray.GetLength(0) * 0.5f - 0.5f), Vector2.one);
                    }
                }
            }
        }
    }
#endif
    // Start is called before the first frame update
    void Start()
    {
        bool[,] cellsBool = placement.To2DArray();
        cells = new SpriteRenderer[cellsBool.GetLength(0), cellsBool.GetLength(1)];
        cellsCollisionSystem = new CellCollision[cellsBool.GetLength(0), cellsBool.GetLength(1)];
        nextCells = new int[cellsBool.GetLength(0), cellsBool.GetLength(1)];
        cellPrefab.gameObject.SetActive(true);
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                if(cellsBool[j,i])
                {
                    cells[j, i] = Instantiate<SpriteRenderer>(cellPrefab,
                     new(j - cells.GetLength(1) * 0.5f + 0.5f, -i + cells.GetLength(0) * 0.5f - 0.5f), Quaternion.identity);
                    cellsCollisionSystem[j, i] = cells[j, i].GetComponent<CellCollision>();
                }
            }
        }
        //StartCoroutine(SetOtherSells());
        StartCoroutine(GetNextCells());
        InvokeRepeating("CellsUpdate", firstTime, span);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            CellsUpdate();
        }*/
    }

    void CellsUpdate()
    {
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                if (nextCells[j, i] > 0)
                {
                    if (cellsInPool.Count(n => !n.spriteRenderer.gameObject.activeSelf) > 0)
                    {
                        CellComponents cellcom = cellsInPool.First(n => !n.spriteRenderer.gameObject.activeSelf);
                        cells[j, i] = cellcom.spriteRenderer;
                        cellsCollisionSystem[j, i] = cellcom.cellCollision;
                        cells[j, i].transform.position = new(j - cells.GetLength(1) * 0.5f + 0.5f, -i + cells.GetLength(0) * 0.5f - 0.5f);
                        Material cellM = cells[j, i].material;
                        int surroundingsMode = nextCells[j, i] - 1;
                        if(surroundingsMode == 0)
                        {
                            cellM.SetInt("_Right", 1);
                            cellM.SetInt("_Left", 1);
                            cellM.SetInt("_Up", 1);
                            cellM.SetInt("_Down", 1);
                            cellM.SetInt("_Reverse", 1);
                            cellM.SetFloat("_Value", 0);
                            cellM.DOFloat(1, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad);
                        }
                        else
                        {
                            cellM.SetInt("_Right", surroundingsMode % 2 == 1 ? 1 : 0);
                            cellM.SetInt("_Left", surroundingsMode % 4 >= 2 ? 1 : 0);
                            cellM.SetInt("_Up", surroundingsMode % 8 >= 4 ? 1 : 0);
                            cellM.SetInt("_Down", surroundingsMode >= 8 ? 1 : 0);
                            cellM.SetInt("_Reverse", 0);
                            cellM.SetFloat("_Value", 1);
                            cellM.DOFloat(0, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad);
                        }
                        cells[j, i].gameObject.SetActive(true);
                        cellsCollisionSystem[j, i].Generating(surroundingsMode, 0.2f);
                        cellsInPool.RemoveAll(n => n.spriteRenderer == cells[j, i]);
                    }
                    else
                    {
                        cells[j, i] = Instantiate<SpriteRenderer>(cellPrefab,
                            new(j - cells.GetLength(1) * 0.5f + 0.5f, -i + cells.GetLength(0) * 0.5f - 0.5f), Quaternion.identity);
                        cellsCollisionSystem[j, i] = cells[j, i].GetComponent<CellCollision>();
                        Material cellM = cells[j, i].material;
                        int surroundingsMode = nextCells[j, i] - 1;
                        if (surroundingsMode == 0)
                        {
                            cellM.SetInt("_Right", 1);
                            cellM.SetInt("_Left", 1);
                            cellM.SetInt("_Up", 1);
                            cellM.SetInt("_Down", 1);
                            cellM.SetInt("_Reverse", 1);
                            cellM.SetFloat("_Value", 0);
                            cellM.DOFloat(1, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad);
                        }
                        else
                        {
                            cellM.SetInt("_Right", surroundingsMode % 2 == 1 ? 1 : 0);
                            cellM.SetInt("_Left", surroundingsMode % 4 >= 2 ? 1 : 0);
                            cellM.SetInt("_Up", surroundingsMode % 8 >= 4 ? 1 : 0);
                            cellM.SetInt("_Down", surroundingsMode >= 8 ? 1 : 0);
                            cellM.SetInt("_Reverse", 0);
                            cellM.SetFloat("_Value", 1);
                            cellM.DOFloat(0, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad);
                        }
                        cellsCollisionSystem[j, i].Generating(surroundingsMode, 0.2f);
                    }
                    nextCells[j, i] = 0;
                }
                else if (nextCells[j, i] < 0)
                {
                    cellsInPool.Add(new CellComponents(cells[j, i], cellsCollisionSystem[j, i]));
                    Material cellM = cells[j, i].material;
                    int surroundingsMode = -nextCells[j, i] - 1;
                    if (surroundingsMode == 0)
                    {
                        cellM.SetInt("_Right", 1);
                        cellM.SetInt("_Left", 1);
                        cellM.SetInt("_Up", 1);
                        cellM.SetInt("_Down", 1);
                        cellM.SetInt("_Reverse", 1);
                        cellM.SetFloat("_Value", 1);
                        GameObject cellObj = cells[j, i].gameObject;
                        cellM.DOFloat(0, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad)
                                .OnComplete(() => cellObj.SetActive(false));
                    }
                    else
                    {
                        cellM.SetInt("_Right", surroundingsMode % 2 == 1 ? 1 : 0);
                        cellM.SetInt("_Left", surroundingsMode % 4 >= 2 ? 1 : 0);
                        cellM.SetInt("_Up", surroundingsMode % 8 >= 4 ? 1 : 0);
                        cellM.SetInt("_Down", surroundingsMode >= 8 ? 1 : 0);
                        cellM.SetInt("_Reverse", 0);
                        cellM.SetFloat("_Value", 0);
                        GameObject cellObj = cells[j, i].gameObject;
                        cellM.DOFloat(1, "_Value", 0.2f)
                                .SetEase(Ease.InOutQuad)
                                .OnComplete(() => cellObj.SetActive(false));
                    }
                    cellsCollisionSystem[j, i].Deleting(0.2f);
                    cells[j, i] = null;
                    cellsCollisionSystem[j, i] = null;
                    nextCells[j, i] = 0;
                }
            }
        }
        StartCoroutine(GetNextCells());
    }

    Vector2Int CellCheck(int posx,int posy)
    {
        Vector2Int min = -Vector2Int.one;
        Vector2Int max = Vector2Int.one;
        if (posx <= 0) min.x = 0;
        else if (posx >= cells.GetLength(0) - 1) max.x = 0;
        if (posy <= 0) min.y = 0;
        else if (posy >= cells.GetLength(1) - 1) max.y = 0;
        int num = 0;
        int mode = 0;
        for (int i = min.y; i <= max.y; i++)
        {
            for (int j = min.x; j <= max.x; j++)
            {
                if ((j != 0 || i != 0) && cells[posx + j, posy + i] && cells[posx + j, posy + i].gameObject.activeSelf)
                {
                    num++;
                    if (i == 0)
                    {
                        if (j == 1) mode += 1;
                        else mode += 2;
                    }
                    if (j == 0)
                    {
                        if (i == -1) mode += 4;
                        else mode += 8;
                    }
                }
            }
        }
        return new(num, mode);
    }

    int SecondCellCheck(int posx, int posy)
    {
        Vector2Int min = -Vector2Int.one;
        Vector2Int max = Vector2Int.one;
        if (posx <= 0) min.x = 0;
        else if (posx >= cells.GetLength(0) - 1) max.x = 0;
        if (posy <= 0) min.y = 0;
        else if (posy >= cells.GetLength(1) - 1) max.y = 0;
        int mode = 0;
        for (int i = min.y; i <= max.y; i++)
        {
            for (int j = min.x; j <= max.x; j++)
            {
                if ((j != 0 || i != 0) && ((cells[posx + j, posy + i] && cells[posx + j, posy + i].gameObject.activeSelf) || 
                    nextCells[posx + j,posy + i] >= 1))
                {
                    if (i == 0)
                    {
                        if (j == 1) mode += 1;
                        else mode += 2;
                    }
                    if (j == 0)
                    {
                        if (i == -1) mode += 4;
                        else mode += 8;
                    }
                }
            }
        }
        return mode;
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
                Vector2Int info = CellCheck(j, i);
                if (cells[j, i])
                {
                    if (info.x < 2 || info.x > 3) nextCells[j, i] = -1;
                }
                else if(info.x == 3)
                {
                    increaseCellsCount++;
                    nextCells[j, i] = info.y + 1;
                    if(cellsInPool.Count < increaseCellsCount)
                    {
                        SpriteRenderer pref = Instantiate<SpriteRenderer>(cellPrefab);
                        cellsInPool.Add(new CellComponents(pref,pref.GetComponent<CellCollision>()));
                    }
                }
            }
        }
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            for (int j = 0; j < cells.GetLength(0); j++)
            {
                int surround = SecondCellCheck(j, i);
                if (nextCells[j, i] <= -1)
                {
                    nextCells[j, i] = -surround - 1;
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
