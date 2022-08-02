using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropDownSetting : MonoBehaviour
{
    private Dropdown drop;
    private bool open = true;
    private void Awake()
    {
        drop = GetComponent<Dropdown>();
    }
    private void ControllerDropDown()
    {
        drop.options.Clear();
        
    }
    /*
    private void OnMouseDown()
    {
        drop.transform.GetChild(2).gameObject.SetActive(open);
    }
    */
    public void ShowDropDown()
    {
        drop.transform.GetChild(2).gameObject.SetActive(open);
    }
}
