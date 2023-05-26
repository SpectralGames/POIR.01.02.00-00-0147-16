using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalAccessories : MonoBehaviour
{
  public GameObject[] helmetList;
  public GameObject[] weaponList;
  public GameObject[] shieldList;


  public GameObject GetRandomHelmet()
  {
    int randomHelmet = Random.Range(0, helmetList.Length + 1);
    if (randomHelmet < helmetList.Length)
      return helmetList[randomHelmet];
    else
      return null;
  }

  public GameObject GetRandomWeapon()
  {
    int randomWeapon = Random.Range(0, weaponList.Length);
    return weaponList[randomWeapon];
  }

  public GameObject GetRandomShield()
  {
    int randomShield = Random.Range(0, shieldList.Length);
    return shieldList[randomShield];
  }


}
