using UnityEngine;

public class BitOperationDemo : MonoBehaviour
{
    int lights = 0;              // 000 (처음엔 전부 꺼짐)
    public GameObject[] cubes;   // 큐브 3개 연결

    void Update()
    {
        // OR: 첫 전등 "무조건 켜기"
        if (Input.GetKeyDown(KeyCode.Alpha1))
            lights = lights | (1 << 0);        // 001

        // AND: 첫 전등 "무조건 끄기"
        if (Input.GetKeyDown(KeyCode.Alpha2))
            lights = lights & ~(1 << 0);       // ~001 = ...111110
                                               // AND 하면 해당 비트만 0

        // XOR: 첫 전등 "토글(반전)"
        if (Input.GetKeyDown(KeyCode.Alpha3))
            lights = lights ^ (1 << 0);        // 0↔1 뒤집기

        // NOT: 전체 반전 (보여주기용)
        if (Input.GetKeyDown(KeyCode.Alpha4))
            lights = ~lights;                  // 000→111, 101→010 …

        // 화면에 반영
        for (int i = 0; i < cubes.Length; i++)
        {
            bool on = (lights & (1 << i)) != 0; // i번째 전등이 켰는지 확인
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
