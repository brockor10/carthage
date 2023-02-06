using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class NPC : MonoBehaviourPunCallbacks
{
    private GameObject place;
    private GameObject tmp;
    OpenClose floor;
    OpenClose floor2;

    public bool block;
    public int Sec;
    public bool bathCount;
    public bool dinner;
    public bool startTime;
    private float radius;
    private Vector3 rotation;
    private Vector3 collise;
    private bool Str;

    private Vector3 savedPos;
    private Quaternion savedRot;
    private bool sleeping;
    Animator anim;
    private NavMeshAgent agent;
    public List<GameObject> list = new List<GameObject>();
    private GameObject saveGameObj;
    public GameObject chair;
    public GameObject bed;
    private Vector3 spawn;

    private void Awake()
    {   
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
        spawn = new Vector3(-53.18f, 0.50f, -3.05f);
        place = GameObject.Find("carthage");
        floor = FindChildByName(place, "bathroom_floor").GetComponent<OpenClose>();
        floor2 = FindChildByName(place, "bathroom_floor2").GetComponent<OpenClose>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (gameObject.name.Contains("Player1NPC")){
            list.Add(FindChildByName(place, "chair_archille"));
            list.Add(FindChildByName(place, "milie_chair.001"));
            list.Add(FindChildByName(place, "chair.006"));
            list.Add(FindChildByName(place, "chair.009"));
            bed = FindChildByName(place, "bed1");
            chair = FindChildByName(place, "chair.006");
        }
        if (gameObject.name.Contains("Player2NPC")){
            list.Add(FindChildByName(place, "Panel17.002"));
            list.Add(FindChildByName(place, "milie_chair.003"));
            list.Add(FindChildByName(place, "chair.002"));
            list.Add(FindChildByName(place, "DoorGlass.002"));
            bed = FindChildByName(place, "bed2");
            chair = FindChildByName(place, "chair.002");
        }
        if (gameObject.name.Contains("Player3NPC")){
            list.Add(FindChildByName(place, "window2"));
            list.Add(FindChildByName(place, "milie_chair.002"));
            list.Add(FindChildByName(place, "chair.001"));
            list.Add(FindChildByName(place, "Cube.061"));
            bed = FindChildByName(place, "bed2");
            chair = FindChildByName(place, "chair.001");
        }
        if (gameObject.name.Contains("Player4NPC")){
            list.Add(FindChildByName(place, "chair_archille (1)"));
            list.Add(FindChildByName(place, "milie_chair.003"));
            list.Add(FindChildByName(place, "chair.005"));
            list.Add(FindChildByName(place, "Drawer_Mid.001_Cube.054"));
            bed = FindChildByName(place, "bed4");
            chair = FindChildByName(place, "chair.005");
        }
        if (gameObject.name.Contains("Player5NPC")){
            list.Add(FindChildByName(place, "chair_archille (2)"));
            list.Add(FindChildByName(place, "milie_chair.003"));
            list.Add(FindChildByName(place, "chair.003"));
            list.Add(FindChildByName(place, "Door_Cube.001"));
            bed = FindChildByName(place, "bed3");
            chair = FindChildByName(place, "chair.003");
        }
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(Sec);
        startTime = true;
        block = false;
    }

    void Update()
    {
        if (photonView.IsMine){
            if (sleeping == false) agent.enabled = true;
            if (GameObject.Find("Player1NPC(Clone)") && GameObject.Find("Player1(Clone)")) PhotonNetwork.Destroy(GameObject.Find("Player1NPC(Clone)"));
            if (!GameObject.Find("Player1NPC(Clone)") && !GameObject.Find("Player1(Clone)")) PhotonNetwork.InstantiateRoomObject("Player1NPC", spawn, Quaternion.identity);
            if (GameObject.Find("Player2NPC(Clone)") && GameObject.Find("Player2(Clone)")) PhotonNetwork.Destroy(GameObject.Find("Player2NPC(Clone)"));
            if (!GameObject.Find("Player2NPC(Clone)") && !GameObject.Find("Player2(Clone)"))  PhotonNetwork.InstantiateRoomObject("Player2NPC", spawn, Quaternion.identity);
            if (GameObject.Find("Player3NPC(Clone)") && GameObject.Find("Player3(Clone)")) PhotonNetwork.Destroy(GameObject.Find("Player3NPC(Clone)"));
            if (!GameObject.Find("Player3NPC(Clone)") && !GameObject.Find("Player3(Clone)"))  PhotonNetwork.InstantiateRoomObject("Player3NPC", spawn, Quaternion.identity);
            if (GameObject.Find("Player4NPC(Clone)") && GameObject.Find("Player4(Clone)")) PhotonNetwork.Destroy(GameObject.Find("Player4NPC(Clone)"));
            if (!GameObject.Find("Player4NPC(Clone)") && !GameObject.Find("Player4(Clone)")) PhotonNetwork.InstantiateRoomObject("Player4NPC", spawn, Quaternion.identity);
            if (GameObject.Find("Player5NPC(Clone)") && GameObject.Find("Player5(Clone)")) PhotonNetwork.Destroy(GameObject.Find("Player5NPC(Clone)"));
            if (!GameObject.Find("Player5NPC(Clone)") && !GameObject.Find("Player5(Clone)")) PhotonNetwork.InstantiateRoomObject("Player5NPC", spawn, Quaternion.identity);

            if (block == false) updateAgent();

            if (agent.velocity.magnitude > 0 ){
                Vector3 targetLookRotation = (Vector3.up + agent.velocity) - Vector3.up;
                if (targetLookRotation != Vector3.zero) transform.rotation = Quaternion.LookRotation(targetLookRotation);
                anim.SetBool("Run",true);
            }
            else
            {
                anim.SetBool("Run",false);
            }

            if (floor.trans != null && floor2.trans != null && floor.trans != transform && floor2.trans != transform && (tmp.name == "shower" || tmp.name == "Toilet-01")){
                agent.ResetPath();
            }
            else if (floor2.trans != null && floor2.trans != transform && tmp.name == "Toilet-01"){
                 tmp = FindChildByName(place, "shower");
                 GoToObject(tmp.transform.position);//photonView.RPC("GoToObject", RpcTarget.All, tmp.transform.position);
            }
            else if (floor.trans != null && floor.trans != transform && tmp.name == "shower"){
                 tmp = FindChildByName(place, "Toilet-01");
                 GoToObject(tmp.transform.position);//photonView.RPC("GoToObject", RpcTarget.All, tmp.transform.position);
            }

            if (startTime == true){
                startTime = false;
                StartCoroutine(Timeout());
            }
            if (!agent.pathPending && agent.isActiveAndEnabled)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        var diff = (tmp.transform.position - transform.position);
                        var curDistance = diff.sqrMagnitude;
                        if (curDistance < 1.5f){
                            if (tmp.name.StartsWith("bed") || tmp.name.StartsWith("chair") || tmp.name.StartsWith("milie_chair")) photonView.RPC("Sleep", RpcTarget.All, tmp.name);
                        }

                    }
                }
            }
        }else{
            agent.enabled = false;
        }
    }
    
    //[PunRPC]
    public void updateAgent()
    {
        DayAndNightControl NightScript = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        string[] bathroom = new [] {"Toilet-01", "shower"};
        startTime = true;
        if (NightScript.state == "morning" || NightScript.state == "afternoon"){
            block = true;
            Sec = Random.Range(25, 45);
            if (Random.Range(0, 3) == 0 && bathCount == false && (floor2.trans == null || floor.trans == null)) {
                Sec = 25;
                bathCount = true;
                tmp = FindChildByName(place, bathroom[Random.Range(0, bathroom.Length)]);
            }
            else tmp = list[Random.Range(0, list.Count)];
            if (!agent.isActiveAndEnabled && saveGameObj == tmp) return;
            saveGameObj = tmp;
            if (sleeping == true) photonView.RPC("Wake", RpcTarget.All, savedPos, savedRot);
            if (tmp)  GoToObject(tmp.transform.position);//photonView.RPC("GoToObject", RpcTarget.All, tmp.transform.position);
        }
        if (NightScript.state == "night"){
            bathCount = false;
            block = true;
            Sec = Random.Range(20, 40);
            if (dinner == false){
                dinner = true;
                tmp = chair;
                Sec = 40;
            }
            else if (Random.Range(0, 2) == 0 && bathCount == false && (floor2.trans == null || floor.trans == null)) {
                Sec = 25;
                bathCount = true;
                tmp = FindChildByName(place, bathroom[Random.Range(0, bathroom.Length)]);
            }
            else tmp = list[Random.Range(0, list.Count)];
            if (!agent.isActiveAndEnabled && saveGameObj == tmp) return;
            saveGameObj = tmp;
            if (sleeping == true) photonView.RPC("Wake", RpcTarget.All, savedPos, savedRot);
            if (tmp)  GoToObject(tmp.transform.position);//photonView.RPC("GoToObject", RpcTarget.All, tmp.transform.position);
        }
        if (NightScript.state == "midnight"){
            bathCount = false;
            block = true;
            dinner = false;
            tmp = bed;
            Sec = Random.Range(10, 20);
            if (!agent.isActiveAndEnabled && saveGameObj == tmp) return;
            saveGameObj = tmp;
            if (sleeping == true) photonView.RPC("Wake", RpcTarget.All, savedPos, savedRot);
            if (tmp) GoToObject(tmp.transform.position);//photonView.RPC("GoToObject", RpcTarget.All, tmp.transform.position);
        }
        
    }

    public void GoToObject(Vector3 pos)
    {
        agent.SetDestination(pos); 
        Debug.Log(transform.GetComponent<PhotonView>().ViewID + " - " + pos);
    }

    [PunRPC]
    public void Sleep(string name)
    {
        GameObject obj = FindChildByName(place, name);
        if (!obj) return;
        Vector3 meshPos = obj.transform.position;

        meshPos.y += 0.3f; //above height
        if (obj.name == "bed2") meshPos.z += 0.2f; //left or right side
        if (obj.name == "milie_chair.003") meshPos.x += 0.4f; //left or right side
        //collission 
        if (obj.name.StartsWith("bed"))
        { 
            collise = new Vector3(0f, 0.45f, 0.2f);
            float add = obj.transform.eulerAngles.y - obj.transform.eulerAngles.x;
            radius = 0.3f;
            rotation = new Vector3(0, add, 0);
            if (obj.name == "bed1") rotation = new Vector3(-90, 0, 0);
            else if (obj.name.StartsWith("bed")) rotation = new Vector3(-90, 0, -90);
        }
        else if (obj.name.StartsWith("milie_chair")){ 
            collise = new Vector3(0f, 1f, 0f);
            float add = obj.transform.eulerAngles.y;
            radius = 0.3f;
            rotation = new Vector3(0, add, 0);
        }
        else if (obj.name.StartsWith("chair")){
            collise = new Vector3(0f, 1.2f, 0f);
            float add = obj.transform.eulerAngles.y - obj.transform.eulerAngles.x;
            radius = 0.1f;
            rotation = new Vector3(0, add, 0);
        }
        //space
        int space = obj.GetComponent<UIButtons>().space;
        for (int i = 0; i < space; i++)
        {
            Collider[] hitColliders = Physics.OverlapSphere(meshPos, radius);
            Str = OnUpdateSelected(hitColliders);
            if (obj.name == "bed2" && Str == true) meshPos.z -= 0.4f; //left or right side
            if (obj.name == "milie_chair.003" && Str == true) meshPos.x -= 0.4f; //left or right side
        }
        if (Str == true) return;
        sleeping = true;
        if (photonView.IsMine) agent.enabled = false;
        savedPos = transform.position;
        savedRot = transform.rotation;
        GetComponent<CapsuleCollider>().center = collise;
        transform.rotation = Quaternion.Euler(rotation);
        transform.position = meshPos;
    }

    [PunRPC]
    public void Wake(Vector3 savedPos, Quaternion savedRot)
    {       
        sleeping = false;
        GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.45f, 0f);
        transform.rotation = savedRot;
        transform.position = savedPos;
        if (photonView.IsMine) agent.enabled = true;
    }

    public bool OnUpdateSelected(Collider[] hit)
    {
        bool collide = false;
        foreach (var hitCollider in hit)
        {
            if(hitCollider.gameObject.name.Contains("Player")) collide = true;
        }
        return collide;
    }

    private GameObject FindChildByName(GameObject obj, string name)
    {
        if (obj){
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (obj.transform.GetChild(i).name == name) return obj.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }

}
