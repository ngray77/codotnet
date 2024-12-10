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
        while (qty-- > 0 && c.Body.Length < 50)
        {
            RecordChange newChg = new RecordChange(
                c.Id,
                id: StaticSequence.next(),
                body: " " + c.Body + ":" + qty.ToString(),
                recordType: recordDefinition.RecordType
            );
            changes.Add(newChg);
        }
        return changes;
    }
}