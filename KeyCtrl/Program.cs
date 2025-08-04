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


		/// <summary>
		/// アプリケーションのエントリポイント
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			icon = new NotifyIcon()
			{
				Icon = Properties.Resources.Icons,
				Visible = true,
				Text = "TaskTray-Only App"
			};

			var contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("メッセージ表示", null, (s, e) => MessageBox.Show("こんにちは！"));
			contextMenu.Items.Add("Quit", null, (s, e) => Application.Exit());
			icon.ContextMenuStrip = contextMenu;
			Application.ApplicationExit += (s, e) =>
			{
				
			};
			KeyCtrl.begin();

			Application.Run();
		}
		

	}
}