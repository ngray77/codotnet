public interface IAdapter
{
    public Model GetModel(string recordType);
    public void PushEvent(RecordChange rc);

    public void ClearDirty();
}



public class FakeXLIOAdapter : IAdapter
{
    public FakeXLIOAdapter()
    {
        FakeFileData = new List<Model>();
    }
    public List<Model> FakeFileData { get; }

    public Model GetModel(string recordType)
    {
        return FakeFileData.Single(m => m.Name == recordType);
    }
    public List<Record> GetDirtyRecords(string recordType)
    {
        return FakeFileData.Single(m => m.Name == recordType).Records.FindAll(r => r.IsDirty);
    }
    public List<RecordChange> GetRecordChangeEvents(string recordType)
    {
        var result = new List<RecordChange>();
        var dr = GetDirtyRecords(recordType);
        foreach (var r in dr)
        {
            result.Add(new RecordChange(null, EventSequence.next(), r, recordType));
        }
        return result;
    }

    public void ClearDirty()
    {
        foreach (var f in FakeFileData)
        {
            foreach (var r in f.Records)
            {
                r.ResetDirty();
            }
        }
    }

    public void PushEvent(RecordChange rc)
    {
        var recs = GetModel(rc.RecordType).Records;
        var matchingRec = recs.Find(r => r.Id == rc.ChangedRecord.Id);
        if (matchingRec == null)
            recs.Add(rc.ChangedRecord);
    }
}