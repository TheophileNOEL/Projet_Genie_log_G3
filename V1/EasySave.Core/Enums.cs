namespace EasySave.Core;

public enum BackupType
{
    Full,
    Differential
}

public enum BackupState
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}
public enum LogFormat
{
    Json,
    Xml
}