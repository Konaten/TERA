using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("Munitions")]
    public int maxAmmo = 30;
    public int currentAmmo = -1; 

    void Start()
    {
        // On ne remplit que si le vendeur n'a pas déjà mis de balles
        if (currentAmmo == -1)
        {
            currentAmmo = maxAmmo;
        }
    }

    public bool TryUseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            return true;
        }
        return false;
    }
}