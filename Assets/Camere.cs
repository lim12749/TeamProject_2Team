using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThirdCamera : MonoBehaviour
{
    public GameObject target;
    public float offsetX;
    public float offsetY;
    public float offsetZ;

    private float fixedY; // 고정 y값

    void Start()
    {
        // 카메라의 시작 y값을 저장
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