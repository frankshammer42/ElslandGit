using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilding : MonoBehaviour
{
    //an array of buildings to spawn 2 in front, 2 in back 
    //an array of spawn points - front and back random range between front +/-10, random range between back +/-10
    public GameObject buildingfront;

    public GameObject frontSpawnPoint, farSpwanPoint;
    public int buildintCount;
    public float spawnWaitfront = 2f;    //slower
    public float spawnWaitfar = 1.2f;    //faster
    public float startWait = 0f;
    //public float waveWait = 2f;

    void Start()
    {
        StartCoroutine(SpawnBuildings());
    }


    IEnumerator SpawnBuildings()
    {
        while (isActiveAndEnabled)
        {
            for(var i = 0; i < buildintCount; i++)
            {
                Vector3 spawnPositionF = new Vector3(frontSpawnPoint.transform.position.x, Random.Range(frontSpawnPoint.transform.position.y - 3f, frontSpawnPoint.transform.position.y + 3f), frontSpawnPoint.transform.position.z);
                //Vector3 spawnPositionB = new Vector3(farSpwanPoint.transform.x, Random.Range(farSpwanPoint.transform.y - 5f, farSpwanPoint.transform.y + 5f), farSpwanPoint.transform.z);
                Instantiate(buildingfront, spawnPositionF, buildingfront.transform.rotation);
                yield return new WaitForSeconds(spawnWaitfront);
            }
            
        }
        //yield return new WaitForSeconds(startWait);
           //spawn a building in the front
              

    }




 }
