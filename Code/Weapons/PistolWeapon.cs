using Sandbox;

public sealed class PistolWeapon : WeaponBase
{
	protected override void OnStart()
	{
		base.OnStart();

		WeaponName = "Pistol";
		FireRate = 4.5f;
		ProjectileDamage = 34f;
		ProjectileSpeed = 1000f;
		ProjectileLifeTime = 1.1f;
	}
}

