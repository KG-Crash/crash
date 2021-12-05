using System;
using FixMath.NET;
using Shared;

public class ClientException : Exception
{
    public ClientExceptionCode _code;

    public ClientException(ClientExceptionCode code, string message) : base(message)
    {
        _code = code;
    }

    public override string ToString()
    {
        return $"{base.ToString()}({_code})";
    }
}