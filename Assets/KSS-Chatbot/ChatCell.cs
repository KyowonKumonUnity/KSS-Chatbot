using echo17.EnhancedUI.EnhancedGrid;
using LCHFramework.Extensions;
using TMPro;
using UnityEngine;

public class ChatCell : BasicGridCell
{
    public RectTransform backgroundRectTransform;
    public string cellIdentifier;
        
    public TMP_Text[] ChatLabels => _chatLabels.IsEmpty() ? _chatLabels = GetComponentsInChildren<TMP_Text>() : _chatLabels;
    private TMP_Text[] _chatLabels;

    public void UpdateCell(Chat data, float cellTemplateWidthWithPadding)
    {
        ChatLabels.ForEach(chatLabel => chatLabel.text = data.text);

        // set the rect transform's size based on the calculated size of the cell stored in the data
        backgroundRectTransform.sizeDelta = new Vector2(cellTemplateWidthWithPadding, data.cellHeight);
    }

    public override string GetCellTypeIdentifier()
    {
        return cellIdentifier;
    }
}