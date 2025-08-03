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
	public class KeyCtrl
	{
		public static string DeviceID = ""; // デフォルトのCOMポート名
		public static string PNPDeviceID = ""; // デフォルトのCOMポート名
		public static string Description = ""; // デフォルトのCOMポート名
		public static string ActiveApp = ""; // デフォルトのCOMポート名
		public static string ActiveAppPath = ""; // デフォルトのCOMポート名
		private static string ActiveAppBak = ""; // デフォルトのCOMポート名;

		public static bool GetPortName()
		{
			bool ret = false;
			DeviceID = "";
			PNPDeviceID="";
			Description = "";
			foreach (var port in new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort").Get())
			{
				string pnp = port["PNPDeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string dev = port["DeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string des = port["Description"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				
				
				if (pnp != null && pnp.Contains("VID_CAFE") && pnp.Contains("PID_3899"))
				{
					PNPDeviceID = pnp;
					DeviceID = dev;
					Description = des;
					ret = true;
					break; // 最初の一致でループを抜ける
				}
			}
			Debug.WriteLine($"DeviceID:{DeviceID}");
			return ret;
		}

		// WinAPI定数
		public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
		public const uint WINEVENT_OUTOFCONTEXT = 0;
		// WinEventフック用デリゲート
		public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
			IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		public static WinEventDelegate dele = new WinEventDelegate(WinEventProc);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
			WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		
		public static Process? GetActiveProcess()
		{
			Process? process = null;
			ActiveApp = "";
			ActiveAppPath = "";
			IntPtr hWnd = GetForegroundWindow();
			if (hWnd == IntPtr.Zero) return process;

			GetWindowThreadProcessId(hWnd, out uint processId);
			try
			{
				process = Process.GetProcessById((int)processId);
				if(process == null) return process;
				ActiveApp = process.ProcessName;
				if (process.MainModule != null)
				{
					ActiveAppPath = process.MainModule.FileName;
				}
			}
			catch
			{
				process = null;
			}
			Debug.WriteLine($"ActiveApp:{ActiveApp}");
			return process;
		}
		
		public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			if(DeviceID == "")
			{
				GetPortName();
				if (DeviceID == "")
				{
					return;
				}
			}
			GetActiveProcess();
			string s = DeviceID;
			string s2 = ActiveApp;
			Debug.WriteLine($"switch{ActiveApp}/{ActiveAppBak}");
			if (ActiveApp != ActiveAppBak)
			{
				switch (ActiveApp)
				{
					case "Photoshop":
						SendSerilApp("Photoshop");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "Illustrator":
						SendSerilApp("Illustrator");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "inkscape":
						SendSerilApp("Inkscape");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "gimp-3":
					case "gimp-2.10":
						SendSerilApp("GIMP");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "AfterFX":
						SendSerilApp("After Effects");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "FireAlpaca":
						SendSerilApp("FireAlpaca");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
					case "explorer":
					default:
						SendSerilApp("explorer");
						ActiveAppBak = ActiveApp;
						Debug.WriteLine($"send:{ActiveApp}");
						break;
				}
			}
		}

		public static void SetWinEventHook()
		{
			SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
		}
		public static bool SendSerilApp(string s)
		{
			string sendText = $"{{\"profileName\":\"{s}\"}}";
			return SendSeril(sendText);
		}
		public static bool SendSeril(string s)
		{
			bool ret = false;
			if(DeviceID=="") return ret;
			int baudRate = 115200;    // ← 使用するボーレートに合わせて変更
			using (SerialPort serialPort = new SerialPort(DeviceID, baudRate))
			{
				// ポート設定
				serialPort.Parity = Parity.None;
				serialPort.DataBits = 8;
				serialPort.StopBits = StopBits.One;
				serialPort.Handshake = Handshake.None;
				serialPort.ReadTimeout = 1000;
				serialPort.WriteTimeout = 1000;
				serialPort.DtrEnable = false; // DTRを有効にする
				serialPort.RtsEnable = false; // RTSを有効にする
				try
				{
					if (!serialPort.IsOpen)
					{
						serialPort.Open();
					}

					serialPort.WriteLine(s);
					ret = true; // 書き込み成功
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"エラー: {ex.Message}");
					ret = false;
				}
				finally
				{
					if (serialPort.IsOpen)
					{
						serialPort.Close();
					}
				}
			}
			return ret;
		}
		

	}
}
