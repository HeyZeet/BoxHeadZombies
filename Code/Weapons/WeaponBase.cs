using Sandbox;

public abstract class WeaponBase : Component
{
	[Header( "Weapon" )]
	[Property] public string WeaponName { get; set; } = "Weapon";
	[Property] public float FireRate { get; set; } = 4f;
	[Property] public GameObject ProjectilePrefab { get; set; }
	[Property] public float ProjectileSpeed { get; set; } = 900f;
	[Property] public float ProjectileDamage { get; set; } = 25f;
	[Property] public float ProjectileLifeTime { get; set; } = 1.4f;
	[Property] public Vector3 MuzzleOffset { get; set; } = new( 34f, 0f, 24f );

	public DeadboxPlayerController Owner { get; set; }

	private TimeSince TimeSincePrimaryAttack { get; set; }

	protected override void OnStart()
	{
		TimeSincePrimaryAttack = 999f;
	}

	public bool TryPrimaryAttack()
	{
		if ( TimeSincePrimaryAttack < 1f / FireRate )
			return false;

		TimeSincePrimaryAttack = 0f;
		PrimaryAttack();
		return true;
	}

	protected virtual void PrimaryAttack()
	{
		if ( !Owner.IsValid() )
			Owner = Components.Get<DeadboxPlayerController>( FindMode.EverythingInSelfAndAncestors );

		if ( !Owner.IsValid() )
			return;

		SpawnProjectile( Owner.AimDirection );
	}

	protected void SpawnProjectile( Vector3 direction )
	{
		var spawnPosition = Owner.WorldPosition + Owner.WorldRotation * MuzzleOffset;
		var projectileObject = ProjectilePrefab.IsValid()
			? ProjectilePrefab.Clone( spawnPosition, Rotation.LookAt( direction, Vector3.Up ) )
			: new GameObject( "Pistol Projectile" );

		projectileObject.WorldPosition = spawnPosition;
		projectileObject.WorldRotation = Rotation.LookAt( direction, Vector3.Up );

		var projectile = projectileObject.Components.Get<Projectile>() ?? projectileObject.Components.Create<Projectile>();
		projectile.Direction = direction.Normal;
		projectile.Speed = ProjectileSpeed;
		projectile.Damage = ProjectileDamage;
		projectile.LifeTime = ProjectileLifeTime;
		projectile.Owner = Owner.GameObject;
	}
}

