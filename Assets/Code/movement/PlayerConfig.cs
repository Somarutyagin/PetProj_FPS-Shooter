using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public float WalkSpeed = 3f;
    public float RunSpeed = 5f;
    public float JumpForce = 5f;
    public float MouseSensitivity = 2f;
    public float AimingFov = 40f;
    public float NormalFov = 60f;
    public float fovScalerRelativelySpeed = 0.2f;

    // Bunnyhop mechanics
    public float AirAcceleration = 10f; // How fast you accelerate in air
    public float GroundFriction = 6f; // Ground friction
    public float AirFriction = 0.1f; // Air friction (much lower)
    public float GroundAcceleration = 15f; // Ground acceleration
}