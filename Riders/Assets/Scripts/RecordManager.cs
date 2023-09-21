using System.Text;
using TMPro;
using UnityEngine;

public class RecordManager
{
    private TextMeshProUGUI recordText;
    public void ShowRecord() // Display Lap Time On Text Box
    {
        recordText = GameObject.Find("DirtRecordText").GetComponent<TextMeshProUGUI>();
        recordText.text = "";
        if(GetRecordCount() == 0) // When Record Count Is Zero
        {
            recordText.text = "NO RECORD";
        }
        else // At Least One Record Available
        {
            recordText.text = "";
			if (PlayerPrefs.HasKey(RecordDataKey) == true)
			{
                builder.Clear();
				builder.Append(PlayerPrefs.GetString(RecordDataKey));
				string[] records = builder.ToString().Split(Devider);
				for (int i = 0; i < records.Length; i++)
				{
					recordText.text += ($"{records[i]} \n"); // Print Record
				}
			}
        }
    }
    public const string RecordDataKey = "Records";
    private const char Devider = '$';
    private StringBuilder builder = new StringBuilder();
    public void AddRecord(string record)
    {
        builder.Clear();
        if (PlayerPrefs.HasKey(RecordDataKey) == true)
        {
            builder.Append(PlayerPrefs.GetString(RecordDataKey));
        }
        builder.Append(GetRecordCount() + 1);
        builder.Append(" / ");
        builder.Append(record);
        builder.Append(Devider);
        PlayerPrefs.SetString(RecordDataKey, builder.ToString());
    }
    public int GetRecordCount()
    {
		builder.Clear();
        if (PlayerPrefs.HasKey(RecordDataKey) == true)
        {
            builder.Append(PlayerPrefs.GetString(RecordDataKey));
            return builder.ToString().Split(Devider).Length;
        }
        else
        {
            return 0;
        }
	}
}
