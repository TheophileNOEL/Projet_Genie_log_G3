using System;

namespace EasySave_G3_V1
{
    // defining the backup types
    public enum BackupType
    {
        Full,         
        Differential   
    }

    // defining the possible states of the backup process
    public enum BackupState
    {
        Pending,       
        Running,       
        Completed,    
        Failed,        
        Cancelled      
    }

    // Class representing a backup scenario with its properties and execution logic
    public class Scenario
    {
        // Properties of the backup scenario
        public int Id { get; set; }             // Unique identifier for the scenario
        public string Name { get; set; }        // Name of the scenario (description or title)
        public string Source { get; set; }      // Source directory or file to backup
        public string Target { get; set; }      // Target directory or location to store the backup
        public BackupType Type { get; set; }    // Type of backup (Full or Differential)
        public BackupState State { get; private set; }  // Current state of the backup (Pending, Running, etc.)
        public string Description { get; set; } // Additional description about the backup scenario
        
        // Default constructor that initializes properties with default values
        public Scenario()
        {
            Id = -1;                
            Name = string.Empty;    
            Source = string.Empty;  
            Target = string.Empty;  
            Type = BackupType.Full; // Default type is Full backup
            State = BackupState.Pending; // Default state is Pending
            Description = string.Empty; 
        }

        // Constructor with parameters to initialize the scenario with specific values
        public Scenario(string name, string source, string target, BackupType type, string description)
        {
            Id = -1;                
            Name = name;            
            Source = source;        
            Target = target;        
            Type = type;            
            State = BackupState.Pending;          
            Description = description; 
        }

        // Method to execute the backup scenario
        public bool Execute()
        {
            try
            {
                State = BackupState.Running;
                Console.WriteLine($"Backup '{Name}' is running...");

                runSave();

                State = BackupState.Completed;
                Console.WriteLine($"Backup '{Name}' completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                State = BackupState.Failed;
                Console.WriteLine($"Error during backup '{Name}': {ex.Message}");
                return false;
            }
        }



        private void runSave()
        {
            if (!Directory.Exists(Source))
                throw new DirectoryNotFoundException($"Source path '{Source}' not found.");
            if (!Directory.Exists(Target))
                throw new DirectoryNotFoundException($"Target path '{Target}' not found.");

            foreach (string filePath in Directory.GetFiles(Source, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(Source, filePath);
                string targetPath = Path.Combine(Target, relativePath);

                bool shouldCopy = false;

                switch (Type)
                {
                    case BackupType.Full:
                        shouldCopy = true; // On copie tout
                        break;

                    case BackupType.Differential:
                        shouldCopy = !File.Exists(targetPath) ||
                                     File.GetLastWriteTimeUtc(filePath) > File.GetLastWriteTimeUtc(targetPath);
                        break;
                }

                if (shouldCopy)
                {

                    var start = DateTime.Now;
                    try
                    {
                        File.Copy(filePath, targetPath, true);
                        var duration = DateTime.Now - start;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }


        public void Cancel()
        {
            if (State == BackupState.Running)
            {
                State = BackupState.Cancelled;
                Console.WriteLine($"Backup '{Name}' has been cancelled.");
            }
            else
            {
                Console.WriteLine($"Cannot cancel backup '{Name}' as it is not currently running.");
            }
        }

    }
}
