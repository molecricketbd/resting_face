using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class knobDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [HideInInspector] public float knobValue;

    public Transform targetGraphic;

    [Header("params")]
    public float MaxValue = 1;
    public int Loops = 0;

    public bool SnapToPosition = false;
    public int SnapStepsPerLoop = 10;

    public KnobFloatValueEvent OnValueChanged;
    private float _currentLoops = 0;
    private float _previousValue = 0;
    private float _initAngle;
    private float _currentAngle;
    private Vector2 _currentVector;
    private Quaternion _initRotation;
    private bool _canDrag = false;

    public void OnPointerUp(PointerEventData eventData)
    {
        _canDrag = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canDrag = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _canDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _canDrag = true;
        
        _initRotation = targetGraphic.rotation;
        _currentVector = eventData.position - (Vector2) Camera.main.WorldToScreenPoint(targetGraphic.position);
        _initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!_canDrag) return;

        _currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(targetGraphic.position);
        _currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;

        Quaternion addRotation = Quaternion.AngleAxis(_currentAngle - _initAngle, targetGraphic.transform.forward);
        addRotation.eulerAngles = new Vector3(0, 0, addRotation.eulerAngles.z);

        Quaternion finalRotation = _initRotation * addRotation;

        knobValue = 1 - (finalRotation.eulerAngles.z / 360f);

        if(SnapToPosition)
        {
            SnapToPositionValue(ref knobValue);
            finalRotation.eulerAngles = new Vector3 (0, 0, 360 - 360 * knobValue);
        }

        UpdateKnobValue();

        targetGraphic.rotation = finalRotation;
        InvokeEvents(knobValue + _currentLoops);
        _previousValue = knobValue;

    }

    private void UpdateKnobValue()
    {
        //PREVENT OVERROTATION
            if (Mathf.Abs(knobValue - _previousValue) > 0.5f)
            {
                if (knobValue < 0.5f && Loops > 1 && _currentLoops < Loops - 1)
                {
                    _currentLoops++;
                }
                else if (knobValue > 0.5f && _currentLoops >= 1)
                {
                    _currentLoops--;
                }
                else
                {
                    if (knobValue > 0.5f && _currentLoops == 0)
                    {
                        knobValue = 0;
                        targetGraphic.localEulerAngles = Vector3.zero;
                        InvokeEvents(knobValue + _currentLoops);
                        return;
                    }
                    else if (knobValue < 0.5f && _currentLoops == Loops - 1)
                    {
                        knobValue = 1;
                        targetGraphic.localEulerAngles = Vector3.zero;
                        InvokeEvents(knobValue + _currentLoops);
                        return;
                    }
                }
            }

            //CHECK MAX VALUE
            if (MaxValue > 0)
            {
                if (knobValue + _currentLoops > MaxValue)
                {
                    knobValue = MaxValue;
                    float maxAngle = 360f - 360f * MaxValue;
                    targetGraphic.localEulerAngles = new Vector3(0, 0, maxAngle);
                    InvokeEvents(knobValue);
                    return;
                }
            }
    }

    public void SetKnobValue(float value, int loops = 0)
    {
        Quaternion newRoation = Quaternion.identity;
            knobValue = value;
            _currentLoops = loops;

            if (SnapToPosition)
            {
                SnapToPositionValue(ref knobValue);
            }
            
            newRoation.eulerAngles = new Vector3(0, 0, 360 - 360 * knobValue);

            UpdateKnobValue();

            targetGraphic.rotation = newRoation;
            InvokeEvents(knobValue + _currentLoops);

            _previousValue = knobValue;
    }

    private void SnapToPositionValue(ref float knobValue)
    {
        float snapStep = 1 / (float) SnapStepsPerLoop;
        float newValue = Mathf.Round(knobValue / snapStep) * snapStep;
        knobValue = newValue;
    }

    private void InvokeEvents (float value)
    {
        OnValueChanged.Invoke(value);
    }
}

[System.Serializable]
public class KnobFloatValueEvent : UnityEvent<float> { }