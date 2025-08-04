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


		/// <summary>
		/// �A�v���P�[�V�����̃G���g���|�C���g
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
			contextMenu.Items.Add("���b�Z�[�W�\��", null, (s, e) => MessageBox.Show("����ɂ��́I"));
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