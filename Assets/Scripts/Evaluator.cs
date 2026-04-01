using TMPro;

public static class Evaluator
{
    private const int P_VAL = 100;
    private const int N_VAL = 320;
    private const int B_VAL = 330;
    private const int R_VAL = 500;
    private const int Q_VAL = 900;
    private const int K_VAL = 20000;

    public static bool IsEndgame(BoardState boardContext)
    {
        int boardMaterial = 0;

        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                var cell = boardContext.whatIsAt(col, row);
                if (cell.HasValue && cell.Value.getPieceType() != PieceType.King)
                {
                    boardMaterial += GetMaterialValue(cell.Value.getPieceType());
                }
            }
        }

        return boardMaterial < 1500;
    }

    public static int Evaluate(BoardState boardContext)
    {
        int whiteAdvantage = 0;
        int blackAdvantage = 0;
        bool isLateGame = IsEndgame(boardContext);

        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                var cell = boardContext.whatIsAt(col, row);
                if (!cell.HasValue) continue;

                PieceType pType = cell.Value.getPieceType();
                PieceColor pColor = cell.Value.GetPieceColor();

                int rawValue = GetMaterialValue(pType);
                int positionValue = PieceSquareTables.GetPST(pType, pColor, col, row, isLateGame);

                int totalPieceValue = rawValue + positionValue;

                if (!isLateGame)
                {
                    if (pType == PieceType.Knight || pType == PieceType.Bishop)
                    {
                        if (pColor == PieceColor.White && row == 0)
                        {
                            totalPieceValue -= 30; 
                        }
                        else if (pColor == PieceColor.Black && row == 7)
                        {
                            totalPieceValue -= 30; 
                        }
                    }
                }

                if (pColor == PieceColor.White)
                {
                    whiteAdvantage += totalPieceValue;
                }
                else
                {
                    blackAdvantage += totalPieceValue;
                }
            }
        }

        return whiteAdvantage - blackAdvantage;
    }

    public static int GetMaterialValue(PieceType pt)
    {
        if (pt == PieceType.Pawn) return P_VAL;
        if (pt == PieceType.Knight) return N_VAL;
        if (pt == PieceType.Bishop) return B_VAL;
        if (pt == PieceType.Rook) return R_VAL;
        if (pt == PieceType.Queen) return Q_VAL;
        if (pt == PieceType.King) return K_VAL;

        return 0;
    }
}