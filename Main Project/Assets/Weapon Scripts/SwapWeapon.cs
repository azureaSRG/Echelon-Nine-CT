using UnityEngine;

public class SwapWeapon : MonoBehaviour
{
    public int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        HandleWeaponSwitch();
    }

    void SelectWeapon()
    {
        int index = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(index == selectedWeapon);
            index++;
        }
    }

    void HandleWeaponSwitch()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon = (selectedWeapon + 1) % transform.childCount;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeapon--;
            if (selectedWeapon < 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }
}
