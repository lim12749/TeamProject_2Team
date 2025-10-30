using UnityEngine;


public class WeaponController : MonoBehaviour
{
    public WeaponBase currentWeapon;
    public PlayerInputReader input;

    void Start()
    {
         LockCursor(); 
    }
    void Update()
    {
        if (input.IsFiring && currentWeapon != null && currentWeapon.CanFire() && input.IsAiming)
        {
            Debug.Log("무기 발사 시도");
            currentWeapon.Fire();
        }
        
        // ESC 키를 누르면
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
    }
        void LockCursor()
    {
        // 마우스 커서를 잠그고 보이지 않게 합니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Debug.Log("마우스 커서 잠김 및 숨김.");
    }

    void UnlockCursor()
    {
        // 마우스 커서를 풀고 보이게 합니다.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Debug.Log("마우스 커서 잠금 해제 및 표시.");
    }
}
