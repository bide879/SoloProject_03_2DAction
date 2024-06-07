using UnityEngine;

public class Factory : Singleton<Factory>
{
    GhostPool ghostPool;
    DashEffectPool dashPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        ghostPool = GetComponentInChildren<GhostPool>();
        dashPool = GetComponentInChildren<DashEffectPool>();
        ghostPool?.Initialize();
        dashPool?.Initialize();
    }

    public GhostChild GetSpownGhost(GameObject ghostPrefab, Vector3 position, Quaternion rotation)
    {
        GhostChild ghost = ghostPool?.GetObject();
        ghost.transform.rotation = rotation;
        ghost.transform.position = position;
        return ghost;
    }

    public DashEffect GetSpownDashEffect(Vector3 position, Quaternion rotation)
    {
        DashEffect dash = dashPool?.GetObject();
        dash.transform.rotation = rotation;
        dash.transform.position = position;
        return dash;
    }
}
