#if DEBUG

using System;
using System.IO;
using System.Text;

namespace Bot
{
	public static class Logger
	{
		private const string FileName = "botlog.txt";

		static Logger()
		{
			Enabled = true;
		}

		private static FileStream _fileStream;

		public static bool Enabled { get; set; }

		public static void Log(string message)
		{
			if (!Enabled) return;
			if (_fileStream == null)
			{
				FileMode fileMode = FileMode.Append;
				if (!File.Exists(FileName)) fileMode = FileMode.CreateNew;
				_fileStream = new FileStream(FileName, fileMode, FileAccess.Write);
			}
			int length = message.IndexOf("\n");
			string modifiedMessage = length > 0 ? message.Substring(0, length) : message;

			byte[] byteData = Encoding.ASCII.GetBytes(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss ") + modifiedMessage + "\n");
			_fileStream.Write(byteData, 0, byteData.Length);
			_fileStream.Flush();

			if (length > 0)
			{
				Log(message.Substring(length + 1, message.Length - length - 1));
			}
		}
	}
}
#endif