using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    Vector3 meshPos;
    public Vector3 scale;
    public GameObject closed;
    public GameObject opened;
    public Transform trans;

    void Start()
    {
        meshPos = transform.position;
    }
    

    void Update()
    {
        Collider[] area = Physics.OverlapBox(meshPos, scale);
        foreach (var collide in area)
        {
            if(collide.gameObject.name.Contains("Player")){
                trans = collide.transform;
                closed.SetActive(true);
                opened.SetActive(false);
            }else{
                trans = null;
                closed.SetActive(false);
                opened.SetActive(true);
            }
        }
    }
}
