using TMPro;
using UnityEngine;

public class SegmentCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        var manager = FindObjectOfType<SegmentManager>();
        if(manager)
        {
            manager.OnSegmentChange.AddListener(OnSegmentChange);
            _text.text = manager.PlayerLength.ToString();
        }
    }

    private void OnSegmentChange(int numSegments)
    {
        _text.text = numSegments.ToString();
    }
}
