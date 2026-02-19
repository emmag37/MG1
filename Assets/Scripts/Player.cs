using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Set the player's sprite to a new one */
    public void SetSprite(Sprite newSprite)
    {
        sr.sprite = newSprite;
    }
}
