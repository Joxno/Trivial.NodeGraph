using System;

namespace Trivial.Domain.Options;

public class DiagramZoomOptions
{
    private float _minimum = 0.1f;
    private float _scaleFactor = 1.05f;

    public bool Enabled { get; set; } = true;
    public bool Inverse { get; set; }
    public float Minimum
    {
        get => _minimum;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"Minimum can't be less than zero");
            
            _minimum = value;
        }
    }
    public float Maximum { get; set; } = 2;
    public float ScaleFactor
    {
        get => _scaleFactor;
        set
        {
            if (value is < 1.01f or > 2)
                throw new ArgumentException($"ScaleFactor can't be lower than 1.01 or greater than 2");
                    
            _scaleFactor = value;
        }
    }
}