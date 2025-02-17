using System;

namespace Trivial.Graph.Domain.Options;

public class DiagramZoomOptions
{
    private float m_Minimum = 0.1f;
    private float m_ScaleFactor = 1.05f;

    public bool Enabled { get; set; } = true;
    public bool Inverse { get; set; }
    public float Minimum
    {
        get => m_Minimum;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"Minimum can't be less than zero");
            
            m_Minimum = value;
        }
    }
    public float Maximum { get; set; } = 2;
    public float ScaleFactor
    {
        get => m_ScaleFactor;
        set
        {
            if (value is < 1.01f or > 2)
                throw new ArgumentException($"ScaleFactor can't be lower than 1.01 or greater than 2");
                    
            m_ScaleFactor = value;
        }
    }
}