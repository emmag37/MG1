using UnityEngine;

// creates empty pieces to be used as either a player or a cell
// note: all pieces in this class will have game object set active to false

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

    private Piece head;
    private Piece tail;


    // ==================================================
    // Public Methods
    // ==================================================

    // create all game pieces upon board creation
        // this is good
    public void Initialize(GameObject piecePrefab, Bounds boardBounds, Vector3 spawnPoint)
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

    public Piece Create()
    {
        // return the head of free list, removing it from the free list
        Piece piece = head;

        head = piece.Next;
        piece.Next = null;
        head.Prev = null;

        return piece;
    }

    public void Remove(Piece piece)
    {
        Debug.Assert(!piece.gameObject.activeSelf, "piece must be turned off"); // might replace with something else later

        AppendToFreeList(piece);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private void AppendToFreeList(Piece piece)
    {
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
}
