using System.Threading;
using Python.Runtime;

using System.Runtime.InteropServices;

namespace ServerAlphaWebsite.PythonEngines
{
	public static class PythonNet
	{
		private static readonly object lockObject = new object();
		public static void InitPythonEngine()
		{
			if (PythonEngine.IsInitialized) { return; }

			lock (lockObject)
			{
				if (!PythonEngine.IsInitialized)
				{
					Runtime.PythonDLL = Environment.GetEnvironmentVariable("MARTIAN_PYTHON_SO_PATH");
					PythonEngine.Initialize();
				}
			}
		}
	}
}