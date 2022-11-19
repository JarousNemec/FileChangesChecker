using System.Numerics;

namespace FileChangesChecker;

public class CheckedFileProperties
{
    public string Path { get; set; }
    public string Name { get; set; }
    public DateTime LastChange { get; set; }
    public string Hash { get; set; }

    public FileState State { get; set; }

    public override string ToString()
    {
        return $"{Path};{Name};{LastChange};{Hash};{(int)State}";
    }
}