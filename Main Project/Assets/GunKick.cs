using UnityEngine;

public class GunKick : MonoBehaviour
{
    private Vector3 currentRotation, targetRotation, currentPosition, targetPosition, startingPosition;

    [SerializeField]
    private float recoilX;
    [SerializeField]
    private float recoilY;
    [SerializeField]
    private float recoilZ;
    [SerializeField]
    private float kickBackZ;

    private float xrecoil, yrecoil, zrecoil, zkickback, snap, returns;

    [SerializeField]
    private Transform initialPosition;
    private float snappiness = 8f;
    private float returnAmount = 8f;

    void Start()
    {
        startingPosition = initialPosition.transform.localPosition;
        ResetRecoil();

    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        Back();
    }

    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Back()
    {
        targetPosition = Vector3.Lerp(targetPosition, startingPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

    public void ResetRecoil()
    {
        // Reset all recoil and transform values
        targetRotation = Vector3.zero;
        currentRotation = Vector3.zero;
        targetPosition = startingPosition;
        currentPosition = startingPosition;

        transform.localRotation = Quaternion.identity;
        transform.localPosition = startingPosition;

    }

    private void OnEnable()
    {
        ResetRecoil();
    }

    private void OnDisable()
    {
        ResetRecoil();
    }
}