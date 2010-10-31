#undef DEBUG

#if DEBUG
using System;
using System.IO;
using System.Text;

namespace Bot
{
	public static class Logger
	{
		private const string FILE_NAME = "botlog.txt";

		static Logger()
		{
			Enabled = true;
		}

		private static FileStream fileStream;

		private static bool enabled;
		public static bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public static void Log(string message)
		{
			if (!Enabled) return;
			if (fileStream == null)
			{
				fileStream = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write);
			}
			int length = message.IndexOf("\n\r");
			string modifiedMessage = length > 0 ? message.Substring(0, length) : message;

			byte[] byteData = Encoding.ASCII.GetBytes(
				//DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss ") + 
				modifiedMessage + "\n\r");
			if (fileStream != null)
			{
				try
				{
					fileStream.Write(byteData, 0, byteData.Length);
					fileStream.Flush();
				}
				catch (Exception)
				{
					
					throw;
				}
				
			}

			if (length <= 0) return;
			Log(message.Substring(length + 1, message.Length - length - 1));
		}

		public static void Close()
		{
			//fileStream.Close();
		}
	}
}
#endif