using Sandbox;

public sealed class PlayerHealth : Component
{
	[Property] public float MaxHealth { get; set; } = 100f;
	[Property] public float HitCooldown { get; set; } = 0.35f;

	public float Health { get; private set; }
	public bool IsAlive => Health > 0f;

	private TimeSince TimeSinceHit { get; set; }

	protected override void OnStart()
	{
		Health = MaxHealth;
		TimeSinceHit = HitCooldown;
	}

	public void TakeDamage( float amount )
	{
		if ( !IsAlive || TimeSinceHit < HitCooldown )
			return;

		Health = (Health - amount).Clamp( 0f, MaxHealth );
		TimeSinceHit = 0f;

		if ( !IsAlive )
			Log.Info( "Deadbox player died." );
	}
}

