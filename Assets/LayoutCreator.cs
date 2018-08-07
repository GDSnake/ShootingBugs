using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LayoutCreator : MonoBehaviour
{
    public enum TileType
    {
        Floor, Ramp, Tall
    }

    public GameObject debugImage;
    public int maxNumberOfRamps = 6;
    public float boardHolderX = 0;
    public float boardHolderY = 0;
    public float yPadding = 0.85f;
    public float subterrainYPadding = 0.55f;
    public float tallBlockyPadding = 0.41f;
    public int Columns = 12;
    public int Rows = 5;
    public GameObject[] floorTiles;
    public GameObject[] rampTiles;

    private TileType[][] tiles;
    private GameObject boardHolder;
	// Use this for initialization
	void Start () {
        boardHolder = new GameObject("BoardHolder");
	    boardHolder.transform.position=new Vector3(boardHolderX, boardHolderY, 0);
		SetupTilesMatrix();
        SetTileType();
        InstantiateTiles();
	}

    void SetupTilesMatrix()
    {
        tiles = new TileType[Columns][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[Rows];
        }
    }

    void SetTileType()
    {
        bool isTall = false;

        

        for (int i = 0; i < Rows; i++)
        {
            int rampsLeft = Random.Range(0,maxNumberOfRamps);
            if (rampsLeft % 2 != 0)
            {
                rampsLeft--;
            }
            int lastRampIndex = -1;
            for (int j = 0; j < Columns; j++)
            {
               
                if (rampsLeft>0&& Random.Range(0, 2) != 0)
                {
                    isTall = !isTall;
                    
                    tiles[j][i] = TileType.Ramp;
                    rampsLeft--;
                    lastRampIndex = j;
                }
                else if (isTall)
                {
                    tiles[j][i] = TileType.Tall;
                }
                else
                {
                    tiles[j][i] = TileType.Floor;
                }

                
            }
            if (rampsLeft % 2 != 0) {
                if (lastRampIndex != Columns - 1) {
                    int tempIndex = Random.Range(lastRampIndex + 1, Columns - 1);

                    
                        tiles[tempIndex][i] = TileType.Ramp;
                    
                    for (int k = tempIndex + 1; k < Columns; k++) {
                        tiles[k][i] = TileType.Floor;
                    }
                } else {
                    tiles[lastRampIndex][i] = TileType.Floor;
                }

            }
        }
      
    }

    void InstantiateTiles()
    {
        
        for (int i = Rows-1;  i> -1; i--)
        {
            bool hasRamp = false;
            for (int j = Columns-1; j > -1; j--)
            {
                InstatiateFromTileArray(j, i,false);

                if (i == 0) {
                    InstatiateFromTileArray(j, i, true);
                }
                if (tiles[j][i].Equals(TileType.Ramp))
                {

                    InstatiateFromRampArray(j,i,hasRamp);
                    hasRamp = !hasRamp;
                    

                }
                if (tiles[j][i].Equals(TileType.Tall))
                {
                    InstatiateFromTileArray(j, i*tallBlockyPadding,false);

                }




            }
        }
    }

    void InstatiateFromRampArray(float x, float y, bool east)
    {
        Vector3 position = new Vector3(x, ((y*yPadding)+0.41f), 0f);
        GameObject rampInstance;
        if (east)
        {
             rampInstance = Instantiate(rampTiles[0]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
        }
        else
        {
            rampInstance = Instantiate(rampTiles[1]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
        }
        rampInstance.transform.parent = boardHolder.transform;
        rampInstance.transform.localPosition = position;
        rampInstance.transform.rotation = Quaternion.identity;


    }
    void InstatiateFromTileArray(float x, float y, bool subterrain)
    {
        int randomIndex = Random.Range(0, floorTiles.Length);
        Vector3 position;
        GameObject tileInstance = Instantiate(floorTiles[randomIndex]);
        
        
        if (!subterrain)
        {
            position = new Vector3(x, (y * yPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int) (Rows - y);
            if (tiles[(int)x][(int)y].Equals(TileType.Tall))
            {
                Vector3 tallPosition = new Vector3(x, ((y * yPadding) + tallBlockyPadding),0f);
                //GameObject tileInstanceTall = Instantiate(floorTiles[randomIndex]);
                GameObject tileInstanceTall = Instantiate(debugImage);
                tileInstanceTall.transform.parent = boardHolder.transform;
                tileInstanceTall.transform.localPosition = tallPosition;
                tileInstanceTall.transform.rotation = Quaternion.identity;
                tileInstanceTall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
            }
        }
        else 
        {
            position = new Vector3(x, ((y-1) * subterrainYPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        tileInstance.transform.parent = boardHolder.transform;
        tileInstance.transform.localPosition = position;
        tileInstance.transform.rotation = Quaternion.identity;


    }

	// Update is called once per frame
	void Update () {
		
	}
}
