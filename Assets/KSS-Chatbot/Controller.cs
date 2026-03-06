using System;
using System.Collections.Generic;
using echo17.EnhancedUI.EnhancedGrid;
using LCHFramework.Extensions;
using Newtonsoft.Json;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

/// <summary>
/// Example showing a chat between two devices, each with their own grid.
/// The cells' sizes are calculated based on the text contents using Unity's
/// content size fitter component.
///
/// Note: check the inspector for the EnhancedGrid components, noting they
/// both have their content minimum size set to a value that pushes the content
/// down to the bottom. They also have their flow set to BottomToTopLeftToRight
/// </summary>
public class Controller : MonoBehaviour, IEnhancedGridDelegate
{
    private static readonly Uri AIChatUri = new("https://n8n.smartkumon.co.kr/webhook/create");
    
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void InitCtrlEnterListener();
#endif
    
    
    
    public Loading loading;
    public CanvasScaler canvasScaler;
    public EnhancedGrid person1Grid;
    public GameObject chatFromMeCellPrefab;
    public GameObject chatFromOtherPersonCellPrefab;
    public RectOffset chatCellPadding;
    
    /// <summary>
	/// Template text label to calculate the text size based on Unity's
	/// content size fitter component
	/// </summary>
    public TMP_Text chatTemplateText;
    
    public TMP_InputField person1ChatInputField;
    
    private List<Chat> _chats;
    private RectTransform _chatTemplateRectTransform;
    private RectTransform _person1GridRt;
    private bool _isApplicationFocused = true;

    void Awake()
    {
        Application.targetFrameRate = 60;

        _chatTemplateRectTransform = chatTemplateText.GetComponent<RectTransform>();
        _person1GridRt = person1Grid.GetComponent<RectTransform>();
        _isApplicationFocused = Application.isFocused;
    }

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        InitCtrlEnterListener();
#endif
        person1Grid.InitializeGrid(this);

        _chats = new List<Chat>();

        // recalculate the grids, setting their scroll position to the bottom.
        // Note: the y position in EnhancedGrid is reversed from Unity's UI y values,
        // where EnhancedGrid uses 0 for the top and 1 for the bottom.

        _person1GridRt.ObserveEveryValueChanged(t => t.rect.size)
            .Subscribe(_ => person1Grid.RecalculateGrid(scrollNormalizedPositionY: person1Grid.ScrollNormalizedPosition.y))
            .AddTo(this);

        person1ChatInputField.ActivateInputField();
    }
    
#if UNITY_EDITOR
    void Update()
    {
        // if the enter key is pressed, send the chat

        if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            PersonSendButton_OnClick();
        }
    }
#endif
    
    private void OnApplicationFocus(bool focus) => _isApplicationFocused = focus;
    
    public int GetCellCount(EnhancedGrid grid)
    {
        return _chats.Count;
    }

    /// <summary>
	/// Set different cell prefabs based on whether the person using the device sent the chat
	/// </summary>
	/// <param name="grid">There are two grids in this example, so this will be the requesting grid</param>
	/// <param name="dataIndex">Data index of the cell</param>
	/// <returns></returns>
    public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
    {
        // check which grid sent the request and return the appropriate cell prefab

        if (grid == person1Grid ? _chats[dataIndex].fromPersonID == 1 : _chats[dataIndex].fromPersonID == 2)
        {
            return chatFromMeCellPrefab;
        }
        else
        {
            return chatFromOtherPersonCellPrefab;
        }
    }

    public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
    {
        Canvas.ForceUpdateCanvases();
        return new CellProperties()
        {
            minSize = new Vector2(_person1GridRt.rect.size.x - 20, _chats[dataIndex].cellHeight),
            expansionWeight = 0f
        };
    }

    public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
    {
        (cell as ChatCell).UpdateCell(_chats[dataIndex], _chats[dataIndex].cellWidth);
    }

    /// <summary>
	/// Send the chat
	/// </summary>
	/// <param name="personID">Which device is sending</param>
    public void PersonSendButton_OnClick()
    {
        var chatInputField = person1ChatInputField;
        if (string.IsNullOrWhiteSpace(chatInputField.text)) return;
        if (isReceivingAIChat) return;

        var chatInputFieldText = chatInputField.text.Trim(); 
        _AddChat(1, chatInputFieldText);

        chatInputField.text = "";

        // recalculate the grids, scrolling to the bottom
        
        person1Grid.RecalculateGrid(scrollNormalizedPositionY: 1);
        
        ReceiveAIChat(chatInputFieldText).Forget();
    }

    private bool isReceivingAIChat;
    private async Awaitable ReceiveAIChat(string chatInput)
    {
        isReceivingAIChat = true;
        loading.Show();
        
        
        var body = JsonConvert.SerializeObject(new RequestAIChat
        {
            messages = new RequestAIChatMessage[]
            {
                new() { role = "user", content = chatInput}
            }
        });
        using var request = UnityWebRequest.Post(AIChatUri, body, "application/json");
        request.certificateHandler = new BypassCertificate();
        await request.SendWebRequest();
        var respond = request.result != UnityWebRequest.Result.Success 
            ? $"Request {request.result}: {request.error}"
            : JsonConvert.DeserializeObject<ResultAIChat>(request.downloadHandler.text)?.output;
        
        
        loading.Hide();
        _AddChat(2, respond ?? string.Empty);
        person1Grid.RecalculateGrid(scrollNormalizedPositionY: person1Grid.ScrollNormalizedPosition.y);
        isReceivingAIChat = false;
    }
    
    private class BypassCertificate : CertificateHandler { protected override bool ValidateCertificate(byte[] certificateData) => true; }

    private void _AddChat(string text) => _AddChat(1, text);
    
    private void _AddChat(int personID, string text)
    {
        _chats.Insert(0, _GetChat(personID, text));
    }

    private Chat _GetChat(int personID, string text)
    {
        // turn on the template text UI element and set its text
        chatTemplateText.gameObject.SetActive(true);
        chatTemplateText.text = text;
        chatTemplateText.ForceMeshUpdate();

        // recalculate the size of the text template object
        Canvas.ForceUpdateCanvases();

        // insert the chat into the data at the beginning (since the flow is BottomToTopLeftToRight)
        // the cell's height is based on the calculated value from the template label object
        var textWidth = chatTemplateText.preferredWidth + chatCellPadding.horizontal;
        var rectWidth = _chatTemplateRectTransform.sizeDelta.x + chatCellPadding.horizontal;
        var result = new Chat
        {
            fromPersonID = personID,
            date = DateTime.Now,
            text = text,
            cellWidth = /*textWidth < 180 ? 180 :*/ textWidth < rectWidth ? textWidth : rectWidth,
            cellHeight = _chatTemplateRectTransform.sizeDelta.y + chatCellPadding.top + chatCellPadding.bottom
        };

        // hide the template label object now that we are done with it
        chatTemplateText.gameObject.SetActive(false);

        return result;
    }
}