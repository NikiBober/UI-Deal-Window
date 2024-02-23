using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private float _maxScaleDistance = 1f;
    [SerializeField] private float _minScaleDistance = 100f;
    [SerializeField] [Range(0f, 1f)] private float _maxScale = 1f;
    [SerializeField] [Range(0f, 1f)] private float _minScale = 0.7f;
    [SerializeField] private float _centeringSensivity = 200f;
    [SerializeField] private float _centeringSpeed = 10f;

    [SerializeField] private RectTransform[] _elements;
    [SerializeField] private RectTransform[] _dots;
    [SerializeField] private RectTransform _selectedDot;


    private ScrollRect _scrollRect;
    private RectTransform _contentPanel;
    private HorizontalLayoutGroup _horizontalLayoutGroup;
    private float _centerPosition;
    private float _unitWidth;
    bool _isSnapped;

    private void Start()
    {
        _centerPosition = Screen.width / 2;
        _elements[0].position = Vector3.right * _centerPosition;

        _scrollRect = gameObject.GetComponent<ScrollRect>();
        _contentPanel = _scrollRect.content;
        _horizontalLayoutGroup = _contentPanel.GetComponent<HorizontalLayoutGroup>();
        _unitWidth = _elements[0].rect.width + _horizontalLayoutGroup.spacing;
    }
    private void Update()
    {
        float speed = _centeringSpeed * Time.deltaTime;
        int currentIndex = Mathf.RoundToInt(_contentPanel.localPosition.x / _unitWidth);

        // Scaling
        foreach (RectTransform element in _elements)
        {
            float distance = Mathf.Abs(element.position.x - _centerPosition);
            float scaleChange = Mathf.Clamp01((distance - _minScaleDistance) / (_maxScaleDistance - _minScaleDistance));
            float scale = Mathf.Lerp(_minScale, _maxScale, scaleChange);
            element.localScale = Vector3.one * scale;
        }

        // Centering
        if (_scrollRect.velocity.magnitude < _centeringSensivity && !_isSnapped)
        {
            float goalPosition = _unitWidth * currentIndex;
            float newPosition = Mathf.MoveTowards(_contentPanel.localPosition.x, goalPosition, speed);;
            _contentPanel.localPosition = new Vector3(newPosition, _contentPanel.localPosition.y);
            
            if (_contentPanel.localPosition.x == goalPosition)
            {
                _isSnapped = true;
            }
        }

        if (_scrollRect.velocity.magnitude > _centeringSensivity)
        {
            _isSnapped = false;
        }


        _selectedDot.SetParent(_dots[Mathf.Abs(currentIndex)], false);
    }
}
