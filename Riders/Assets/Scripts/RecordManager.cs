using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordManager
{
    private List<string> dirtRecord = new List<string>();
    public List<string> DirtRecord { get { return dirtRecord; } set { dirtRecord = value; } }
    private int recordCount = 0; // How Many Record Available?
    public int RecordCount { get { return recordCount; } set { recordCount = value; } }

    private TextMeshProUGUI recordText;
    public void ShowRecord() // Display Lap Time On Text Box
    {
        recordText = GameObject.Find("DirtRecordText").GetComponent<TextMeshProUGUI>();
        recordText.text = "";
        if(dirtRecord.Count == 0) // When Record Count Is Zero
        {
            recordText.text = "NO RECORD";
        }
        else // At Least One Record Available
        {
            recordText.text = "";
            for (int i = 0; i < dirtRecord.Count; i++)
            {
                recordText.text += ($"{dirtRecord[i]} \n"); // Print Record
            }
        }
    }
}
