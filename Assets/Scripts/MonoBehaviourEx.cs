using System;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

//------------------------------------------------------------------------------
public class MonoBehaviourEx : MonoBehaviour
{
    public void AddTimer(float time, Action callback)
    {
        AddTimer(time, callback, false, false);
    }

    public void AddTimer(float time, Action callback, bool cyclic)
    {
        AddTimer(time, callback, cyclic, false);
    }

    public void AddTimer(float time, Action callback, bool cyclic, bool ignoreTimeScale)
    {
        if (time != 0)
        {
            if (cyclic)
            {
                StartCoroutine(TimerCyclic(time, callback));
            }
            else if (ignoreTimeScale)
            {
                Debug.Log("ignoreTimeScale");
                StartCoroutine(TimerIgnoreTimescale(time, callback));
            }
            else
            {
                StartCoroutine(Timer(time, callback));
            }
        }
        else
        {
            try
            {
                callback();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }


    IEnumerator TimerCyclic(float time, Action callback)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            try
            {
                callback();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static IEnumerator WaitForRealSeconds(float delay)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay)
        {
            yield return null;
        }
    }

    private IEnumerator TimerIgnoreTimescale(float delay, Action callback)
    {
        yield return StartCoroutine(WaitForRealSeconds(delay));
        try
        {
            callback();
        }
        catch (Exception)
        {
            throw;
        }
    }

    IEnumerator Timer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);

        try
        {
            callback();
        }
        catch (Exception)
        {
            throw;
        }
    }
}