using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public MenuManager skin;
    [Header("Status")]
    public bool gameEnded = false;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    //public PlayerController[] players;
    private int playersInGame;
    private int num;

    private void Start()
    {
        //playerPrefabLocation = different characters
        skin = FindObjectOfType<MenuManager>();
        Debug.Log(skin.selectedSkin);

        if (skin.selectedSkin == 0) {playerPrefabLocation = "Player1";num = 0;}
        if (skin.selectedSkin == 1) {playerPrefabLocation = "Player2";num = 3;}
        if (skin.selectedSkin == 2) {playerPrefabLocation = "Player3";num = 3;}
        if (skin.selectedSkin == 3) {playerPrefabLocation = "Player4";num = 1;}
        if (skin.selectedSkin == 4) {playerPrefabLocation = "Player5";num = 2;}
        //GameObject toReplace = GameObject.Find(playerPrefabLocation);
        //toReplace.SetActive(false);
        
		if(photonView.IsMine){
            PhotonNetwork.InstantiateRoomObject("Player1NPC", spawnPoints[0].position, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Player2NPC", spawnPoints[3].position, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Player3NPC", spawnPoints[3].position, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Player4NPC", spawnPoints[1].position, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("Player5NPC", spawnPoints[2].position, Quaternion.identity);
        }
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[num].position, Quaternion.identity);

        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, playerPrefabLocation);
    }
}
