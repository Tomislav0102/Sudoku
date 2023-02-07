using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EEM_DefineBlock : MonoBehaviour
{
    [SerializeField] Tile tile;
    public enum BlockValue
    {
        _0, _1, _2, _3, _4, _5, _6, _7, _8
    }
    public BlockValue BlockVal;


    private void OnValidate()
    {
        tile.MarkBlock((int)BlockVal);
    }
}
