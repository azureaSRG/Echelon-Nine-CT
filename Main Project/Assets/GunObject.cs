using UnityEngine;

[CreateAssetMenu(fileName = "GunObject", menuName = "Guns/GunObject", order = 0)]
public class GunObject : ScriptableObject
{
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public GunConfig Configuration;
    public TrailConfig TrailObject;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    //private ObjectPool<TrailRenderer> TrailPool;
}
