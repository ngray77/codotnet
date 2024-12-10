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
        RecordChange r;
        while (eventManager.Events.Count > 0)
        {
            // Emit to console
            r = eventManager.Events.Dequeue();
            Console.WriteLine(r.Body + " [" + r.RecordType + ", Id " + r.Id + " child of " + r.ParentEventId + "]");

            // Find subscribing plugins to the event
            var subscribers = engineConfig.RegisteredRecordTypes.FindAll(
                rp => rp.TriggeredBy.Exists(
                    tb => tb.RecordType == r.RecordType
                    )
                );

            foreach (var sub in subscribers)
            {
                // Run each plugin
                FakePlugin fp = new FakePlugin(sub);
                var newRCs = fp.Process(r);

                while (newRCs.Count > 0)
                {
                    // Publish new returned events to the queue
                    var rc = newRCs[0];
                    newRCs.Remove(rc);
                    this.EnqueueEvent(rc);
                    //eventManager.Events.Enqueue(rc);
                }
            }
        }
    }
}