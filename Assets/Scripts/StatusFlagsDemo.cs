using System;
using UnityEngine;

[Flags]
public enum Status
{
    None    = 0,
    Poison  = 1 << 0, // 중독: 초당 -5
    Burn    = 1 << 1, // 화상: 초당 -7
    Freeze  = 1 << 2, // 빙결: 이동불가 느낌 (여기선 데미지 없음)
    Regen   = 1 << 3, // 재생: 초당 +4
}

public class StatusFlagsDemo : MonoBehaviour
{
    float hp = 100f;
    Status s = Status.None;

    void Update()
    {
        // 상태 주기
        if (Input.GetKeyDown(KeyCode.P)) Add(Status.Poison); // 중독
        if (Input.GetKeyDown(KeyCode.B)) Add(Status.Burn);   // 화상
        if (Input.GetKeyDown(KeyCode.F)) Add(Status.Freeze); // 빙결
        if (Input.GetKeyDown(KeyCode.R)) Add(Status.Regen);  // 재생

        // 해제/치료
        if (Input.GetKeyDown(KeyCode.H)) Heal(); // 힐: Poison 해제
        if (Input.GetKeyDown(KeyCode.C)) ClearAll();

        // 1초 틱(계산)
        if (Input.GetKeyDown(KeyCode.T)) Tick();

        if (Input.anyKeyDown) Print();
    }

    void Add(Status x)    { s |= x; }
    void Remove(Status x) { s &= ~x; }
    bool Has(Status x)    { return (s & x) != 0; }

    void Heal()
    {
        // 요구사항: 중독이면 힐로 치료
        if (Has(Status.Poison))
        {
            Remove(Status.Poison);
            Debug.Log("힐로 중독이 치료되었습니다!");
        }
        // 선택 규칙: 힐은 화상엔 즉효 없음(필요시 여기에 추가)
    }

    void ClearAll() { s = Status.None; }

    void Tick()
    {
        // 상호작용: 불이 빙결을 녹인다
        if (Has(Status.Burn) && Has(Status.Freeze))
        {
            Remove(Status.Freeze);
            Debug.Log("불이 빙결을 녹였습니다!");
        }

        // 초당 변화
        if (Has(Status.Poison)) hp -= 5f;
        if (Has(Status.Burn))   hp -= 7f;
        if (Has(Status.Regen))  hp += 4f;

        hp = Mathf.Clamp(hp, 0, 100);
    }

    void Print()
    {
        string bin = Convert.ToString((int)s, 2).PadLeft(4,'0');
        Debug.Log($"HP={hp}  Status={s}  bin={bin}");
    }
}
