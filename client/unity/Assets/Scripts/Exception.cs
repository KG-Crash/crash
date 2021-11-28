using System;
using FixMath.NET;
using Shared;

public class ClientException : Exception
{
    public ClientExceptionCode _code;

    protected ClientException(ClientExceptionCode code, string message) : base(message)
    {
        _code = code;
    }

    public override string ToString()
    {
        return $"{base.ToString()}({_code})";
    }
}

public class InvalidCellAccessException : ClientException
{
    public InvalidCellAccessException(string prefix, FixVector3 position) :
        base(ClientExceptionCode.InvalidCellAccess, $"{prefix}, _map[{position}] == null")
    {
    }
}

public class NotWalkableNextCellException : ClientException
{
    public NotWalkableNextCellException(KG.Map.Cell nextCell, int regionPathLength) :
        base(ClientExceptionCode.NotWalkableNextCell, $"next({nextCell}) cell of region is not walkable, {regionPathLength}")
    {
    }
}

public class ZeroCellPathException : ClientException
{
    public ZeroCellPathException() :
        base(ClientExceptionCode.ZeroCellPath, "_cellPath.Count == 0")
    {
    }
}

public class NotFoundUIAttributeException : ClientException
{
    public NotFoundUIAttributeException() :
        base(ClientExceptionCode.NotFoundUIAttribute, "Not found UI attribute.")
    {
    }
}

public class NotContainUIScript : ClientException
{
    public NotContainUIScript(string typeGOName, string typeName) :
        base(ClientExceptionCode.NotContainUIScript, $"{typeGOName} does not contains {typeName} script.")
    {
    }
}