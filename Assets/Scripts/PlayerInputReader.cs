using UnityEngine;
using UnityEngine.Animations.Rigging;
/// <summary>
/// 레거시 Input 방식으로 키보드와 마우스 입력을 저장하는 클래스입니다.
/// TPS용 기본 입력값 (이동, 점프, 마우스 회전)을 제공합니다.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool IsAiming => isAiming;
    public bool isAiming;
    private bool isFiring;

    public bool IsFiring => isFiring;

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public Rig aimLayer;
    float aimDuration = 0.3f;
    void Update()
    {
        MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        JumpPressed = Input.GetButtonDown("Jump");

        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
        isFiring = Input.GetButton("Fire1");
        isAiming = Input.GetMouseButton(1);
        /*
        if (Input.GetMouseButton(1))
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }*/
        aimLayer.weight = 1.0f; // Aim 레이어의 가중치를 1로 설정하여 항상 활성화 상태로 유지합니다.
    }

    /*
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public float MouseX { get; private set; }

    void Update()
    {
        // 이동 입력 저장 (A/D, W/S)
        float x = Input.GetAxis("Horizontal");  
        float y = Input.GetAxis("Vertical");
        MoveInput = new Vector2(x, y);

        // 마우스 X축 움직임 (좌우 회전용)
        MouseX = Input.GetAxis("Mouse X");

        // 점프 키 (Spacebar)
        JumpPressed = Input.GetButtonDown("Jump");
    }*/
}