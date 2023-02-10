using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FirstCollection;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    bool _korutineSwitch;
    [SerializeField] Transform parGrid;
    readonly Tile[,] _tiles = new Tile[9, 9];
    readonly Tile[] _allTilesByOrdinal = new Tile[81];

    HashSet<Tile> _diagonalUp = new HashSet<Tile>();
    HashSet<Tile> _diagonalDown = new HashSet<Tile>();
    HashSet<Tile> _diagonals = new HashSet<Tile>();
    HashSet<int> _fixedOrdinals = new HashSet<int>();
    int DuplicateOrdinal(UniqueValGroup valGroup, int val, Vector2Int tilePos)
    {
        switch (valGroup)
        {
            case UniqueValGroup.Column:
                for (int i = 0; i < 9; i++)
                {
                    if (_tiles[tilePos.x, i].NumValue == val)
                    {
                        //  print("Column");
                        return _tiles[tilePos.x, i].ordinal;
                    }
                }
                return 1000;

            case UniqueValGroup.Row:
                for (int i = 0; i < 9; i++)
                {
                    if (_tiles[i, tilePos.y].NumValue == val)
                    {
                        //  print("Row");
                        return _tiles[i, tilePos.y].ordinal;
                    }
                }
                return 1000;

            case UniqueValGroup.Block3:
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
                            return _tiles[i, j].ordinal;
                        }
                    }
                }
                return 1000;

            case UniqueValGroup.FixedValues:
                //int ordinal = _tiles[tilePos.x, tilePos.y].ordinal;
                //int FindPreviousOrdinal(int ord)
                //{
                //    int rez = ord - 1;
                //    if (rez <= 0) return 0;
                //    else if (_fixedOrdinals.Contains(rez)) return FindPreviousOrdinal(rez);

                //    return 1000;
                //}
                //if (_fixedOrdinals.Contains(ordinal) && _allTilesByOrdinal[ordinal].NumValue == val) return FindPreviousOrdinal(ordinal);
                return 1000;
        }

        return 1000;
    }

    int _counterGridGeneration;
    [Header("Controls")]
    [SerializeField] bool diagonal;

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

                if ( i + j == 8)
                {
                    _diagonalDown.Add(tl);
                    _diagonals.Add(tl);
                }
                if ( i == j)
                {
                    _diagonalUp.Add(tl);
                    _diagonals.Add(tl);
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            _allTilesByOrdinal[i].NumValue = i;
            _fixedOrdinals.Add(i);
        }
        for (int i = 72; i < 81; i++)
        {
            int val = i - 72 + 1;
            if (val > 8) val = 0;
            _allTilesByOrdinal[i].NumValue = val;
            _fixedOrdinals.Add(i);
        }
    }

    private void Start()
    {
        Stopwatch sw = Stopwatch.StartNew();
        GenerateGrid(0);
        print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");
    }



    void GenerateGrid(int ord)
    {
        _counterGridGeneration++;

        for (int i = 0; i < _allTilesByOrdinal.Length; i++)
        {
            if (i >= ord && !_fixedOrdinals.Contains(i)) _allTilesByOrdinal[i].NumValue = -1;
        }

        for (int i = ord; i < _allTilesByOrdinal.Length; i++)
        {
            if (_fixedOrdinals.Contains(i)) continue;

            Tile tl = _allTilesByOrdinal[i];
            List<int> chosen = HelperScript.RandomList(9);
            int val = 0;
            bool canFit = false;
            int[] dupOrd = new int[4];
            for (int k = 0; k < chosen.Count; k++)
            {
                val = chosen[k];
                canFit = true;
                for (int m = 0; m < dupOrd.Length; m++)
                {
                    dupOrd[m] = DuplicateOrdinal((UniqueValGroup)m, val, tl.poz);
                    if (_fixedOrdinals.Contains(dupOrd[m]))
                    {
                        canFit = false;
                        continue;
                    }
                    if (dupOrd[m] != 1000)
                    {
                        canFit = false;
                        break;
                    }
                }
                if (canFit) break;
            }

            if (canFit) tl.NumValue = val;
            else
            {
                for (int m = 0; m < dupOrd.Length; m++)
                {
                    if (dupOrd[m] != 1000)
                    {
                        //int tar = 0;
                        //if (_allTilesByOrdinal[i].ordinal > 30 && dupOrd[m] < 30)
                        //{
                        //    tar = 30;
                        //}
                        //else tar = dupOrd[m];
                        //GenerateGrid(tar);
                        //return;

                        GenerateGrid(dupOrd[m]);
                        return;
                    }
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
        _fixedOrdinals.Clear();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            Stopwatch sw = Stopwatch.StartNew();
            GenerateGrid(0);
            print($"Counter is {_counterGridGeneration} and elapsed time is {sw.ElapsedMilliseconds} ms.");
        }
    }
}







/*
 
    void GenerateGrid(int ord)
    {
        _counterGridGeneration++;

        for (int i = 0; i < _allTilesByOrdinal.Length; i++)
        {
            if (i >= ord) _allTilesByOrdinal[i].NumValue = -1;
        }

        for (int i = ord; i < _allTilesByOrdinal.Length; i++)
        {
            Tile tl = _allTilesByOrdinal[i];
            List<int> chosen = HelperScript.RandomList(9);
            int val = 0;
            bool canFit = false;
            int[] dupOrd = new int[4];
            for (int k = 0; k < chosen.Count; k++)
            {
                val = chosen[k];
                canFit = true;
                for (int m = 0; m < dupOrd.Length; m++)
                {
                    dupOrd[m] = DuplicateOrdinal((UniqueValGroup)m, val, tl.poz);
                    if (dupOrd[m] != 1000)
                    {
                        canFit = false;
                        break;
                    }
                }
                if (canFit) break;
            }

            if (canFit) tl.NumValue = val;
            else
            {
                for (int m = 0; m < dupOrd.Length; m++)
                {
                    if (dupOrd[m] != 1000)
                    {
                        //int tar = 0;
                        //if (_allTilesByOrdinal[i].ordinal > 30 && dupOrd[m] < 30)
                        //{
                        //    tar = 30;
                        //}
                        //else tar = dupOrd[m];
                        //GenerateGrid(tar);
                        //return;

                        GenerateGrid(dupOrd[m]);
                        return;
                    }
                }
            }
        }
    }


    int DuplicateOrdinal(UniqueValGroup valGroup, int val, Vector2Int tilePos)
    {
        switch (valGroup)
        {
            case UniqueValGroup.Column:
                break;
            case UniqueValGroup.Row:
                break;
            case UniqueValGroup.Block3:
                break;
            case UniqueValGroup.Diagonal:
                break;
        }
        #region//COLUMN
        for (int i = 0; i < tilePos.y; i++)
        {
            if (_tiles[tilePos.x, i].NumValue == val)
            {
                //  print("Column");
                return _tiles[tilePos.x, i].ordinal;
            }
        }
        #endregion
        #region//ROW
        for (int i = 0; i < tilePos.x; i++)
        {
            if (_tiles[i, tilePos.y].NumValue == val)
            {
                //  print("Row");
                return _tiles[i, tilePos.y].ordinal;
            }
        }
        #endregion
        #region//3x3
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
                    return _tiles[i, j].ordinal;
                }
            }
        }
        #endregion
        #region//DIAGONAL
        if (diagonal)
        {
            Tile tajl = _tiles[tilePos.x, tilePos.y];
            List<int> list = Lista(_diagonalDown, tajl);
            if(list.Count > 0)
            {
              //  print(list[0]);
                return list[0];
            }

            List<int> Lista(HashSet<Tile> hs, Tile tl)
            {
                List<int> ll = new List<int>();
                if (hs.Contains(tl))
                {
                    foreach (Tile item in hs)
                    {
                        if (item.NumValue == val && item != tl) ll.Add(item.ordinal);
                    }
                }
                ll.Sort();
                return ll;
            }

        }

        #endregion

        return 1000;
    }


     bool CanFit(int val, Vector2Int tilePos)
    {
        #region//COLUMN
        for (int i = 0; i < tilePos.y; i++)
        {
            if (_tiles[tilePos.x, i].NumValue == val)
            {
              //  print("Column");
                return false;
            }
        }
        #endregion
        #region//ROW
        for (int i = 0; i < tilePos.x; i++)
        {
            if (_tiles[i, tilePos.y].NumValue == val)
            {
              //  print("Row");
                return false;
            }
        }
        #endregion
        #region//3x3
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
        #endregion
        //#region//DIAGONAL
        //if (diagonal)
        //{
        //    Tile tajl = _tiles[tilePos.x, tilePos.y];
        //  //  if (HasDuplicate(_diagonalUp, tajl)) return false;
        //    if (HasDuplicate(_diagonalDown, tajl)) return false;
        //}
        //bool HasDuplicate(HashSet<Tile> hs, Tile tl)
        //{
        //    if (hs.Contains(tl))
        //    {
        //        foreach (Tile item in hs)
        //        {
        //            if (item.NumValue == val && item != tl) return true;
        //        }
        //    }
        //    return false;
        //}
        //#endregion

        return true;
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
            Tile tl = _tiles[i, j];
            List<int> chosen = HelperScript.RandomList(9);
            int val = 0;
            List<int> duplicateOrdinals = new List<int>();
            bool canFiit = false;
            for (int k = 0; k < chosen.Count; k++)
            {
                val = chosen[k];
                int dupOrd = DuplicateOrdinal(val, currentPos);
                if (dupOrd == 1000)
                {
                    canFiit = true;
                    break;
                }
                else
                {
                    duplicateOrdinals.Add(dupOrd);
                }
            }

            if (canFiit)
            {
                tl.NumValue = val;
            }
            else
            {
                duplicateOrdinals.Sort();
                GenerateGrid(_allTilesByOrdinal[duplicateOrdinals[0]].poz);
                return;
            }
        }
    }

}



void LoadDiagonalTiles()
{
    for (int j = 0; j < 9; j++)
    {
        for (int i = 0; i < 9; i++)
        {
            if (i == j)
            {
                //_diagonalUp.Add(_tiles[i, j]);
                //_diagonals.Add(_tiles[i, j]);
            }
            if (i + j == 8)
            {
                _diagonalDown.Add(_tiles[i, j]);
                _diagonals.Add(_tiles[i, j]);
            }
        }
    }
    //foreach (Tile item in _diagonals)
    //{
    //    item.NumValue = Random.Range(0, 9);
    //}

    //GenerateDiagonalTiles(Vector2Int.zero);
}
void GenerateDiagonalTiles(Vector2Int startPos)
{
    for (int j = 0; j < 9; j++)
    {
        for (int i = 0; i < 9; i++)
        {
            if (i >= startPos.x && j >= startPos.y && _diagonals.Contains(_tiles[i, j])) _tiles[i, j].NumValue = -1;
        }
    }

    for (int j = 0; j < 9; j++)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!_diagonals.Contains(_tiles[i, j])) continue;

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
                    canFit = true;
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
                GenerateDiagonalTiles(target);
                return;
            }
        }
    }

}*/
