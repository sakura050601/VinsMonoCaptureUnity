using System.IO;
using UnityEngine;

namespace VinsMonoCapture.Export
{
    public class JsonWriterService
    {
        public void WriteJson<T>(string filePath, T model)
        {
            var jsonContent = JsonUtility.ToJson(model, true);
            File.WriteAllText(filePath, jsonContent);
        }
    }
}
