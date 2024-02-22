using Antymology.Terrain;
//using Antymology.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Queen : AntLogic
{
    //[SerializeField]
    //private static float startingHealth = 7500.0f;
    //[SerializeField]
    //private static float queenHealth;
    //[SerializeField]
    //private static float maxQueenHealth = 15000.0f;
    // Start is called before the first frame update
    [SerializeField]
    public int TotalNestBlocks = 0;
    [SerializeField]
    public Vector3 queenPosition;

    void Start()
    {
        //queenHealth = startingHealth;
        this.antHealth = ConfigurationManager.Instance.Starting_Queen_health;
        this.isQueen = true;
        this.gameObject.tag = "Ant";

    }

    // Update is called once per frame
    void Update()
    {
        this.queenPosition = this.antPosition; 
        Vector3 currPos = this.CurrentPosition();
        string currLevel = this.GetBlock(BLOCK_LEVEL, currPos);
        bool isAcidic = false;
        //decrease health every frame? or should it happen differently? 
        if (currLevel == ACIDIC_BLOCK)
        {
            isAcidic = true;
        }
        this.DecreaseHealth(isAcidic);
        if(this.antHealth <= 0)
        {
            this.KillAnt();
        }
        //GetBlock();

        //Move();
        //BuildNestBlock();
    }

    void BuildNestBlock()
    {
        Vector3 currPosition = CurrentPosition();
        currPosition.y += 0.5f;
        int amntHealthToBuild = Mathf.RoundToInt(0.33333f * ConfigurationManager.Instance.Maximum_Queen_Health);
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
