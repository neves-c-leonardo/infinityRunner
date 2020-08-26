using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moeda : MonoBehaviour
{
    

    void Start(){
        
    }

    void Update(){
        transform.Rotate(new Vector3(0,0,90)*Time.deltaTime);
    }

}