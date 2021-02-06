using System.Diagnostics;

namespace TelegramTTSBot
{
    class PythonTTS
    {
        public static string TextToSpeech(string text, string outputName)
        {
            var scriptPath = @"Script path";
            var psi = new ProcessStartInfo()
            {
                FileName = @"Python path",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"\"{scriptPath}\" \"{text}\" \"{outputName}\""
            };
            var error = "";

            using (var process = Process.Start(psi))
            {
                error = process.StandardError.ReadToEnd();
            }

            return error;
        }
    }
}
