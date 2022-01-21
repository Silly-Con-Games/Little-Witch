using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HighlightController : MonoBehaviour
{
    [Tooltip("Persistent objects, like next button, grayBackground")]
    public List<GameObject> persistentActivate;
    [Tooltip("Persistent objects, like the controls header")]
    public List<GameObject> persistentHighlight;

    [Tooltip("Groups of objects that should activate one by one")]
    public List<ListTouple> list;

    [Serializable]
    public class ListTouple
    {
        [Tooltip("things like texts, arrows and additional content")]
        public List<GameObject> toActivate;

        [Tooltip("things that are normal part of ui and should be highlighted")]
        public List<GameObject> toHighlight;

        [Tooltip("Something you want to happen on the next click")]
        public UnityEvent actions;
    }

    int counter = 0;

    public bool ShouldStartTutorial => PlayerPrefs.GetInt("sawTutorial", 0) == 0;

    public void StartTutorial()
    {
        if (ShouldStartTutorial)
        {
            counter = 0;
            ActivateList(persistentActivate);
            HighlightList(persistentHighlight);
            PlayerPrefs.SetInt("sawTutorial", 1);
            Next();
        }
    }

    // skip
    public void EndTutorial()
    {
        DeactivateList(persistentActivate);
        HighlightOffList(persistentHighlight);
        DeactivateAndHighlightOffTouple(list[counter - 1]);
    }

    // button
    public void Next()
    {

        if(counter > 0)
        {
            list[counter - 1].actions.Invoke();
            DeactivateAndHighlightOffTouple(list[counter - 1]);
        }
        if (counter < list.Count)
        {
            ActivateAndHighlightTouple(list[counter]);
        }
        else
            EndTutorial();
        counter++;
    }

    void ActivateAndHighlightTouple(ListTouple l)
    {
        ActivateList(l.toActivate);
        HighlightList(l.toHighlight);
    }

    void DeactivateAndHighlightOffTouple(ListTouple l)
    {
        DeactivateList(l.toActivate);
        HighlightOffList(l.toHighlight);
    }

    void ActivateList(IEnumerable<GameObject> enumerable)
    {
        foreach (var o in enumerable)
            o.SetActive(true);
    }

    void DeactivateList(IEnumerable<GameObject> enumerable)
    {
        foreach (var o in enumerable)
            o.SetActive(false);
    }

    void HighlightList(IEnumerable<GameObject> enumerable)
    {
        foreach(var o in enumerable)
            Highlight(o);
    }

    void HighlightOffList(IEnumerable<GameObject> enumerable)
    {
        foreach (var o in enumerable)
            HighlightOff(o);
    }

    void Highlight(GameObject o)
    {
        var canvas = o.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 2;
        o.AddComponent<GraphicRaycaster>();
    }

    void HighlightOff(GameObject o)
    {
        GraphicRaycaster raycaster = o.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            Destroy(raycaster);
        }
        Canvas canvas = o.GetComponent<Canvas>();
        if(canvas != null)
        {
            Destroy(canvas);
        }
    }
}
