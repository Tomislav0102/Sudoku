using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using System.Diagnostics;
using System;

public class SudokuManager : MonoBehaviour
{
    public int tlOrd, blockOrd, failCol, failRow, failBlock;
    static int[,] grid = new int[9, 9];
    [SerializeField] Transform parGrid;
    readonly Tile[,] _tiles = new Tile[9, 9];
    readonly Tile[] _allTilesByOrdinal = new Tile[81];
    List<int> FreeValues(Vector2Int poz)
    {
        List<int> list = HelperScript.RandomList(9);
        int val = 0;
        //column
        for (int i = 0; i < 9; i++)
        {
            val = _tiles[poz.x, i].NumValue;
            if (list.Contains(val)) list.Remove(val);
        }
        if (list.Count <= 0)
        {
            failCol++;
            return list;
        }
        //row
        for (int i = 0; i < 9; i++)
        {
            val = _tiles[i, poz.y].NumValue;
            if (list.Contains(val)) list.Remove(val);
        }
        if (list.Count <= 0)
        {
            failRow++;
            return list;
        }

        //blocks OVO NIJE DOBRO
        int bv = _tiles[poz.x, poz.y].blockOrdinal;
        for (int i = 0; i < 9; i++)
        {
            val = _allTilesByOrdinal[blocks3x3[bv].ordinals[i]].NumValue;
            if (list.Contains(val)) list.Remove(val);
        }
        if (list.Count <= 0)
        {
            failBlock++;
            return list;
        }
        //  if (list.Count <= 0) print($"tile is {_tiles[poz.x, poz.y]} and block is {bv}");


        //3x3
        //Vector2Int startPos = new Vector2Int((poz.x / 3) * 3, (poz.y / 3) * 3);
        //for (int i = startPos.y; i < startPos.y + 3; i++)
        //{
        //    for (int j = startPos.x; j < startPos.x + 3; j++)
        //    {
        //        val = _tiles[j, i].NumValue;
        //        if (list.Contains(val)) list.Remove(val);
        //    }
        //}

        return list;
    }
    HashSet<Tile> _diagonalUp = new HashSet<Tile>();
    HashSet<Tile> _diagonalDown = new HashSet<Tile>();
    HashSet<int> _fixedOrdinals = new HashSet<int>();

    [System.Serializable]
    public struct Block3x3
    {
        public Color col;
        public List<int> ordinals;
    }
    public Block3x3[] blocks3x3;

    int _counterGridGeneration;
    public bool diagonal;


    private void Awake()
    {
        int counter = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                Tile tl = parGrid.GetChild(counter).GetComponent<Tile>();
                _tiles[i, j] = tl;
                tl.Ini(new Vector2Int(i, j));
                _allTilesByOrdinal[counter] = tl;
                counter++;

                if (i + j == 8)
                {
                    _diagonalDown.Add(tl);
                }
                if (i == j)
                {
                    _diagonalUp.Add(tl);
                }

            }
        }


    }
    #region//NOVO
    void Init(ref int[,] grid)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                grid[i, j] = (i * 3 + i / 3 + j) % 9 + 1;

            }
        }
    }
    void ChangeTwoCell(ref int[,] grid, int findValue1, int findValue2)
    {
        int xParm1, yParm1, xParm2, yParm2;
        xParm1 = yParm1 = xParm2 = yParm2 = 0;
        for (int i = 0; i < 9; i += 3)
        {
            for (int k = 0; k < 9; k += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (grid[i + j, k + z] == findValue1)
                        {
                            xParm1 = i + j;
                            yParm1 = k + z;

                        }
                        if (grid[i + j, k + z] == findValue2)
                        {
                            xParm2 = i + j;
                            yParm2 = k + z;

                        }
                    }
                }
                grid[xParm1, yParm1] = findValue2;
                grid[xParm2, yParm2] = findValue1;
            }
        }
    }

    void UpdateGrid(ref int[,] grid, int shuffleLevel)
    {
        for (int repeat = 0; repeat < shuffleLevel; repeat++)
        {
            ChangeTwoCell(ref grid, UnityEngine.Random.Range(1, 10), UnityEngine.Random.Range(1, 10));
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                _tiles[i, j].NumValue = grid[i, j];

            }
        }

    }
    #endregion
    private void DefineFixedOrdinals()
    {
        _fixedOrdinals.Clear();

        List<int> list = HelperScript.RandomList(9);
        int counter = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == j)
                {
                    _tiles[i, j].NumValue = list[counter];
                    _fixedOrdinals.Add(_tiles[i, j].ordinal);
                    counter++;
                }
            }
        }


        //for (int i = 0; i < 9; i++)
        //{
        //    _allTilesByOrdinal[i].NumValue = i;
        //    _fixedOrdinals.Add(i);
        //}
        //for (int i = 72; i < 81; i++)
        //{
        //    int val = i - 72 + 1;
        //    if (val > 8) val = 0;
        //    _allTilesByOrdinal[i].NumValue = val;
        //    _fixedOrdinals.Add(i);
        //}
    }

    private void Start()
    {
        //Init(ref grid);
        //UpdateGrid(ref grid, 10);


        Stopwatch sw = Stopwatch.StartNew();
        StartCoroutine(Generate(0));
        print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _counterGridGeneration = 0;
            for (int i = 0; i < _allTilesByOrdinal.Length; i++)
            {
                _allTilesByOrdinal[i].NumValue = -1;
            }
            //Init(ref grid);
            //UpdateGrid(ref grid, 10);

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                Generate(0);
                print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");
            }
            catch
            {
                print("erroe");
                Generate(0);
                print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");

            }

        }
    }
    IEnumerator Generate(int ord)
    {
        _counterGridGeneration++;

        for (int i = 0; i < _allTilesByOrdinal.Length; i++)
        {
            if (i >= ord) _allTilesByOrdinal[i].NumValue = -1;
        }

        for (int i = ord; i < _allTilesByOrdinal.Length; i++)
        {
            Tile tl = _allTilesByOrdinal[i];
            List<int> tileValue = FreeValues(tl.poz);
            if (tileValue.Count <= 0)
            {
              //  StartCoroutine(Generate(tl.ordinal /2));
                StartCoroutine(Generate(0));
                //  Generate(0);
                yield break;
            }
            else
            {
                tl.NumValue = tileValue[0];
                yield return null;
            }
        }

    }
}
