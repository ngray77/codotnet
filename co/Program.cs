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

// Init the engine and adapter
Engine eng = new Engine(cfg);
cfg.RegisteredAdapters.Add(fakeXL);

// Init a starting goal
var newGoal = new Record { Body = "Put a man on the moon", Id = RecordSequence.next() };
fakeXL.GetModel(goalRd.RecordType).Records.Add(newGoal);
Console.WriteLine("\nThis is an Initial Starting 'Goal', the only data in the set".PadRight(100, '*'));
fakeXL.FakeFileData.ForEach(m => m.PrettyConsole());

// Run the engine for the starting goal
fakeXL.GetRecordChangeEvents(goalRd.RecordType).ForEach(c => eng.EnqueueEvent(c));
eng.Run();
cfg.RegisteredAdapters.ForEach(ra => ra.ClearDirty());
Console.WriteLine("\nAnd here is the Result after the engine runs, a fully populated model".PadRight(100, '*'));
fakeXL.FakeFileData.ForEach(m => m.PrettyConsole());

// Retrigger the engine by changing one record
Console.WriteLine("\nRetrigger the engine by changing one record".PadRight(100, '*'));
fakeXL.GetModel(needRd.RecordType).Records.First().Body = "A little tweak";
fakeXL.GetRecordChangeEvents(needRd.RecordType).ForEach(c => eng.EnqueueEvent(c));
fakeXL.FakeFileData.ForEach(m => m.PrettyConsole());
eng.Run();
Console.WriteLine("\nAnd the result of the 'Retrigger' after the engine runs".PadRight(100, '*'));
fakeXL.FakeFileData.ForEach(m => m.PrettyConsole());