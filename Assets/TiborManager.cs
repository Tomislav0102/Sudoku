using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TiborManager: MonoBehaviour
{
    [SerializeField] Transform parGrid;
    readonly Tile[,] _tiles = new Tile[9, 9];
    readonly Tile[] _allTilesByOrdinal = new Tile[81];
    List<int> values = new List<int>();

    int[,] board = new int[9,9];

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

    }
    private void Start()
    {
        values = HelperScript.RandomList(9);

        board = shuffleAndGet();
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                _tiles[i, j].NumValue = board[i, j];
            }
        }

    }

    public int[,] shuffleAndGet()
    {
        var firstCell = 0;
        var ignored = fillCellWithBacktracking(firstCell);
        return board;
    }

    private bool fillCellWithBacktracking(int startingCell)
    {
        if (startingCell == 9 * 9)
        {
            return true;
        }
        int row = startingCell / 9;
        int col = startingCell % 9;

       // values = HelperScript.RandomList(9);
        for (int i = 0; i < values.Count; i++)
        {
            if (safeToPlace(values[i], row, col))
            {
                board[row,col] = values[i];
                if (fillCellWithBacktracking(startingCell + 1))
                {
                    return true;
                }
                board[row,col] = -7;

            }
        }
        return false;
    }

    protected bool safeToPlace(int value, int row, int col)
    {
        return safeInRow(value, row)
                && safeInCol(value, col)
                && safeInBox(value, toNearestBox(row), toNearestBox(col))
                && safeInLeftDiagonal(value, row, col)
                && safeInRightDiagonal(value, row, col);
    }

    private int toNearestBox(int rowOrCol)
    {
        return rowOrCol - (rowOrCol % 3);
    }

    private bool safeInRow(int value, int row)
    {
        for (int col = 0; col < 9; col++)
        {
            if (board[row,col] == value)
            {
                return false;
            }
        }
        return true;
    }

    private bool safeInCol(int value, int col)
    {
        for (int row = 0; row < 9; row++)
        {
            if (board[row,col] == value)
            {
                return false;
            }
        }
        return true;
    }

    private bool safeInBox(int value, int topmostRow, int leftmostCol)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var row = i + topmostRow;
                var col = j + leftmostCol;
                if (board[row,col] == value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool safeInLeftDiagonal(int value, int row, int col)
    {
        if (row != col)
        {
            return true;
        }
        for (int i = 0; i < 9; i++)
        {
            if (board[i,i] == value)
            {
                return false;
            }
        }
        return true;
    }

    private bool safeInRightDiagonal(int value, int row, int col)
    {
        if (row + col != 8)
        {
            return true;
        }
        for (int i = 0; i < 9; i++)
        {
            if (board[i,8 - i] == value)
            {
                return false;
            }
        }
        return true;
    }
}
