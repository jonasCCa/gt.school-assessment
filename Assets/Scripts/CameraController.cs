using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform targetStack;
    public Vector3 distanceFromTarget;
    public Vector3 targetRot;
    
    [SerializeField] bool isRotating;
    public float orbitSpeed;
    
    [SerializeField] bool isMoving;
    public float moveTime;

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                isRotating = true;
            if (Input.GetKeyUp(KeyCode.Mouse1))
                isRotating = false;

            if (isRotating)
                transform.RotateAround(targetStack.position, Vector3.up, Input.GetAxis("Mouse X") * orbitSpeed);
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        targetStack = newTarget;

        StartCoroutine(LerpPosition(targetStack.position + distanceFromTarget));
    }

    IEnumerator LerpPosition(Vector3 targetPos)
    {
        isMoving = true;

        float time = 0;
        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;
        while (time < moveTime)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, time / moveTime);
            transform.rotation = Quaternion.Lerp(startingRot, Quaternion.Euler(targetRot), time / moveTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRot);
        isMoving = false;
    }
}
