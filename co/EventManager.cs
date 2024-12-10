using System.Collections;

public class RecordChange
{
    public RecordChange(int? parentEventId, int eventId, Record changedRecord, string recordType = "")
    {
        EventId = eventId;
        ChangedRecord = changedRecord;
        RecordType = recordType;
        ParentEventId = parentEventId;
    }
    public int EventId { get; set; }
    public Record ChangedRecord { get; set; }
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


public static class RecordSequence
{
    private static int seq = 1;
    public static int next()
    {
        return seq++;
    }
}

public static class EventSequence
{
    private static int seq = 1000;
    public static int next()
    {
        return seq++;
    }
}