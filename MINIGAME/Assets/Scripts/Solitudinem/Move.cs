using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public int currRow, currCol, movRow, movCol;

    public void AddMove(int currRow, int currCol, int movRow, int movCol)
    {
        this.currRow = currRow;
        this.currCol = currCol;
        this.movRow = movRow;
        this.movCol = movCol;
    }
}
