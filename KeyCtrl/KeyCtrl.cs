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
	public class SerialInfo
	{
		public SerialPort? serialPort = null;
		public string PNPDeviceID { get; set; } = ""; // 例: "USB\\VID_303A&PID_1001\\..."
		public string DeviceID { get; set; } = ""; // 例: "USB\\VID_303A&PID_1001\\..."
		public string Description { get; set; } = ""; // 例: "USB Serial Port"
		public SerialInfo()
		{
			Clear();
		}
		public void Clear()
		{
			PNPDeviceID = "";
			DeviceID = "";
			Description = "";
			ClosePort();
		}
		public bool OpenPort()
		{
			bool ret = false;
			if (serialPort != null)
			{
				if (serialPort.IsOpen) return true; // 既に開いている場合はtrueを返す
			}
			if (DeviceID == "") FindPort();
			if(DeviceID == "") return ret; // デバイスIDが空なら終了
			serialPort = new SerialPort(DeviceID, 115200); // ボーレートは適宜変更
			serialPort.Parity = Parity.None;
			serialPort.DataBits = 8;
			serialPort.StopBits = StopBits.One;
			serialPort.Handshake = Handshake.None;
			serialPort.ReadTimeout = 1000;
			serialPort.WriteTimeout = 1000;
			serialPort.DtrEnable = false; // DTRを有効にする
			serialPort.RtsEnable = false; // RTSを有効にする
			// ポート設定
			try
			{
				serialPort.Open();
				ret = true; // 書き込み成功
			}
			catch (Exception ex)
			{
				Clear(); // エラーが発生したらポート情報をクリア
				Debug.WriteLine($"エラー: {ex.Message}");
				ret = false;
			}
			
			return ret;
		}
		public bool FindPort()
		{
			bool ret = false;

			// シリアルポートが開いていたらおわり
			if ((DeviceID != "") && (serialPort != null) && (serialPort.IsOpen == true))
			{
				ret = true;
				return ret;
			}
			Clear();
			string NPD = "";
			string Dev = "";
			string Des = "";
			foreach (var port in new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort").Get())
			{
				NPD = port["PNPDeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				Dev = port["DeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				Des = port["Description"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."


				if (NPD != null && NPD.Contains("VID_CAFE") && NPD.Contains("PID_3899"))
				{
					if ((Dev != null) && (Des != null))
					{
						PNPDeviceID = NPD;
						DeviceID = Dev;
						Description = Des;
						ret = true;
						break; // 最初の一致でループを抜ける
					}
				}
			}
			return ret;
		}
		public bool ClosePort()
		{
			bool ret = false;
			if (serialPort != null)
			{
				if (serialPort.IsOpen)
				{
					try
					{
						serialPort.Close();
						serialPort.Dispose(); // リソースを解放
						serialPort = null; // ポートをnullに設定
						ret = true; // 閉じるのに成功
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Error closing serial port: {ex.Message}");
						ret = false; // 閉じるのに失敗
					}
				}
			}
			return ret;
		}
		public bool SendSeril(string s)
		{
			bool ret = false;
			OpenPort();
			if (serialPort != null && serialPort.IsOpen)
			{
				try 
				{ 
					serialPort.WriteLine(s);
					ret = true; // 書き込み成功
				}
				catch (Exception ex) { 
					Debug.WriteLine($"エラー: {ex.Message}");
					ret = false;
				}
			}

			return ret;
		}
		public bool IsOpen
		{
			get
			{
				return ((serialPort != null) && (serialPort.IsOpen));
			}
		}
	}
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
	public class KeyCtrl
	{
		private static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		public static SerialInfo serialInfo = new SerialInfo();
		public static AppInfo appInfo = new AppInfo();
		public static string[] targetAPP= new string[] { "explorer", "Photoshop", "Illustrator", "Inkscape", "Gimp", "After Effects", "FireAlpaca" };
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

		
		public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			serialInfo.OpenPort();
			if(serialInfo.IsOpen==false) return;

			appInfo.GetActiveApp();
			Debug.WriteLine($"switch{appInfo.AppName}/{appInfo.AppPath}");
			if (appInfo.AppName != appInfo.AppNameBak)
			{
				foreach(var app in targetAPP)
				{
					string ap = app.ToLower();
					if ((ap == "after effects")|| (ap == "aftereffects")|| (ap == "ae"))
					{
						ap = "afterfX"; // After Effectsの略称
					}
					if (appInfo.AppName.ToLower().Contains(ap))
					{
						Debug.WriteLine($"match:{appInfo.AppName}");
						appInfo.AppNameBak = appInfo.AppName;
						serialInfo.SendSeril(app);
						return; // 一致したら終了
					}
				}
				
			}
		}

		public static void begin()
		{
			timer.Interval = 1000*10; // 10秒ごとに実行
			timer.Tick += (s, e) =>
			{
				serialInfo.OpenPort();
				Debug.WriteLine($"Timer Tick: {appInfo.AppName}");
			};
			serialInfo.FindPort();
			SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
			timer.Start();
		}
		public static void end()
		{
			timer.Stop();
			serialInfo.Clear();
		}


	}
}
