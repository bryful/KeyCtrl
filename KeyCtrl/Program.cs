using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeyCtrl
{
	internal static class Program
	{
		// NotifyIconをstaticフィールドとして保持
		static NotifyIcon? icon;

		/*
		// WinAPI定数
		const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
		const uint WINEVENT_OUTOFCONTEXT = 0;


		// WinEventフック用デリゲート
		delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
			IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		static WinEventDelegate dele = new WinEventDelegate(WinEventProc);

		[DllImport("user32.dll")]
		static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
			WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

		[DllImport("user32.dll")]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		
		public static Process? GetActiveProcess()
		{
			IntPtr hWnd = GetForegroundWindow();
			if (hWnd == IntPtr.Zero) return null;

			GetWindowThreadProcessId(hWnd, out uint processId);
			try
			{
				return Process.GetProcessById((int)processId);
			}
			catch
			{
				return null;
			}
		}
		static void WinEventProc(IntPtr hWinEventHook, uint eventType,
	IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			const int nChars = 256;
			var buff = new StringBuilder(nChars);
			Process? ps = GetActiveProcess();
			if (ps != null)
			{
				string processInfo = $"プロセス: {ps.ProcessName} (PID: {ps.MainModule.FileName})";
				Debug.WriteLine(processInfo);
				ShowBalloon("アクティブプロセス", processInfo);
			}
			//if (GetWindowText(hwnd, buff, nChars) > 0)
			//{
			//	string message = $"切り替わり検出: {buff}";
			//	ShowBalloon(message, buff.ToString());
			//	Debug.WriteLine(message);
			//}
			//
		}
		*/
		// WinEventフック用デリゲート

		/// <summary>
		/// アプリケーションのエントリポイント
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			icon = new NotifyIcon
			{
				Icon = SystemIcons.Application,
				Visible = true,
				Text = "タスクトレイ常駐アプリ"
			};

			var contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("メッセージ表示", null, (s, e) => MessageBox.Show("こんにちは！"));
			contextMenu.Items.Add("終了", null, (s, e) => Application.Exit());
			icon.ContextMenuStrip = contextMenu;

			KeyCtrl.SetWinEventHook();

			Application.Run();
		}
		

	}
}