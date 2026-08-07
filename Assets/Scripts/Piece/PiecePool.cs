using UnityEngine;

// can create players/cells (all of the same type piece)
    // consider subclassing, this is starting to get confusing

public class PiecePool
{
    // ==================================================
    // Constants
    // ==================================================
    private const int PlayerIdx = GameConstants.RowSize * GameConstants.RowSize;

    // ==================================================
    // Private Fields
    // ==================================================
    private Piece[] pieces = new Piece[GameConstants.RowSize * GameConstants.RowSize + 1];  // game board + player

    private Piece head = null;
    private Piece tail = null;


    // ==================================================
    // Public Methods
    // ==================================================

    // create all game pieces upon board creation
    public PiecePool(GameObject piecePrefab, Bounds boardBounds, Vector3 spawnPoint)
    {
        // eager initialization for all pieces
        for (int i = 0; i < pieces.Length; i++)
        {
            // create the piece
            pieces[i] = Object.Instantiate(piecePrefab).GetComponent<Piece>();
            pieces[i].InitializeComponents(boardBounds, spawnPoint);
            pieces[i].gameObject.SetActive(false);  // hidden

            AppendToFreeList(pieces[i]);
        }
    }

    public Piece CreatePlayer(CellColor color)
    {
        Piece player = GetFreePiece();
        
        player.InitializeAsPlayer(color);

        return player;
    }

    public Piece CreateCell(CellColor color, Vector3 position)
    {
        Piece cell = GetFreePiece();
        cell.InitializeAsCell(color, position);

        return cell;
    }

    public void Remove(Piece piece)
    {
        if (piece == null) return;      // do this for now, more robust null check in the future

        piece.gameObject.SetActive(false);      // crash here

        AppendToFreeList(piece);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private void AppendToFreeList(Piece piece)
    {
        Debug.Log("append to free list");

        if (head == null)
        {
            head = piece;
            piece.Prev = null;
        }
        else
        {
            tail.Next = piece;
            piece.Prev = tail;
        }


        piece.Next = null;
        tail = piece;
    }

    private Piece GetFreePiece()
    {
        if (head == null)
        {
            Debug.Log("no new pieces");
            return null;
        }

        // return the head of free list, removing it from the free list
        Piece piece = head;

        head = piece.Next;
        piece.Next = null;
        head.Prev = null;

        piece.gameObject.SetActive(true);

        return piece;
    }
}
