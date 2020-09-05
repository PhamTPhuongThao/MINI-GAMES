using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public class Node : MonoBehaviour
{
    static System.Random r = new System.Random();
    static double epsilon = 1e-6;
    static double Cp = 2 * (1 / Math.Sqrt(2));

    public List<Node> children;
    public double nVisits, totValue;
    public CheckerBoard gameState;
    //public int numberOfNode;

    private Vector2 startDrag;
    private Vector2 endDrag;
    // save in node
    public int[] state;

    public void GetCurrentCheckerBoard(CheckerBoard igameState)
    {
        gameState = igameState;
        state = new int[69];
        state = CheckerBoardToState(gameState);
    }

    public int[] CheckerBoardToState(CheckerBoard checkerboard)
    {
        int i = 0;
        if (checkerboard.isWhiteTurn == true)
        {
            state[0] = 1;
            i++;
        }
        else
        {
            state[0] = -1;
            i++;
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (checkerboard.pieces[x, y] == null)
                {
                    state[i] = 0;
                }
                else
                {
                    // white
                    if (checkerboard.pieces[x, y].isWhitePiece == true)
                    {
                        state[i] = 2;
                        // white vs king
                        if (checkerboard.pieces[x, y].isKing == true)
                        {
                            state[i] = 3;
                        }
                    }
                    else // black
                    {
                        state[i] = -2;
                        // black vs king
                        if (checkerboard.pieces[x, y].isKing == true)
                        {
                            state[i] = -3;
                        }
                    }
                }
                i++;
            }
        }
        return state;
    }

    public void ChangeMovingInState(int firstx, int firsty, int finalx, int finaly)
    {
        int stamp = state[8 * firstx + firsty + 1];
        state[8 * firstx + firsty + 1] = state[8 * finalx + finaly + 1];
        state[8 * finalx + finaly + 1] = stamp;
        state[65] = firstx;
        state[66] = firsty;
        state[67] = finalx;
        state[68] = finaly;
    }

    public void selectAction()
    {
        LinkedList<Node> visited = new LinkedList<Node>();
        Node cur = this;
        visited.AddLast(this);

        // if not win and cur is a leaf
        if (gameState.CheckVictory(state) == -9)
        {
            cur.expand();
            Node newNode = cur.select();
            gameState.Moving((int)newNode.state[65], (int)newNode.state[66], (int)newNode.state[67], (int)newNode.state[68]);
            if (newNode != null)
            {
                //newNode.GetCurrentCheckerBoard();
                visited.AddLast(newNode);
                // caculate value of all child node
                double value = rollOut(newNode);
                foreach (Node node in visited)
                {
                    node.updateStats(value);
                }
            }
            else Debug.LogError("select action selected a null node");
        }
        else
        {
            double value = 0;
            switch (cur.gameState.CheckVictory(state))
            {
                case 8:
                    value = 1.0;
                    break;
                case -8:
                    value = 0.0;
                    break;
                default:
                    break;
            }
            foreach (Node node in visited)
            {
                node.updateStats(value);
            }
        }

        //return
    }

    public void expand()
    {
        children = new List<Node>();
        List<Piece> possiblePieceForcedToMove = gameState.ScanForAllForcedPiece();
        if (possiblePieceForcedToMove.Count > 0)
        {
            int i = 0;
            foreach (Piece selectedPiece in possiblePieceForcedToMove)
            {
                List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionForcedToMoveTo;
                startDrag = gameState.UpdateDrag(selectedPiece);
                foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
                {
                    endDrag = possiblePosition;
                    children.Add(gameObject.AddComponent<Node>());
                    children[i].GetCurrentCheckerBoard(gameState);
                    children[i].ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                    i++;
                }
            }
        }
        else
        {
            List<Piece> possiblePieceToMove = gameState.ScanForAllPiecePossibleToMove();
            if (possiblePieceToMove.Count > 0)
            {
                int i = 0;
                foreach (Piece selectedPiece in possiblePieceToMove)
                {
                    List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionToMoveTo;
                    startDrag = gameState.UpdateDrag(selectedPiece);
                    foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
                    {
                        endDrag = possiblePosition;
                        children.Add(gameObject.AddComponent<Node>());
                        children[i].GetCurrentCheckerBoard(gameState);
                        children[i].ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                        i++;
                    }
                }
            }
        }
    }

    private Node select()
    {
        Node selected = null;
        double bestValue = Double.MinValue;
        int i = 0;
        foreach (Node c in children)
        {
            double uctValue =
               c.totValue / (c.nVisits + epsilon) +
                  Cp * Math.Sqrt(2 * Math.Log(nVisits + 1) / (c.nVisits + epsilon)) +
                  r.NextDouble() * epsilon;
            if (uctValue > bestValue)
            {
                selected = c;
                bestValue = uctValue;
            }
            i++;
        }
        return selected;
    }

    public bool isLeaf()
    {
        return children == null;
    }

    public double rollOut(Node tn)
    {
        CheckerBoard rollGS = gameObject.AddComponent<CheckerBoard>();
        rollGS = tn.gameState;
        bool stillPlaying = true;
        double rc = 0;
        int pos;
        int move;

        List<Piece> possiblePieceForcedToMove = rollGS.ScanForAllForcedPiece();
        if (possiblePieceForcedToMove.Count > 0)
        {
            while ((possiblePieceForcedToMove.Count > 0) && stillPlaying)
            {
                pos = r.Next(possiblePieceForcedToMove.Count - 1);
                startDrag = gameState.UpdateDrag(possiblePieceForcedToMove[pos]);
                List<Vector2> possiblePositionsToMoveTo = possiblePieceForcedToMove[pos].possiblePositionForcedToMoveTo;
                move = r.Next(possiblePositionsToMoveTo.Count - 1);
                endDrag = possiblePositionsToMoveTo[move];

                tn.ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);

                switch (rollGS.CheckVictory(state))
                {
                    case -9: // 1 won - ai
                        stillPlaying = false;
                        if (!rollGS.whoseMove())
                        {
                            rc = 1.0;
                        }
                        break;
                    default: /* keep playing */
                        possiblePieceForcedToMove = rollGS.ScanForAllForcedPiece();
                        break;
                }
            }
        }
        else
        {
            List<Piece> possiblePieceToMove = gameState.ScanForAllPiecePossibleToMove();
            while ((possiblePieceToMove.Count > 0) && stillPlaying)
            {
                pos = r.Next(possiblePieceToMove.Count - 1);
                startDrag = gameState.UpdateDrag(possiblePieceToMove[pos]);
                List<Vector2> possiblePositionsToMoveTo = possiblePieceToMove[pos].possiblePositionToMoveTo;
                move = r.Next(possiblePositionsToMoveTo.Count - 1);
                endDrag = possiblePositionsToMoveTo[move];

                tn.ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);

                switch (rollGS.CheckVictory(state))
                {
                    case -9: // 1 won - ai
                        stillPlaying = false;
                        if (!rollGS.whoseMove())
                        {
                            rc = 1.0;
                        }
                        break;
                    default: /* keep playing */
                        possiblePieceToMove = rollGS.ScanForAllForcedPiece();
                        break;
                }
            }
        }


        return rc;
    }

    public void updateStats(double value)
    {
        nVisits++;
        totValue += value;
    }

    public int arity()
    {
        return children == null ? 0 : children.Count;
    }

    public Node bestChild()
    {
        // if only one child choose it
        // check if a win for computer, if so make it best and stop  
        // look ahead one move and check of win for person, if so, pick best alternate  <== not yet implmented
        Node bestChild = null;
        bool bestChildLoses = false;

        for (int i = 0; i < children.Count; i++)
        {
            if (bestChild == null)
            {
                bestChild = children[i];
                if (bestChild.gameState.CheckVictory(state) == 1)
                {  // computer can win
                    break;  // stop looking
                }
                bestChildLoses = OpponentCanWin(bestChild);  // test best child for later
            }
            else
            {
                if (children[i].gameState.CheckVictory(state) == 1)
                {  // computer can win
                    bestChild = children[i];
                    break;  // stop looking
                }
                else
                {
                    if (bestChildLoses)
                    { // if best current move is losing, pick anything
                        bestChild = children[i];
                        bestChildLoses = OpponentCanWin(bestChild);  // test best child for later
                    }
                    else
                    { // check new move
                        if (OpponentCanWin(children[i]) == false)
                        { // avoid trap, skip move if true
                            if (children[i].totValue > bestChild.totValue)
                            { //not a trap, is it better?
                                bestChild = children[i];
                            }
                        }
                    }
                }
            }
        }
        return bestChild;
    }

    public bool OpponentCanWin(Node currentPosition)
    {
        bool canWin = false;

        List<Piece> possiblePieceForcedToMove = currentPosition.gameState.ScanForAllForcedPiece();

        if (possiblePieceForcedToMove.Count > 0)
        {
            foreach (Piece selectedPiece in possiblePieceForcedToMove)
            {
                CheckerBoard lookAhead = gameObject.AddComponent<CheckerBoard>();
                lookAhead = currentPosition.gameState;
                List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionForcedToMoveTo;
                startDrag = gameState.UpdateDrag(selectedPiece);

                foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
                {
                    endDrag = possiblePosition;
                    currentPosition.ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                    if (lookAhead.CheckVictory(state) == 1)
                    {
                        canWin = true;
                        break;
                    }
                }
            }
        }
        else
        {
            List<Piece> possiblePieceToMove = gameState.ScanForAllPiecePossibleToMove();
            if (possiblePieceToMove.Count > 0)
            {
                foreach (Piece selectedPiece in possiblePieceToMove)
                {
                    CheckerBoard lookAhead = gameObject.AddComponent<CheckerBoard>();
                    lookAhead = currentPosition.gameState;
                    List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionForcedToMoveTo;
                    startDrag = gameState.UpdateDrag(selectedPiece);

                    foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
                    {
                        endDrag = possiblePosition;
                        currentPosition.ChangeMovingInState((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                        if (lookAhead.CheckVictory(state) == 1)
                        {
                            canWin = true;
                            break;
                        }
                    }
                }
            }
        }
        return canWin;
    }
}
