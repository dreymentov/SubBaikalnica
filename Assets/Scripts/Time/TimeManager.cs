using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Internal Clock")]
    [SerializeField]
    GameTimeStamp timestamp;
    public float timeScale = 1.0f;

    [Header("Day and Night Cycle")]
    //the transform of the directional light (sun)
    public Transform sunTransform;
    Vector3 sunAngle;

    //list of objects to inpform of changes to the time
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //set the static instance to this instance
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialise the time stamp
        timestamp = new GameTimeStamp(1, 6, 0);
        StartCoroutine(TimeUpdate());
    }

    IEnumerator TimeUpdate()
    {
        while (true)
        {
            Tick();

            yield return new WaitForSeconds(1 / timeScale);
        }
    }

    void Tick()
    {
        timestamp.UpdateClock();
        UpdateSunMovement();

        //inform each listener of the new timestate
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }
    }

    //Day and Night cycle
    void UpdateSunMovement()
    {
        //convert the current time to minutes
        int timeInMinutes = GameTimeStamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        //during daytime
        //sun moves 0.2 degrees in a minute
        //during nighttime
        //sun moves 0.6 degrees in a minute

        float sunAngle = 0;
        if(timeInMinutes <= 15 * 60)
        {
            sunAngle = 0.2f * timeInMinutes;
        }
        else if(timeInMinutes > 15* 60)
        {
            sunAngle = 180f + 0.6f * (timeInMinutes - (15 * 60));
        }

        //Apply angle to the directional light
        //sunTransform.eulerAngles = new Vector3(sunAngle, 0, 0);
        this.sunAngle = new Vector3(sunAngle, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        sunTransform.rotation = Quaternion.Slerp(sunTransform.rotation, Quaternion.Euler(sunAngle), 1f * Time.deltaTime);
    }

    //function to skip time
    public void SkipTime(GameTimeStamp timeToSkipTo)
    {
        //convert to minutes
        int timeToSkipInMinutes = GameTimeStamp.TimestampInMinutes(timeToSkipTo);
        Debug.Log("Time Skip to:" + timeToSkipInMinutes);
        int timeNowInMinutes = GameTimeStamp.TimestampInMinutes(timestamp);
        Debug.Log("Time now: " + timeNowInMinutes);

        int differenceInMinutes = timeToSkipInMinutes - timeNowInMinutes;
        Debug.Log("Skip " + differenceInMinutes + " minutes");

        //Check if the timestamp to skip to has already been reached
        if (differenceInMinutes <= 0) return;

        for (int i=0; i < differenceInMinutes; i++)
        {
            Tick();
        }
    }

    public GameTimeStamp GetGameTimestamp()
    {
        //return a cloned instance
        return new GameTimeStamp(timestamp);
    }

    //handle listeners list

    //add an object to the list
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    //Remove an object from the list
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
}
