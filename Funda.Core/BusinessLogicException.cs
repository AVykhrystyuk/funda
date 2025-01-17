﻿namespace Funda.Core;

public class BusinessLogicException : Exception
{
    public BusinessLogicException()
    {
    }

    public BusinessLogicException(string message)
        : base(message)
    {
    }

    public BusinessLogicException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public ErrorCode Code { get; init; }
}
