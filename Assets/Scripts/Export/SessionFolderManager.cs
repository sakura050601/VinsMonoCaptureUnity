using System;
using System.IO;

namespace VinsMonoCapture.Export
{
    public class SessionFolderManager
    {
        public SessionPaths CreateSessionFolders(string rootDirectoryPath, string sessionName)
        {
            var sanitizedSessionName = SanitizeName(sessionName);
            var sessionIdentifier = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{sanitizedSessionName}";

            var sessionDirectoryPath = Path.Combine(rootDirectoryPath, sessionIdentifier);
            var imageDirectoryPath = Path.Combine(sessionDirectoryPath, "images");
            var logsDirectoryPath = Path.Combine(sessionDirectoryPath, "logs");

            Directory.CreateDirectory(sessionDirectoryPath);
            Directory.CreateDirectory(imageDirectoryPath);
            Directory.CreateDirectory(logsDirectoryPath);

            return new SessionPaths
            {
                SessionIdentifier = sessionIdentifier,
                SessionDirectoryPath = sessionDirectoryPath,
                ImageDirectoryPath = imageDirectoryPath,
                LogsDirectoryPath = logsDirectoryPath
            };
        }

        private static string SanitizeName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return "capture_session";
            }

            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                rawName = rawName.Replace(invalidCharacter, '_');
            }

            return rawName.Trim().Replace(' ', '_').ToLowerInvariant();
        }
    }

    public class SessionPaths
    {
        public string SessionIdentifier = string.Empty;
        public string SessionDirectoryPath = string.Empty;
        public string ImageDirectoryPath = string.Empty;
        public string LogsDirectoryPath = string.Empty;
    }
}
