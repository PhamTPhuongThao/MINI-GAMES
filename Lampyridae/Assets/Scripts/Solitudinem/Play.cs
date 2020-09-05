using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    // public CheckerBoard init;
    public GameObject blackPiecePrefab;
    public GameObject whitePiecePrefab;
    public GameObject mblackPiecePrefab;
    public GameObject mwhitePiecePrefab;
    private Vector2 mouseOver;

    public CheckerBoard igameState;
    public Node test;
    public MinimaxAIMove aiMove;

    void Start()
    {
        igameState = gameObject.AddComponent<CheckerBoard>();
        igameState.GenerateBoard(blackPiecePrefab, whitePiecePrefab);
        igameState.isWhiteTurn = true;
        igameState.forcedPieces = new List<Piece>();
        // test = gameObject.AddComponent<Node>();
        // test.GetCurrentCheckerBoard(igameState);
        // test.expand();
        aiMove = gameObject.AddComponent<MinimaxAIMove>();

    }

    void Update()
    {
        //if it's my turn
        if (igameState.isWhiteTurn)
        {
            UpdateMouseOver();
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (igameState.selectedPiece != null)
            {
                UpdatePieceDrag(igameState.selectedPiece);
            }
            if (Input.GetMouseButtonDown(0))
            {
                SelectPieces(x, y);

            }

            if (Input.GetMouseButtonUp(0))
            {
                igameState.Moving((int)igameState.startDrag.x, (int)igameState.startDrag.y, x, y);
            }
        }
        else
        {
            // test.GetCurrentCheckerBoard(igameState);
            // test.selectAction();
            aiMove.getAIMove(igameState, mblackPiecePrefab, mwhitePiecePrefab);

        }
    }


    // ********************************************** Begin Mouse Over and Select Piece ********************************************
    private void UpdateMouseOver()
    {
        // if it is my turn
        // check the main camera
        if (!Camera.main)
        {
            Debug.Log("Unable to find the camera");
            return;
        }

        // check the hit
        RaycastHit hit;
        //bool True if the ray intersects with a Collider, otherwise false.
        //public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance = Mathf.Infinity,int layerMask = DefaultRaycastLayers,QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - igameState.boardOffset.x);
            mouseOver.y = (int)(hit.point.y - igameState.boardOffset.y);
            if (mouseOver.x > 7)
            {
                mouseOver.x = 7;
            }
            if (mouseOver.y > 7)
            {
                mouseOver.y = 7;
            }
        }
        else // dont hit st
        {
            mouseOver.x = -2;
            mouseOver.y = -2;
        }
    }

    public void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find the camera");
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.forward;
        }
    }

    private void SelectPieces(int x, int y)
    {
        // if out of bounds
        if (x < 0 || x > 8 || y < 0 || y > 8)
        {
            return;
        }
        else
        {
            Piece p = igameState.pieces[x, y];
            if (p != null && p.isWhitePiece == igameState.isWhiteTurn)
            {
                if (igameState.forcedPieces.Count == 0)
                {
                    igameState.selectedPiece = p;
                    igameState.startDrag = mouseOver;
                }
                else
                {
                    // look for the piece in the force pieces list
                    if (igameState.forcedPieces.Find(fp => fp == p) == null)
                    {
                        return;
                    }
                    igameState.selectedPiece = p;
                    igameState.startDrag = mouseOver;
                }
            }
        }
    }
    // ********************************************** End Mouse Over and Select Piece **********************************************
}
