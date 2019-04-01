using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : AUnit
{
	[Space, Header("Attack")]

	[SerializeField]
	private float _AttackRadius = 5f;

	[SerializeField]
	private float _PushRadius = 2f;

	[SerializeField, Tooltip("Time in Seconds")]
	private float _AttackCD = 1f;

	[SerializeField]
	private LayerMask _UnitsLayerMask;

	private NavMeshAgent _Agent;

    private float _AgentNormalSpeed; // Initial speed of the AI Agent 
    private IEnumerator _CurrentState = null;
	private Outpost _TargetOutpost = null;
	private AUnit _TargetEnemy = null;
    private Healpost _TargetHealpost = null;

	protected override void UnitAwake()
	{
		_Agent = GetComponent<NavMeshAgent>();
        _AgentNormalSpeed = _Agent.speed;
	}

	private void Start()
	{
		SetState(State_Idle());
	}

	private void Update()
	{
		_Anim.SetFloat("ForwardMovement", _Agent.velocity.magnitude);
	}

	public override void Die() 
	{
		base.Die();
		SetState(State_Dead());
		_Agent.isStopped = true;
		_Agent.ResetPath();
		_TargetOutpost = null;
		Destroy(GetComponent<Collider>());
	}

	#region StateMachine

		private void SetState(IEnumerator newState)
		{
			if(_CurrentState != null)
			{
				StopCoroutine(_CurrentState);
			}

			_CurrentState = newState;
			StartCoroutine(_CurrentState);
		}

		private IEnumerator State_Idle()
		{
			while(_TargetOutpost == null)
			{
				LookForOutpost();
				LookForEnemy();
				yield return null;
			}

			SetState(State_MovingToOutpost());
		}

		private IEnumerator State_MovingToOutpost()
		{
			_Agent.SetDestination(_TargetOutpost.transform.position);
			while(_Agent.remainingDistance > _Agent.stoppingDistance)
			{
				LookForEnemy();
				yield return null;
			}

			SetState(State_CapturingOutpost());
		}

		private IEnumerator State_CapturingOutpost()
		{
			while(_TargetOutpost.CurrentTeam != TeamNumber || _TargetOutpost.CaptureValue < 1f)
			{
				LookForEnemy();
				yield return null;
			}

			_TargetOutpost = null;
			SetState(State_Idle());
		}
        
        // When the AI reaches that state, the AI will move to the HealingPost
        private IEnumerator State_MovingToHeal() 
        {
            // increasing the speed of the AI when it needs to heal
            _Agent.speed = _AgentNormalSpeed * 5;
            // Setting the destination to a heal post
            _Agent.SetDestination(_TargetHealpost.transform.position);
            while (_Agent.remainingDistance > _Agent.stoppingDistance)
            {
                yield return null;
            }
            // When the AI reaches the HealPost, changing state to "Healing"
            SetState(State_Healing());

        }
        // Healing State
        private IEnumerator State_Healing()
        {
            // Returning the speed of the agent to the selected one.
            _Agent.speed = _AgentNormalSpeed;
            while (GetHealth() < GetMaxHealth())
            {
                yield return null;
            }
            // When the Health is equal to MaxHealth, the state will return to Idle
            _TargetHealpost = null;
            SetState(State_Idle());
        }
        
        private IEnumerator State_Dead() 
		{
			yield return null;
		}

		private IEnumerator State_AttackingEnemy() 
		{
			_Agent.isStopped = true;
			_Agent.ResetPath();

			float shootTimer = 0f;
		
			while(_TargetEnemy !=null && _TargetEnemy.GetIsAlive()) 
			{
				shootTimer += Time.deltaTime;
				transform.LookAt(_TargetEnemy.transform);
				transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
				if (shootTimer >= _AttackCD) 
				{
					shootTimer = 0;
					ShootLasersFromEyes(_TargetEnemy.transform.position + Vector3.up, _TargetEnemy.transform);
				}
                // Checks if the Health of the AI is lower than 33%
                CheckingIfHealthIsLow();
                
                yield return null;
			}
			
			_TargetEnemy = null;
            SetState(State_Idle());
		}

    #endregion StateMachine
    // if the health of the AI is lower than 33%, the AI will shoot the crazy bomb
    private void CheckingIfHealthIsLow()
    {
        if (GetHealth() < GetMaxHealth() / 3)
        {
            // Shoot CrazyBomb and Look for the Healing Post
            ShootCrazyBomb();
            LookForHealPost();
            
            if (_TargetHealpost!= null)
            {
                _TargetEnemy = null;
                _TargetOutpost = null;
                // Set the State to Moving To Heal
                SetState(State_MovingToHeal());
            }
        }
    }

    private void LookForEnemy()
	{
		var aroundMe = Physics.OverlapSphere(transform.position, _AttackRadius, _UnitsLayerMask);
		foreach (var item in aroundMe) 
		{
			var otherUnit = item.GetComponent<AUnit>();
			if(otherUnit != null && otherUnit.TeamNumber != TeamNumber && otherUnit.GetIsAlive()) 
			{
				_TargetEnemy = otherUnit;
				SetState(State_AttackingEnemy());
				return;
			}
		}
	}

	private void LookForHealPost() 
	{
        // Gets the first and only healpost
        var healPost = Healpost.HealpostList[0];
		if (healPost != null) 
        {
            // Assign the healpost to the _targetHealPost
            _TargetHealpost = healPost;
        }
        else 
        {
            return;
        }
    }

	private void LookForOutpost()
	{
		var outpost = Outpost.OutpostList.GetRandomOutpost();
		if(outpost == null) return;
		if(outpost.CurrentTeam != TeamNumber || outpost.CaptureValue < 1f)
		{
			_TargetOutpost = outpost;
		}
		else
		{
			_TargetOutpost = null;
		}
	}

}
    