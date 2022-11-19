using System.Numerics;
using System.Security.Cryptography;

namespace FileChangesChecker;

public class FileChecker
{
    public void Check(string CheckDataPath, string folderToCheckPath)
    {
        List<CheckedFileProperties>? lastCheck = LoadLastCheckData(CheckDataPath);
        List<CheckedFileProperties> filesToCheck = LoadFilesDataFromCheckedFolder(folderToCheckPath);


        if (lastCheck == null)
        {
            foreach (var file in filesToCheck)
            {
                file.State = FileState.New;
            }

            SaveChecks(filesToCheck, CheckDataPath);
            return;
        }
        CheckFilesChanges(lastCheck, filesToCheck);
        SaveChecks(filesToCheck, CheckDataPath);
    }

    private void CheckFilesChanges(List<CheckedFileProperties> lastCheck,
        List<CheckedFileProperties> filesToCheck)
    {
        foreach (var file in filesToCheck)
        {
            CheckedFileProperties? sameFile = lastCheck.Find(x => x.Name == file.Name);
            if (sameFile == null)
            {
                file.State = FileState.New;
                file.LastChange = sameFile.LastChange;
                continue;
            }

            if (file.Hash == sameFile.Hash)
            {
                file.State = FileState.Same;
                file.LastChange = sameFile.LastChange;
                continue;
            }

            file.State = FileState.Changed;
            file.LastChange = DateTime.Now;
        }
    }

    private List<CheckedFileProperties>? LoadLastCheckData(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        List<CheckedFileProperties> files = new List<CheckedFileProperties>();
        var data = File.ReadAllLines(path);
        foreach (var aboutFile in data)
        {
            files.Add(GetFilePropertiesFromLastLog(aboutFile));
        }

        return files;
    }

    private CheckedFileProperties GetFilePropertiesFromLastLog(string aboutFile)
    {
        var data = aboutFile.Split(';');
        CheckedFileProperties properties = new CheckedFileProperties()
        {
            Path = data[0],
            Name = data[1],
            LastChange = DateTime.Parse(data[2]),
            Hash = data[3],
            State = (FileState)int.Parse(data[4]),
        };
        return properties;
    }

    private List<CheckedFileProperties> LoadFilesDataFromCheckedFolder(string path)
    {
        var files = Directory.GetFiles(path);
        List<CheckedFileProperties> filesData = new List<CheckedFileProperties>();
        foreach (var file in files)
        {
            filesData.Add(GetFilePropertiesFromFile(file));
        }

        return filesData;
    }

    private CheckedFileProperties GetFilePropertiesFromFile(string path)
    {
        FileInfo info = new FileInfo(path);
        CheckedFileProperties properties = new CheckedFileProperties()
        {
            Path = path,
            Name = info.Name,
            LastChange = DateTime.Now,
            Hash = GetHashCode(path)
        };
        return properties;
    }

    private void SaveChecks(List<CheckedFileProperties> checks, string path)
    {
        List<string> properties = new List<string>();
        foreach (var check in checks)
        {
            properties.Add(check.ToString());
        }

        File.WriteAllLines(path, properties.ToArray());
    }

    private string GetHashCode(string filePath)
    {
        using (var service = new SHA256CryptoServiceProvider())
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var hash = service.ComputeHash(fileStream);
                var hashString = Convert.ToBase64String(hash);
                return hashString.TrimEnd('=');
            }
        }
    }
}