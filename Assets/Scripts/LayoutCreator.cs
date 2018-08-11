using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// This Class creates the random generated layout
/// </summary>
public class LayoutCreator : MonoBehaviour {

    public enum TileType {
        Floor, Ramp, Tall
    }

    /// <summary>
    /// Max number of ramps that a map has,it must be a pair number(one up and a down ramp pair)
    /// </summary>

    public int MinimumUnconcealedBlocks = 3;
    public int MaxNumberOfBridges = 2;
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

    public static int numberOfRows;

    private TileType[][] _tiles;
    private GameObject _boardHolder;

    // Use this for initialization
    void Start() {
        _boardHolder = new GameObject("BoardHolder");
        _boardHolder.transform.position = new Vector3(BoardHolderX, BoardHolderY, 0);
        SetupTilesMatrix();
        SetTileType();
        InstantiateTiles();
        SpawnBoundaries();
        numberOfRows = Rows;
    }

    /// <summary>
    /// This function creates the map matrix, with the default value of every cell as Floor
    /// </summary>
    private void SetupTilesMatrix() {
        _tiles = new TileType[Columns][];

        for (int i = 0; i < _tiles.Length; i++) {
            _tiles[i] = new TileType[Rows];
        }
    }

    private int SetSortingOrder(int y) {
        return Rows - y;
    }

    /// <summary>
    /// This class spawns the boundaries of the map, offscreen, the spawn location is on the left side, and the right side spawns the Boundary tiles to destroy objects that go off screen
    /// </summary>
    private void SpawnBoundaries() {
        for (int i = Rows - 1; i > -1; i--) {

            Vector3 spawnerPosition = new Vector3(-1f, i * YPadding, 0f);
            Vector3 boundaryPosition = new Vector3(Columns, i * YPadding, 0f);
            GameObject spawner = Instantiate(FloorTiles[0]);
            GameObject boundary = Instantiate(Boundary);

            boundary.tag = "Boundary";
            spawner.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(i);
            boundary.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(i);
            spawner.transform.parent = _boardHolder.transform;
            spawner.transform.localPosition = spawnerPosition;
            spawner.transform.rotation = Quaternion.identity;
            boundary.transform.parent = _boardHolder.transform;
            boundary.transform.localPosition = boundaryPosition;
            boundary.transform.rotation = Quaternion.identity;
        }

    }
    /// <summary>
    /// This function sets the type of each tile randommly
    /// </summary>
    private void SetTileType() {
        for (int y = 0; y < Rows; y++) {

            bool isTall = false; //variable to control if puts tall block between two ramps
            int rampsLeft = Random.Range(0, MaxNumberOfBridges + 1) * 2;

            int lastRampIndex = -1; //used to check if there is a left ramp without a matching right ramp pair
            for (int x = 0; x < Columns; x++) {

                if (rampsLeft > 0 && Random.Range(0, 2) != 0 && (x == 0 || !_tiles[x - 1][y].Equals(TileType.Ramp))) // the range checks for a 50/50 probability of having a ramp or not
                {                                                                                               // if has probability to create a ramp, checks if block before is tall, to avoid directly adjacent ramps
                    isTall = !isTall;
                    _tiles[x][y] = TileType.Ramp;
                    rampsLeft--;
                    lastRampIndex = x;
                } else if (isTall) {
                    _tiles[x][y] = TileType.Tall;
                } else {
                    _tiles[x][y] = TileType.Floor;
                }
            }

            if (rampsLeft % 2 != 0) { // if ramps left is not a pair number, the ramps placed are odd, meaning we have to delete the last one, and subsequent tall blocks
                for (int x = lastRampIndex; x < Columns; x++) {
                    _tiles[x][y] = TileType.Floor;
                }
                int unconcealed = 0; //used to check if at least 3 consecutive unconcealed blocks exist ( ramps and floor are treated as unconcealing)
                for (int x = lastRampIndex; x < Columns; x++) {
                    if (_tiles[x][y] == TileType.Floor || _tiles[x][y] == TileType.Ramp) {
                        unconcealed++;
                    } else {
                        unconcealed = 0;
                    }
                    if (unconcealed == MinimumUnconcealedBlocks) {
                        break;
                    }
                }

                if (unconcealed < MinimumUnconcealedBlocks) {
                    /*
                    *if the pathway has less than three unconcealed blocks, by our verification on the main loop, the third block is tall, 
                    *because 2 ramps must have to have a floor between them, this only happens a "bridge" is generated from beggining to the end of the pathway
                    */
                    if (_tiles[Columns - 3][y] == TileType.Tall) {

                        _tiles[Columns - 3][y] = TileType.Ramp;
                        _tiles[Columns - 2][y] = TileType.Floor;
                        _tiles[Columns - 1][y] = TileType.Floor;
                    }

                }
            }
        }

    }
    /// <summary>
    /// This function, travels the tiles type jagged array, and calls the tile or ramp instantiator funcion
    /// </summary>
    private void InstantiateTiles() {
        for (int y = Rows - 1; y > -1; y--) {
            bool hasRamp = false;
            for (int x = Columns - 1; x > -1; x--) {
                InstatiateFromTileArray(x, y, false);

                if (y == 0) {
                    InstatiateFromTileArray(x, y, true);
                }

                if (_tiles[x][y].Equals(TileType.Ramp)) {

                    InstatiateFromRampArray(x, y, hasRamp);
                    hasRamp = !hasRamp;
                } else if (_tiles[x][y].Equals(TileType.Tall) && !_tiles[x - 1][y].Equals(TileType.Floor)) {
                    InstatiateFromTileArray(x, y, false);
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
    private void InstatiateFromRampArray(int x, int y, bool east) {
        Vector3 position = new Vector3(x, ((y * YPadding) + TallBlockyPadding), 0f);
        GameObject rampInstance;
        if (east) {
            rampInstance = Instantiate(RampTiles[0]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(y - 1);
        } else {
            rampInstance = Instantiate(RampTiles[1]);
            rampInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(y - 1);
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
    private void InstatiateFromTileArray(int x, int y, bool subterrain) {
        int randomIndex = Random.Range(0, FloorTiles.Length - 1);
        Vector3 position;
        GameObject tileInstance = Instantiate(FloorTiles[randomIndex]);

        if (subterrain) {
            position = new Vector3(x, ((y - 1) * SubterrainYPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        } else {
            position = new Vector3(x, (y * YPadding), 0f);
            tileInstance.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(y);
            if (_tiles[(int)x][(int)y].Equals(TileType.Tall)) {
                Vector3 tallPosition = new Vector3(x, ((y * YPadding) + TallBlockyPadding), 0f);
                GameObject tileInstanceTall = Instantiate(FloorTiles[FloorTiles.Length - 1]);
                tileInstanceTall.transform.parent = _boardHolder.transform;
                tileInstanceTall.transform.localPosition = tallPosition;
                tileInstanceTall.transform.rotation = Quaternion.identity;
                tileInstanceTall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(y - 1);
            }
        }
        tileInstance.transform.parent = _boardHolder.transform;
        tileInstance.transform.localPosition = position;
        tileInstance.transform.rotation = Quaternion.identity;
    }
}
