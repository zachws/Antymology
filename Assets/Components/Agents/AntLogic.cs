using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Terrain;
using UnityEngine.UIElements;
using System.Linq;


/*
 * This file is used to hold information pertinent to a particular ant. (i.e., object which holds all of the information for 1 ant) 
 * 
 *   Variables: 
 *      1. Starting Ant Health Variable -> startingHealth
 *      2. Ant Health -> antHealth
 *      3. Decrease Ant Health -> decreaseAntHealthAmt
 *      4. Time Step -> timer
 *      5. Ant Prefab -> antPrefab
 *      6. Ants Position in the simulation -> antPosition
 *      
 *  Functions:
 *      1. Ant Death Function -> killAnt()
 *          //Ant dies when health = 0 
 *      2. Ant Eats Mulch -> eatMulch()
 *          //Increment health after eating mulch and destroys eaten block/mulch 
 *      3. Ant Digs Block -> digBlock()
 *          //Can't dig up blocks of type ContainerBlock
 *      4. Time Step Logic -> timeStepActions()
 *          //Decrease Health each Time Step 
 */


public class AntLogic : MonoBehaviour
    {
        //public WorldManager myWorldManager; 
        //Serialize to make private variables accessible in Unity editor without making them public 
        public float antHealth;
        [SerializeField]
        private float timer;
        public Vector3 antPosition;
        public List<Vector3> possibleMovementChoices;
        private List<AntLogic> neighborList;
        private float closestNeighborDist; 
        public bool isQueen; 

    //Maybe move these to the config manager; will change later 
        [SerializeField]
        private int sharedHealthCount = 0; //shared to queen
        [SerializeField]
        private int moveToQueenCount = 0;
        [SerializeField]
        private int mulchCount = 0;
    [SerializeField]
    private int sharedWithQueen = 0; 

/*        private static float distanceToQueenWeight = -0.1f;
    private static float queenTotalHealthWeight = 0.5f;
    private static float nestBlocksBuiltWeight = 1.0f; */

        public static int BLOCK_LEVEL = 0;
        public static int BLOCK_BELOW = 1;
        public static int BLOCK_ABOVE = 2;
        public static string AIR_BLOCK = "Air";
        public static string CONTAINER_BLOCK = "Container";
        public static string ACIDIC_BLOCK = "Acidic";
        public static string GRASS_BLOCK = "Grass";
        public static string MULCH_BLOCK = "Mulch";
        public static string NEST_BLOCK = "Nest";
        public static string STONE_BLOCK = "Stone";


    //static variables for Neural Network stuff
    public NeuralNetwork network;
    public static int MOVE_FORWARD = 0; 
    public static int MOVE_BACKWARD = 1;
    public static int MOVE_LEFT = 2;
    public static int MOVE_RIGHT = 3; 
    public static int DIG = 4;
    public static int EAT_MULCH = 5;
    public static int SHARE_HEALTH = 6;
    public static int BUILD_NEST = 7;
    public static int MOVE_TO_QUEEN = 8; // might just use normal functions with a check to determine weight if we truly want to go towards the queen. 
    public static int SKIP_TURN = 9; //do nothing

    //input for neural network/ant behavior 
    private float[] input = new float[10]; // 10 actions 

    /*
     * 
     * Output Logic: i.e., what we feed to NeuralNetwork.FeedForward to get the input list back for our movement decisions
     *  
     *  Normal Ant: 
     *      1. Current Health
     *      2. Mulch Consumed
     *      3. Blocks Dug
     *      4. Amount of times health donated to Queen 
     *      5. Distance from queen
     *  
     *  Queen Ant: 
     *      1. Current Health
     *      2. Mulch Consumed
     *      3. Number of nest blocks built
     *      4. How many neighbors nearby 
     *      5. Distance to nearby neighbors 
     * 
     */

    private float[] PackageAntNeuralNetwork(float currentHealth, float mulchCount, float blocksDug, float sharedHealthCount, float distFromBae)
    {
        float[] input = new float[] { currentHealth, mulchCount, blocksDug, sharedHealthCount, distFromBae };
        return network.FeedForward(input);
    }

    private float[] PackageQueenNeuralNetwork(float currentHealth, float mulchCount, float numNestBlocksBuilt, float numNeighborsNear, float distToNearNeighbors)
    {
        float[] input = new float[] { currentHealth, mulchCount, numNestBlocksBuilt, numNeighborsNear, distToNearNeighbors };
        return network.FeedForward(input);
    }

    private void Awake()
    {
        //myWorldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
    }

    // Start is called before the first frame update
    void Start()
        {
            //At the start of the ant creation, set their health to the startingHealth constant (will change as I progress through the assignment) 
            this.antHealth = ConfigurationManager.Instance.Starting_Health;
            //Initialize the timer to zero, will base timeStep off of timers difference from the current local time 
            this.timer = 0.0f;
            isQueen = false; 
            possibleMovementChoices = new List<Vector3>();
            neighborList = new List<AntLogic>();
            closestNeighborDist = -1;
        this.gameObject.tag = "Ant";

    }

        // Update is called once per frame
        void Update()
        {
            Vector3 currPos = CurrentPosition();
            string currLevel = GetBlock(BLOCK_LEVEL, currPos);
            bool isAcidic = false; 
            //decrease health every frame? or should it happen differently? 
            if(currLevel == ACIDIC_BLOCK)
            {
                isAcidic = true; 
            }
            DecreaseHealth(isAcidic);
        //check if the ant should die after health decrease 
        if (this.antHealth <= 0.0f)
        {
            KillAnt();
        }
        Move();

        //TESTING SHARE HEALTH
        //
        List<AntLogic> antList = WorldManager.Instance.Ants;
        Vector3 queenPos = WorldManager.Instance.queenLogic.CurrentPosition();
        queenPos.y += 0.5f; 
        antList[0].transform.position =queenPos;
        PopulateNeighborList(antList[0].transform.position, 5.0f);
        DonateHealth(); 



            //DigBlock(); 
            //update time step??

            //had to do this so that the prefab coordinates would actually match the "ants coordinates" and display properly
            //transform.position = this.antPosition;
        }

    /// <summary>
    /// Finds neighbors within a predefined distance via temporary Collider
    /// </summary>
    /// <source link="https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html"></source>
    /// <param name="posOfCaller"></param>
    /// <param name="radius"></param>
    public void PopulateNeighborList(Vector3 posOfCaller, float radius)
    {
        neighborList.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 5.0f);
        closestNeighborDist = 5000;

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("ok");
            if(hitCollider.gameObject.CompareTag("Ant") ) //|| hitCollider.tag.Equals("thaQueen"))
            {
                Debug.Log("whew");
                if (!hitCollider.gameObject.Equals(this.gameObject))
                {
                    //continue; 
                    if ((transform.position - hitCollider.gameObject.transform.position).magnitude < closestNeighborDist)
                    {
                        closestNeighborDist = (transform.position - hitCollider.gameObject.transform.position).magnitude;
                    }

                    neighborList.Add(hitCollider.gameObject.GetComponent<AntLogic>());
                }



                
            }
        }
    }

    public void Move()
    {
        //Debug.Log($"Ant Position before move: x: {this.transform.position.x}, y: {this.transform.position.y}, z: {this.transform.position.z}");
        List<Vector3> validMovementChoices = GetValidMovements();
        if (validMovementChoices.Count > 0 )
        {
            Vector3 blockToMoveTo = MakeRandomMovementChoice(validMovementChoices);
            this.transform.position = blockToMoveTo;
            this.antPosition = blockToMoveTo;
        }
        else
        {
           transform.position = this.transform.position;
        }

    }






    private List<Vector3> GetValidMovements()
    {
        Vector3 currentPosition = CurrentPosition(); 

        List<Vector3> allPositionChoices = new List<Vector3>(); 
        List<Vector3> validPositionChoices = new List<Vector3>();

        //Debug.Log($"Current Ant Position: x: {currentPosition.x}, y: {currentPosition.y}, z: {currentPosition.z}");


        //check forward (i.e., x + 1) 
        Vector3 forwardPosition = new Vector3(currentPosition.x + 1, currentPosition.y, currentPosition.z);
        //Debug.Log($"forward applied, now position is: x: {forwardPosition.x}, y: {forwardPosition.y}, z: {forwardPosition.z}");
        allPositionChoices.Add(new Vector3(currentPosition.x + 1, currentPosition.y, currentPosition.z));
        //check backward (i.e., x - 1)
        allPositionChoices.Add(new Vector3(currentPosition.x - 1, currentPosition.y, currentPosition.z));
        //check left (i.e., z + 1) 
        allPositionChoices.Add(new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + 1));
        //check right (i.e., z - 1)
        allPositionChoices.Add(new Vector3(currentPosition.x, currentPosition.y, currentPosition.z - 1));

        //loop through each fd, bw, l,r movements for up and down possiblities ( y +- 1)
        int count = allPositionChoices.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 checkPosition = allPositionChoices[i];
            string currentLevel = GetBlock(BLOCK_LEVEL, checkPosition); 
            Vector3 upPosition = new Vector3(checkPosition.x, checkPosition.y + 1, checkPosition.z);
            string blockLevelUp = GetBlock(BLOCK_LEVEL, upPosition);
            string blockUpUp = GetBlock(BLOCK_ABOVE, upPosition);
            Vector3 downPosition = new Vector3(checkPosition.x, checkPosition.y - 1, checkPosition.z);
            string blockLevelDown = GetBlock(BLOCK_LEVEL, downPosition);
            //Debug.Log($"Current Level: {currentLevel}, x:{checkPosition.x} y:{checkPosition.y} z: {checkPosition.z};     Block Above: {currentUP} x: {upPosition.x} y: {upPosition.y} z: {upPosition.z};  Block Below: {currentDown} x: {downPosition.x} y: {downPosition.y} z: {downPosition.z}");

            //check for upward movement
            if (blockLevelUp != AIR_BLOCK && blockUpUp == AIR_BLOCK)
            {
                allPositionChoices.Add(new Vector3(checkPosition.x, checkPosition.y + 1, checkPosition.z));
            }
            //check for downward movement (i.e., y - 1)
            if(blockLevelDown != AIR_BLOCK && currentLevel == AIR_BLOCK)
            {
                allPositionChoices.Add(new Vector3(checkPosition.x, checkPosition.y - 1, checkPosition.z));
            }
           
        }
        

        foreach (var positionToCheck in allPositionChoices)
        {
            String blockLevel = GetBlock(BLOCK_LEVEL, positionToCheck);
            String blockAbove = GetBlock(BLOCK_ABOVE, positionToCheck);

            if(blockAbove == AIR_BLOCK && blockLevel != AIR_BLOCK) //&& blockBelow != "Air")
            {
                validPositionChoices.Add(positionToCheck);
            }

        }

        possibleMovementChoices = validPositionChoices;
        //calling method will handle logic if list is/isn't populated
        return validPositionChoices;
    }


    public string GetBlock(int blockChoice, Vector3 positionToCheck)
    {
        
        //Vector3 currPosition = CurrentPosition(); 
        int x = Mathf.RoundToInt(positionToCheck.x);
        int z = Mathf.RoundToInt(positionToCheck.z);
        String blockType = "";
        AbstractBlock blockToCheck; 
        if (blockChoice == BLOCK_LEVEL)
        {
            //Logic for block type at current level
            int y = Mathf.RoundToInt(positionToCheck.y);
            blockToCheck = WorldManager.Instance.GetBlock(x,y,z);
            blockType = blockToCheck.BlockType; 

        }
        else if(blockChoice == BLOCK_BELOW) 
        {
            //Logic for block type above current position
            int y = Mathf.RoundToInt(positionToCheck.y - 1);
            blockToCheck = WorldManager.Instance.GetBlock(x, y, z);
            blockType = blockToCheck.BlockType;
        }
        else if(blockChoice == BLOCK_ABOVE) 
        { 
            //Logic for block type below current position
            int y = Mathf.RoundToInt(positionToCheck.y + 1);
            blockToCheck = WorldManager.Instance.GetBlock(x, y, z);
            blockType = blockToCheck.BlockType;
        }
        else
        {
            //this should never be reached

            Debug.Log("Invalid block type choice");
        }
        return blockType; 

    }

    public Vector3 CurrentPosition()
    {
        Vector3 pos = this.transform.position;
        pos.y = Mathf.Floor(pos.y);
        return pos;
    }

   
    private Vector3 MakeRandomMovementChoice(List<Vector3> validChoices)
    {
        Vector3 theChoice = validChoices[UnityEngine.Random.Range(0, validChoices.Count)];
        theChoice.y = theChoice.y + 0.5f; //ant offset. 
        return theChoice; //validChoices[UnityEngine.Random.Range(0, validChoices.Count - 1)];
    }


