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

        currentColors.InitializeNextColor();
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

    public bool PlacePlayer(Vector3 position, Vector2Int index)
    {
        if (playerPiece == null)
        {
            Debug.LogError("[PieceRegistry] PlacePlayer called with no active playerPiece");
            return false;
        }

        int idx = TwoDimToFlatIndex(index);

        Debug.Assert(playerPiece.Color != CellColor.Empty, $"[PieceRegistry] playerPiece has invalid CellColor.Empty in PlacePlayer");
        if (playerPiece.Color == CellColor.Mask && !piecePool.RemoveObject(registry[idx]))
        {
            Debug.LogError("[PieceRegistry] Unsucessful mask removal from piece pool in PlacePlayer");
            return false;
        }

        playerPiece.PlacePlayer(position);

        registry[idx] = playerPiece;        // moves reference from player piece to the registry
        playerPiece = null;

        ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.PlacePlayer);        

        return true;
    }

    public void ReturnPlayerToStart()
    {
        if (playerPiece == null)
        {
            Debug.LogError("[PieceRegistry] ReturnPlayerToStart called with no active playerPiece");
            return;
        }

        playerPiece.ReturnPlayer();
    }

    public void PausePlayer(bool pause)
    {
        if (playerPiece == null)
        {
            Debug.LogError("[PieceRegistry] PausePlayer called with no active playerPiece");
            return;
        }

        playerPiece.Pause(pause);
    }


    // ==================================================
    // Public Methods - Cell
    // ==================================================

    public bool SetPieceColor(Vector2Int index, CellColor color)
    {
        int idx = TwoDimToFlatIndex(index);
        if (registry[idx] == null)
        {
            Debug.LogError($"[PieceRegistry] Attempted SetPieceColor on null piece at index {index}");
            return false;
        }

        registry[idx].SetColor(color);
        return true;
    }

    public void ResetPieces()
    {
        currentColors.InitializeNextColor();

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

            if (piece != null)
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
            Vector2Int index = new Vector2Int(cell.x, cell.y);
            Vector3 position = BoardGeometry.BoardIndexToTransform(index);
            int flatIndex = TwoDimToFlatIndex(index);                       // throws error if out of bounds

            if (!Enum.IsDefined(typeof(CellColor), cell.color) || (CellColor)cell.color == CellColor.Empty)
            {
                Debug.LogError($"[PieceRegistry] Invalid color in cell entry passed to LoadBoardPieces: {cell}");
                continue;
            }
            if (!piecePool.TryGetObject(out Piece newPiece))
            {
                Debug.LogError("[PieceRegistry] Failed to retrieve memory for new piece in LoadBoardPieces");
                return numLoaded;
            }

            newPiece.SetToCell((CellColor)cell.color, position);
            registry[flatIndex] = newPiece;

            numLoaded++;
        }

        return numLoaded;
    }

    // Coroutine for popping pieces animation
    public IEnumerator PopPieces(BoardLogic.PlayResult r, Vector2Int index)
    {
        int cleared = 0;
        int total = (r.ClearRow ? 4 : 0) + (r.ClearCol ? 4 : 0) + (r.ClearRDiag ? 4 : 0) + (r.ClearLDiag ? 4 : 0) + 1;
        if (total == 1)
            yield break;

        // helpers
        List<Piece> pending = new List<Piece>();
        void OnPopFinished(Piece piece)
        {
            cleared++;

            piece.PopFinished -= OnPopFinished;
            pending.Remove(piece);
            piecePool.RemoveObject(piece);
        }
        void PopPieceAt(int row, int col)
        {
            Piece piece = RemoveFromRegistry(row, col);
            if (piece == null)
            {
                Debug.LogError($"[PieceRegistry] Attempted to pop piece at a null index: ({row}, {col})");
                return;
            }

            piece.PopFinished += OnPopFinished;
            pending.Add(piece);
            piece.Pop();
        }
        
        try
        {
            PopPieceAt(index.x, index.y);   // pop the player

            // pop the pieces in filled lines EXCEPT player and ensures that each piece is only ever popped once
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

            float timeout = 1.5f;
            yield return new WaitUntil(() => cleared >= total || (timeout -= Time.deltaTime) <= 0);
            if (cleared < total)
                Debug.LogError($"[PieceRegistry] PopPieces timed out — {cleared}/{total} pieces reported finished");
        }
        finally
        {
            // cleanup in case this function exits abnormally
            foreach (Piece piece in pending)
                piece.PopFinished -= OnPopFinished;
        }
        
    }

    // ==================================================
    // Private Methods
    // ==================================================

    // returns -1 if invalid
    private int TwoDimToFlatIndex(Vector2Int index)
    {
        int idx = index.x * GameConstants.RowSize + index.y;     // x: row, y: column
        if (idx < 0 || idx >= GameConstants.NumberCells)
            throw new IndexOutOfRangeException($"Index {index} is out of range for row size {GameConstants.RowSize}");

        return idx;
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
