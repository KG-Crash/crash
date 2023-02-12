public class User
{ 
    public int sequence { get; private set; }
    public string id { get; private set; }

    public User(string id, int sequence)
    {
        this.sequence = sequence;
        this.id = id;
    }
}
