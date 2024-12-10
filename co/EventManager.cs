using System.Collections;

public class RecordChange
{
    public RecordChange(int? parentEventId, int id, string body = "", string recordType = "")
    {
        Id = id;
        Body = body;
        RecordType = recordType;
        ParentEventId = parentEventId;
    }
    public int Id { get; set; }
    public string Body { get; set; }
    public string RecordType { get; set; }
    public int? ParentEventId { get; set; }
}

public class EventManager
{
    public EventManager()
    {
        Events = new Queue<RecordChange>();
    }
    public Queue<RecordChange> Events { get; }
}

public static class StaticSequence
{
    private static int seq = 1;
    public static int next()
    {
        return seq++;
    }
}