using UnityEngine;
using System.Collections;

public class SpinOrigin : MonoBehaviour {

        public float speed;
        public bool spinX;
        public bool spinY;
        public bool spinZ;
        
        void Start () {
                
        }
        
        void Update () {
                if(spinX){
                        gameObject.transform.Rotate(Vector3.right * (Time.deltaTime * speed));
                }
                if(spinY){
                        gameObject.transform.Rotate(Vector3.up * (Time.deltaTime * speed));
                }
                if(spinZ){
                        gameObject.transform.Rotate(Vector3.forward * (Time.deltaTime * speed));
                }
        }
}