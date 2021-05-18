namespace Protocol
{
    public interface IProtocol
    {
        public uint Identity { get; }

        public byte[] Serialize();
    }
}
