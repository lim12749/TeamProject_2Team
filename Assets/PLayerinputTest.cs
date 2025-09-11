using UnityEngine;

public class PLayerinputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("¾È³ç???????");
        }

        transform.Rotate(0, Input.GetAxis("Horizontal") * 10, 0);
        transform.Rotate(Input.GetAxis("Vertical") * 10, 0, 0);
    }
}
