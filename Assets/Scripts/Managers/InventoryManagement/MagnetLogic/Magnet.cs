using UnityEngine;

public class Magnet : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();

        if (collectible != null)
        {
            collectible.ActivateTracking();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();

        if (collectible != null)
        {
            collectible.DeactivateTracking();
        }
    }
}
