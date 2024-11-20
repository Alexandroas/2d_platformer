using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollMultiplier = 0.1f; // Adjust this to control scroll speed
    [SerializeField] private PlayerController playerController; // Reference to player
    private Material backgroundMaterial;

    void Start()
    {
        // Get the material from the renderer
        backgroundMaterial = GetComponent<Renderer>().material;
        
        // Find player if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    void Update()
    {
        if (playerController != null)
        {
            var (isMoving, playerSpeed) = playerController.GetMovementInfo();
            
            if (isMoving)
            {
                // Calculate scroll amount based on player speed
                float scrollAmount = playerSpeed * scrollMultiplier * Time.deltaTime;
                
                // Update texture offset
                Vector2 offset = backgroundMaterial.mainTextureOffset;
                offset.x += scrollAmount;
                backgroundMaterial.mainTextureOffset = offset;
            }
        }
    }
}