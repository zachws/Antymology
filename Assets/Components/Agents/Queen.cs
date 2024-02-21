using Antymology.Terrain;
//using Antymology.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Queen : AntLogic
{
    [SerializeField]
    private static float startingHealth = 7500.0f;
    [SerializeField]
    private static float queenHealth;
    [SerializeField]
    private static float maxQueenHealth = 15000.0f;
    // Start is called before the first frame update
    [SerializeField]
    public Vector3 queenPosition;
    [SerializeField]
    public int TotalNestBlocks = 0; 

    void Start()
    {
        //queenHealth = startingHealth;
        this.antHealth = startingHealth;

    }

    // Update is called once per frame
    void Update()
    {
        //GetBlock();

        //Move();
        //BuildNestBlock();
    }

    void BuildNestBlock()
    {
        Vector3 currPosition = CurrentPosition();
        currPosition.y += 0.5f;
        int amntHealthToBuild = Mathf.RoundToInt(0.33333f * maxQueenHealth);
        if (this.antHealth > amntHealthToBuild) 
        {
            this.antHealth -= amntHealthToBuild;
            TotalNestBlocks++;
            currPosition.y += 0.5f;
            WorldManager.Instance.SetBlock(Mathf.RoundToInt(currPosition.x), Mathf.RoundToInt(currPosition.y), Mathf.RoundToInt(currPosition.z), new NestBlock());
            this.transform.position = new Vector3(currPosition.x, currPosition.y + 0.5f, currPosition.z);

        }
    }
}
