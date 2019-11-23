using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class IntroBlocksManager : MonoBehaviour{
    public GameObject introBlock;
    public List<GameObject> introBlocks = new List<GameObject>();
    public int numCols;
    public int numRows;
    public float scale = 2;
    public Vector3 startPosition;
    public bool shouldUpdate = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update(){
        if (shouldUpdate){
            foreach (var block in introBlocks){
                DestroyImmediate(block);
            }
            introBlocks.Clear();
            for (int i = 0; i < numRows; i++){
                for (int j = 0; j < numCols; j++){
                    Vector3 newPosition = new Vector3(j*scale+startPosition.x, i*scale+startPosition.y, 10);
                    GameObject newIntroBlock = GameObject.Instantiate(introBlock, newPosition, Quaternion.identity);
                    introBlocks.Add(newIntroBlock);
                }
            }
        }
    }
}
