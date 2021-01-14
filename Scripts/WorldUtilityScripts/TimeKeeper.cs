using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeOfDay
{
    Default,
    Morning,
    Afternoon,
    Evening,
    Night,
}

public class TimeKeeper : MonoBehaviour
{
    [SerializeField]
    Weather wx;
    [Header("Speed Settings")]
    public float daySpeed = 0;
    public float nightSpeed = 0;

    [Header("Time Info")]
    public int day = 0;
    public int hour = 0;
    public int min = 0;
    public float sec = 0;
    public float counter = 0;
    public float timeScale = 1;
    const float rotSec = 0.0041666667f;

    float setTimeScale;
    public TimeOfDay currentTOD = TimeOfDay.Default;
    void Start()
    {
        setTimeScale = timeScale;
        nightSpeed = timeScale * nightSpeed ;
        StartCoroutine("UpdateAgentStatus");

    }
    IEnumerator UpdateAgentStatus()
    {
        while (hour >= 0)
        {
            yield return new WaitForSeconds(1f);
            UpdateWorldTime();
        }
        

    }


    void FixedUpdate()
    {
        transform.Rotate(rotSec * timeScale * Time.deltaTime, 0, 0);
        counter += Time.deltaTime * timeScale;
      //  Debug.Log("Counter " + counter);
        sec = counter % 60;
        min = (int)(counter / 60) % 60;
        hour = (int)(counter / 3600) % 24;
        day = (int)counter / 86400;

       
    }
    void OnGUI()
    {
        
        if (GUI.Button(new Rect(5, 25, 100, 22), "TOD: Morning"))
        {
            transform.rotation = Quaternion.Euler(75, 0, 0);
            ChangeTimeOfDay(18000);
        }
        if (GUI.Button(new Rect(5, 50, 100, 22), "TOD: Afternoon"))
        {

            transform.rotation = Quaternion.Euler(-180, 0, 0);
            ChangeTimeOfDay(43200);
        }
        if (GUI.Button(new Rect(5, 75, 100, 22), "TOD: Evening"))
        {

            transform.rotation = Quaternion.Euler(-105, 0, 0);
            ChangeTimeOfDay(61200);
        }
        if (GUI.Button(new Rect(5, 100, 100, 22), "TOD: Night"))
        {

            transform.rotation = Quaternion.Euler(-75, 0, 0);
            ChangeTimeOfDay(75600);
        }
    }

    void ChangeTimeOfDay(int time)
    {
        counter = time;
        Debug.Log("Time of Day changed");

        

    }

    void UpdateWorldTime()
    {
           
        
        if (Gworld.Instance.GetWorld().HasState(TimeOfDay.Morning.ToString(),true))
        {
            Gworld.Instance.GetWorld().RemoveState(TimeOfDay.Morning.ToString());
        }
        if (Gworld.Instance.GetWorld().HasState(TimeOfDay.Afternoon.ToString(), true))
        {
            Gworld.Instance.GetWorld().RemoveState(TimeOfDay.Afternoon.ToString());
        }
        if (Gworld.Instance.GetWorld().HasState(TimeOfDay.Evening.ToString(), true))
        {
            Gworld.Instance.GetWorld().RemoveState(TimeOfDay.Evening.ToString());
        }
        if (Gworld.Instance.GetWorld().HasState(TimeOfDay.Night.ToString(), true))
        {
            Gworld.Instance.GetWorld().RemoveState(TimeOfDay.Night.ToString());
        }

        if (hour >= 5 && hour < 12) //Morning
        {

            if (!Gworld.Instance.GetWorld().HasState(TimeOfDay.Morning.ToString(), true))
                Gworld.Instance.GetWorld().SetState(TimeOfDay.Morning.ToString(), true);
            timeScale = setTimeScale;
            wx.cTime = Weather.TimeOfDay.Morning;
            currentTOD = TimeOfDay.Morning;
        }

       else  if (hour >= 12 && hour < 17) //Afternoon
        {

            if (!Gworld.Instance.GetWorld().HasState(TimeOfDay.Afternoon.ToString(), true))
                Gworld.Instance.GetWorld().SetState(TimeOfDay.Afternoon.ToString(), true);
            timeScale = setTimeScale;
            wx.cTime = Weather.TimeOfDay.Afternoon;
            currentTOD = TimeOfDay.Afternoon;
        }

      else   if (hour >= 17 && hour < 21) // Evening
        {

            if (!Gworld.Instance.GetWorld().HasState(TimeOfDay.Evening.ToString(), true))
                Gworld.Instance.GetWorld().SetState(TimeOfDay.Evening.ToString(), true);
            timeScale = setTimeScale;
            wx.cTime = Weather.TimeOfDay.Evening;
            currentTOD = TimeOfDay.Evening;
        }

      else   if (hour >= 21 && hour <= 24 || hour >= 0 && hour < 5) // Night
        {
            if (!Gworld.Instance.GetWorld().HasState(TimeOfDay.Night.ToString(), true))
                Gworld.Instance.GetWorld().SetState(TimeOfDay.Night.ToString(), true);
              timeScale = nightSpeed;
            wx.cTime = Weather.TimeOfDay.Night;
            currentTOD = TimeOfDay.Night;
        }
        
    //   Gworld.Instance.GetWorld().SetState("Day", tr);
        
    }


}
