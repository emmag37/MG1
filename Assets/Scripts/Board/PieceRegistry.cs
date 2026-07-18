using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// script is clean and ready to remove event bus

public class PieceRegistry : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject piecePrefab;

    // ==================================================
    // Private Fields
    // ==================================================
    //private GameDataService gameData;
    private PlayerPicker picker;

    private Piece playerPiece;
    private Bounds playerBounds;

    // eventually turn this into an object pool to reuse objects
    private Piece[] pieces = new Piece[GameConstants.RowSize * GameConstants.RowSize];


    // ==================================================
    // Initializer
    // ==================================================

    public void Initialize(bool loadGame, Bounds boardBounds)
    {
        //this.gameData = gameData;

        Vector3 min = boardBounds.min;
        min.y = spawnPoint.position.y;

        playerBounds = boardBounds;
        playerBounds.SetMinMax(min, playerBounds.max);

        picker = new PlayerPicker();

        /*
        if (loadGame)
        {
            IGamePlayData playData = gameData.GetGamePlayData();

            // load the active player
            var colors = new PlayerPicker.PlayerColors { Color = playData.CurrentPlayer, NextColor = playData.NextPlayer };
            SpawnNewPlayer(colors);

            // load the pieces on the board
            LoadBoardPieces(gameData.GetGamePlayData().Board.Cells);
        }
        */
    }


    // ==================================================
    // Public Methods - Player
    // ==================================================

    // optional param to spawn a player with given colors, always returns non-null colors
    public PlayerPicker.PlayerColors SpawnNewPlayer(PlayerPicker.PlayerColors colors = default)
    {
        Debug.Log("spawn new player");

        Debug.Assert(playerPiece == null, "Tried to instantiate a player when one already exists");

        if (colors.Equals(default(PlayerPicker.PlayerColors)))
        {
            colors = picker.CalculateNewPlayerColors();
            //gameData.SavePlayerColors(colors.Color, colors.NextColor);
        }

        playerPiece = Instantiate(piecePrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Piece>();
        playerPiece.InitializeAsPlayer(colors.Color, playerBounds);

        return colors;
    }

    public void PlacePlayer(Vector3 position, Vector2Int index)
    {
        playerPiece.PlacePlayer(position);

        int idx = TwoDimToFlatIndex(index);
        if (playerPiece.Color == CellColor.Mask)
            DestroyPieceAt(idx);

        pieces[idx] = playerPiece;
        playerPiece = null;

        EventBus.Publish(new PlacePlayerEvent { Index = index } );
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
        if (pieces[idx] == null) return false;

        pieces[idx].SetColor(color);
        return true;
    }

    public void ResetPieces()
    {
        picker.Reset();
        if (playerPiece != null) DestroyPlayer();

        for (int i = 0; i < pieces.Length; i++)
        {
            DestroyPieceAt(i);
        }
    }

    public void LoadBoardPieces(IReadOnlyList<CellEntry> cells)
    {
        foreach (CellEntry cell in cells)
        {
            Vector2Int index = new Vector2Int(cell.x, cell.y);
            Vector3 position = BoardGeometry.BoardIndexToTransform(index);

            Piece newPiece = Instantiate(piecePrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Piece>();
            newPiece.InitializeAsCell((CellColor)cell.color, position, playerBounds);

            int flatIndex = TwoDimToFlatIndex(index);
            pieces[flatIndex] = newPiece;
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
        }

        // helper
        void PopPieceAt(int row, int col)
        {
            Piece piece = RemovePieceAt(row, col);
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

        Piece player = RemovePieceAt(index.x, index.y);
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

    private void DestroyPlayer()
    {
        Debug.Assert(playerPiece != null, "Tried to destroy non-existent player");  // triggers on game over

        Destroy(playerPiece.gameObject);
        playerPiece = null;
    }

    private Piece RemovePieceAt(int row, int col)
    {
        int idx = TwoDimToFlatIndex(new Vector2Int(row, col));
        Piece piece = pieces[idx];
        pieces[idx] = null;

        return piece;
    }

    private void DestroyPieceAt(int index)
    {
        if (!pieces[index]) return;

        Vector2Int idx = FlatToTwoDimIndex(index);

        Destroy(pieces[index].gameObject);
        pieces[index] = null;
    }

    

}
