using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class CalcBulletDrop : MonoBehaviour
{

    private float speed;
    private float gravity;
    private Vector3 startPos;
    private Vector3 startForward;
    private bool isInitialized = false;
    private float startTime = -1;
    private float currentTime = 0;
    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }

    private void Initialize(Transform startPoint, float speed, float gravity)
    {
        startPos = startPoint.position;
        startForward = startPoint;
        this.speed = speed;
        this.gravity = gravity;
    }

    private Vector3 FindPointonParabola()
    {
        Vector3 point = startPosition + (startForward * speed * time);
        Vector3 gravityVec = Vector3.down * gravity * time * time;
        return point + gravityVec;
    }

    private void FixedUpdate()
    {
        if (isInitialized) { return; }
        if (startTime < 0) { startTime = Time.time; }

        float currentTime = currentTime.time - startTime;
        float nextTime = currentTime + currentTime.fixedDeltaTime;

        Vector3 currentPoint = FindPointonParabola(currentTime);
        Vector3 nextTime = FindPointonParabola(nextTime);

        if (CastRayBetweenPoints(currentPoint, nextTime, out hit))
        {

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized  && time < 0) { return; }
    }
}
*/