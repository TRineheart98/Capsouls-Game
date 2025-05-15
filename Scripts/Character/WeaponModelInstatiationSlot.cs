using UnityEngine;

public class WeaponModelInstatiationSlot : MonoBehaviour
{
    public WeaponModelSlot weaponModelSlot;
    public GameObject currentWeaponModel;

    public void UnloadWeaponModel()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(GameObject weaponModel)
    {
        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
    }


}
