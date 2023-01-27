using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    GameManager _gm;
    public Vector2Int poz;
    public int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            _display.text = (_numValue + 1).ToString();
            _display.text = poz.ToString();
        }
    }
    int _numValue;
    TextMeshProUGUI _display;
    public int ordinal;

    public void Ini(GameManager gm, Vector2Int position, int ord)
    {
        _gm = gm;
        poz = position;
        _display = GetComponent<TextMeshProUGUI>();
        NumValue = -1;
        ordinal = ord;
    }
}
