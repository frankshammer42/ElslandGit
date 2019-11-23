using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CutManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _cut = false;
    public GameObject trainToCut;
    private float _sizeX;
    private float _sizeY;
    private List<Pair2D> _cutPoints = new List<Pair2D>();
    private List<Vector2D> _centerPoints = new List<Vector2D>();
    public float[] blockCutPoints;
    public int segmentNumber = 6 ;
    public float halfLength;
    public float blockLength;
    public int totalCut = 10;
    public int cutsForEachBlock;
    
    void Start(){
        segmentNumber = 6; 
        cutsForEachBlock = totalCut / segmentNumber;
        blockCutPoints = new float[segmentNumber*cutsForEachBlock*4];
        GenerateCutPoints();
        GenerateCutArraysToSend();
//        GenerateCenterPoints();
//        GenerateBlockCutPoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_cut && Input.GetKey(KeyCode.T)){
            Cut();
//            Destroy(GameObject.Find("Ground"));
            _cut = true;
            Debug.Log("Start Cut");
        }
    }

    void Cut(){
        foreach (var cutPair in _cutPoints){
            List<Slice2D> sliceResult = Slicer2D.LinearSliceAll(cutPair, Slice2DLayer.Create());
            foreach (var slice in sliceResult){
                List<GameObject> slicedGameObjects = slice.GetGameObjects();
                float coin = Random.Range(0.0F, 1.0F);
                if (coin >= 0.5){
                    Debug.Log("Shit is weird");
                    foreach (var sg in slicedGameObjects){
                        sg.GetComponent<Rigidbody2D>().AddForce(transform.up*1.2F, ForceMode2D.Impulse);
                    }
                }
                else{
                    Debug.Log("Am I here");
                    foreach (var sg in slicedGameObjects){
                        sg.GetComponent<Rigidbody2D>().AddForce(transform.up*-1.2F, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private void GenerateCenterPoints(){
        Bounds spriteBounds = trainToCut.GetComponent<SpriteRenderer>().bounds;
        Vector3 minBounds = spriteBounds.min;
        Vector3 maxBounds = spriteBounds.max;
        float length = maxBounds.x - minBounds.x;
        float y_position = maxBounds.y - minBounds.y;
        blockLength = length / segmentNumber;
        halfLength = blockLength / 2;
        Vector2D start = new Vector2D(minBounds.x + halfLength, y_position); 
        _centerPoints.Add(start);
        for (int i = 1; i < segmentNumber; i++){
            Vector2D point = new Vector2D(start.x + i*blockLength, start.y);
            _centerPoints.Add(point);
        }
    }

    private void GenerateBlockCutPoints(){
        Bounds spriteBounds = trainToCut.GetComponent<SpriteRenderer>().bounds;
        Vector3 minBounds = spriteBounds.min;
        Vector3 maxBounds = spriteBounds.max;
        float yMin = minBounds.y - (float)0.1;
        float yMax = maxBounds.y + (float)0.1;
        for (int i = 0; i < segmentNumber; i++){
            Vector2D currentCenter = _centerPoints[i];
            for (int j = 0; j < cutsForEachBlock; j++){
                float xMin = (float)currentCenter.x - halfLength;
                float xMax = (float) currentCenter.x + halfLength;
                float xCutStart = Random.Range(xMin, xMax);
                float xCutEnd = Random.Range(xMin, xMax);
                blockCutPoints[i * cutsForEachBlock * 4 + j*4] = xCutStart;
                blockCutPoints[i * cutsForEachBlock * 4 + j*4+1] = xCutStart;
                blockCutPoints[i * cutsForEachBlock * 4 + j*4+2] = xCutStart;
                blockCutPoints[i * cutsForEachBlock * 4 + j*4+3] = xCutStart;
//                blockCutPoints[i, j * 4] = xCutStart;
//                blockCutPoints[i, j * 4+1] = yMin;
//                blockCutPoints[i, j * 4+2] = xCutEnd;
//                blockCutPoints[i, j * 4+3] = yMax;
            }
        }
    }
    
    //Helper Functions for generating cut points
    //Generate cut points for cutting 
    private void GenerateCutPoints(){
        Bounds spriteBounds = trainToCut.GetComponent<SpriteRenderer>().bounds;
        Vector3 minBounds = spriteBounds.min;
        Vector3 maxBounds = spriteBounds.max;
        float xMinBound = minBounds.x - trainToCut.transform.position.x;
        float yMinBound = minBounds.y - trainToCut.transform.position.y;
        float boundToSendX = xMinBound * -2f;
        float boundToSendY = yMinBound * -2f;
        GameObject unityClient = GameObject.Find("UnityClient");
//        unityClient.GetComponent<UnitySocketClient>().cutBound = new float[2];
//        unityClient.GetComponent<UnitySocketClient>().cutBound[0] = boundToSendX;
//        unityClient.GetComponent<UnitySocketClient>().cutBound[1] = boundToSendY;
//        unityClient.GetComponent<UnitySocketClient>().xTrainOffset = trainToCut.transform.position.x;
//        unityClient.GetComponent<UnitySocketClient>().yTrainOffset = trainToCut.transform.position.y;
        float xMin = minBounds.x - (float)0.02;
        float xMax = maxBounds.x + (float)0.02;
        float yMin = minBounds.y - (float)0.1;
        float yMax = maxBounds.y + (float)0.1;
        for (int i = 0; i < totalCut; i++){
            Debug.Log("Adding Point to cutPair");
            float xCutStart = Random.Range(xMin, xMax);
            float xCutEnd = Random.Range(xMin, xMax);
//            Vector2D pointA = new Vector2D(xCutStart - trainToCut.transform.position.x, yMin - trainToCut.transform.position.y); 
//            Vector2D pointB = new Vector2D(xCutEnd - trainToCut.transform.position.x, yMax - trainToCut.transform.position.y);
            Vector2D pointA = new Vector2D(xCutStart, yMin); 
            Vector2D pointB = new Vector2D(xCutEnd, yMax);
            Pair2D cutPair = new Pair2D(pointA, pointB);
            _cutPoints.Add(cutPair);
        }
    }

    //Generate array of doubles that contains cut points that can sent to server
    private void GenerateCutArraysToSend(){
        int numCutPoints = _cutPoints.Count;
//        GameObject unityClient = GameObject.Find("UnityClient");
//        unityClient.GetComponent<UnitySocketClient>().cutPoints = new double[numCutPoints*4];
        int counter = 0;
//        foreach (var cutPoint in _cutPoints){
//            unityClient.GetComponent<UnitySocketClient>().cutPoints[counter*4] = cutPoint.A.x;
//            unityClient.GetComponent<UnitySocketClient>().cutPoints[counter*4+1]= cutPoint.A.y;
//            unityClient.GetComponent<UnitySocketClient>().cutPoints[counter*4+2] = cutPoint.B.x;
//            unityClient.GetComponent<UnitySocketClient>().cutPoints[counter*4+3] = cutPoint.B.y;
//            counter++;
//        }
//        Debug.Log(unityClient.GetComponent<UnitySocketClient>().cutPoints[0]);
//        Debug.Log(unityClient.GetComponent<UnitySocketClient>().cutPoints[1]);
//        Debug.Log(unityClient.GetComponent<UnitySocketClient>().cutPoints[2]);
//        Debug.Log(unityClient.GetComponent<UnitySocketClient>().cutPoints[3]);
    } 
    
    
}
