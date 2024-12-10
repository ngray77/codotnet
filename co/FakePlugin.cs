using System.Diagnostics;
/// <summary>
/// Plugins consume configured upstream sources, reflect on
/// their own record type, and yield added/changed/deleted
/// events of their record type.
/// </summary>
public class FakePlugin
{
    private RecordTypeDefinition recordDefinition;

    public FakePlugin(RecordTypeDefinition rd)
    {
        recordDefinition = rd;
    }

    public List<RecordChange> Process(RecordChange c)
    {
        // Create a random number of changes of this record type in response
        // to the upstream change, within reason
        List<RecordChange> changes = new List<RecordChange>();
        int qty = Math.Min(Math.Max(1, ((new Random()).Next() % 3) + 1), 3);
        while (qty-- > 0 && c.ChangedRecord.Body.Length < 50)
        {
            var newRec = new Record();
            newRec.Id = RecordSequence.next();
            newRec.ParentId = c.ChangedRecord.Id;
            newRec.Body = " " + c.ChangedRecord.Body + ":" + qty.ToString();

            RecordChange newChg = new RecordChange(
                parentEventId: c.EventId,
                eventId: EventSequence.next(),
                changedRecord: newRec,
                recordType: recordDefinition.RecordType
            );
            changes.Add(newChg);
        }
        return changes;
    }
}