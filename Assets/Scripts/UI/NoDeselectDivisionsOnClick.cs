using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoDeselectDivisionsOnClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool BlockDeselect;
    public void OnPointerEnter(PointerEventData eventData)
    {
        //GetComponent<Image>().color = Color.red;
        BlockDeselect = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BlockDeselect = false;
        //GetComponent<Image>().color = Color.white;
    }
}
