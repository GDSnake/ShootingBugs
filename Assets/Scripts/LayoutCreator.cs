using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// This Class creates the random generated layout
/// </summary>
public class LayoutCreator : MonoBehaviour
{
    
    public enum TileType
    {
        Floor, Ramp, Tall
    }
    /// <summary>
    /// Max number of ramps that a map has,it must be a pair number(one up and a down ramp pair)
    /// </summary>
    public int MaxNumberOfRamps = 6;
    public float BoardHolderX = 0;
    public float BoardHolderY = 0;
    public float YPadding = 0.85f;
    public float SubterrainYPadding = 0.55f;
    public float TallBlockyPadding = 0.41f;
    public int Columns = 12;
    public int Rows = 5;
    public GameObject[] FloorTiles;
    public GameObject[] RampTiles;
    public GameObject Boundary;

    private TileType[][] _tiles;
    private GameObject _boardHolder;
    
	// Use this for initialization
	void Start ()
	{
        _boardHolder = new GameObject("BoardHolder");
	    _boardHolder.transform.position=new Vector3(BoardHolderX, BoardHolderY, 0);
		SetupTilesMatrix();
        SetTileType();
        InstantiateTiles();
        SpawnBoundaries();
	}

    /// <summary>
    /// This function creates the map matrix, with the default value of every cell as Floor
    /// </summary>
    void SetupTilesMatrix()
    {
        _tiles = new TileType[Columns][];

        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i] = new TileType[Rows];
        }
    }
    /// <summary>
    /// This class spawns the boundaries of the map, offscreen, the spawn location is on the left side, and the right side spawns the Boundary tiles to destroy objects that go off screen
    /// </summary>
    void SpawnBoundaries()
    {
        for (int i = Rows - 1; i > -1; i--)
        {
            
            Vector3 position= new Vector3(-1f,i*YPadding,0f);
            Vector3 position2 = new Vector3(Columns, i * YPadding, 0f);
            GameObject tileInstance = Instantiate(FloorTiles[0]);
            GameObject tileInstance2 = Instantiate(Boundary);
            
            tileInstance2.tag = "Boundary";
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - i);
            tileInstance2.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - i);
            tileInstance.transform.parent = _boardHolder.transform;
            tileInstance.transform.localPosition = position;
            tileInstance.transform.rotation = Quaternion.identity;
            tileInstance2.transform.parent = _boardHolder.transform;
            tileInstance2.transform.localPosition = position2;
            tileInstance2.transform.rotation = Quaternion.identity;
        }

    }
    /// <summary>
    /// This function sets the type of each tile randommly
    /// </summary>
    void SetTileType()
    {
        for (int i = 0; i < Rows; i++)
        {
           
            bool isTall = false; //variable to control if puts tall block between two ramps
            int rampsLeft = Random.Range(0,MaxNumberOfRamps); 
            if (rampsLeft % 2 != 0) // checks if ramp number is pair, if not decreases 1 ramp to make it pair
            {
                rampsLeft--;
            }
            
            int lastRampIndex = -1; //used to check if there is a left ramp without a matching right ramp pair
            for (int j = 0; j < Columns; j++)
            {
               
                if (rampsLeft>0&& Random.Range(0, 2) != 0&& (j==0 || !_tiles[j - 1][i].Equals(TileType.Ramp))) // the range checks for a 50/50 probability of having a ramp or not
                {                                                                                               // if has probability to create a ramp, checks if block before is tall, to avoid directly adjacent ramps
                    isTall = !isTall;
                    _tiles[j][i] = TileType.Ramp;
                    rampsLeft--;
                    lastRampIndex = j;
                }
                else if (isTall)
                {
                    _tiles[j][i] = TileType.Tall;
                }
                else
                {
                    _tiles[j][i] = TileType.Floor;
                } 
            }
            if (rampsLeft % 2 != 0) { // if ramps left is not a pair number, the ramps placed are odd, meaning we have to delete the last one, and subsequent tall blocks
                for (int k = lastRampIndex; k < Columns; k++) {
                    _tiles[k][i] = TileType.Floor;
                }
                int unconcealed = 0; //used to check if at least 3 consecutive unconcealed blocks exist ( ramps and floor are treated as unconcealing)
                for (int j = lastRampIndex; j < Columns; j++) {
                    if (_tiles[j][i] == TileType.Floor || _tiles[j][i] == TileType.Ramp)
                    {
                        unconcealed++;
                    }
                    else
                    {
                        unconcealed = 0;
                    }
                    if (unconcealed == 3)
                    {
                        break;
                    }
                }
                if (unconcealed < 3)
                {
                    /*
                    *if the pathway has less than three unconcealed blocks, by our verification on the main loop, the third block is tall, 
                    *because 2 ramps must have to have a floor between them, this only happens a "bridge" is generated from beggining to the end of the pathway
                    */
                    if (_tiles[Columns - 3][i] == TileType.Tall) 
                    {

                        _tiles[Columns - 3][i] = TileType.Ramp;
                        _tiles[Columns - 2][i] = TileType.Floor;
                        _tiles[Columns - 1][i] = TileType.Floor;
                    }
                    
                }
            }
        }
      
    }
    /// <summary>
    /// This function, travels the tiles type jagged array, and calls the tile or ramp instantiator funcion
    /// </summary>
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
                if (_tiles[j][i].Equals(TileType.Ramp))
                {

                    InstatiateFromRampArray(j,i,hasRamp);
                    hasRamp = !hasRamp;            
                }
                else if (_tiles[j][i].Equals(TileType.Tall)&& !_tiles[j-1][i].Equals(TileType.Floor))
                {
                    InstatiateFromTileArray(j, i,false);

                }
            }
        }
    }
    /// <summary>
    /// Instantiates a ramp object, left or right ramp according to the bool east, and gives a yPadding to be over the base floor block
    /// </summary>
    /// <param name="x">X position on the jagged array</param>
    /// <param name="y">Y position on the jagged array</param>
    /// <param name="east">verifies if is a east or west ramp</param>
    void InstatiateFromRampArray(float x, float y, bool east)
    {
        Vector3 position = new Vector3(x, ((y*YPadding)+TallBlockyPadding), 0f);
        GameObject rampInstance;
        if (east)
        {
             rampInstance = Instantiate(RampTiles[0]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
        }
        else
        {
            rampInstance = Instantiate(RampTiles[1]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
        }
        rampInstance.transform.parent = _boardHolder.transform;
        rampInstance.transform.localPosition = position;
        rampInstance.transform.rotation = Quaternion.identity;


    }
    /// <summary>
    /// Instantiates tiles from the tiles array, randomly. 
    /// If subterrain gives a negative padding.
    /// If tall gives a positive padding and chooses the wall block gameobject to simulate a bridge.
    /// It uses the boardHolder gameobject as parent transform.
    /// </summary>
    /// <param name="x">X position on the jagged array</param>
    /// <param name="y">Y position on the jagged array</param>
    /// <param name="subterrain">Checks if its a front row subterrain block</param>
    void InstatiateFromTileArray(float x, float y, bool subterrain)
    {
        int randomIndex = Random.Range(0, FloorTiles.Length-1);
        Vector3 position;
        GameObject tileInstance = Instantiate(FloorTiles[randomIndex]);
        
        
        if (!subterrain)
        {
            position = new Vector3(x, (y * YPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int) (Rows - y);
            if (_tiles[(int)x][(int)y].Equals(TileType.Tall))
            {
                Vector3 tallPosition = new Vector3(x, ((y * YPadding) + TallBlockyPadding),0f);
                randomIndex = Random.Range(0, FloorTiles.Length);
                GameObject tileInstanceTall = Instantiate(FloorTiles[FloorTiles.Length-1]);
                tileInstanceTall.transform.parent = _boardHolder.transform;
                tileInstanceTall.transform.localPosition = tallPosition;
                tileInstanceTall.transform.rotation = Quaternion.identity;
                tileInstanceTall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(Rows - y+1);
            }
        }
        else 
        {
            position = new Vector3(x, ((y-1) * SubterrainYPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        tileInstance.transform.parent = _boardHolder.transform;
        tileInstance.transform.localPosition = position;
        tileInstance.transform.rotation = Quaternion.identity;


    }

	
}
