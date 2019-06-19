using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class BackgroundTiles : Tile
{
    [Header("Background Tile Settings")]
    public string m_tileType;


    
    public Sprite[] m_tileSprites;

    
    public Sprite m_errorSprite;

    
    public Sprite m_tilePreview;

    #region Tile Set up
    public override bool StartUp(Vector3Int p_position, ITilemap p_tilemap, GameObject p_go)
    {
        return base.StartUp(p_position, p_tilemap, p_go);
    }

    //Refreshes the tile image
    public override void RefreshTile(Vector3Int p_position, ITilemap p_tilemap)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {

                //gets the neighbor tile position
                Vector3Int neighborPos = new Vector3Int(p_position.x + x, p_position.y + y, p_position.z);

                if (IsSameTile(p_tilemap, neighborPos))
                {
                    p_tilemap.RefreshTile(neighborPos);
                }
            }
        }

    }

    #endregion

    //This is what makes all the different tiles change and stuff
    public override void GetTileData(Vector3Int p_position, ITilemap p_tilemap, ref TileData p_tileData)
    {
        //Creates a string of the surrounding tiles
        string nTiles = string.Empty;

        //Needs this to activate the colliders
        base.GetTileData(p_position, p_tilemap, ref p_tileData);


        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {

                    //If the current neighbouring tile is the same type of tile, ie a wall is the same as a wall, add Y to the string
                    if (IsSameTile(p_tilemap, new Vector3Int(p_position.x + x, p_position.y + y, p_position.z)))
                    {
                        nTiles += 'Y';

                        //If its something different, ie wall and floor, add N to string
                    }
                    else
                    {
                        nTiles += "N";
                    }
                }

            }

        }

        //Defaults to bight purple if a case was not accounted for
        p_tileData.sprite = m_errorSprite;


        int tileIndex = 4;
        //The basic ones

        if (nTiles[0] == 'N' && nTiles[1] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'Y' && nTiles[6] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'N' && nTiles[6] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'Y' && nTiles[6] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'N' && nTiles[6] == 'Y' && nTiles[7] == 'Y')
        {
            tileIndex = 0;
        }

        if (nTiles[1] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[6] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[2] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'N' )
        {
            tileIndex = 1;
        }

        if (nTiles[0] == 'N' && nTiles[1] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'Y' && nTiles[6] == 'N' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'N' && nTiles[6] == 'N' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'N' && nTiles[5] == 'Y' && nTiles[6] == 'N' ||
            nTiles[0] == 'N' && nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'Y')
        {
            tileIndex = 2;
        }

        if (nTiles[1] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[6] == 'Y' ||
            nTiles[0] == 'N' && nTiles[1] == 'Y' && nTiles[2] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'Y')
        {
            tileIndex = 3;
        }



        if (nTiles[1] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[6] == 'N' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'N' && nTiles[6] == 'Y' && nTiles[7] == 'N')
        {
            tileIndex = 5;
        }

        if (nTiles[1] == 'N' && nTiles[2] == 'N' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'Y' ||
            nTiles[1] == 'N' && nTiles[2] == 'Y' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'N' ||
            nTiles[1] == 'N' && nTiles[2] == 'Y' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'N')
        {
            tileIndex = 6;
        }

        if (nTiles[1] == 'Y' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'Y' ||
            nTiles[0] == 'N' && nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'N' && nTiles[6] == 'Y' && nTiles[7] == 'Y')
        {
            tileIndex = 7;
        }

        if (nTiles[1] == 'Y' && nTiles[2] == 'N' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'N' && nTiles[7] == 'Y' ||
            nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'N' && nTiles[7] == 'N' ||
            nTiles[1] == 'Y' && nTiles[2] == 'Y' && nTiles[3] == 'N' && nTiles[4] == 'Y' && nTiles[6] == 'N' && nTiles[7] == 'Y' ||
            nTiles[0] == 'Y' && nTiles[1] == 'Y' && nTiles[2] == 'N' && nTiles[3] == 'Y' && nTiles[4] == 'Y' && nTiles[5] == 'Y' && nTiles[6] == 'Y' && nTiles[7] == 'Y')
        {
            tileIndex = 8;
        }

        if (nTiles[4] == '4' && nTiles[3] == 'N')
        {
            tileIndex = 9;
        }



        if (tileIndex < m_tileSprites.Length)
        {
            p_tileData.sprite = m_tileSprites[tileIndex];
        }

    }


    //Checks to see if the tile neighbouring tile being checked is the same as the current tile
    private bool IsSameTile(ITilemap tMap, Vector3Int pos)
    {

        //returns true if it is the same tilemap type
        return tMap.GetTile(pos) == this;
    }


    //Only appears in unity editor
#if UNITY_EDITOR

    //creates a cliclabke thing to create this scriptable object

    //found under Assets, create, tiles, and there
    [MenuItem("Assets/Create/Tiles/AdjustableTile")]


    //Saves it in the porject?
    public static void CreateAdjustableTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save AdjustableTile", "New AdjustableTile", "asset", "Save AdjustableTile", "Assets");

        if (path == "")
        {
            return;
        }
        //When clicked, creates a new scriptable object
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BackgroundTiles>(), path);
    }


#endif

}
