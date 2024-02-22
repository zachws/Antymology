using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    //NOTE: I USED THIS VIDEO TO HELP WITH THE NEURAL NETWORK LOGIC -> https://www.youtube.com/watch?v=Yq0SfuiOVYE
    //Found InitBias function from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs


    #region NetworkFields
    //Variables pertinent to the Neural Netowrk. 
    private int[] layers;
    private float[][] neurons;
    private float[][] biases; 
    private float[][][] weights;
    public float fitness = 0;
    #endregion


    /// <summary>
    /// Function which takes an input array of the network dimensions and populates all the arrays. (Constructor)
    /// </summary>
    /// <param name="layers"></param>
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];

        }
        InitNeurons();
        InitBiases();
        InitWeights(); 
    }

    /*    //Deep copy constructor
        public NeuralNetwork(NeuralNetwork copyNetwork)
        {
            this.layers = new int[copyNetwork.layers.Length];
            for(int i = 0; i <  copyNetwork.layers.Length; i++)
            {
                this.layers[i] = copyNetwork.layers[i];
            }

            InitNeurons();
            InitWeights();

            CopyWeights(copyNetwork.weights);

            //Deepcopy weights
        }*/

    /// <summary>
    /// Deep copy constructor for the Neural Network (copies another network into this one) 
    /// </summary>
    /// <param name="networkToCopy"></param>
    /// <returns></returns>
    public NeuralNetwork Copy(NeuralNetwork networkToCopy)
    {
        for(int i = 0; i <  biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                networkToCopy.biases[i][j] = biases[i][j];
            }
        }

        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    networkToCopy.weights[i][j][k] = weights[i][j][k];
                }
            }
        }

        return networkToCopy; 
    }


    /// <summary>
    /// Create empty storage array for the neurons in the network.
    /// </summary>
    void InitNeurons()
    {
        //Create list then convert to jagged array
        List<float[]> neuronList = new List<float[]>(); //Each layer has it's own float array (i.e., number of neurons in that array)

        //Iterate through layers and create new float array based on number of neurons in the layer and adding it to the neuron list 
            //list has .toArray to convert to jagged array 
        for(int i = 0; i< layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }
        neurons = neuronList.ToArray(); //convert from list float arrays to jagged array 
    }


    /// <summary>
    /// Initialize biases; size of biases is same size as size of neurons
    /// </summary>
    void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();//Data struct to hold temporary bias info to populate biases global variable. 

        for(int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];

            for(int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }

            biasList.Add(bias); 
        }

        biases = biasList.ToArray();
    }

   /// <summary>
   /// Initializes random array for the weights being held in the network.
   /// </summary>
    void InitWeights()
    {

        List<float[][]> weightList = new List<float[][]>(); //list of 2D float jagged arrays 

        //iterate in for loop through numer of neurons and weight connections that each neuron has 
        //start with first hidden layer (index 1); each layer has its own weight matrix for a neuron 
        for(int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();//float arrays represents actual weight for every single neuron
            int neuronsInPreviousLayer = layers[i - 1]; //how many neurons in previous layer 

            //iterate over all neurons in current layer 
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];//neuron weights 

                for(int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //give random weights to neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                //add neuron weights to weightList 
                layerWeightList.Add(neuronWeights);
            }
            //convert to jagged 2D float array 
            weightList.Add(layerWeightList.ToArray());

        }

        //convert weightList to 3D jaged float array
        weights = weightList.ToArray();
    }

    /*    private void CopyWeights(float[][][] copyWeights)
        {
            //iterate through all weights 
            for(int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length;k++)
                    {
                        weights[i][j][k] = copyWeights[i][j][k];
                    }
                }
            }
        }*/


    /// <summary>
    /// This is the FeedForward algorithm, which takes input parameters (weighted/biased) and returns an output array of floats which determines the actions an ant will make depending on input variables. 
    /// Using Tanh as activation function as it allows for both positive and negative values (i.e., positive reinforcement & negative reinforcement)
    /// </summary>
    /// <source link="https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs"></source>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        for(int i = 0; i < inputs.Length; i++) //iterate through inputs and put into input layer in neuron matrix, then iterate over every neuron that has a weight connection. 
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; i++) //2nd index so we start at 1 to address properly 
        {
            for (int j = 0; j < neurons[i].Length; j++) //iterate over every neuron in each layer 
            {
                float value = 0f; 
                //float value = 0.25f; //constant bias of 0.25f will change this later when I implement biases 
                for(int k = 0; k < neurons[i - 1].Length; k++)
                {
                    //iterate over all neurons in previous layer
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = Activate(value + biases[i][j]); //apply Tanh activation function
            }
        }
        return neurons[neurons.Length - 1]; //returning the last layer (output layer) 
    }

    /// <summary>
    /// Tanh activation function. Can take in both positive and negative values, allowing for positive & negative reinforcement of ant behaviors for training.
    /// </summary>
    /// <source link="https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs"></source>
    /// <param name="value"></param>
    /// <returns></returns>
    public float Activate(float value)
    {
        return (float)Math.Tanh(value); 
    }

    /// <summary>
    /// Mutation function which applies genetic mutations from one generation to the next depending on fitness values
    /// </summary>
    /// <param name="chance"></param>
    /// <param name="val"></param>
    public void Mutate(int chance, float val)
    {

        for(int i = 0; i < biases.Length; i++)
        {
            for(int j = 0; j < biases[i].Length ; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length ; j++)
            {
                for(int k = 0; k < weights[i][j].Length ; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];
                }
            }
        }
    }

    /// <summary>
    /// Comparing 2 NeuralNetworks performances based on fitness value(s)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(NeuralNetwork other)
    {
        if(other == null)
        {
            return 1;
        }

        if (fitness > other.fitness)
        {
            return 1; 
        }
        else if(fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0; 
        }
    }

    /// <summary>
    /// Clone learnable values (weights & biases) within a file into the Neural Network.
    /// </summary>
    /// <source link="https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs"></source>
    /// <param name="path"></param>
    public void Load(string path)
    {
        TextReader inStream = new StreamReader(path);
        int numberOfLines = (int)new FileInfo(path).Length; 
        string[] listLines = new string[numberOfLines];
        int index = 1; 

        for(int i = 1; i < numberOfLines; i++)
        {
            listLines[i] = inStream.ReadLine();
        }
        inStream.Close();

        if(new FileInfo(path).Length > 0)
        {

            for(int i = 0; i < biases.Length; i++)
            {
                for(int j = 0; j< biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(listLines[index]);
                    index++;
                }
            }

            for(int i = 0; i < weights.Length; i++)
            {
                for(int j = 0; j< weights[i].Length; j++)
                {
                    for(int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(listLines[index]);
                        index++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Saves learned biases & weights to file so that we can load new generation/test from last trained ants data. 
    /// </summary>
    /// <source link="https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs"></source>
    /// <param name="path"></param>
    public void Save(string path)
    {

        File.Create(path).Close();
        StreamWriter outStream = new StreamWriter(path, true);

        for(int i = 0; i < biases.Length;i++)
        {
            for(int j = 0; j< biases[i].Length; j++)
            {
                outStream.WriteLine(biases[i][j]);  
            }
        }

        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    outStream.WriteLine(weights[i][j][k]);
                }
            }
        }
        outStream.Close();

    }

    /// <summary>
    /// Setter to allow increment/decrement to fitness value within the Neural Network
    /// </summary>
    /// <param name="fit"></param>
    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    /// <summary>
    /// Setter to initialize/change the fitness value of this Neural Network with a different value. 
    /// </summary>
    /// <param name="fit"></param>
    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    /// <summary>
    /// Getter to allow other classes to get the fitness of this particular instance of the Neural Network (comparing fitnesses)
    /// </summary>
    /// <returns></returns>
    public float GetFitness()
    {
        return fitness;
    }
}
