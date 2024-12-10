public class Model
{
    public string Name {get;}
    public Model(string name)
    {
        Name = name;
        Records = new List<Record>();
    }
    public List<Record> Records {get;}
}

public class Record
{
    private int? _id;
    private int? _parentId;
    private string _body = "";

    private readonly HashSet<string> _dirtyFields = new();

    public int? Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                _dirtyFields.Add(nameof(Id));
            }
        }
    }

    public int? ParentId
    {
        get => _parentId;
        set
        {
            if (_parentId != value)
            {
                _parentId = value;
                _dirtyFields.Add(nameof(ParentId));
            }
        }
    }

    public string Body
    {
        get => _body;
        set
        {
            if (_body != value)
            {
                _body = value;
                _dirtyFields.Add(nameof(Body));
            }
        }
    }

    public bool IsDirty => _dirtyFields.Count > 0;

    public void ResetDirty()
    {
        _dirtyFields.Clear();
    }
}