using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public static class ExtensionMethods
{
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            var r = Random.Range(i, array.Length);
            var aux = array[i];
            array[i] = array[r];
            array[r] = aux;
        }
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        var retVal = new List<T>(list);
        for (int i = 0; i < list.Count - 1; i++)
        {
            var r = Random.Range(i, list.Count);
            (retVal[i], retVal[r]) = (retVal[r], retVal[i]);
        }

        return retVal;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        return list is { Count: 0 } ? default : list[Random.Range(0, list.Count)];
    }

    public static T GetRandom<T>(this T[] array)
    {
        return array is { Length: 0 } ? default : array[Random.Range(0, array.Length)];
    }
    
    public static T GetRandomAndRemove<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("GetRandomAndRemove: List is null or empty");
            return default;
        }
        
        var index = Random.Range(0, list.Count);
        var retVal = list[index];
        list.RemoveAt(index);
        return retVal;
    }

    public static bool DivisionIsInteger(double numerator, double denominator)
    {
        return (decimal)numerator / (decimal)denominator % 1 == 0;
    }
    
    public static void SetupDropdown(this Dropdown dropdown, Array values, UnityAction<int> callback)
    {
        dropdown.ClearOptions();
        var options = new List<Dropdown.OptionData>();
        foreach (var value in values)
        {
            options.Add(new Dropdown.OptionData(value.ToString()));
        }

        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(callback);
    }

    // public static void SetupDropdown(this TMPro.TMP_Dropdown dropdown, Array values, UnityAction<int> callback)
    // {
    //     dropdown.ClearOptions();
    //     var options = new List<TMPro.TMP_Dropdown.OptionData>();
    //     foreach (var value in values)
    //     {
    //         options.Add(new TMPro.TMP_Dropdown.OptionData(value.ToString()));
    //     }
    //
    //     dropdown.AddOptions(options);
    //     dropdown.onValueChanged.AddListener(callback);
    // }

    public static void EncodeVector3Binary(this BinaryWriter bw, Vector3 vector3)
    {
        bw.Write(vector3.x);
        bw.Write(vector3.y);
        bw.Write(vector3.z);
    }

    public static Vector3 DecodeVector3Binary(this BinaryReader br)
    {
        var v = new Vector3
        {
            x = br.ReadSingle(),
            y = br.ReadSingle(),
            z = br.ReadSingle()
        };
        return v;
    }
    
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    
    public static string GetAnimatorPath(this Transform transform)
    {
        // Use a list to collect path segments, then reverse for better performance
        var pathSegments = new List<string>();
        var target = transform;

        // Find the Animator
        var animator = transform.GetComponentInParent<Animator>();
        if (animator == null)
        {
            // No animator found, return just the transform name
            return transform.name;
        }
        
        while (target != null)
        {
            pathSegments.Add(target.name);

            // Check if parent has Animator before moving up
            if (target.parent == null || target.parent == animator.transform)
            {
                break;
            }

            target = target.parent;
        }

        // Reverse and join with StringBuilder for optimal string concatenation
        var sb = new System.Text.StringBuilder();
        for (var i = pathSegments.Count - 1; i >= 0 ; i--)
        {
            if (i != pathSegments.Count - 1) sb.Append('/');
            sb.Append(pathSegments[i]);
        }

        return sb.ToString();
    }
    
    public static Coroutine InvokeAfterDelay(this MonoBehaviour mono, float delay, Action callback)
    {
        if (mono == null)
        {
            Debug.LogError("MonoBehaviour is null. Cannot start coroutine.");
            return null;
        }

        if (callback == null)
        {
            Debug.LogError("Callback is null. No action will be performed after the delay.");
            return null;
        }

        var routine = mono.StartCoroutine(InvokeAfterDelayCoroutine(delay, callback));
        return routine;
    }

    private static IEnumerator InvokeAfterDelayCoroutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    public static Vector3 RandomPointInside(this Bounds bounds)
    {
        var x = Random.Range(bounds.min.x, bounds.max.x);
        var y = Random.Range(bounds.min.y, bounds.max.y);
        var z = Random.Range(bounds.min.z, bounds.max.z);
        //Bounds min and max already take into account the center of the bounds
        return new Vector3(x, y, z);
    }
}