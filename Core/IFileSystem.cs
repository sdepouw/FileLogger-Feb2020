using System;

public interface IFileSystem
{
    void AppendAllText(string path, string contents);
    bool Exists(string path);
    DateTime GetLastWriteTime(string path);
    void Move(string sourceFileName, string destFileName);
}