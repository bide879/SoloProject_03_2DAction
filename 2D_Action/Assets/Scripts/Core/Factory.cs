using UnityEngine;

public class Factory : Singleton<Factory>
{
    GhostPool ghostPool;
    DashEffectPool dashPool;
    SkillEffectPool skillPool;
    UltimateEffectPool ultimateEffectPool;
    MarkPool markPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        ghostPool = GetComponentInChildren<GhostPool>();
        dashPool = GetComponentInChildren<DashEffectPool>();
        skillPool = GetComponentInChildren<SkillEffectPool>();
        ultimateEffectPool = GetComponentInChildren<UltimateEffectPool>();
        //markPool = GetComponentInChildren<MarkPool>();

        ghostPool?.Initialize();
        dashPool?.Initialize();
        skillPool?.Initialize();
        ultimateEffectPool?.Initialize();
        //markPool?.Initialize();

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

    public SkillEffect GetSpownSkillEffect(Vector3 position, Quaternion rotation)
    {
        SkillEffect skill = skillPool?.GetObject();
        skill.transform.rotation = rotation;
        skill.transform.position = position;
        return skill;
    }

    public UltimateEffect GetSpownUltimateEffect()
    {
        UltimateEffect Ultimate = ultimateEffectPool?.GetObject();
        Ultimate.transform.position = new Vector3 (0,0,-1);
        return Ultimate;
    }
}
