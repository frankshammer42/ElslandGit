using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientFinder : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _unityClient;
    void Start(){
        _unityClient = GameObject.Find("UnityClient");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendCut(){
        _unityClient.GetComponent<UnitySocketClient>().SendCut();
    }
}
