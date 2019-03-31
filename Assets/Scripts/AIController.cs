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

	[SerializeField, Tooltip("Time in Seconds")]
	private float _CrazyBulletsCD = 10f;

	[SerializeField]
	private LayerMask _UnitsLayerMask;

	private NavMeshAgent _Agent;
	private IEnumerator _CurrentState = null;
	private Outpost _TargetOutpost = null;
	private AUnit _TargetEnemy = null;

	protected override void UnitAwake()
	{
		_Agent = GetComponent<NavMeshAgent>();
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
				EnemyCloser();
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
				EnemyCloser();
				yield return null;
			}

			SetState(State_CapturingOutpost());
		}

		private IEnumerator State_CapturingOutpost()
		{
			while(_TargetOutpost.CurrentTeam != TeamNumber || _TargetOutpost.CaptureValue < 1f)
			{
				LookForEnemy();
				EnemyCloser();
				yield return null;
			}

			_TargetOutpost = null;
			SetState(State_Idle());
		}

		private IEnumerator State_PushingEnemy() 
		{
			_Agent.isStopped = true;
			_Agent.ResetPath();

			float shootTimer = 0f;
			while(_TargetEnemy != null && _TargetEnemy.GetIsAlive()) 
			{
				shootTimer += Time.deltaTime;
				transform.LookAt(_TargetEnemy.transform);
				transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
				if (shootTimer >= _AttackCD) 
				{
					shootTimer = 0;
					//rigidbody.velocity = new Vector3(speedX, speedY, speedZ);
					_TargetEnemy.GetComponent<Rigidbody>().AddForce(new Vector3(1000f, 1000f, 1500f));
					Debug.Log("AI EM HERE");
				}
				yield return null;
			}
			
			_TargetEnemy = null;
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
			float crazyBombTimer = 10f;
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

				// if the health of the AI is lower than 33%, the AI will shoot the crazy bomb with crazy bullet
				if (GetHealth() < GetMaxHealth() /3 && crazyBombTimer >= _CrazyBulletsCD) 
				{
					crazyBombTimer = 0f;
					ShootCrazyBomb();
				}

				yield return null;
			}
			
			_TargetEnemy = null;
            SetState(State_Idle());
		}
		
	#endregion StateMachine

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

	private void EnemyCloser() 
	{
		// If the enemy is too close, push the enemy away.
		var closerToMe = Physics.OverlapSphere(transform.position, _PushRadius, _UnitsLayerMask);
		foreach (var item in closerToMe) 
		{
			var otherUnit = item.GetComponent<AUnit>();
			if(otherUnit != null && otherUnit.TeamNumber != TeamNumber && otherUnit.GetIsAlive()) 
			{
				_TargetEnemy = otherUnit;
				SetState(State_PushingEnemy());
				return;
			}
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
