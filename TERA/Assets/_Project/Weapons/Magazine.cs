using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("Munitions")]
    public int maxAmmo = 30;
    public int currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
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