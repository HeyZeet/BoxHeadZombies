using Sandbox;
using System;

public class TopDownCamera : Component
{
	[Property] public DeadboxPlayerController Target { get; set; }
	[Property] public bool Orthographic { get; set; } = true;
	[Property] public float OrthographicHeight { get; set; } = 950f;

	[Header( "Framing" )]
	[Property] public Vector3 Offset { get; set; } = new( -420f, -520f, 760f );
	[Property] public Angles CameraAngles { get; set; } = new( 60f, 38f, 0f );
	[Property] public Vector2 DeadZoneHalfSize { get; set; } = new( 180f, 120f );
	[Property] public float FollowSharpness { get; set; } = 8f;

	private CameraComponent Camera { get; set; }
	private Vector3 FocusPosition { get; set; }
	private Vector3 DesiredFocusPosition { get; set; }

	protected override void OnStart()
	{
		Camera = Components.Get<CameraComponent>();
		ApplyCameraSettings();

		if ( Target.IsValid() )
			DesiredFocusPosition = Target.WorldPosition;
		else
			DesiredFocusPosition = WorldPosition - Offset;

		FocusPosition = DesiredFocusPosition;
	}

	protected override void OnUpdate()
	{
		if ( !Target.IsValid() )
			Target = DeadboxPlayerController.Local;

		if ( !Target.IsValid() )
			return;

		ApplyCameraSettings();
		UpdateFocusPosition();

		var desiredPosition = FocusPosition + Offset;
		WorldPosition = Vector3.Lerp( WorldPosition, desiredPosition, GetFollowT() );
		WorldRotation = CameraAngles.ToRotation();
	}

	private void UpdateFocusPosition()
	{
		var targetPosition = Target.WorldPosition;
		var delta = targetPosition - DesiredFocusPosition;

		if ( delta.x > DeadZoneHalfSize.x )
			DesiredFocusPosition = DesiredFocusPosition.WithX( targetPosition.x - DeadZoneHalfSize.x );
		else if ( delta.x < -DeadZoneHalfSize.x )
			DesiredFocusPosition = DesiredFocusPosition.WithX( targetPosition.x + DeadZoneHalfSize.x );

		if ( delta.y > DeadZoneHalfSize.y )
			DesiredFocusPosition = DesiredFocusPosition.WithY( targetPosition.y - DeadZoneHalfSize.y );
		else if ( delta.y < -DeadZoneHalfSize.y )
			DesiredFocusPosition = DesiredFocusPosition.WithY( targetPosition.y + DeadZoneHalfSize.y );

		FocusPosition = Vector3.Lerp( FocusPosition, DesiredFocusPosition, GetFollowT() );
	}

	private float GetFollowT()
	{
		return 1f - MathF.Exp( -FollowSharpness * Time.Delta );
	}

	private void ApplyCameraSettings()
	{
		if ( !Camera.IsValid() )
			Camera = Components.Get<CameraComponent>();

		if ( !Camera.IsValid() )
			return;

		Camera.IsMainCamera = true;
		Camera.Orthographic = Orthographic;
		Camera.OrthographicHeight = OrthographicHeight;
	}
}

