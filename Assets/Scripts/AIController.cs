using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


//public class PlayerController : MonoBehaviour
public class AIController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject ui; 

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] Item[] items;

    [SerializeField] Image healthBarImage;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;

    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView pv;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        //if (!pv.IsMine)
        //{
        //    Destroy(GetComponentInChildren<Camera>().gameObject);
        //    Destroy(rb);
        //}

        if (pv.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    void Update()
    {
        if (!pv.IsMine) { return; }
        Look();
        Move();
        Jump();


        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < -10f) // Die if you fall out of the world
        {
            Die();
        }
    }

    public void SetGroundedState(bool __grounded)
    {
        grounded = __grounded;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }


    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }


    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) { return; }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {

        currentHealth -= damage;
        healthBarImage.fillAmount = currentHealth / maxHealth;
        
        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();  
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
