using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using DG.Tweening;
abstract public class PopupTemplate : ObserverEventManager
{
    [SerializeField] bool isStartup;
    [SerializeField] GameObject root;
    [SerializeField] RectTransform panel;
    [SerializeField] PopupIDs id;
    [SerializeField] protected bool isOverlay = false;
    private UnityEngine.Events.UnityAction _callback;
    public abstract void UpdateInformation(object _data);
    public PopupIDs ID { get { return id; } }
    public bool IsActive => root.activeSelf;
    public bool IsOverlay => isOverlay;
    [SerializeField] protected PopupAnimationType animationType = PopupAnimationType.ScaleFromCenter;
    [SerializeField] protected float animationDuration = 0.3f;
    private Vector3 startPosition;

    public virtual void Start()
    {

        if (_callback != null)
            notify(EventKeys.GameController.ADD_LOADER.ToString(), _callback);
        // ChangeState(isStartup);
        root.gameObject.SetActive(isStartup);
    }

    public void ChangeState(bool status)
    {
        if (startPosition == Vector3.zero)
            startPosition = panel.position;

        if (status)
        {
            root.SetActive(true);

            Vector3 fromPos = startPosition;
            Vector3 fromScale = Vector3.one;

            switch (animationType)
            {
                case PopupAnimationType.SlideFromTop:
                    fromPos += Vector3.up * Screen.height;
                    break;
                case PopupAnimationType.SlideFromBottom:
                    fromPos += Vector3.down * Screen.height;
                    break;
                case PopupAnimationType.SlideFromLeft:
                    fromPos += Vector3.left * Screen.width;
                    break;
                case PopupAnimationType.SlideFromRight:
                    fromPos += Vector3.right * Screen.width;
                    break;
                case PopupAnimationType.ScaleFromCenter:
                    fromScale = Vector3.zero;
                    break;
            }

            if (animationType.ToString().Contains("Slide"))
            {
                panel.position = fromPos;
                panel.DOMove(startPosition, animationDuration)
                     .SetEase(Ease.OutCubic);
            }
            else if (animationType == PopupAnimationType.ScaleFromCenter)
            {
                panel.localScale = fromScale;
                panel.DOScale(Vector3.one, animationDuration)
                     .SetEase(Ease.OutCubic);
            }
        }
        else
        {
            Vector3 toPos = startPosition;

            switch (animationType)
            {
                case PopupAnimationType.SlideFromTop:
                    toPos += Vector3.up * Screen.height;
                    break;
                case PopupAnimationType.SlideFromBottom:
                    toPos += Vector3.down * Screen.height;
                    break;
                case PopupAnimationType.SlideFromLeft:
                    toPos += Vector3.left * Screen.width;
                    break;
                case PopupAnimationType.SlideFromRight:
                    toPos += Vector3.right * Screen.width;
                    break;
            }

            if (animationType.ToString().Contains("Slide"))
            {
                panel.position = startPosition;
                panel.DOMove(toPos, animationDuration)
                     .SetEase(Ease.InCubic)
                     .OnComplete(() => root.SetActive(false));
            }
            else if (animationType == PopupAnimationType.ScaleFromCenter)
            {
                panel.DOScale(Vector3.zero, animationDuration)
                     .SetEase(Ease.InCubic)
                     .OnComplete(() => root.SetActive(false));
            }
            else
            {
                root.SetActive(false);
            }
        }
    }
    public void ClosePopup()
    {
        if (isOverlay)
        {
            ChangeState(false);
            PopupManager.Instance.RemoveOverlay(this);
            return;
        }
        PopupManager.instance.ClosePopup();
        OnPopupClosed();
    }
    protected virtual void OnPopupClosed() { }
    protected void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
        DG.Tweening.DOTween.Kill(this);
    }
}
