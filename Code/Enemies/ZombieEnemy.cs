using Sandbox;

public sealed class ZombieEnemy : Component
{
	[Header( "Stats" )]
	[Property] public float MaxHealth { get; set; } = 50f;
	[Property] public float MoveSpeed { get; set; } = 120f;
	[Property] public int ScoreValue { get; set; } = 100;

	[Header( "Attack" )]
	[Property] public float AttackDamage { get; set; } = 12f;
	[Property] public float AttackRange { get; set; } = 58f;
	[Property] public float StopDistance { get; set; } = 4f;
	[Property] public float AttackCooldown { get; set; } = 0.65f;
	[Property] public float TurnSharpness { get; set; } = 12f;

	public bool IsAlive => Health > 0f;

	private float Health { get; set; }
	private TimeSince TimeSinceAttack { get; set; }

	protected override void OnStart()
	{
		Health = MaxHealth;
		TimeSinceAttack = AttackCooldown;
	}

	protected override void OnUpdate()
	{
		if ( !IsAlive )
			return;

		var target = FindTarget();

		if ( !target.IsValid() )
			return;

		var toTarget = target.WorldPosition - WorldPosition;
		toTarget.z = 0f;

		var distance = toTarget.Length;

		if ( distance <= AttackRange )
			TryAttack( target );

		if ( distance <= 0.01f )
			return;

		var direction = toTarget / distance;

		if ( distance > StopDistance )
		{
			var moveDistance = (MoveSpeed * Time.Delta).Clamp( 0f, distance - StopDistance );
			WorldPosition += direction * moveDistance;
		}

		WorldRotation = Rotation.Lerp( WorldRotation, Rotation.LookAt( direction, Vector3.Up ), TurnSharpness * Time.Delta );
	}

	public void TakeDamage( float amount )
	{
		if ( !IsAlive )
			return;

		Health = (Health - amount).Clamp( 0f, MaxHealth );

		if ( IsAlive )
			return;

		DeadboxGameManager.Instance?.AddKill( ScoreValue );
		GameObject.Destroy();
	}

	private static DeadboxPlayerController FindTarget()
	{
		var local = DeadboxPlayerController.Local;

		if ( local.IsValid() )
			return local;

		return DeadboxPlayerController.All.FirstOrDefault();
	}

	private void TryAttack( DeadboxPlayerController target )
	{
		if ( TimeSinceAttack < AttackCooldown )
			return;

		TimeSinceAttack = 0f;

		var health = target.Components.Get<PlayerHealth>();

		if ( health.IsValid() )
			health.TakeDamage( AttackDamage );
	}
}
