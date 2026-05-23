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

	protected override void OnStart()
	{
		TimeSinceSpawned = 0f;
	}

	protected override void OnUpdate()
	{
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

			if ( WorldPosition.Distance( zombie.WorldPosition ) > HitRadius )
				continue;

			zombie.TakeDamage( Damage );
			return true;
		}

		return false;
	}
}

