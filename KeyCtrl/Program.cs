using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeyCtrl
{
	internal static class Program
	{
		// NotifyIcon��static�t�B�[���h�Ƃ��ĕێ�
		static NotifyIcon? icon;

		/*
		// WinAPI�萔
		const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
		const uint WINEVENT_OUTOFCONTEXT = 0;


		// WinEvent�t�b�N�p�f���Q�[�g
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
				string processInfo = $"�v���Z�X: {ps.ProcessName} (PID: {ps.MainModule.FileName})";
				Debug.WriteLine(processInfo);
				ShowBalloon("�A�N�e�B�u�v���Z�X", processInfo);
			}
			//if (GetWindowText(hwnd, buff, nChars) > 0)
			//{
			//	string message = $"�؂�ւ�茟�o: {buff}";
			//	ShowBalloon(message, buff.ToString());
			//	Debug.WriteLine(message);
			//}
			//
		}
		*/
		// WinEvent�t�b�N�p�f���Q�[�g

		/// <summary>
		/// �A�v���P�[�V�����̃G���g���|�C���g
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
				Text = "�^�X�N�g���C�풓�A�v��"
			};

			var contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("���b�Z�[�W�\��", null, (s, e) => MessageBox.Show("����ɂ��́I"));
			contextMenu.Items.Add("�I��", null, (s, e) => Application.Exit());
			icon.ContextMenuStrip = contextMenu;

			KeyCtrl.SetWinEventHook();

			Application.Run();
		}
		

	}
}