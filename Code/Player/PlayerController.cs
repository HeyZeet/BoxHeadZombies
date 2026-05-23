using Sandbox;

public sealed class DeadboxPlayerController : Component
{
	private static readonly List<DeadboxPlayerController> AllPlayers = new();

	public static IReadOnlyList<DeadboxPlayerController> All => AllPlayers;
	public static DeadboxPlayerController Local { get; private set; }

	[Header( "Movement" )]
	[Property] public float MoveSpeed { get; set; } = 260f;
	[Property] public float SprintMultiplier { get; set; } = 1.25f;
	[Property] public Vector2 ArenaHalfSize { get; set; } = new( 900f, 650f );

	[Header( "Combat" )]
	[Property] public WeaponBase StartingWeapon { get; set; }

	public Vector3 AimDirection { get; private set; } = Vector3.Forward;
	public bool IsLocalPlayer => !GameObject.Network.Active || GameObject.Network.IsOwner;

	private WeaponBase CurrentWeapon { get; set; }

	protected override void OnStart()
	{
		if ( !AllPlayers.Contains( this ) )
			AllPlayers.Add( this );

		if ( IsLocalPlayer )
			Local = this;

		CurrentWeapon = StartingWeapon;

		if ( !CurrentWeapon.IsValid() )
			CurrentWeapon = Components.Get<WeaponBase>( FindMode.EverythingInSelfAndDescendants );
	}

	protected override void OnDestroy()
	{
		AllPlayers.Remove( this );

		if ( Local == this )
			Local = null;
	}

	protected override void OnUpdate()
	{
		if ( !IsLocalPlayer )
			return;

		UpdateMovement();
		UpdateWeapon();
	}

	private void UpdateMovement()
	{
		var input = GetMoveInput();

		if ( input.Length > 1f )
			input = input.Normal;

		var speed = Input.Down( "Run" ) ? MoveSpeed * SprintMultiplier : MoveSpeed;
		var movement = new Vector3( input.x, input.y, 0f ) * speed * Time.Delta;

		WorldPosition = ClampToArena( WorldPosition + movement );

		if ( input.Length > 0.01f )
		{
			AimDirection = new Vector3( input.x, input.y, 0f ).Normal;
			WorldRotation = Rotation.LookAt( AimDirection, Vector3.Up );
		}
	}

	private void UpdateWeapon()
	{
		if ( !CurrentWeapon.IsValid() )
			return;

		CurrentWeapon.Owner = this;

		if ( Input.Down( "Attack1" ) || Input.Down( "attack1" ) )
			CurrentWeapon.TryPrimaryAttack();
	}

	private static Vector2 GetMoveInput()
	{
		var input = Vector2.Zero;

		if ( Input.Down( "Forward" ) ) input.y += 1f;
		if ( Input.Down( "Backward" ) ) input.y -= 1f;
		if ( Input.Down( "Left" ) ) input.x -= 1f;
		if ( Input.Down( "Right" ) ) input.x += 1f;

		return input;
	}

	private Vector3 ClampToArena( Vector3 position )
	{
		position.x = position.x.Clamp( -ArenaHalfSize.x, ArenaHalfSize.x );
		position.y = position.y.Clamp( -ArenaHalfSize.y, ArenaHalfSize.y );
		return position;
	}
}

