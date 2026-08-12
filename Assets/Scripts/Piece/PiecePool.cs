using UnityEngine;
using System;


public interface ILinkable<TSelf, TData> where TSelf : ILinkable<TSelf, TData>
{
    TSelf Next { get; set; }
    TSelf Prev { get; set; }

    void Initialize(TData data);
}

public class GameObjectPool<TObj, TData> where TObj : Component, ILinkable<TObj, TData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private TObj[] objects;

    private TObj head = null;
    private TObj tail = null;

    // ==================================================
    // Constructors
    // ==================================================
    public GameObjectPool(int size, GameObject prefab, TData data)  // optional params: maybe parent/other values for instantiate
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), "Object pool requires a valid prefab.");

        if (!prefab.TryGetComponent<TObj>(out _))
            throw new ArgumentException($"Prefab '{prefab.name}' has no {typeof(TObj)} component.", nameof(prefab));

        objects = new TObj[size];
        for (int i = 0; i < size; i++)
        {
            objects[i] = UnityEngine.Object.Instantiate(prefab).GetComponent<TObj>();
            objects[i].Initialize(data);
            AppendToFreeList(objects[i]);
        }
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public bool TryGetObject(out TObj obj)
    {
        obj = GetFreeObject();
        return obj != null;
    }

    public bool RemoveObject(TObj obj)
    {
        if (obj == null || obj == head || obj.Next != null || obj == tail)  // test to make sure it catches all items in free list, and none outside of it - move these to append free list
        {
            Debug.LogError("[GameObjectPool] Attempted to remove a null object");
            return false;
        }

        return AppendToFreeList(obj);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private bool AppendToFreeList(TObj obj)
    {
        Debug.Assert(obj != null, "[GameObjectPool] Attempted to add a null object to free list");  // should never happen

        if (obj == head || obj.Next != null || obj == tail)
        {
            Debug.LogError("[GameObjectPool] Attempted a double free");
            return false;
        }

        if (head == null)
        {
            head = obj;
            obj.Prev = null;
        }
        else
        {
            tail.Next = obj;
            obj.Prev = tail;
        }

        obj.Next = null;
        tail = obj;
        obj.gameObject.SetActive(false);

        return true;
    }

    private TObj GetFreeObject()
    {
        if (head == null)
        {
            Debug.LogWarning($"[GameObjectPool] No available objects - all {objects.Length} objects in use");
            return null;
        }

        // removes and returns the head of the free list
        TObj obj = head;
        head = head.Next;

        if (head == null)  // no more pieces in the list
            tail = null;
        else
            head.Prev = null;

        obj.Next = null;
        obj.gameObject.SetActive(true);

        return obj;
    }
}


/*
public class PiecePool
{
    // ==================================================
    // Constants
    // ==================================================
    private const int PlayerIdx = GameConstants.NumberCells;

    // ==================================================
    // Private Fields
    // ==================================================
    private Piece[] pieces = new Piece[GameConstants.NumberCells + 1];  // game board + player

    private Piece head = null;
    private Piece tail = null;


    // ==================================================
    // Public Methods
    // ==================================================

    // create all game pieces upon board creation
    public PiecePool(GameObject piecePrefab, Bounds boardBounds, Vector3 spawnPoint)
    {
        // note: add the try catch to bootstrap
        if (piecePrefab == null)
            throw new ArgumentNullException(nameof(piecePrefab), "PiecePool requires a valid prefab.");

        if (!piecePrefab.TryGetComponent<Piece>(out _))
            throw new ArgumentException($"Prefab '{piecePrefab.name}' has no Piece component.", nameof(piecePrefab));


        // eager initialization for all pieces
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i] = UnityEngine.Object.Instantiate(piecePrefab).GetComponent<Piece>();
            pieces[i].InitializeComponents(boardBounds, spawnPoint);            
            pieces[i].gameObject.SetActive(false);                              // hidden

            AppendToFreeList(pieces[i]);
        }
    }

    public Piece CreatePlayer(CellColor color)
    {
        Piece player = GetFreePiece();  // need to null check
        if (player == null)
        {
            Debug.LogError("[Piece Pool] No available pieces - active pieces exceeds maximum");
            return null;
        }

        player.SetToPlayer(color);          

        return player;
    }

    public Piece CreateCell(CellColor color, Vector3 position)
    {
        Piece cell = GetFreePiece();        // need to null check
        if (cell == null)
        {
            Debug.LogError("[PiecePool] No available pieces - active pieces exceeds maximum");
            return null;
        }

        cell.SetToCell(color, position);

        return cell;
    }

    public void Remove(Piece piece)
    {
        if (piece == null)
        {
            Debug.LogError("[PiecePool] Attempted to remove a null piece");
            return;
        }
        else if (piece == head || piece.Next != null || piece == tail)
        {
            Debug.LogError("[PiecePool] Piece has already been removed");
        }

        piece.gameObject.SetActive(false);
        AppendToFreeList(piece);
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private void AppendToFreeList(Piece piece)
    {
        Debug.Assert(piece != null, "[PiecePool] Attempted to add null piece to free list");

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
            Debug.Log("[PiecePool] No free pieces");
            return null;
        }

        // return the head of free list, removing it from the free list
        Piece piece = head;
        head = head.Next;

        if (head == null)  // no more pieces in the list
            tail = null;
        else
            head.Prev = null;

        piece.Next = null;
        piece.gameObject.SetActive(true);

        return piece;
    }
}
*/
