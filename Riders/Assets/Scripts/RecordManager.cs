using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecordManager
{
    private List<string> dirtRecord = new List<string>();
    public List<string> DirtRecord { get { return dirtRecord; } set { dirtRecord = value; } }
    private int recordCount = 0;
    public int RecordCount { get { return recordCount; } set { recordCount = value; } }

    private TextMeshProUGUI recordText;
    public void ShowRecord()
    {
        recordText = GameObject.Find("DirtRecordText").GetComponent<TextMeshProUGUI>();
        recordText.text = "";
        if(dirtRecord.Count == 0)
        {
            recordText.text = "NO RECORD";
        }
        else
        {
            recordText.text = "";
            for (int i = 0; i < dirtRecord.Count; i++)
            {
                recordText.text += (dirtRecord[i] + "\n");
            }
        }
    }
}
