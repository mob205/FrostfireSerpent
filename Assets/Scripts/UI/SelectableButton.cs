using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableButton : MonoBehaviour, IPointerEnterHandler
{
    private MainMenu _menu;
    private void Awake()
    {
        _menu = GetComponentInParent<MainMenu>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _menu.SetSelector(transform.position.y);
    }
}
