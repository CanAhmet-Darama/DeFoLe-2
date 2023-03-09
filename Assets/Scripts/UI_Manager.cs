using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI curAmmoText;
    public TextMeshProUGUI totalAmmoText;

    MainCharacter mainChar;

    void Start()
    {
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
    }

    void Update()
    {
        curAmmoText.text = Convert.ToString(mainChar.currentWeapon.currentAmmo);
        totalAmmoText.text = Convert.ToString(mainChar.ammoCounts[(int)mainChar.currentWeapon.weaponType]);
    }
}
