using UnityEngine;
using System.Collections;

public class Timer
{
    private static MonoBehaviour behaviour;

    public delegate void Task();

    private static MonoBehaviour MonoSupreme;

    public static Coroutine Schedule(MonoBehaviour _behaviour, float delay, Task task) //Sau "delay" giây thì thực hiện "task" này ở đối tượng "_behaviour" này
    {
        behaviour = _behaviour;
        if (_behaviour == null || _behaviour.gameObject.activeSelf == false) return null;
        return behaviour.StartCoroutine(DoTask(task, delay));
    }

    public static Coroutine SchedulePro(MonoBehaviour _behaviour, float delay, Task task) //Sau "delay" giây thì thực hiện "task" này ở đối tượng "_behaviour" này ko theo scale time
    {
        behaviour = _behaviour;
        if (_behaviour == null || _behaviour.gameObject.activeSelf == false) return null;
        return behaviour.StartCoroutine(DoTaskPro(task, delay));
    }

    public static Coroutine ScheduleSupreme(float delay, Task task)
    {
        if (MonoSupreme == null)
        {
            var tmp = new GameObject();
            MonoSupreme = tmp.AddComponent<TimerMonoRoot>();
            Object.DontDestroyOnLoad(tmp);
        }

        return MonoSupreme.StartCoroutine(DoTaskPro(task, delay));
    }

    public static Coroutine ScheduleFrame(Task task) //Sau "delay" giây thì thực hiện "task" này ở đối tượng "_behaviour" này ko theo scale time
    {
        if (MonoSupreme == null)
        {
            var tmp = new GameObject();
            MonoSupreme = tmp.AddComponent<TimerMonoRoot>();
            Object.DontDestroyOnLoad(tmp);
        }

        return MonoSupreme.StartCoroutine(DoTaskFrmae(task));
    }

    public static Coroutine ScheduleCondition(System.Func<bool> condition, Task task)
    {
        if (MonoSupreme == null)
        {
            var tmp = new GameObject();
            MonoSupreme = tmp.AddComponent<TimerMonoRoot>();
            Object.DontDestroyOnLoad(tmp);
        }

        return MonoSupreme.StartCoroutine(DoTAskCondition(condition, task));
    }

    public static void KillAllTimer()
    {
        if (MonoSupreme != null) MonoSupreme.StopAllCoroutines();
    }

    public static void StopTimerSupreme(Coroutine coroutine)
    {
        if (coroutine != null) MonoSupreme.StopCoroutine(coroutine);
    }

    public static Coroutine PlayCoroutine(IEnumerator enumerator)
    {
        return MonoSupreme.StartCoroutine(enumerator);
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return Yielders.Get(delay);
        task?.Invoke();
    }

    private static IEnumerator DoTaskPro(Task task, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        task?.Invoke();
    }

    private static IEnumerator DoTAskCondition(System.Func<bool> condition, Task task)
    {
        yield return new WaitUntil(condition);
        task?.Invoke();
    }


    private static IEnumerator DoTaskFrmae(Task task)
    {
        yield return null;
        //if(SceneManager.ins.async == null)
        task?.Invoke();
    }
}