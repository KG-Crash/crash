namespace Protocol
{
    public interface IProtocol
    {
        uint Identity { get; }

        byte[] Serialize();
    }
}
