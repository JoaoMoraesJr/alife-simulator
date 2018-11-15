
#region Includes
using UnityEngine;
#endregion

/// <summary>
/// Class representing a controlling container for a 2D physical simulation
/// of a car with 5 front facing sensors, detecting the distance to obstacles.
/// </summary>
public class AlifeController : MonoBehaviour
{
    #region Members

    public float timeAlive = 0;
    private EnergyControll EnergyCtrl;
    public bool isDead = false;

    private int generation = 0;

    #region IDGenerator
    // Used for unique ID generation
    private static int idGenerator = 0;
    /// <summary>
    /// Returns the next unique id in the sequence.
    /// </summary>
    private static int NextID
    {
        get { return idGenerator++; }
    }
    #endregion

    // Maximum delay in seconds between the collection of two checkpoints until this car dies.
    private const float MAX_CHECKPOINT_DELAY = 7;

    /// <summary>
    /// The underlying AI agent of this car.
    /// </summary>
    public Agent Agent
    {
        get;
        set;
    }

    public float CurrentCompletionReward
    {
        get { return Agent.Genotype.Evaluation; }
        set { Agent.Genotype.Evaluation = value; }
    }

    /// <summary>
    /// Whether this car is controllable by user input (keyboard).
    /// </summary>
    public bool UseUserInput = false;

    /// <summary>
    /// The movement component of this car.
    /// </summary>
    public AlifeMovement Movement
    {
        get;
        private set;
    }

    /// <summary>
    /// The current inputs for controlling the CarMovement component.
    /// </summary>
    public double[] CurrentControlInputs
    {
        get { return Movement.CurrentInputs; }
    }

    /// <summary>
    /// The cached SpriteRenderer of this car.
    /// </summary>
    public SpriteRenderer SpriteRenderer
    {
        get;
        private set;
    }

    private Sensor[] sensors;
    private float timeSinceLastCheckpoint;
    #endregion

    #region Constructors
    void Awake()
    {
        //Cache components
        Movement = GetComponent<AlifeMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        EnergyCtrl = GetComponent<EnergyControll>();
        sensors = GetComponentsInChildren<Sensor>();
    }
    void Start()
    {
        //Movement.HitWall += Die;

        //Set name to be unique
        this.name = "Agent (" + NextID + ")";
    }
    #endregion

    #region Methods
    /// <summary>
    /// Restarts this car, making it movable again.
    /// </summary>
    public void Restart()
    {
        SpriteRenderer.enabled = true;
        Movement.enabled = true;
        timeSinceLastCheckpoint = 0;
        EnergyCtrl.Restart();
        timeAlive = 0;
        //foreach (Sensor s in sensors)
         //   s.Show();

        Agent.Reset();
        this.enabled = true;
        Debug.Log(Agent.FNN.ToString());
    }

    // Unity method for normal update
    void Update()
    {
        //timeSinceLastCheckpoint += Time.deltaTime;
        timeAlive += Time.deltaTime;
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        //Get control inputs from Agent
        if (!UseUserInput)
        {
            //Get readings from sensors
            double[] NNInputs = new double[sensors.Length + 1];
            for (int i = 0; i < sensors.Length; i++)
            {
                NNInputs[i] = sensors[i].Output;
                //Debug.Log(sensorOutput[i]);
            }
            NNInputs[sensors.Length] = EnergyCtrl.energy;
            //Debug.Log(Agent.FNN.ToString());
            double[] controlInputs = Agent.FNN.ProcessInputs(NNInputs);
            //Movement.SetInputs();//controlInputs);
            //Debug.Log("sensor1: " + NNInputs[0] + " energy: " + NNInputs[1] + " h: " + controlInputs[0] + " v:" + controlInputs[1]);
            Movement.SetInputs(controlInputs);
            string s = " ";
            for (int i = 0; i < NNInputs.Length; i++)
            {
                s += " " + i + ": " + NNInputs[i];
            }
            //Debug.Log(s);
        }

        if (EnergyCtrl.energy <= 0)
        {
            Die();
        }
    }

    // Makes this car die (making it unmovable and stops the Agent from calculating the controls for the car).
    private void Die()
    {
        this.enabled = false;
        Movement.Stop();
        Movement.enabled = false;
        SpriteRenderer.enabled = false;

        // foreach (Sensor s in sensors)
        //   s.Hide();
        //Debug.Log(Agent.Genotype.Fitness);
        Agent.Genotype.Fitness = timeAlive;
        Agent.Kill(timeAlive);
    }

    public void CheckpointCaptured()
    {
        timeSinceLastCheckpoint = 0;
    }
    #endregion

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            EnergyCtrl.gainEnergy (other.gameObject.GetComponent<Food>().Kill());
        }

        if (other.tag == "Wall")
        {
            Movement.Stop();
            //Kill();
        }
    }


    public void Kill()
    {
        isDead = true;
    }

    public void DestroyAgent()
    {

        Destroy(this.gameObject);
    }

}
