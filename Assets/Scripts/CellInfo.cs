using UnityEngine;
using System;

public class CellInfo : MonoBehaviour {

    //Observer pattern
    public static event Action<CellInfo> CellClicked = delegate { };

    public int cellNumber = 0;

    private void OnMouseDown()
    {
        CellClicked(this);
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
