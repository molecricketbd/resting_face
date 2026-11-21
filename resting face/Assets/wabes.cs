using UnityEngine;

//period support
//starting pos support
//render everything below wave?

public class wabes : MonoBehaviour
{
    private LineRenderer liner;
    public enum WaveType { sine, tri, square };

    public int points;
    public float lineLength = 1f;
    public float period = 1f;
    
    [Header("Osc 1")]
    public float amplitudeOne = 1;
    public WaveType waveOne = WaveType.sine;

    [Header("Osc 2")]
    public float amplitudeTwo = 0;
    public WaveType waveTwo = WaveType.sine;

    void Start()
    {
        liner = GetComponent<LineRenderer>();
    }  

    void Update()
    {
        Draw();
    } 

    void Draw()
    {

        liner.positionCount = points;
        for(int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            float progress = (float) currentPoint/(points - 1);
            float x = Mathf.Lerp(0, lineLength, progress);
            float y = amplitudeOne * WaveValue(waveOne, x) + amplitudeTwo * WaveValue(waveTwo, x);
            
            Mathf.Sin(x + Time.timeSinceLevelLoad);
            liner.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }

    float WaveValue(WaveType typo, float x)
    {
        float y = 0;
        float shiftX = x + Time.timeSinceLevelLoad;

        if(typo == WaveType.sine)
        {
            y = Mathf.Sin(shiftX / (1 / (2 * Mathf.PI))) / 2 + 0.5f;
        }
        else if(typo == WaveType.square)
        {
            y = (shiftX % 1) < 0.5 ? 1 : 0;
        }
        else if(typo == WaveType.tri)
        {
            y = 4 * Mathf.Abs(((shiftX - (1/4)) % 1) - 0.5f) - 1;
        }

        return y;
    }
}
