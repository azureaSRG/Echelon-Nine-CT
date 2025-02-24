using UnityEngine;

public class SwapWeapon : MonoBehaviour
{
    public int selectedWeapon = 0;

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
            ModularGunSystem modularGunSystem = weapon.GetComponent<ModularGunSystem>();
            bool isSelectable = modularGunSystem == null || modularGunSystem.selection;

            weapon.gameObject.SetActive(index == selectedWeapon && isSelectable);
            index++;
        }
    }

    void HandleWeaponSwitch()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            do
            {
                selectedWeapon = (selectedWeapon + 1) % transform.childCount;
            } while (!IsWeaponSelectable(selectedWeapon));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.C) && !(Input.GetAxis("Mouse ScrollWheel") < 0f && Input.GetKeyDown(KeyCode.C)))
        {
            do
            {
                selectedWeapon--;
                if (selectedWeapon < 0)
                {
                    selectedWeapon = transform.childCount - 1;
                }
            } while (!IsWeaponSelectable(selectedWeapon));
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private bool IsWeaponSelectable(int index)
    {
        Transform weapon = transform.GetChild(index);
        ModularGunSystem modularGunSystem = weapon.GetComponent<ModularGunSystem>();
        return modularGunSystem == null || modularGunSystem.selection;
    }
}