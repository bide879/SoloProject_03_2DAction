using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Factory : Singleton<Factory>
{
    GhostPool ghostPool;
    DashEffectPool dashPool;
    SkillEffectPool skillPool;
    SkillHitEffectPool skillHitEffectPool;
    UltimateEffectPool ultimateEffectPool;
    MarkPool markPool;
    Enemy_01_BulletPool enemy01_bulletPool;
    Enemy_01_BulletHitPool enemy01_bulletHitPool;
    Enemy_02_BulletPool enemy02_bulletPool;
    Enemy_02_BulletHitPool enemy02_bulletHitPool;
    BossAttack01BulletPool bossAttack01BulletPool;
    BossAttack03BulletPool bossAttack03BulletPool;
    BossAttack04BulletPool bossAttack04BulletPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 플레이어
        ghostPool = GetComponentInChildren<GhostPool>();
        dashPool = GetComponentInChildren<DashEffectPool>();
        skillPool = GetComponentInChildren<SkillEffectPool>();
        skillHitEffectPool = GetComponentInChildren<SkillHitEffectPool>();
        ultimateEffectPool = GetComponentInChildren<UltimateEffectPool>();
        markPool = GetComponentInChildren<MarkPool>();

        // 적
        enemy01_bulletPool = GetComponentInChildren<Enemy_01_BulletPool>();
        enemy01_bulletHitPool = GetComponentInChildren<Enemy_01_BulletHitPool>();
        enemy02_bulletPool = GetComponentInChildren<Enemy_02_BulletPool>();
        enemy02_bulletHitPool = GetComponentInChildren<Enemy_02_BulletHitPool>();

        // 보스
        bossAttack01BulletPool = GetComponentInChildren<BossAttack01BulletPool>();
        bossAttack03BulletPool = GetComponentInChildren<BossAttack03BulletPool>();
        bossAttack04BulletPool = GetComponentInChildren<BossAttack04BulletPool>();

        ghostPool?.Initialize();
        dashPool?.Initialize();
        skillPool?.Initialize();
        skillHitEffectPool?.Initialize();
        ultimateEffectPool?.Initialize();
        markPool?.Initialize();

        enemy01_bulletPool?.Initialize();
        enemy01_bulletHitPool?.Initialize();
        enemy02_bulletPool?.Initialize();
        enemy02_bulletHitPool?.Initialize();

        bossAttack01BulletPool?.Initialize();
        bossAttack03BulletPool?.Initialize();
        bossAttack04BulletPool?.Initialize();
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

    public SkillHitEffect GetSpownSkillHitEffect(Vector3 position, Quaternion rotation)
    {
        SkillHitEffect skillHit = skillHitEffectPool?.GetObject();
        skillHit.transform.rotation = rotation;
        skillHit.transform.position = position;
        return skillHit;
    }

    public UltimateEffect GetSpownUltimateEffect()
    {
        UltimateEffect Ultimate = ultimateEffectPool?.GetObject();
        Ultimate.transform.position = new Vector3 (0,0,-1);
        return Ultimate;
    }

    public Mark GetSpownMark(GameObject target)
    {
        Mark mark = markPool?.GetObject();
        mark.target = target;
        mark.transform.position = target.transform.position;
        return mark;
    }

    public Enemy_01_Bullet GetSpownEnemy01_Bullet(Vector3 position, float forward, float damage)
    {
        Enemy_01_Bullet bullet = enemy01_bulletPool?.GetObject();
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * forward, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.transform.position = position;
        bullet.forward = forward;
        bullet.damage = damage;
        return bullet;
    }

    public Enemy_01_BulletHit GetSpownEnemy01_BulletHit(Vector3 position)
    {
        Enemy_01_BulletHit bulletHit = enemy01_bulletHitPool?.GetObject();
        bulletHit.transform.position = position;
        return bulletHit;
    }

    public Enemy_02_Bullet GetSpownEnemy02_Bullet(Vector3 position, float forward, float damage)
    {
        Enemy_02_Bullet bullet = enemy02_bulletPool?.GetObject();
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * forward, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.transform.position = position;
        bullet.forward = forward;
        bullet.damage = damage;
        return bullet;
    }

    public Enemy_02_BulletHit GetSpownEnemy02_BulletHit(Vector3 position)
    {
        Enemy_02_BulletHit bulletHit = enemy02_bulletHitPool?.GetObject();
        bulletHit.transform.position = position;
        return bulletHit;
    }

    public BossAttack01Bullet GetSpownBossAttack01Bullet(Vector3 position, float forward, float damage)
    {
        BossAttack01Bullet bullet = bossAttack01BulletPool?.GetObject();
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * forward, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.transform.position = position;
        bullet.forward = forward;
        bullet.damage = damage;
        return bullet;
    }

    public BossAttack03Bullet GetSpownBossAttack03Bullet(Vector3 position, float forward, float damage)
    {
        BossAttack03Bullet bullet = bossAttack03BulletPool?.GetObject();
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * forward, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.transform.position = position;
        bullet.forward = forward;
        bullet.damage = damage;
        return bullet;
    }

    public BossAttack04Bullet GetSpownBossAttack04Bullet(Vector3 position, float forward, float damage)
    {
        BossAttack04Bullet bullet = bossAttack04BulletPool?.GetObject();
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * forward, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.transform.position = position;
        bullet.forward = forward;
        bullet.damage = damage;
        return bullet;
    }
}
