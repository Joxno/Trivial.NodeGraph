using System;

namespace Trivial.Graph;

public class BlazorDiagramsException : Exception
{
    public BlazorDiagramsException(string? message) : base(message)
    {
    }
}