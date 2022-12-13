using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	PhotonView PV;

    bool canUseUlti = false;
    bool recentlyUsedUlti = false;

    //-----------------------------------------------------------------
    //public float targetTime = 5.0f;

    //void Update()
    //{

    //    targetTime -= Time.deltaTime;

    //    if (targetTime <= 0.0f)
    //    {
    //        timerEnded();
    //    }

    //}

    public override void timerEnded()
	{
        if (itemInfo.itemName == "Sniper")
        {

            canUseUlti = true;

            //Debug.Log("Gun: " + itemInfo.itemName + ", end time");
        } 
            
        //targetTime = 5.0f;
    }
    public override bool getUltiStatus()
    {
        if (itemInfo.itemName != "Sniper") return false;

        if (recentlyUsedUlti)
        {
            recentlyUsedUlti = false;
            return true;
        }
        return false;
    }
    //-----------------------------------------------------------------


    void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	public override void Use()
	{
		Debug.Log("Using gun: " + itemInfo.itemName);
		Shoot();
	}

	void Shoot()
	{
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (canUseUlti == false && itemInfo.itemName == "Sniper") return;
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            if (itemInfo.itemName == "Sniper")
            {
                Debug.Log("Ulti");
                canUseUlti = false;
                recentlyUsedUlti = true;
            }

			PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}