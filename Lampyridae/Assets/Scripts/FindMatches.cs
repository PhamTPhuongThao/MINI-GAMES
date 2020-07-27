using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // join 2 list together

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMaches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMaches.Union(GetRowPieces(j));
                                }
                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMaches.Union(GetColumnPieces(i));
                                }
                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMaches.Union(GetColumnPieces(i - 1));
                                }
                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMaches.Union(GetColumnPieces(i + 1));
                                }

                                if (!currentMaches.Contains(leftDot))
                                {
                                    currentMaches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMaches.Contains(rightDot))
                                {
                                    currentMaches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMaches.Contains(currentDot))
                                {
                                    currentMaches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColumnBomb || downDot.GetComponent<Dot>().isColumnBomb || upDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMaches.Union(GetColumnPieces(i));
                                }
                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMaches.Union(GetRowPieces(j));
                                }
                                if (upDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMaches.Union(GetRowPieces(j + 1));
                                }
                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMaches.Union(GetRowPieces(j - 1));
                                }
                                if (!currentMaches.Contains(upDot))
                                {
                                    currentMaches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMaches.Contains(downDot))
                                {
                                    currentMaches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
                                if (!currentMaches.Contains(currentDot))
                                {
                                    currentMaches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }



    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        // if move?
        if (board.currentDot != null)
        {
            if (board.currentDot.isMatched)
            {
                // make it unmatched
                board.currentDot.isMatched = false;
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle > -135 && board.currentDot.swipeAngle <= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
        }
        // other piece matched?
        else if (board.currentDot.otherDot != null)
        {
            Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            // is the other dot matched?
            if (otherDot.isMatched)
            {
                otherDot.isMatched = false;
                if ((otherDot.swipeAngle > -45 && otherDot.swipeAngle <= 45) || (otherDot.swipeAngle > -135 && otherDot.swipeAngle <= 135))
                {
                    otherDot.MakeRowBomb();
                }
                else
                {
                    otherDot.MakeColumnBomb();
                }
            }
        }
    }
}

