using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    
    public GameObject floorTile;
    public int floorLength;
    public int floorWidth;

    public float heightScale;
    public float xScale;
    public float waveScale;
    public float waveSpeed;
    
    private float offset;
    private GameObject[] tiles;

    private int tilePressCount;
    private int stoppedTileCount;

	void Start ()
    {
        tilePressCount = 0;
        tiles = new GameObject[floorLength * floorWidth];
        for (int k = 0; k < floorLength; k++) {
            for (int z = 0; z < floorWidth; z++) {
                GameObject newTile = Instantiate<GameObject>(floorTile, transform);
                float height = Math.Abs(heightScale * 5 * Mathf.PerlinNoise(k * xScale, heightScale * z * waveScale));
                newTile.transform.localScale = new Vector3(floorTile.transform.localScale.x, height, floorTile.transform.localScale.z);
                newTile.transform.position = new Vector3(z*20, height / 2, k);
                newTile.name = "Tile" + (k * floorWidth + z);
                tiles[k * floorWidth + z] = newTile;
            }
        }
	}
    
    void Update()
    {
  

        offset += waveSpeed * Time.deltaTime;
        for (int k = 0; k < floorLength; k++) {
            for (int z = 0; z < floorWidth; z++)
            {
                int index = k * floorWidth + z; 
                GameObject tile = tiles[index];
                if (tilePressCount < 6 && index >= tilePressCount || Math.Abs(tile.transform.localScale.y - 1) > 0.05)
                {
                    float height = Math.Abs(heightScale * 5 * Mathf.PerlinNoise(k * xScale + offset, heightScale * z * waveScale + offset));
                    tile.transform.localScale = new Vector3(floorTile.transform.localScale.x, height, floorTile.transform.localScale.z);
                    tile.transform.position = new Vector3(z*8.5f, height / 2, k*8.5f); 
                }
            }
        }

    }

    public void PressTile()
    {
        tilePressCount += 1;
    }
}
