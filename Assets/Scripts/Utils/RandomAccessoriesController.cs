using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAccessoriesController : MonoBehaviour 
{
  public GameObject helmetPivot, weaponPivot, shieldPivot;
	// Use this for initialization
	void Start () 
	{
		GameObject additionalAccessoriesGO = Resources.Load("AdditionalAccessories") as GameObject;
		AdditionalAccessories accessories = additionalAccessoriesGO.GetComponent<AdditionalAccessories>();

		this.WearRandomHelmet(accessories);
		this.WearRandomWeapon (accessories);
    this.WearRandomShield(accessories);
	}

	private void WearRandomHelmet(AdditionalAccessories accessories)
	{
		if(helmetPivot == null) return;
		GameObject randomHelmet = accessories.GetRandomHelmet();
		if(randomHelmet != null)
		{
			GameObject randomHelmetOnScene = GameObject.Instantiate(randomHelmet, helmetPivot.transform);
			randomHelmetOnScene.transform.localPosition = Vector3.zero;
			randomHelmetOnScene.transform.localRotation = Quaternion.identity;
		}
	}

	private void WearRandomWeapon(AdditionalAccessories accessories)
	{
		if(weaponPivot == null) return;
		GameObject randomWeapon = accessories.GetRandomWeapon();

		GameObject randomWeaponOnScene = GameObject.Instantiate(randomWeapon, weaponPivot.transform);
		randomWeaponOnScene.transform.localPosition = Vector3.zero;
		randomWeaponOnScene.transform.localRotation = Quaternion.identity;

	}

  private void WearRandomShield(AdditionalAccessories accessories)
  {
    if (shieldPivot == null) return;
    GameObject randomShield = accessories.GetRandomShield();
    if(randomShield != null)
    {
      GameObject randomShieldOnScene = GameObject.Instantiate(randomShield, shieldPivot.transform);
      randomShieldOnScene.transform.localPosition = Vector3.zero;
      randomShieldOnScene.transform.localRotation = Quaternion.identity;
    }
  }


}
