using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private Vector3 moveVector;
    private Vector3 savedPos;
    private Quaternion savedRot;
    private bool freeze;
    Animator anim;

    [SerializeField] private Camera myCam;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotation = 80f;


    public Player photonPlayer;
    public static PlayerController instance;

    [PunRPC]
    public void Initialize(Player player, string playerPrefabLocation)
    {
        photonPlayer = player;
        Debug.Log("player --> " + player);

        if (!photonView.IsMine)
        {
           myCam.gameObject.SetActive(false);
        }
        else
        {
           myCam.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {   
        anim = GetComponent<Animator>();
        if(instance == null || instance == this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (photonPlayer.IsLocal)
        {
            moveVector = Vector3.zero;
            if (controller.isGrounded == false)
            {
                moveVector += Physics.gravity;
            }
            controller.Move(moveVector * Time.deltaTime);
            Movements(0, 0);
        }
    }

    public void Movements(int num1, int num2)
    {
        if (freeze == true) return;
        float hori = Input.GetAxis("Horizontal");
        if (hori == 0) hori = num1;
        float verti = Input.GetAxis("Vertical");
        if (verti == 0) verti = num2;

        if (hori > 0)
        {
            transform.Rotate(0, hori * rotation * Time.deltaTime, 0);
        }
        else if (hori < 0)
        {
            transform.Rotate(0, hori * rotation * Time.deltaTime, 0);
        }

        if (verti < 0 || verti > 0){
            anim.SetBool("Run",true); 
            Vector3 move = transform.TransformDirection(new Vector3(0f, 0f, verti));
            controller.Move( move * Time.deltaTime * speed);
        }else{
            anim.SetBool("Run",false); 
        }
    }

     public void Sleep(Vector3 vector, Vector3 vector2, Vector3 vector3, Boolean boole)
     {         
        if (freeze == true) return;
        savedPos = transform.position;
        savedRot = transform.rotation;
        anim.SetBool("Run",false); 
        freeze = true;
        controller.center = vector3;
        transform.rotation = Quaternion.Euler(vector2);
        transform.position = vector;
        if (boole == true){
            transform.GetChild(4).gameObject.GetComponent<ThirdPersonCamera>().enabled = false;
            transform.GetChild(4).localRotation = Quaternion.Euler(20,180,0);
            transform.GetChild(4).localPosition = new Vector3(0,1,3);
        }
     }
     public void Wake()
     {       
        controller.center = new Vector3(0f, 0.45f, 0f);
        freeze = false;
        transform.rotation = savedRot;
        transform.position = savedPos;
        transform.GetChild(4).gameObject.GetComponent<ThirdPersonCamera>().enabled = true;
        transform.GetChild(4).localRotation = Quaternion.Euler(20,0,0);
     }

}
