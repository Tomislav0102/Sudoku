using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FirstCollection;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform parGrid;
    Tile[,] _tiles = new Tile[9, 9];
    bool CanFit(int val, Vector2Int tilePos)
    {
        //column
        for (int i = 0; i < tilePos.y; i++)
        {
            if (_tiles[tilePos.x, i].NumValue == val)
            {
              //  print("Column");
                return false;
            }
        }

        //row
        for (int i = 0; i < tilePos.x; i++)
        {
            if (_tiles[i, tilePos.y].NumValue == val)
            {
              //  print("Row");
                return false;
            }
        }

        //3x3
        int px = tilePos.x / 3;
        switch (px)
        {
            case 1:
                px = 3;
                break;
            case 2:
                px = 6;
                break;
        }
        int py = tilePos.y / 3;
        switch (py)
        {
            case 1:
                py = 3;
                break;
            case 2:
                py = 6;
                break;
        }
        for (int j = py; j < 3 + py; j++)
        {
            for (int i = px; i < 3 + px; i++)
            {
                if (i == tilePos.x && j == tilePos.y) continue;

                if (_tiles[i, j].NumValue == val)
                {
                  //  print($"duplicate tile in position {new Vector2Int(i, j)}");
                    return false;
                }
            }
        }

        return true;
    }
    int GetOrdinal(Vector2Int position)
    {
        return position.x + position.y * 9;
    }

    int _counterGridGeneration;
    private void Awake()
    {
        int counter = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                _tiles[i, j] = parGrid.GetChild(counter).GetComponent<Tile>();
                _tiles[i, j].Ini(this, new Vector2Int(i, j), GetOrdinal(new Vector2Int(i, j)));
                counter++;
            }
        }
    }

    private void Start()
    {
        Stopwatch sw = Stopwatch.StartNew();
        GenerateGrid(Vector2Int.zero);
        print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds}");
    }

    void GenerateGrid(Vector2Int startPos)
    {
        _counterGridGeneration++;

        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i >= startPos.x && j >= startPos.y) _tiles[i, j].NumValue = -1;
            }
        }


        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {

                Vector2Int currentPos = new Vector2Int(i, j);
                List<int> chosen = HelperScript.RandomList(9);
                int val = 0;
                bool canFit = false;
                HashSet<Tile> duplicates = new HashSet<Tile>();
                for (int k = 0; k < chosen.Count; k++)
                {
                    val = chosen[k];
                    if (!CanFit(val, currentPos))
                    {
                        duplicates.Add(_tiles[i, j]);
                    }
                    else
                    {
                        canFit= true;
                        break;
                    }
                }

                if (canFit) _tiles[i, j].NumValue = val;
                else
                {
                    int ordinal = 10000;
                    Vector2Int target = Vector2Int.zero;
                    foreach (Tile item in duplicates)
                    {
                        if (ordinal > item.ordinal)
                        {
                            ordinal = item.ordinal;
                            target = item.poz;
                        }
                    }
                    GenerateGrid(target);
                    return;
                }
            }
        }

    }

    private void Reset()
    {
        _counterGridGeneration = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                _tiles[i, j].NumValue = -1;
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            Stopwatch sw = Stopwatch.StartNew();
            GenerateGrid(Vector2Int.zero);
            print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds}");

        }
    }
}
