using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax : MonoBehaviour
{
    public List<Minimax> children;
    private int score;
    private Move move;

    // get current state of checker board and add into node
    public void NewMinimax(Move move, int score)
    {
        children = new List<Minimax>();
        this.score = score;
        this.move = move;
    }

    public Move getMove()
    {
        return move;
    }

    public int getScore()
    {
        return score;
    }

    public List<Minimax> getChildren()
    {
        return children;
    }

    public int getNumChildren()
    {
        return children.Count;
    }

    public void setScore(int newVal)
    {
        score = newVal;
    }

    public Minimax getChild(int index)
    {
        return children[index];
    }

    public void addChild(Minimax child)
    {
        children.Add(child);
    }

    public void addChildren(List<Minimax> children)
    {
        foreach (Minimax child in children)
        {
            addChild(child);
        }
    }
}
