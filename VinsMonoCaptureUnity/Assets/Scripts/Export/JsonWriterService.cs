using System.IO;
using System.Text.Json;

namespace VinsMonoCapture.Export
{
    public class JsonWriterService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public void WriteJson<T>(string filePath, T model)
        {
            var jsonContent = JsonSerializer.Serialize(model, SerializerOptions);
            File.WriteAllText(filePath, jsonContent);
        }
    }
}
