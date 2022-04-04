using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class localHeadMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 normalizedDirections;

    // Start is called before the first frame update
    void Start()
    {

    }

    void SetTargetPosition()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float point = 0f;
        if (plane.Raycast(ray, out point))
        {
            targetPosition = ray.GetPoint(point);
        }
    }
    private Vector2 NormalizedDirection(Vector3 end)
    {
        Vector2 startVector = new Vector2(transform.position.x, transform.position.z);
        Vector2 endVector = new Vector2(end.x, end.z);
        Vector2 normalized = endVector - startVector;

        return normalized.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SetTargetPosition();

        }


        normalizedDirections = NormalizedDirection(targetPosition);
        float horizontal = normalizedDirections.x;
        float vertical = normalizedDirections.y;

        Vector3 move = new Vector3(horizontal, 90f, vertical);
        var rotationTemp = Quaternion.LookRotation(move);

        float eulerAngleY = rotationTemp.eulerAngles.y;

        if (eulerAngleY > 0f && eulerAngleY < 22.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
        if (eulerAngleY > 22.5f && eulerAngleY < 67.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 45, 0);
        }
        if (eulerAngleY > 67.5f && eulerAngleY < 112.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 90, 0);
        }
        if (eulerAngleY > 112.5f && eulerAngleY < 157.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 135, 0);
        }
        if (eulerAngleY > 157.5f && eulerAngleY < 202.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 180, 0);
        }
        if (eulerAngleY > 202.5f && eulerAngleY < 247.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 225, 0);
        }
        if (eulerAngleY > 247.5f && eulerAngleY < 292.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 270, 0);
        }
        if (eulerAngleY > 292.5f && eulerAngleY < 337.5f)
        {
            transform.rotation = Quaternion.Euler(-90, 315, 0);
        }
        if (eulerAngleY > 337.5f && eulerAngleY < 360f)
        {
            transform.rotation = Quaternion.Euler(-90, 360, 0);
        }

    }
}
