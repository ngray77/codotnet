using System.Data;
using System.Security.Cryptography.X509Certificates;

public class RecordTypeDefinition
{
    public RecordTypeDefinition()
    {
        RecordType = "";
        this.TriggeredBy = new List<RecordTypeDefinition>();
    }

    public string RecordType { get; set; }

    public List<RecordTypeDefinition> TriggeredBy { get; }
}

public class EngineConfig
{
    public List<RecordTypeDefinition> RegisteredRecordTypes { get; }
    public EngineConfig()
    {
        Models = new List<Model>();
        RegisteredRecordTypes = new List<RecordTypeDefinition>();
        RegisteredAdapters = new List<IAdapter>();
    }

    public List<Model> Models{get;}
    public List<IAdapter> RegisteredAdapters{get;}
}

public class Engine
{
    private EngineConfig engineConfig;

    public Engine(EngineConfig cfg)
    {
        engineConfig = cfg;
    }

    private EventManager eventManager = new EventManager();

    public void EnqueueEvent(RecordChange recordChange)
    {
        engineConfig.RegisteredAdapters.ForEach(ra=>ra.PushEvent(recordChange));
        eventManager.Events.Enqueue(recordChange);
    }

    public void Run()
    {
        RecordChange rc;
        while (eventManager.Events.Count > 0)
        {
            // Emit to console
            rc = eventManager.Events.Dequeue();
            Console.WriteLine(rc.ChangedRecord.Body + " [" + rc.RecordType + " Id " + rc.ChangedRecord.Id + "]");

            // Find subscribing plugins to the event
            var subscribers = engineConfig.RegisteredRecordTypes.FindAll(
                rp => rp.TriggeredBy.Exists(
                    tb => tb.RecordType == rc.RecordType
                    )
                );

            foreach (var sub in subscribers)
            {
                // Run each plugin
                FakePlugin fp = new FakePlugin(sub);
                var newRCs = fp.Process(rc);

                while (newRCs.Count > 0)
                {
                    // Publish new returned events to the queue
                    var newRC = newRCs[0];
                    newRCs.Remove(newRC);
                    this.EnqueueEvent(newRC);
                }
            }
        }
    }
}