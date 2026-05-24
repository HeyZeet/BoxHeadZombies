using Sandbox;

public sealed class Projectile : Component
{
	[Property] public float Speed { get; set; } = 900f;
	[Property] public float Damage { get; set; } = 25f;
	[Property] public float HitRadius { get; set; } = 28f;
	[Property] public float LifeTime { get; set; } = 1.5f;

	public Vector3 Direction { get; set; } = Vector3.Forward;
	public GameObject Owner { get; set; }

	private TimeSince TimeSinceSpawned { get; set; }
	private Vector3 PreviousPosition { get; set; }

	protected override void OnStart()
	{
		TimeSinceSpawned = 0f;
		PreviousPosition = WorldPosition;
	}

	protected override void OnUpdate()
	{
		PreviousPosition = WorldPosition;
		WorldPosition += Direction.Normal * Speed * Time.Delta;

		if ( TryHitZombie() || TimeSinceSpawned > LifeTime )
			GameObject.Destroy();
	}

	private bool TryHitZombie()
	{
		foreach ( var zombie in Scene.GetAllComponents<ZombieEnemy>() )
		{
			if ( !zombie.IsValid() || !zombie.IsAlive )
				continue;

			if ( zombie.GameObject == Owner )
				continue;

			if ( GetDistanceToProjectilePath( zombie.WorldPosition ) > HitRadius )
				continue;

			zombie.TakeDamage( Damage );
			return true;
		}

		return false;
	}

	private float GetDistanceToProjectilePath( Vector3 point )
	{
		var start = new Vector2( PreviousPosition.x, PreviousPosition.y );
		var end = new Vector2( WorldPosition.x, WorldPosition.y );
		var target = new Vector2( point.x, point.y );
		var segment = end - start;
		var segmentLengthSquared = segment.LengthSquared;

		if ( segmentLengthSquared <= 0.001f )
			return target.Distance( end );

		var targetAlongSegment = Vector2.Dot( target - start, segment ) / segmentLengthSquared;
		var closestPoint = start + segment * targetAlongSegment.Clamp( 0f, 1f );

		return target.Distance( closestPoint );
	}
}

