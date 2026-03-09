using echo17.EnhancedUI.EnhancedGrid;
using KumonProjectManager;
using TMPro;
using UnityEngine;

public class ChatCell : BasicGridCell
{
    public RectTransform backgroundRectTransform;
    public string cellIdentifier;
        
    private TMP_Text[] ChatLabels => _chatLabels ??= GetComponentsInChildren<TMP_Text>();
    private TMP_Text[] _chatLabels;

    public void UpdateCell(Chat data, float cellTemplateWidthWithPadding)
    {
        UpdateCellText(data.text);

        // set the rect transform's size based on the calculated size of the cell stored in the data
        backgroundRectTransform.sizeDelta = new Vector2(cellTemplateWidthWithPadding, data.cellHeight);
    }

    public void UpdateCellText(string text)
    {
        ChatLabels.ForEach(chatLabel => chatLabel.text = text);
    }

    public override string GetCellTypeIdentifier()
    {
        return cellIdentifier;
    }
}