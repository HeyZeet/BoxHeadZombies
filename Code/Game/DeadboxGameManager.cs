using Sandbox;
using Sandbox.UI.Deadbox;

public sealed class DeadboxGameManager : Component
{
	public static DeadboxGameManager Instance { get; private set; }

	[Header( "Waves" )]
	[Property] public GameObject ZombiePrefab { get; set; }
	[Property] public List<GameObject> SpawnPoints { get; set; } = new();
	[Property] public float TimeBetweenWaves { get; set; } = 4f;
	[Property] public int BaseZombiesPerWave { get; set; } = 6;
	[Property] public int ZombiesAddedPerWave { get; set; } = 3;
	[Property] public float SpawnHeightAbovePlane { get; set; } = 24f;

	[Header( "HUD" )]
	[Property] public bool AutoCreateHud { get; set; } = true;

	public int Wave { get; private set; }
	public int Score { get; private set; }
	public int Kills { get; private set; }
	public bool WaveActive => RemainingToSpawn > 0 || AliveZombies > 0;

	private int RemainingToSpawn { get; set; }
	private int AliveZombies { get; set; }
	private TimeSince TimeSinceWaveEnded { get; set; }
	private TimeSince TimeSinceSpawnedZombie { get; set; }
	private GameObject HudObject { get; set; }

	protected override void OnStart()
	{
		Instance = this;
		TimeSinceWaveEnded = TimeBetweenWaves;

		if ( AutoCreateHud )
			CreateHudIfNeeded();
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
			Instance = null;

		if ( HudObject.IsValid() )
			HudObject.Destroy();
	}

	protected override void OnUpdate()
	{
		AliveZombies = Scene.GetAllComponents<ZombieEnemy>().Count( zombie => zombie.IsValid() && zombie.IsAlive );

		if ( RemainingToSpawn > 0 )
		{
			TrySpawnZombie();
			return;
		}

		if ( AliveZombies <= 0 && TimeSinceWaveEnded > TimeBetweenWaves )
			StartNextWave();
	}

	public void AddKill( int scoreValue )
	{
		Kills++;
		Score += scoreValue;
		TimeSinceWaveEnded = 0f;
	}

	private void CreateHudIfNeeded()
	{
		if ( Scene.GetAllComponents<DeadboxHud>().Any() )
			return;

		HudObject = new GameObject( "Deadbox HUD" );
		HudObject.Components.Create<ScreenPanel>();
		HudObject.Components.Create<DeadboxHud>();
	}

	private void StartNextWave()
	{
		Wave++;
		RemainingToSpawn = BaseZombiesPerWave + (Wave - 1) * ZombiesAddedPerWave;
		TimeSinceSpawnedZombie = 99f;

		Log.Info( $"Deadbox wave {Wave} started. Zombies={RemainingToSpawn}" );
	}

	private void TrySpawnZombie()
	{
		if ( TimeSinceSpawnedZombie < 0.25f )
			return;

		TimeSinceSpawnedZombie = 0f;
		RemainingToSpawn--;

		var spawnPoint = GetSpawnPoint();
		var position = GetSpawnPosition( spawnPoint );
		var zombieObject = ZombiePrefab.IsValid()
			? ZombiePrefab.Clone( position )
			: new GameObject( "Zombie" );

		zombieObject.WorldPosition = position;

		if ( !zombieObject.Components.Get<ZombieEnemy>().IsValid() )
			zombieObject.Components.Create<ZombieEnemy>();
	}

	private GameObject GetSpawnPoint()
	{
		if ( SpawnPoints.Count == 0 )
			return null;

		return SpawnPoints[Game.Random.Int( 0, SpawnPoints.Count - 1 )];
	}

	private Vector3 GetSpawnPosition( GameObject spawnPoint )
	{
		var position = spawnPoint.IsValid() ? spawnPoint.WorldPosition : GetFallbackSpawnPosition();
		position.z = SpawnHeightAbovePlane;
		return position;
	}

	private static Vector3 GetFallbackSpawnPosition()
	{
		var side = Game.Random.Int( 0, 3 );
		var x = Game.Random.Float( -850f, 850f );
		var y = Game.Random.Float( -600f, 600f );

		return side switch
		{
			0 => new Vector3( -900f, y, 0f ),
			1 => new Vector3( 900f, y, 0f ),
			2 => new Vector3( x, -650f, 0f ),
			_ => new Vector3( x, 650f, 0f )
		};
	}
}
