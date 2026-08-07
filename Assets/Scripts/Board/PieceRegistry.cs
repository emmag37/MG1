using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// replace all create/destroy with a call to pool
    // this script will no longer need the board bounds/spawn point

// i'd like for this to no longer be a monobehaviour

public class PieceRegistry : MonoBehaviour
{
    public PlayerColors Colors => currentColors;

    // ==================================================
    // Inspector Fields
    // ==================================================
    //[SerializeField] private Transform spawnPoint;
    //[SerializeField] private GameObject piecePrefab;

    // ==================================================
    // Private Fields
    // ==================================================
    private PlayerColors currentColors = new PlayerColors();    // initializes with next set to a color, player empty

    private Piece playerPiece;
    private Bounds playerBounds;

    private Piece[] registry = new Piece[GameConstants.RowSize * GameConstants.RowSize];  // registry

    private PiecePool pool; // pool where objects are stored in memory


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(Bounds boardBounds, Transform spawnPoint, GameObject piecePrefab)
    {
        // calculate spawn point and bounds
        Scaler.ApplyScaledYPos(spawnPoint);

        Vector3 min = boardBounds.min;
        min.y = spawnPoint.position.y;

        playerBounds = boardBounds;
        playerBounds.SetMinMax(min, playerBounds.max);

        // initialize memory
        pool = new PiecePool(piecePrefab, playerBounds, spawnPoint.position);

        currentColors.Reset();      // initializes the 'next' color
    }

    public void LoadGame(PlayerColors colors, IReadOnlyList<CellEntry> cells)
    {
        currentColors.NextColor = colors.NextColor;
        SpawnNewPlayer(colors.PlayerColor);

        LoadBoardPieces(cells);
    }


    // ==================================================
    // Public Methods - Player
    // ==================================================

    // next should always be set
    public CellColor SpawnNewPlayer(CellColor? color = null)
    {
        if (!color.HasValue)
        {
            color = currentColors.NextColor;
            currentColors.NextColor = PlayerPicker.ChooseColor();
        }

        currentColors.PlayerColor = color.Value;

        playerPiece = pool.CreatePlayer(color.Value);

        return currentColors.NextColor;
    }

    public void PlacePlayer(Vector3 position, Vector2Int index)
    {
        playerPiece.PlacePlayer(position);

        int idx = TwoDimToFlatIndex(index);
        if (playerPiece.Color == CellColor.Mask)
            pool.Remove(registry[idx]);

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
            pool.Remove(playerPiece);
            playerPiece = null;
        }

        for (int i = 0; i < registry.Length; i++)
        {
            Piece piece = registry[i];
            registry[i] = null;

            pool.Remove(piece);
        }
    }

    public void LoadBoardPieces(IReadOnlyList<CellEntry> cells)
    {
        foreach (CellEntry cell in cells)
        {
            Vector2Int index = new Vector2Int(cell.x, cell.y);
            Vector3 position = BoardGeometry.BoardIndexToTransform(index);

            Piece newPiece = pool.CreateCell((CellColor)cell.color, position);

            int flatIndex = TwoDimToFlatIndex(index);
            registry[flatIndex] = newPiece;
        }
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

            pool.Remove(piece); // destroy the piece
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
