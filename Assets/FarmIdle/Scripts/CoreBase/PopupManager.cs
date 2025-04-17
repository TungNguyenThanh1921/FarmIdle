using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using System;
using UnityEngine.Events;
public class PopupManager : ObserverEventManager
{
    public static PopupManager instance;
    public static PopupManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType(typeof(PopupManager)) as PopupManager;

            return instance;
        }
    }

    public List<PopupTemplate> popups;

    private List<PopupTemplate> createdPopups;

    public List<PopupTemplate> queues;

    public PopupTemplate currentPopup;
    public List<PopupTemplate> overlayPopups = new List<PopupTemplate>();

    public object tempObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        createdPopups = new List<PopupTemplate>();
        queues = new List<PopupTemplate>();
        tempObject = null;
        currentPopup = null;

        StartCoroutine(Create());

        attach(EventKeys.UI.OPEN_POPUP, (Action<PopupIDs, object>)OpenPopup);
        attach(EventKeys.UI.CLOSE_POPUP, (Action)ClosePopup);

    }

    IEnumerator Create()
    {
        for (int i = 0; i < popups.Count; i++)
        {
            PopupTemplate temp = Instantiate(popups[i], transform);
            createdPopups.Add(temp);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ClosePopup()
    {
        if (queues.Count == 0) return;

        PopupTemplate closedPopup = currentPopup;
        Hide();

        queues.Remove(closedPopup);

        if (queues.Count > 0)
        {
            currentPopup = queues[queues.Count - 1];

            if (closedPopup.IsOverlay)
            {
                if (!currentPopup.IsActive)
                {
                    currentPopup.ChangeState(true);
                }
            }
            else
            {
                Display();
            }
        }
        else
        {
            currentPopup = null;
        }

    }

    public void OpenPopup(PopupIDs id, object data = null)
    {
        var popup = createdPopups.Find(x => x.ID == id);
        if (popup == null)
        {
            Debug.LogWarning($"Không tìm thấy popup: {id}");
            return;
        }
        Hide();
        currentPopup = popup;
        queues.Add(popup);
        Display(data);
    }
    public void OpenMultiplePopup(PopupIDs id, object data = null)
    {
        var popup = createdPopups.Find(x => x.ID == id);
        PopupTemplate overlay = Instantiate(popups.Find(x => x.ID == id), transform);
        overlayPopups.Add(overlay);
        StartCoroutine(DisplayMultiplePopup(overlay, data));
    }
    private IEnumerator DisplayMultiplePopup(PopupTemplate overlay, object data = null)
    {
        yield return new WaitForEndOfFrame();
        overlay.UpdateInformation(data);
        overlay.ChangeState(true);
    }
    public void RemoveOverlay(PopupTemplate popup)
    {
        if (overlayPopups.Contains(popup))
        {
            overlayPopups.Remove(popup);
            Destroy(popup.gameObject);
        }
    }
    public void Display(object data = null)
    {
        if (!ReferenceEquals(currentPopup, null))
        {
            currentPopup.UpdateInformation(data);
            currentPopup.ChangeState(true);
            tempObject = null;
        }
    }

    public void Hide()
    {
        if (!ReferenceEquals(currentPopup, null))
        {
            currentPopup.ChangeState(false);
        }
    }
}
public enum PopupIDs
{
    SelectFarmSlotRole,
    Inventory,
    Shop,
    Equipment,
}
public enum PopupAnimationType
{
    None,
    SlideFromTop,
    SlideFromBottom,
    SlideFromLeft,
    SlideFromRight,
    ScaleFromCenter
}