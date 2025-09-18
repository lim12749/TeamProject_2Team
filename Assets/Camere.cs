using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThirdCamera : MonoBehaviour
{
    public GameObject target;
    public float offsetX;
    public float offsetY;
    public float offsetZ;

    private float fixedY; // ���� y��

    void Start()
    {
        // ī�޶��� ���� y���� ����
        fixedY = target.transform.position.y + offsetY;
    }

    void Update()
    {
        Vector3 FixedPos = new Vector3(
            target.transform.position.x + offsetX,
            fixedY,
            target.transform.position.z + offsetZ
        );
        transform.position = FixedPos;
    }
}   