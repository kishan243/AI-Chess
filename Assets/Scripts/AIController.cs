using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool aiStartColorBlack = true;
    public int searchDepth = 3;
    public float moveDelay = 0.5f;

    private bool isWorking = false;

    private struct MoveData
    {
        public GameObject pieceRef;
        public Vector2Int targetPos;
        public int evalScore;
    }

    void Update()
    {
        if (isWorking) return;

        string myColorName = aiStartColorBlack ? "black" : "white";

        if (GameManager.instance.currentPlayer.name == myColorName)
        {
            isWorking = true;
            StartCoroutine(CalculateAndPlay());
        }
    }

    private IEnumerator CalculateAndPlay()
    {
        yield return new WaitForSeconds(moveDelay);

        GameManager core = GameManager.instance;
        PieceColor myEnumColor = aiStartColorBlack ? PieceColor.Black : PieceColor.White;

        MoveData bestMoveConfig = new MoveData();
        bestMoveConfig.evalScore = aiStartColorBlack ? MinimaxAB.MAX_BOUND : MinimaxAB.MIN_BOUND;

        bool moveIdentified = false;
        BoardState referenceBoard = BoardState.boardSnapshot();

        foreach (GameObject p in core.currentPlayer.pieces)
        {
            if (p == null) continue;

            Vector2Int startCoord = core.GridForPiece(p);
            List<Vector2Int> legalDestinations = core.MovesForPiece(p);

            foreach (Vector2Int dest in legalDestinations)
            {
                BoardState testBoard = referenceBoard.cloneBoard();
                testBoard.applyMove(new ChessMove(startCoord, dest));

                if (MoveGenerator.isInCheck(testBoard, myEnumColor)) continue;

                testBoard.switchTurn();

                int score = MinimaxAB.search(testBoard, searchDepth - 1, MinimaxAB.MIN_BOUND, MinimaxAB.MAX_BOUND);

                bool updateBest = aiStartColorBlack ? (score < bestMoveConfig.evalScore) : (score > bestMoveConfig.evalScore);

                if (updateBest || !moveIdentified)
                {
                    bestMoveConfig.evalScore = score;
                    bestMoveConfig.pieceRef = p;
                    bestMoveConfig.targetPos = dest;
                    moveIdentified = true;
                }
            }
        }

        if (moveIdentified && bestMoveConfig.pieceRef != null)
        {
            core.SelectPiece(bestMoveConfig.pieceRef);

            if (core.PieceAtGrid(bestMoveConfig.targetPos) != null)
            {
                core.CapturePieceAt(bestMoveConfig.targetPos);
            }

            core.Move(bestMoveConfig.pieceRef, bestMoveConfig.targetPos);
            core.DeselectPiece(bestMoveConfig.pieceRef);
        }

        core.NextPlayer();
        GetComponent<TileSelector>().EnterState();
        isWorking = false;
    }
}