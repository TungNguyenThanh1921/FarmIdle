using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text content;
    public void InitData(string content)
    {
        this.content.text = content;
    }
}
