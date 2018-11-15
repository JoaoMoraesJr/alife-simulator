/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

/// <summary>
/// Singleton class managing the current track and all cars racing on it, evaluating each individual.
/// </summary>
public class TrackManager : MonoBehaviour
{
    #region Members
    public static TrackManager Instance
    {
        get;
        private set;
    }

    // Sprites for visualising best and second best cars. To be set in Unity Editor.
    [SerializeField]
    private Sprite BestCarSprite;
    [SerializeField]
    private Sprite SecondBestSprite;
    [SerializeField]
    private Sprite NormalCarSprite;

    //private Checkpoint[] checkpoints;

    /// <summary>
    /// Car used to create new cars and to set start position.
    /// </summary>
    public AlifeController PrototypeAlife;
    public Food PrototypeFood;
    // Start position for cars
    private Vector3 startPosition;
    private Quaternion startRotation;

    public int foodAmount;

    // Struct for storing the current cars and their position on the track.
    private class AlifeAgent
    {
        public AlifeAgent(AlifeController alife = null, uint checkpointIndex = 1)
        {
            this.Alife = alife;
            this.CheckpointIndex = checkpointIndex;
        }
        public AlifeController Alife;
        public uint CheckpointIndex;
    }
    private List<AlifeAgent> alifes = new List<AlifeAgent>();

    public List<Food> foodList = new List<Food>();

    /// <summary>
    /// The amount of cars currently on the track.
    /// </summary>
    public int AlifeCount
    {
        get { return alifes.Count; }
    }

    #region Best and Second best
    private AlifeController bestAlife = null;
    /// <summary>
    /// The current best car (furthest in the track).
    /// </summary>
    public AlifeController BestAlife
    {
        get { return bestAlife; }
        private set
        {
            if (bestAlife != value)
            {
                //Update appearance
                if (BestAlife != null)
                    BestAlife.SpriteRenderer.sprite = NormalCarSprite;
                if (value != null)
                    value.SpriteRenderer.sprite = BestCarSprite;

                //Set previous best to be second best now
                AlifeController previousBest = bestAlife;
                bestAlife = value;
                if (BestCarChanged != null)
                    BestCarChanged(bestAlife);

                SecondBestCar = previousBest;
            }
        }
    }
    /// <summary>
    /// Event for when the best car has changed.
    /// </summary>
    public event System.Action<AlifeController> BestCarChanged;

    private AlifeController secondBestCar = null;
    /// <summary>
    /// The current second best car (furthest in the track).
    /// </summary>
    public AlifeController SecondBestCar
    {
        get { return secondBestCar; }
        private set
        {
            if (SecondBestCar != value)
            {
                //Update appearance of car
                if (SecondBestCar != null && SecondBestCar != BestAlife)
                    SecondBestCar.SpriteRenderer.sprite = NormalCarSprite;
                if (value != null)
                    value.SpriteRenderer.sprite = SecondBestSprite;

                secondBestCar = value;
                if (SecondBestCarChanged != null)
                    SecondBestCarChanged(SecondBestCar);
            }
        }
    }
    /// <summary>
    /// Event for when the second best car has changed.
    /// </summary>
    public event System.Action<AlifeController> SecondBestCarChanged;
    #endregion

    

