using System;
using System.IO;

public class FileSystem : IFileSystem
{
    public void AppendAllText(string path, string contents) => File.AppendAllText(path, contents);
    public bool Exists(string path) => File.Exists(path);
    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
    public void Move(string sourceFileName, string destFileName) => File.Move(sourceFileName, destFileName);
}
