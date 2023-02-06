using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


 public class UIButtons : MonoBehaviour,IUpdateSelectedHandler,IPointerDownHandler,IPointerUpHandler
 {
     public bool isPressed;
     private bool boole;
     private Vector3 rotation;
     private Vector3 collise;
     private float angle;
     public float radius = 0.2f;
     public int space;
     private bool Str;

     // Start is called before the first frame update
     public void OnUpdateSelected(BaseEventData data)
     {
         if (isPressed)
         {
            if (gameObject.name == "Up") PlayerController.instance.Movements(0, 1);
            if (gameObject.name == "Down") PlayerController.instance.Movements(0, -1);
            if (gameObject.name == "Right") PlayerController.instance.Movements(1, 0);
            if (gameObject.name == "Left") PlayerController.instance.Movements(-1, 0);
            if (gameObject.name == "Leave") {
                GameObject canvas = GameObject.Find("Canvas");
                canvas.transform.GetChild(1).gameObject.GetComponent<Image>().enabled = false;
                canvas.transform.GetChild(1).gameObject.GetComponent<Button>().enabled = false;
                canvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                PlayerController.instance.Wake();
            }
         }
     }
     public void OnPointerDown(PointerEventData data)
     {
         isPressed = true;
     }
     public void OnPointerUp(PointerEventData data)
     {
         isPressed = false;
     }
     public void OnMouseDown()
     {
        if(EventSystem.current.IsPointerOverGameObject()) return;
        Vector3 meshPos = gameObject.transform.position;

        meshPos.y += 0.3f; //above height
        if (gameObject.name == "bed2") meshPos.z += 0.2f; //left or right side
        if (gameObject.name == "milie_chair.003") meshPos.x += 0.4f; //left or right side
        //collission 
        if (gameObject.name.StartsWith("bed"))
        { 
            collise = new Vector3(0f, 0.45f, 0.5f);
            boole = true;
            float add = gameObject.transform.eulerAngles.y - gameObject.transform.eulerAngles.x;
            rotation = new Vector3(0, add, 0);
            if (gameObject.name == "bed1") rotation = new Vector3(-90, 0, 0);
            else if (gameObject.name.StartsWith("bed")) rotation = new Vector3(-90, 0, -90);
        }
        else if (gameObject.name.StartsWith("milie_chair")){ 
            collise = new Vector3(0f, 1f, 0f);
            boole = false; 
            float add = gameObject.transform.eulerAngles.y;
            rotation = new Vector3(0, add, 0);
        }
        else if (gameObject.name.StartsWith("chair")){
            collise = new Vector3(0f, 1.2f, 0f);
            boole = false; 
            float add = gameObject.transform.eulerAngles.y - gameObject.transform.eulerAngles.x;
            rotation = new Vector3(0, add, 0);

        }
        //space
        for (int i = 0; i < space; i++)
        {
            Collider[] hitColliders = Physics.OverlapSphere(meshPos, radius);
            Str = OnUpdateSelected(hitColliders);
            if (gameObject.name == "bed2" && Str == true) meshPos.z -= 0.4f; //left or right side
            if (gameObject.name == "milie_chair.003" && Str == true) meshPos.x -= 0.4f; //left or right side
        }
        if (Str == true) return;
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.GetChild(1).gameObject.GetComponent<Image>().enabled = true;
        canvas.transform.GetChild(1).gameObject.GetComponent<Button>().enabled = true;
        canvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        PlayerController.instance.Sleep(meshPos, rotation, collise, boole);
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
 }