    /// <summary>
    /// The length of the current track in Unity units (accumulated distance between successive checkpoints).
    /// </summary>
    public float TrackLength
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Mulitple instance of TrackManager are not allowed in one Scene.");
            return;
        }

        Instance = this;

        //Get all checkpoints
        //checkpoints = GetComponentsInChildren<Checkpoint>();

        //Set start position and hide prototype
        startPosition = PrototypeAlife.transform.position;
        startRotation = PrototypeAlife.transform.rotation;
        PrototypeAlife.gameObject.SetActive(false);

       // CalculateCheckpointPercentages();
    }

    void Start()
    {
        //Hide checkpoints
        //foreach (Checkpoint check in checkpoints)
            //check.IsVisible = false;
    }
    #endregion

    #region Methods
    // Unity method for updating the simulation
    void Update()
    {
        //Update reward for each enabled car on the track
        for (int i = 0; i < alifes.Count; i++)
        {
            AlifeAgent car = alifes[i];
            if (car.Alife.enabled)
            {
                //car.Car.CurrentCompletionReward = GetCompletePerc(car.Car, ref car.CheckpointIndex);

                //Update best
                /*
                if (BestCar == null || car.Car.CurrentCompletionReward >= BestCar.CurrentCompletionReward)
                    BestCar = car.Car;
                else if (SecondBestCar == null || car.Car.CurrentCompletionReward >= SecondBestCar.CurrentCompletionReward)
                    SecondBestCar = car.Car;
                    */
            }
        }
    }

    public void SetAlifeAmount(int amount)
    {
        //Check arguments
        if (amount < 0) throw new ArgumentException("Amount may not be less than zero.");

        if (amount == AlifeCount) return;

        if (amount > alifes.Count)
        {
            //Add new cars
            for (int toBeAdded = amount - alifes.Count; toBeAdded > 0; toBeAdded--)
            {
                GameObject alifeCopy = Instantiate(PrototypeAlife.gameObject);
                alifeCopy.transform.position = startPosition;
                alifeCopy.transform.rotation = startRotation;
                AlifeController controllerCopy = alifeCopy.GetComponent<AlifeController>();
                alifes.Add(new AlifeAgent(controllerCopy, 1));
                alifeCopy.SetActive(true);
            }
        }
        else if (amount < alifes.Count)
        {
            //Remove existing cars
            for (int toBeRemoved = alifes.Count - amount; toBeRemoved > 0; toBeRemoved--)
            {
                AlifeAgent last = alifes[alifes.Count - 1];
                alifes.RemoveAt(alifes.Count - 1);

                Destroy(last.Alife.gameObject);
            }
        }
    }

    public void SpawnFood(int amount)
    {
        if (foodList.Count > 1)
        {
            foreach (Food f in foodList)
            {
                if (f != null)
                Destroy(f.gameObject);
            }
            foodList.Clear();
        }
        GameObject simulationAreaObj = new GameObject();
        Transform simulationArea = simulationAreaObj.transform;
        simulationArea.position = new Vector3(0, 0, 0);
        simulationArea.localScale = new Vector3(15, 9, 1);
        for (int i = 0; i < amount; i++)
        {
            var food = Instantiate(PrototypeFood);
            food.transform.position = simulationArea.position + new Vector3(UnityEngine.Random.Range(-1f, 1f) * simulationArea.localScale.x / 2, UnityEngine.Random.Range(-1f, 1f) * simulationArea.localScale.y / 2, 0);
            foodList.Add(food);
        }
        Destroy(simulationAreaObj);
        Destroy(simulationArea);
    }

    /// <summary>
    /// Restarts all cars and puts them at the track start.
    /// </summary>
    public void Restart()
    {
        SpawnFood(foodAmount);
        foreach (AlifeAgent alife in alifes)
        {
            alife.Alife.transform.position = startPosition;
            alife.Alife.transform.rotation = startRotation;
            alife.Alife.Restart();
            alife.CheckpointIndex = 1;
        }

        BestAlife = null;
        SecondBestCar = null;
    }

    /// <summary>
    /// Returns an Enumerator for iterator through all cars currently on the track.
    /// </summary>
    public IEnumerator<AlifeController> GetAlifeEnumerator()
    {
        for (int i = 0; i < alifes.Count; i++)
            yield return alifes[i].Alife;
    }

    /// <summary>
    /// Calculates the percentage of the complete track a checkpoint accounts for. This method will
    /// also refresh the <see cref="TrackLength"/> property.
    /// </summary>
    /// 
    /*
    private void CalculateCheckpointPercentages()
    {
        checkpoints[0].AccumulatedDistance = 0; //First checkpoint is start
        //Iterate over remaining checkpoints and set distance to previous and accumulated track distance.
        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].DistanceToPrevious = Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            checkpoints[i].AccumulatedDistance = checkpoints[i - 1].AccumulatedDistance + checkpoints[i].DistanceToPrevious;
        }

        //Set track length to accumulated distance of last checkpoint
        TrackLength = checkpoints[checkpoints.Length - 1].AccumulatedDistance;
        
        //Calculate reward value for each checkpoint
        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].RewardValue = (checkpoints[i].AccumulatedDistance / TrackLength) - checkpoints[i-1].AccumulatedReward;
            checkpoints[i].AccumulatedReward = checkpoints[i - 1].AccumulatedReward + checkpoints[i].RewardValue;
        }
    }
    
    // Calculates the completion percentage of given car with given completed last checkpoint.
    // This method will update the given checkpoint index accordingly to the current position.
    private float GetCompletePerc(CarController car, ref uint curCheckpointIndex)
    {
        //Already all checkpoints captured
        if (curCheckpointIndex >= checkpoints.Length)
            return 1;

        //Calculate distance to next checkpoint
        float checkPointDistance = Vector2.Distance(car.transform.position, checkpoints[curCheckpointIndex].transform.position);

        //Check if checkpoint can be captured
        if (checkPointDistance <= checkpoints[curCheckpointIndex].CaptureRadius)
        {
            curCheckpointIndex++;
            car.CheckpointCaptured(); //Inform car that it captured a checkpoint
            return GetCompletePerc(car, ref curCheckpointIndex); //Recursively check next checkpoint
        }
        else
        {
            //Return accumulated reward of last checkpoint + reward of distance to next checkpoint
            return checkpoints[curCheckpointIndex - 1].AccumulatedReward + checkpoints[curCheckpointIndex].GetRewardValue(checkPointDistance);
        }
    }
    */
    #endregion

}
