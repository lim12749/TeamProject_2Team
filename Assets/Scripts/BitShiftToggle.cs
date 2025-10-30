using UnityEngine;

public class BitShiftToggle : MonoBehaviour
{
    int lights = 0;
    public GameObject[] cubes;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            lights = lights ^ (1 << 0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            lights = lights ^ (1 << 1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            lights = lights ^ (1 << 2);
        if (Input.GetKeyDown(KeyCode.R))
            lights = 0; // 리셋

        for (int i = 0; i < cubes.Length; i++)
        {
            bool on = (lights & (1 << i)) != 0;
            cubes[i].GetComponent<Renderer>().material.color = on ? Color.yellow : Color.gray;
        }
    }
}
