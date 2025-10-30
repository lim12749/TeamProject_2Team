using UnityEngine;

public class BitLightDemo : MonoBehaviour
{
    // 3개의 전등 비트: 1<<0(0001), 1<<1(0010), 1<<2(0100)
    int lights = 0;
    public GameObject[] cubes; // 큐브 3개 드래그

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) lights ^= (1 << 0); // 첫 큐브
        if (Input.GetKeyDown(KeyCode.Alpha2)) lights ^= (1 << 1); // 두 번째
        if (Input.GetKeyDown(KeyCode.Alpha3)) lights ^= (1 << 2); // 세 번째
        if (Input.GetKeyDown(KeyCode.Alpha4)) lights ^= (1 << 3); // 네 번째

        // (선택) R키로 전부 끄기
        if (Input.GetKeyDown(KeyCode.R)) lights = 0;

        // 큐브 색 업데이트
        for (int i = 0; i < cubes.Length; i++)
        {
            bool on = (lights & (1 << i)) != 0;
            cubes[i].GetComponent<Renderer>().material.color = on ? Color.yellow : Color.gray;
        }

        // (선택) 디버그: 현재 비트 상태를 2진수로 보기
        if (Input.anyKeyDown)
        {
            string bin = System.Convert.ToString(lights, 2).PadLeft(4, '0');
            Debug.Log($"lights(bin)={bin}  dec={lights}");
        }
    }
}
