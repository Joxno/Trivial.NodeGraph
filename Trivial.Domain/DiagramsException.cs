﻿using System;

namespace Trivial.Graph.Domain;

public class DiagramsException : Exception
{
    public DiagramsException(string? Message) : base(Message)
    {
    }

    public DiagramsException() : base()
    {
    }

    public DiagramsException(string? Message, Exception? InnerException) : base(Message, InnerException)
    {
    }
}