/*    private bool IsInBounds(Vector3 blockToCheck)
    {
        //WorldManager.Instance.GetBlock()
        bool inBounds = false;

        //Debug.Log($"X: {blockToCheck.worldXCoordinate}, Y: {blockToCheck.worldYCoordinate}, Z: {blockToCheck.worldZCoordinate}");
        if (blockToCheck.x < 1.0f || blockToCheck.z < 1.0f || blockToCheck.y < 8.0f)
        {
            Debug.Log("First inbounds condition");
            inBounds = false;
        }
        else if (blockToCheck.y >= ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter)
        {
            inBounds = false;
            Debug.Log("Second inbounds condition");
        }
        else if (blockToCheck.x > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter || blockToCheck.z >= ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter)
        {
            inBounds = false;
            Debug.Log("Third inbounds condition");
        }
        else
        {
            inBounds = true;
            Debug.Log("It is in bounds");
        }

        Debug.Log($"inbounds is {inBounds}");
        return inBounds; 
    }*/

        public void KillAnt()
        {
            //kill ant stub
            //throw new NotImplementedException();
            this.gameObject.SetActive(false);
            
            //Remove ant from list if it's dead as we obviously don't want to base NN data on dead ants not doing anyting 
            //WorldManager.Instance.Ants.Remove(this);

            //Destroy(this.gameObject);
        }

        public void EatMulch()
        {
        //eat mulch stub
        this.antHealth += 1000;
        this.mulchCount += 1; 
        }

        public void DigBlock()
        {
            Vector3 currPosition = CurrentPosition();
            string blockBelow = GetBlock(BLOCK_LEVEL, currPosition); //as we will be inside of the block so the below block is the level block 
            if(blockBelow != CONTAINER_BLOCK)
            {
                if(blockBelow == MULCH_BLOCK)
                {
                    EatMulch(); 
                }
                //then we can dig 
                WorldManager.Instance.SetBlock(Mathf.RoundToInt(currPosition.x), Mathf.RoundToInt(currPosition.y), Mathf.RoundToInt(currPosition.z), new AirBlock());
                //update ant location 
                this.transform.position = new Vector3(currPosition.x, currPosition.y - 0.5f, currPosition.z);
            }
            //dig block stub
        }

