using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TintMaterial))]
public abstract class AUnit : MonoBehaviour
{
    public int TeamNumber = 0;

    [SerializeField]
    private float _Emission = 15f;
    
    [SerializeField]
    private Laser _LaserPrefab;

    [SerializeField] 
    private float _Health = 100f;

    [SerializeField] 
    private float _AttackDamage = 8f;

    protected bool _IsAlive;
    protected Rigidbody _RB;
    protected Animator _Anim;
    protected Eye[] _Eyes;

    protected abstract void UnitAwake();
    protected void Awake()
    {
        _IsAlive = true;
        _RB = GetComponent<Rigidbody>();
        _Anim = GetComponent<Animator>();
        _Eyes = GetComponentsInChildren<Eye>();

        SetTeam(TeamNumber);

        UnitAwake();
    }

    public bool GetIsAlive() 
    {
        return _IsAlive;
    }

    public void SetTeam(int team)
    {
        TeamNumber = team;
        Color teamColor = GameManager.Instance.TeamColors[TeamNumber];
        GetComponent<TintMaterial>().ApplyTintToMaterials(teamColor, _Emission);
    }

    protected bool CanSee(Vector3 hitPos, Transform other) 
    {
        foreach (var eye in _Eyes) 
        {
            Vector3 startPos = eye.transform.position;
            Vector3 direction = hitPos - startPos;
            Ray ray = new Ray(startPos, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == other) 
            {
                return true;
            }
        }
        return false;
    }


    protected void ShootLasersFromEyes(Vector3 hitPos, Transform other) 
    {
        foreach (var eye in _Eyes) 
        {
            Instantiate(_LaserPrefab).Shoot(eye.transform.position, hitPos);
        }

        AUnit otherUnit = other.GetComponent<AUnit>();
        if (otherUnit != null && otherUnit.TeamNumber != TeamNumber) 
        {
            otherUnit.OnHit(_AttackDamage);
        }
    }

    public void OnHit(float damage) 
    {
        _Health -= damage;
        if (_Health <=0f) 
        {
            _Health = 0f;
            Die();
        }
    }

    public virtual void Die()
    {
        _IsAlive = false;
        _Anim.SetBool("IsAlive", false);
    }
}
