﻿using System;

namespace Trivial.Domain;

public class DiagramsException : Exception
{
    public DiagramsException(string? message) : base(message)
    {
    }

    public DiagramsException() : base()
    {
    }

    public DiagramsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
