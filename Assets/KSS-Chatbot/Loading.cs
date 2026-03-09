using System;
using KumonProjectManager;
using TMPro;
using UniRx;
using UnityEngine;

public class Loading : MonoBehaviour
{
    private GameObject wrapper;
    private TMP_Text text;
    private IDisposable _disposable;
    
    
    
    private void Awake()
    {
        wrapper = transform.GetChild(0).gameObject;
        text = GetComponentInChildren<TMP_Text>(true);
    }
    
    
    
    [ContextMenu(nameof(Show))]
    public void Show()
    {
        wrapper.SetActive(true);
        text.text = ".";
        _disposable = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(t => text.text = new string('.', (int)((t + 1) % 3 + 1)))
            .AddTo(this);
    }
    
    [ContextMenu(nameof(Hide))]
    public void Hide()
    {
        wrapper.SetActive(false);
        IDisposableUtility.DisposeAndSetNull(ref _disposable);
    }
}
