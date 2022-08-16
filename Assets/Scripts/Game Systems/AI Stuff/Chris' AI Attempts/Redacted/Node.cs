using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour // Tree 
    {
    /*
        public List<Node> childNodes = new List<Node>();
        public Node parentNode;

        // Available moves for the unit
        private Cell[] moves;

        // Best cell to move to 
        public Cell moveCell;

        // Utility value of this node
        public int UtilityValue;

        // Depth of the node -- Difficulty will be scaled by how far into the future the AI can see. 
        public int depth;

        private bool currentTurn;

        // resulting score of both players
        public int PlayerScore;
        public int AIScore;

    // Create a node 
        public Node(Cell[] AvailableCells, bool turn)
        {
            moves = AvailableCells;
            currentTurn = turn;

        }
        
        // Make a copy of an old node to modify 
        public Node(Node oldNode)
        {
            moves = oldNode.moves;
            currentTurn = oldNode.currentTurn;
            AIScore = oldNode.AIScore;
            PlayerScore = oldNode.PlayerScore;
            UtilityValue = 0;
            parentNode = null;
            childNodes = new List<Node>();
        }

        // This will be used for the prediction of AI's attack, will calculate whos winning. Not yet complete. 
        public void AttackMove(Unit unit, bool PlayerTurn) 
        {        
        Cell[] ScoreCells;
        ScoreCells = unit.currentCell.GetCellsInRange(6);
        foreach (Cell c in ScoreCells)
        {
            int killPoints = c.cellScore; // Score of the cell.
            if (PlayerTurn)
            {
                PlayerScore += killPoints; // If the player kills an AI troop, add it to the player score
            }
            else
            {
                AIScore += killPoints; // If the AI kills a player troop, add it to the AI score
            }
        }
            currentTurn = !currentTurn;
        }

        public void MakeMove(Cell cell, bool PlayerTurn)
        {
        Cell moveCell;
        // TODO: If the cell you're exploring has a unit on it, return a cell around it
        // if the cell you're exploring is the one you are on, return the function

        // Have something that scales the cells at the start of each minimax to get the furthest cells until an enemy is in range??? 



        // Once turn is done, flip the currentTurn bool to end this prediction. 
        }



    // Create a new child node for every possible move the AI can make in its range
    public void AddChild()
        {
            for (int i = 0; i < moves.Length; i++)
            {
                CreateChild(moves[i]);
            }
        }

    // Create a copy of the current moves to allow the AI to see a couple of moves ahead (using MiniMax)
        public void CreateChild(Cell cell)
        {
            Node newChild = new Node(this);
            newChild.depth++;
            newChild.parentNode = this;
            newChild.MakeMove(cell, currentTurn);
            childNodes.Add(newChild);
        }
    // Returns the value of the game state, aka who is winning.
        public int Utility()
        {
            UtilityValue = AIScore - PlayerScore;
            return UtilityValue;
        }
    */
    }

