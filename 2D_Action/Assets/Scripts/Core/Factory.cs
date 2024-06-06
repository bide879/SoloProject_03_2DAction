using UnityEngine;

public class Factory : Singleton<Factory>
{
    GhostPool ghostPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        ghostPool = GetComponentInChildren<GhostPool>();
        ghostPool?.Initialize();
    }
    //this.ghost, this.transform.position, this.transform.rotation
    public GhostChild GetSpownGhost(GameObject ghostPrefab, Vector3 position, Quaternion rotation)
    {
        GhostChild ghost = ghostPool?.GetObject();
        ghost.transform.rotation = rotation;
        ghost.transform.position = position;
        return ghost;
    }
}