/*        private void DonateHealth()
        {
        float distanceFromQueen;
        Vector3 queenPosition = WorldManager.Instance.queenLogic.CurrentPosition();
        Vector3 antPosition = this.CurrentPosition();

        distanceFromQueen = Vector3.Distance(queenPosition, antPosition);
        if(distanceFromQueen <= 2 && this.antHealth >= 1500.0f)
        {
            sharedHealthCount += 1;
            WorldManager.Instance.queenLogic.antHealth += 500.0f;
            this.antHealth -= 500.0f;

        }
            //stub for donating to the queen 
        }*/

        private void DonateHealth()
        {
            float minDonationHP = ConfigurationManager.Instance.Minimum_Health_To_Donate; 
            AntLogic tempNeighbor = null; 

            foreach (var neighbor in neighborList)
            {
            Debug.Log($"neighbor is {neighbor}");
                float neighborHealth = neighbor.antHealth;
                if( neighborHealth < ConfigurationManager.Instance.Minimum_Health_To_Donate)
                //if(neighborHealth < 15000.0f)
                {
                    minDonationHP = neighborHealth;
                    tempNeighbor = neighbor;
                }

            }

        if (tempNeighbor != null)
        {
            float newHealth = tempNeighbor.antHealth + ConfigurationManager.Instance.Donate_Health_Amount;
            this.antHealth -= ConfigurationManager.Instance.Donate_Health_Amount; 
            tempNeighbor.antHealth = newHealth;

            if (tempNeighbor.isQueen)
            {
                sharedWithQueen += 1; 
            }
            else
            {
                sharedHealthCount += 1;
            }
        }
            //int lowestHealth = 
        }
        private void TimeStepActions()
        {
            //time step action stub
            throw new NotImplementedException();
        }

        public void DecreaseHealth(bool isAcidic)
        {
            //This should decrease the ants health on each timestep 
            if (isAcidic == true)
            {
                this.antHealth -= (2 * ConfigurationManager.Instance.Decrease_Health_Amount);
            }
            else
            {
                this.antHealth -= ConfigurationManager.Instance.Decrease_Health_Amount;
            }
        }

    public void UpdateFitness()
    {
        float healthQueenFitness = sharedHealthCount * ConfigurationManager.Instance.Health_Share_Queen_Weight;
        float moveToQueenFitness = moveToQueenCount * ConfigurationManager.Instance.Move_Towards_Queen_Weight;
        float mulchConsumedFitness = mulchCount * ConfigurationManager.Instance.Mulch_Consumed_Weight;
        this.network.fitness = healthQueenFitness + moveToQueenFitness + mulchConsumedFitness;
    }

        //Queen logic after I get regular ant movement working (may as well just have states for queen rather than a separate C# file - not sure if this is better or worse programming, probably less modular) 

    }

