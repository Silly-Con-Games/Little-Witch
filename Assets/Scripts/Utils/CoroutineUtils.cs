using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class CoroutineUtils
{
    public static IEnumerator CallWithDelay(UnityAction action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public static IEnumerator CallWithDelay<T>(UnityAction<T> action, float delay, T input)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke(input);
    }
}
