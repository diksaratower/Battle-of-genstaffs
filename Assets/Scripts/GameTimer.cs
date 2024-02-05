using System;
using System.Collections;
using UnityEngine;


public class GameTimer : MonoBehaviour, ISaveble
{
    public DateTime DTime;
    [field: SerializeField] public int Speed { get; private set; }
    public bool Pause;
    public static Action HourEnd;
    public static Action DayEnd;
    public static GameTimer Instance;
    public float TimeScale { get; set; } = 0.3f;

    private int Days;

    public void StartTimer()
    {
        Instance = this;
        StartCoroutine(TimerWork());
        Days = DTime.Day;
        HourEnd += CalculateDays;
    }

    private IEnumerator TimerWork()
    {
        while (true)
        {
            if (Pause) 
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(TimeScale / Speed);

            DTime = DTime.AddHours(1);

            foreach (var deleg in HourEnd?.GetInvocationList()) 
            {
                try
                {
                    ((Action)deleg).Invoke();
                }
                catch (Exception e) 
                {
                    Debug.LogException(e);
                    //Debug.LogError("Error " + DTime.ToLongDateString() + e.ToString());
                }
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        HourEnd = null;
        DayEnd = null;
    }

    private void CalculateDays()
    {
        if (Days != DTime.Day)
        {
            DayEnd?.Invoke();
            Days = DTime.Day;
        }
    }

    public void SetSpeed(int speed)
    {
        if (speed <= 0 || speed > 6)
        {
            throw new ArgumentOutOfRangeException();
        }
        Speed = speed;
    }

    string ISaveble.GetFileName()
    {
        return "timer";
    }

    string ISaveble.Save()
    {
        return JsonUtility.ToJson(new TimerSerialize(this));
    }

    void ISaveble.Load(string data)
    {
        var ser = JsonUtility.FromJson<TimerSerialize>(data);
        Speed = ser.Speed;
        DateTime dt = ser.Time;
        DTime = dt;
    }

    Type ISaveble.GetSaveType()
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public class TimerSerialize
    {
        public int Speed;
        public JsonDateTime Time;

        public TimerSerialize(GameTimer timer)
        {
            Speed = timer.Speed;
            Time = timer.DTime;
        }
    }

    [Serializable]
    public struct JsonDateTime
    {
        public long value;
        public static implicit operator DateTime(JsonDateTime jdt)
        {
            //Debug.Log("Converted to time");
            return DateTime.FromFileTimeUtc(jdt.value);
        }
        public static implicit operator JsonDateTime(DateTime dt)
        {
            //Debug.Log("Converted to JDT");
            JsonDateTime jdt = new JsonDateTime();
            jdt.value = dt.ToFileTimeUtc();
            return jdt;
        }
    }
}
