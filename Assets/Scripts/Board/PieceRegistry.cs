using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PieceRegistry
{
    // ==================================================
    // Public Fields
    // ==================================================
    public PlayerColors Colors => currentColors;

    // ==================================================
    // Private Fields
    // ==================================================
    private PlayerColors currentColors = new PlayerColors();    // initializes with next set to a color, player empty
    private Piece playerPiece;

    private Piece[] registry = new Piece[GameConstants.NumberCells];    // reference for piece indices (instant access)
    private GameObjectPool<Piece, PieceData> piecePool;                 // storage in memory (where the instance actually lives)


    // ==================================================
    // Initializers
    // ==================================================

    public PieceRegistry(GameObjectPool<Piece, PieceData> piecePool)
    {
        this.piecePool = piecePool;
        if (piecePool == null)
            throw new ArgumentNullException(nameof(piecePool), "PieceRegistry requires non-null game object pool");

        currentColors.Reset();
        if (currentColors.NextColor == CellColor.Empty)
        {
            Debug.LogError("[PieceRegistry] CurrentColors.NextColor failed initialization");
        }
    }

    public bool LoadGame(PlayerColors colors, IReadOnlyList<CellEntry> cells)
    {
        if (colors == null || colors.NextColor == CellColor.Empty)
        {
            Debug.LogError($"[PieceRegistry] Passed invalid colors to LoadGame: {colors}");
            return false;
        }
        if (cells == null)
        {
            Debug.LogError("[PieceRegistry] Passed null cell entry list to LoadGame");
            return false;
        }

        currentColors.NextColor = colors.NextColor;
        if (!TrySpawnNewPlayer(out _, colors.PlayerColor))
        {
            Debug.LogError("[PieceRegistry] Unsuccessful player spawn in LoadGame");
            return false;
        }

        int cellsLoaded = LoadBoardPieces(cells);
        if (cellsLoaded < cells.Count)
        {
            Debug.LogError($"[PieceRegistry] Unsuccessful LoadBoardPieces, only loaded {cellsLoaded} / {cells.Count} cells");
            return false;
        }

        return true;
    }


    // ==================================================
    // Public Methods - Player
    // ==================================================

    // null - generate a brand new color from next; value - use passed color and maintain current next color
    public bool TrySpawnNewPlayer(out CellColor nextColor, CellColor? color = null)
    {
        nextColor = currentColors.NextColor;

        if (currentColors.NextColor == CellColor.Empty)
            throw new InvalidOperationException("[PieceRegistry] SpawnNewPlayer called with uninitialized NextColor.");

        if (!piecePool.TryGetObject(out playerPiece))
        {
            Debug.LogError("[PieceRegistry] Failed to retrieve memory for player in SpawnNewPlayer");
            return false;
        }

        if (!color.HasValue)
        {
            color = currentColors.NextColor;
            currentColors.NextColor = PlayerPicker.ChooseColor();
        }

        currentColors.PlayerColor = color.Value;
        playerPiece.SetToPlayer(color.Value);

        nextColor = currentColors.NextColor;
        return true;
    }

    public void PlacePlayer(Vector3 position, Vector2Int index)
    {
        playerPiece.PlacePlayer(position);

        int idx = TwoDimToFlatIndex(index);
        if (playerPiece.Color == CellColor.Mask)
            piecePool.RemoveObject(playerPiece);

        registry[idx] = playerPiece;
        playerPiece = null;

        ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.PlacePlayer);
        
    }

    public void ReturnPlayerToStart()
    {
        playerPiece.ReturnPlayer();
    }

    public void PausePlayer(bool pause)
    {
        playerPiece.Pause(pause);
    }


    // ==================================================
    // Public Methods - Cell
    // ==================================================

    public bool TrySetPieceColor(Vector2Int index, CellColor color)
    {
        int idx = TwoDimToFlatIndex(index);
        if (registry[idx] == null) return false;

        registry[idx].SetColor(color);
        return true;
    }

    public void ResetPieces()
    {
        currentColors.Reset();

        if (playerPiece != null)
        {
            piecePool.RemoveObject(playerPiece);
            playerPiece = null;
        }

        // registry should defualt all values to null
        for (int i = 0; i < registry.Length; i++)
        {
            Piece piece = registry[i];
            registry[i] = null;

            piecePool.RemoveObject(piece);
        }
    }

    // returns number of pieces successfully loaded
    public int LoadBoardPieces(IReadOnlyList<CellEntry> cells)
    {
        int numLoaded = 0;

        if (cells == null)
        {
            Debug.LogError("[PieceRegistry] Passed null cells to LoadBoardPieces");
            return numLoaded;
        }
        
        foreach (CellEntry cell in cells)
        {
            if (cell.x < 0 || cell.x >= GameConstants.RowSize || cell.y < 0 || cell.y >= GameConstants.RowSize ||
                !Enum.IsDefined(typeof(CellColor), cell.color) || (CellColor)cell.color == CellColor.Empty)
            {
                Debug.LogError($"[PieceRegistry] Invalid cell entry in cells passed to LoadBoardPieces: {cell}");
                continue;
            }

            if (!piecePool.TryGetObject(out Piece newPiece))
            {
                Debug.LogError("[PieceRegistry] Failed to retrieve memory for new piece in LoadBoardPieces");
                return numLoaded;
            }

            Vector2Int index = new Vector2Int(cell.x, cell.y);
            Vector3 position = BoardGeometry.BoardIndexToTransform(index);
            int flatIndex = TwoDimToFlatIndex(index);

            newPiece.SetToCell((CellColor)cell.color, position);
            registry[flatIndex] = newPiece;

            numLoaded++;
        }

        return numLoaded;
    }

    // Coroutine for popping pieces animation
    // note: the pieces currently destroy themselves after animation, would like to add object pool for later
    public IEnumerator PopPieces(BoardLogic.PlayResult r, Vector2Int index)
    {
        int cleared = 0;
        int total = (r.ClearRow ? 4 : 0) + (r.ClearCol ? 4 : 0) + (r.ClearRDiag ? 4 : 0) + (r.ClearLDiag ? 4 : 0) + 1;

        void OnPopFinished(Piece piece)
        {
            cleared++;
            piece.PopFinished -= OnPopFinished;
            piecePool.RemoveObject(piece);
        }

        // helper
        void PopPieceAt(int row, int col)
        {
            Piece piece = RemoveFromRegistry(row, col);  // on pop finished does not know the index, must remove from array here

            piece.PopFinished += OnPopFinished;
            piece.Pop();
        }

        // pop the pieces in filled lines EXCEPT player
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            if (r.ClearRow && i != index.y)    // i is not the player
                PopPieceAt(index.x, i);

            if (r.ClearCol && i != index.x)
                PopPieceAt(i, index.y);

            if (r.ClearRDiag && i != index.x)
                PopPieceAt(i, i);

            if (r.ClearLDiag && i != index.x)   // start with top left
                PopPieceAt(i, GameConstants.RowSize - 1 - i);
        }

        Piece player = RemoveFromRegistry(index.x, index.y);
        player.PopFinished += OnPopFinished;
        player.Pop();

        yield return new WaitUntil(() => cleared >= total);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private int TwoDimToFlatIndex(Vector2Int index)
    {
        return index.x * GameConstants.RowSize + index.y;     // x: row, y: column
    }

    private Vector2Int FlatToTwoDimIndex(int flatIndex)
    {
        return new Vector2Int(flatIndex / GameConstants.RowSize, flatIndex % GameConstants.RowSize);
    }

    private Piece RemoveFromRegistry(int row, int col)
    {
        int idx = TwoDimToFlatIndex(new Vector2Int(row, col));
        Piece piece = registry[idx];
        registry[idx] = null;

        return piece;
    }

}
