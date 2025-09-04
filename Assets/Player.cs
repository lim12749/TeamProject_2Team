using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x= Input.GetAxis("Horizontal");
        float z= Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(x, 0, z);
        rb.linearVelocity = moveDirection * speed;
    }
}
