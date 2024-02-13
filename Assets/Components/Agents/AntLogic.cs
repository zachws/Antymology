using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

namespace Antymology.Agents
{
    public class AntInformation : MonoBehaviour
    {
        //Serialize to make private variables accessible in Unity editor without making them public 
        [SerializeField]
        private static float startingHealth = 100.0f;
        [SerializeField]
        private float antHealth;
        [SerializeField]
        private static float decreaseAntHealthAmt = 1.0f;
        [SerializeField]
        private float timer;
        [SerializeField]
        private Vector3 antPosition;
        [SerializeField]
        private bool queen; 
        
        // Start is called before the first frame update
        void Start()
        {
            //At the start of the ant creation, set their health to the startingHealth constant (will change as I progress through the assignment) 
            this.antHealth = startingHealth;
            //Initialize the timer to zero, will base timeStep off of timers difference from the current local time 
            this.timer = 0.0f; 
            //Create ant body 
                //Need to create prefab so that I can actually use the AntBody provided and create a physical entity within the world 

        }

        // Update is called once per frame
        void Update()
        {
            //decrease health every frame? or should it happen differently? 
            DecreaseHealth();
            //check if the ant should die after health decrease 
            if(this.antHealth <= 0.0f)
            {
                KillAnt();
            }
            //update time step??
        }

        private void KillAnt()
        {
            //kill ant stub
            throw new NotImplementedException();
        }

        private void EatMulch()
        {
            //eat mulch stub
            throw new NotImplementedException();
        }

        private void DigBlock()
        {
            //dig block stub
            throw new NotImplementedException();
        }

        private void TimeStepActions()
        {
            //time step action stub
            throw new NotImplementedException();
        }

        private void DecreaseHealth()
        {
            //This should decrease the ants health on each timestep 
            this.antHealth -= decreaseAntHealthAmt; 
        }

        //Queen logic after I get regular ant movement working (may as well just have states for queen rather than a separate C# file - not sure if this is better or worse programming, probably less modular) 

    }
}
