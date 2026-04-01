using System.Collections.Generic;
using UnityEngine;

public static class MinimaxAB
{
    public const int MAX_BOUND = 999999;
    public const int MIN_BOUND = -999999;

    public static int search(BoardState currentBoard, int depthLeft, int alphaLimit, int betaLimit)
    {
        var validMoves = MoveGenerator.getLegalMoves(currentBoard, currentBoard.currentTurn);

        if (validMoves.Count == 0)
        {
            bool inCheck = MoveGenerator.isInCheck(currentBoard, currentBoard.currentTurn);
            if (inCheck)
            {
                return currentBoard.currentTurn == PieceColor.White ? MIN_BOUND : MAX_BOUND;
            }
            return 0;
        }

        if (depthLeft == 0)
        {
            return Evaluator.Evaluate(currentBoard);
        }

        bool isMaximizing = (currentBoard.currentTurn == PieceColor.White);

        if (isMaximizing)
        {
            int peakScore = MIN_BOUND;
            foreach (var m in validMoves)
            {
                BoardState sim = currentBoard.cloneBoard();
                sim.applyMove(m);
                sim.switchTurn();

                int score = search(sim, depthLeft - 1, alphaLimit, betaLimit);
                peakScore = Mathf.Max(peakScore, score);
                alphaLimit = Mathf.Max(alphaLimit, score);

                if (betaLimit <= alphaLimit) break;
            }
            return peakScore;
        }
        else
        {
            int floorScore = MAX_BOUND;
            foreach (var m in validMoves)
            {
                BoardState sim = currentBoard.cloneBoard();
                sim.applyMove(m);
                sim.switchTurn();

                int score = search(sim, depthLeft - 1, alphaLimit, betaLimit);
                floorScore = Mathf.Min(floorScore, score);
                betaLimit = Mathf.Min(betaLimit, score);

                if (betaLimit <= alphaLimit) break;
            }
            return floorScore;
        }
    }
}