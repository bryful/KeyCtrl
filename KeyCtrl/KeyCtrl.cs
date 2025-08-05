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
		#region WinAPI定数とデリゲート
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
		#endregion
		public static AppInfo appInfo = new AppInfo();

		public static SerialInfo serialInfo = new SerialInfo();

		private static System.Windows.Forms.Timer timer 
			= new System.Windows.Forms.Timer();
		
		public static string[] targetAPP= new string[] { 
			"explorer", 
			"After Effects", 
			"Photoshop", 
			"Illustrator", 
			"GIMP", 
			"Inkscape",  
			"FireAlpaca" 
		};

		// ******************************************************************************
		public static void FindComPort()
		{
			string des = "USB-Enhanced-SERIAL CH343";
			string com = serialInfo.comInfo.ComID;

			// シリアルポートを取得
			SerialItems si = new SerialItems(des);
			if(si.Count > 0)
			{
				int idx = 0;
				if(com != "")
				{
					idx = si.Find(com);
					if(idx < 0) idx = 0; // 見つからなかった場合は最初のポートを使用
				}
				serialInfo.comInfo = si.Items[idx];
				if (serialInfo.IsOpen==false)
				{
					serialInfo.OpenPort();
				}
				Debug.WriteLine($"Found Serial Port: {serialInfo.comInfo.ComID}");
			}
			else
			{
				serialInfo.ClosePort();
				Debug.WriteLine("No Serial Port Found");
			}
		}
		// ******************************************************************************
		public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			Debug.WriteLine("WinEventProc1");
			appInfo.GetActiveApp();
			Debug.WriteLine("WinEventProc2");
			Debug.WriteLine($"switch{appInfo.AppName}/{appInfo.AppPath}");
			if (serialInfo.comInfo.ComID=="")
			{
				FindComPort();
			}
			Debug.WriteLine("WinEventProc3");

			serialInfo.OpenPort();
			if (serialInfo.IsOpen == false)
			{
				Debug.WriteLine("WinEventProc3- openErr");
				return;
			}
			Debug.WriteLine("WinEventProc4");

			if (appInfo.AppName != appInfo.AppNameBak)
			{
				Debug.WriteLine("WinEventProc5");
				foreach (var app in targetAPP)
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
						serialInfo.Send($"{{\"profileName\":\"{app}\"}}");
						return; // 一致したら終了
					}
				}
				
			}
			Debug.WriteLine("WinEventProc6");
		}

		public static void begin()
		{
			timer.Interval = 1000*15; // 10秒ごとに実行
			timer.Tick += (s, e) =>
			{
				FindComPort();
				Debug.WriteLine($"Timer Tick: {appInfo.AppName}");
			};
			FindComPort();
			SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
			timer.Start();
		}
		public static void end()
		{
			timer.Stop();
			serialInfo.Clear();
		}

		public static void GetProfileData()
		{
			serialInfo.Send($"{{\"getProfiles\":0}}");
			if (serialInfo.Result != "")
			{
				using(MesBox mb = new MesBox())
				{
					mb.Text = serialInfo.Result;
					mb.ShowDialog();
				}
			}
			else
			{
				Debug.WriteLine("GetProfileData: No Result");
			}
		}

	}
}
