using FirstCollection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ManagerNew : MonoBehaviour
{
    [SerializeField] Transform parGrid;
    readonly List<int> _values = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
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
            return list;
        }
        //  if (list.Count <= 0) print($"tile is {_tiles[poz.x, poz.y]} and block is {bv}");

        return list;
    }
    bool CanFit(int num, Vector2Int targetpos)
    {
        //column
        for (int i = 0; i < 9; i++)
        {
            if (_tiles[targetpos.x, i].NumValue == num)
            {
                return false;
            }
        }
        //row
        for (int i = 0; i < 9; i++)
        {
            if (_tiles[i, targetpos.y].NumValue == num)
            {
                return false;
            }
        }
        //diagonal
        //for (int i = 0; i < 9; i++)
        //{
        //    for (int j = 0; j < 9; j++)
        //    {
        //        if (targetpos.x == targetpos.y && i==j && _tiles[i, j].NumValue == num) return false;
        //    }
        //}

        return true;
    }

    [System.Serializable]
    public struct Block3x3
    {
        public Color col;
        public List<int> ordinals;
    }
    public Block3x3[] blocks3x3;

    int _counterGridGeneration;


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
            }
        }
        for (int i = 0; i < 9; i++)
        {
            blocks3x3[i].ordinals = HelperScript.RandomListByType<int>(blocks3x3[i].ordinals);
        }

    }

    private void Start()
    {
        Stopwatch sw = Stopwatch.StartNew();
       // StartCoroutine(Generate(0));
        Generate(0);
        print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");

    }

    void Generate(int startValue)
    {
        for (int i = 0; i < _allTilesByOrdinal.Length; i++)
        {
           if(_allTilesByOrdinal[i].NumValue >= startValue) _allTilesByOrdinal[i].NumValue = -10;
        }
        for (int i = 0; i < 9; i++)
        {
            blocks3x3[i].ordinals = HelperScript.RandomListByType<int>(blocks3x3[i].ordinals);
        }
        _counterGridGeneration++;

        for (int i = startValue; i < _values.Count; i++)
        {
            for (int j = 0; j < blocks3x3.Length; j++)
            {
                bool fits = false;
                for (int k = 0; k < 9; k++)
                {
                    Tile tl = _allTilesByOrdinal[blocks3x3[j].ordinals[k]];
                    if (tl.NumValue < 0 && CanFit(_values[i], tl.poz))
                    {
                        tl.NumValue = _values[i];
                        fits = true;
                      //  yield return null;
                        break;
                    }
                }
                if (!fits)
                {
                    int nVal = i - 1;
                    if (nVal < 0) nVal = 0;
                    //  print(nVal);
                    Generate(nVal);
                    return;
                    //StartCoroutine(Generate(nVal));
                    //yield break;
                }

            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _counterGridGeneration = 0;
            Stopwatch sw = Stopwatch.StartNew();
            Generate(0);
                print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");
        }
    }

}
