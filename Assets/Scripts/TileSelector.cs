using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    void Start()
    {
        this.enabled = true;
    }

    void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            Vector2Int gridPos = Geometry.GridFromPoint(hitInfo.point);

            if (Input.GetMouseButtonDown(0))
            {
                GameObject clickedPiece = GameManager.instance.PieceAtGrid(gridPos);

                if (GameManager.instance.DoesPieceBelongToCurrentPlayer(clickedPiece))
                {
                    GameManager.instance.SelectPiece(clickedPiece);
                    SwitchToMoveState(clickedPiece);
                }
            }
        }
    }

    public void EnterState()
    {
        this.enabled = true;
    }

    private void SwitchToMoveState(GameObject pieceToMove)
    {
        this.enabled = false;
        MoveSelector moveController = GetComponent<MoveSelector>();
        moveController.EnterState(pieceToMove);
    }
}