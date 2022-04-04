using System;
using System.IO;

namespace KG.Util
{
    public enum ExceptionLevel
    { 
        WEAK,
        FATAL
    }

    public interface ILogger
    {
        void Info(string msg);
        void Error(string msg);
    }

    public class FileLogger : ILogger, IDisposable
    {
        private FileStream _stream;
        private StreamWriter _writer;

        public FileLogger(string path)
        {
            try
            {
                var root = Path.GetDirectoryName(path);
                if (Directory.Exists(root) == false)
                    Directory.CreateDirectory(root);

                _stream = File.Open(path, FileMode.OpenOrCreate);
                _writer = new StreamWriter(_stream);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public void Dispose()
        {
            _stream.Close();
        }

        public void Error(string msg) => _writer.WriteLine(msg);

        public void Info(string msg) => _writer.WriteLine(msg);
    }

    public class UnityLogger : ILogger
    {
        public void Error(string msg) => UnityEngine.Debug.Log(msg);

        public void Info(string msg) => UnityEngine.Debug.LogError(msg);
    }
}
