using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    //NOTE: I USED THIS VIDEO TO HELP WITH THE NEURAL NETWORK LOGIC -> https://www.youtube.com/watch?v=Yq0SfuiOVYE
    //Found InitBias function from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
    //private System.Random random; 
    private int[] layers;
    private float[][] neurons; //connections between neurons 
    private float[][] biases; 
    private float[][][] weights;
    //private float[numberOfLayers][] biases;
    //private float[numberOfLayers][][] weights;
    //private int[] activations;

    public float fitness = 0; 

    // Start is called before the first frame update
    //Constructor for NeuralNetwork which takes in layer information to populate data structs here 
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];

        }

        //random = new System.Random(System.DateTime.Today.Millisecond);//random seed 
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

    //Deep copy constructor 
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


    //Generate Neuron array
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

        //throw new NotImplementedException();
    }


    void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
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

    //Generate Weight array
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
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); //(float)random.NextDouble() - 0.5f; //between -0.5 and 0.5 
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

    private void CopyWeights(float[][][] copyWeights)
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
    }


    //https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
    public float[] FeedForward(float[] inputs)
    {
        //iterate through inputs and put into input layer in neuron matrix 
        //iterate over every neuron that has a weight connection 

        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //2nd index so we start at 1 to address properly 
        for (int i = 1; i < layers.Length; i++)
        {
            //int layer = i - 1;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                //iterate over every neuron in each layer 

                float value = 0f; 
                //float value = 0.25f; //constant bias of 0.25f will change this later when I implement biases 
                for(int k = 0; k < neurons[i - 1].Length; k++)
                {
                    //iterate over all neurons in previous layer
                    //
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                //apply activation
                //hyperbolic tangent activaiton (converts value between -1 and 1) 
                //neurons[i][j] = (float)Math.Tanh(value); //new value of current neuron
                neurons[i][j] = Activate(value + biases[i][j]);
            }
        }
        //Return output layer 
        return neurons[neurons.Length - 1]; //returning the last layer (output layer) 
    }

    //found here https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
    public float Activate(float value)
    {
        return (float)Math.Tanh(value); 
    }

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

  /*  //mutation function (chance based); iterate through all layers, neurons (and all connections to previous layer)
    //8% chance of mutation, 2% chance for each individual type of mutation 
    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //random mutation via random float generation between 0 and 1000
                    float randomNumber = UnityEngine.Random.Range(0f, 1000f); //(float)random.NextDouble() * 1000f;

                    //4 different types of mutations 
                    if (randomNumber <= 2f ) 
                    {
                        //flip sign of weight 
                        weight *= -1f; 
                    }
                    else if(randomNumber <= 4f)
                    {
                        //pick random weight between -0.5 and 0.5
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if(randomNumber <= 6f)
                    {
                        //random increase between 0% and 100% 
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if(randomNumber <= 8f)
                    {
                        //random decrease between 0% and 100% 
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }
                }
            }
        }
    }*/

    //Compares 1 NeuralNetwork to another 
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


    //load biases and weights from file
    //found from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
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

    //https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
    //saves biases and weights within network to file
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
    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }
}
