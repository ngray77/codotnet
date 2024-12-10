EngineConfig cfg = new EngineConfig();

// Initialize the ontology
var goalRd = new RecordTypeDefinition { RecordType = "Goal" };
cfg.RegisteredRecordTypes.Add(goalRd);
var personRd = new RecordTypeDefinition { RecordType = "Person" };
personRd.TriggeredBy.Add(goalRd);
cfg.RegisteredRecordTypes.Add(personRd);
var prefRd = new RecordTypeDefinition { RecordType = "Preference" };
prefRd.TriggeredBy.Add(personRd);
cfg.RegisteredRecordTypes.Add(prefRd);
var needRd = new RecordTypeDefinition { RecordType = "Need" };
needRd.TriggeredBy.Add(goalRd);
needRd.TriggeredBy.Add(personRd);
cfg.RegisteredRecordTypes.Add(needRd);
var stuffRd = new RecordTypeDefinition { RecordType = "Item" };
stuffRd.TriggeredBy.Add(needRd);
cfg.RegisteredRecordTypes.Add(stuffRd);

// Initialize the adapters
var fakeXL = new FakeXLIOAdapter();
fakeXL.FakeFileData.Add(new Model(goalRd.RecordType));
fakeXL.FakeFileData.Add(new Model(personRd.RecordType));
fakeXL.FakeFileData.Add(new Model(prefRd.RecordType));
fakeXL.FakeFileData.Add(new Model(needRd.RecordType));
fakeXL.FakeFileData.Add(new Model(stuffRd.RecordType));

// Init a starting goal
Engine eng = new Engine(cfg);
cfg.RegisteredAdapters.Add(fakeXL);

var r = new Record { Body = "Put a man on the moon", Id = RecordSequence.next() };
fakeXL.GetModel(goalRd.RecordType).Records.Add(r);

var chg = fakeXL.GetRecordChangeEvents(goalRd.RecordType);
foreach(var c in chg)
    eng.EnqueueEvent(c);
eng.Run();
cfg.RegisteredAdapters.ForEach(ra=>ra.ClearDirty());


Console.WriteLine("-----Retrigger the engine by changing one record------");
fakeXL.GetModel(needRd.RecordType).Records.First().Body = "A little tweak";
var chg2 = fakeXL.GetRecordChangeEvents(needRd.RecordType);
foreach(var c in chg2)
    eng.EnqueueEvent(c);
eng.Run();