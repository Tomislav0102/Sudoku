using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Tile : MonoBehaviour
{
    public ManagerNew gm;
    public Vector2Int poz;
    public int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
          //  ordinal = poz.x + poz.y * 9;
          if(_numValue < 0) _display.text = "|";
          else  _display.text = _numValue.ToString();
            _display.text = _numValue.ToString();
            //  _display.text = blockOrdinal.ToString();
            //  _display.text = poz.ToString();
            //  _display.text = ordinal.ToString();
        }
    }
    int _numValue;
    [SerializeField] TextMeshProUGUI _display;
    public int ordinal;
    public int blockOrdinal;
    public bool editInInspector;

    public void Ini(Vector2Int position)
    {
        poz = position;
        ordinal = transform.GetSiblingIndex();

        NumValue = -10;
        //gm.blocks3x3[blockOrdinal].ordinals.Add(ordinal);
        //_display.color = gm.blocks3x3[blockOrdinal].col;
        // if(blockOrdinal == 1 || blockOrdinal == 2) print($"Ordinal is {ordinal}, blockvalue is {blockOrdinal}");
        // print(blockOrdinal);
    }

    public void MarkBlock(int blockVal)
    {
        if (!editInInspector) return;

        blockOrdinal = blockVal;
        _display.color = gm.blocks3x3[blockOrdinal].col;
    }
}
