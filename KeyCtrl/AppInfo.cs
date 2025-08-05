using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using System.IO.Ports;

namespace KeyCtrl
{
	public class AppInfo
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		public string AppName { get; set; } = ""; // アプリケーション名
		public string AppPath { get; set; } = ""; // アプリケーションのパス
		public string AppNameBak { get; set; } = ""; // アプリケーションのパス
		public AppInfo()
		{
			Clear();
		}
		public void Clear()
		{
			AppName = "";
			AppPath = "";
			AppNameBak = "";
		}
		public Process? GetActiveApp()
		{
			Process? process = null;
			AppName = "";
			AppPath = "";
			IntPtr hWnd = GetForegroundWindow();
			if (hWnd == IntPtr.Zero) return process;

			GetWindowThreadProcessId(hWnd, out uint processId);
			try
			{
				process = Process.GetProcessById((int)processId);
				if (process == null) return process;
				AppName = process.ProcessName;
				if (process.MainModule != null)
				{
					AppPath = process.MainModule.FileName;
				}
			}
			catch
			{
				process = null;
			}
			Debug.WriteLine($"ActiveApp:{AppName}");
			return process;
		}
	}
}
