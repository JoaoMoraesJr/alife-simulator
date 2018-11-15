/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
#endregion

/// <summary>
/// Class representing a sensor reading the distance to the nearest obstacle in a specified direction.
/// </summary>
public class Sensor : MonoBehaviour
{
    #region Members
    // The layer this sensor will be reacting to, to be set in Unity editor.
    [SerializeField]
    private LayerMask LayerToSense;
    //The crosshair of the sensor, to be set in Unity editor.
    [SerializeField]
    private GameObject SensorLimit;

    // Max and min readings
    public float MAX_DIST = 10f;
    private const float MIN_DIST = 0.01f;

    /// <summary>
    /// The current sensor readings in percent of maximum distance.
    /// </summary>
    public float Output
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    void Start()
    {
        SensorLimit.gameObject.SetActive(true);
        LayerToSense.value = 8;
    }
    #endregion

    #region Methods
    // Unity method for updating the simulation
    void FixedUpdate()
    {
        //Calculate direction of sensor
        Vector2 direction = SensorLimit.transform.position - this.transform.position;
        direction.Normalize();

        //Send raycast into direction of sensor
        RaycastHit hit;
        Physics.Raycast(this.transform.position, direction, out hit, MAX_DIST);

        //Check distance

        if (hit.collider == null)
            hit.distance = MAX_DIST;
        else if (hit.distance < MIN_DIST)
            hit.distance = MIN_DIST;


        this.Output = hit.distance; //transform to percent of max distance
        SensorLimit.transform.position = (Vector2)this.transform.position + direction * hit.distance; //Set position of visual cross to current reading
        Debug.DrawLine(this.transform.position, SensorLimit.transform.position, Color.white, 0f);
    }


    /// <summary>
    /// Hides the crosshair of this sensor.
    /// </summary>
    public void Hide()
    {
        SensorLimit.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the crosshair of this sensor.
    /// </summary>
    public void Show()
    {
        SensorLimit.gameObject.SetActive(true);
    }
    #endregion
}
