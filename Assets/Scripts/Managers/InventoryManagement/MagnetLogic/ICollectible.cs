using UnityEngine;

public interface ICollectible
{
    public Rigidbody2D Rigidbody2D();

    public bool HasTarget();

    public bool WasDropped();

    public Vector3 TargetPosition();

    public Vector3 TargetPosition(Vector3 position);

    public void ActivateTracking();

    public void DeactivateTracking();
}
