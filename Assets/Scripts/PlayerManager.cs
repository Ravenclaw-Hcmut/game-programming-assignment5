using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;

	GameObject controller;

	int kills;
	int deaths;

	private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (pv.IsMine)
        {
            CreateController();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
		//Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		//controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
	}


	public void Die()
	{
		//PhotonNetwork.Destroy(controller);
		//CreateController();

		//deaths++;

		//Hashtable hash = new Hashtable();
		//hash.Add("deaths", deaths);
		//PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}


	public void GetKill()
	{
		//PV.RPC(nameof(RPC_GetKill), PV.Owner);
	}

	[PunRPC]
	void RPC_GetKill()
	{
		//kills++;

		//Hashtable hash = new Hashtable();
		//hash.Add("kills", kills);
		//PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}

	//public static PlayerManager Find(Player player)
	//{
		//return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
	//}
}
