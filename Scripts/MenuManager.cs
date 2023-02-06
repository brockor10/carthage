using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public GameObject mainMenu;    
    public Button createRoomBtn;
    public Text playerList;
    public List<GameObject> skins = new List<GameObject>();
    public int selectedSkin = 0;

    private void Start()
    {
        createRoomBtn.interactable = false;
        // Config Photon
        if (!PhotonNetwork.IsConnected){
            PhotonNetwork.AutomaticallySyncScene = true;
            // Connection to the Photon master server
            PhotonNetwork.ConnectUsingSettings();
            //something
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");
    }

    public override void OnConnectedToMaster()
    {
        createRoomBtn.interactable = true;
    }
    
    void SetMenu(GameObject menu)
    {
        mainMenu.SetActive(true);
    }

    public void NextOption()
    {
       skins[selectedSkin].SetActive(false);
       selectedSkin = selectedSkin + 1;
       if (selectedSkin == skins.Count){
           selectedSkin = 0;
       }
       skins[selectedSkin].SetActive(true);
    }

    public void BackOption()
    {
       skins[selectedSkin].SetActive(false);
       selectedSkin = selectedSkin - 1;
       if (selectedSkin < 0){
           selectedSkin = (skins.Count-1);
       }
       skins[selectedSkin].SetActive(true);
    }

    public void OnCreateRoomBtn()
    {
        NetworkManager.instance.JoinOrCreateRoom("bruh");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("joined room");
            //PhotonNetwork.LoadLevel("Game");
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.MasterClient, "Game");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("left room");
    }


}
