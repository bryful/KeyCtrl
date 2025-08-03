using System.IO.Ports;
using System;
using System.Management;

namespace PortTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{

			List<string> portNames = new List<string>();
			foreach (var port in new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort").Get())
			{
				string name = port["Name"]?.ToString(); // 例: "USB-SERIAL CH340 (COM5)"
				string pnpId = port["PNPDeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string DeviceID = port["DeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string Description = port["Description"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				portNames.Add($"Port: [{name}],   PNP ID: [{pnpId}] DID:{DeviceID} D:[{Description}]");


			}
			textBox1.Lines = portNames.ToArray();

		}

		private void button2_Click(object sender, EventArgs e)
		{
			
		}

		private void button3_Click(object sender, EventArgs e)
		{
			
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string[] portNames = SerialPort.GetPortNames();
			List<string> lsit = new List<string>();
			foreach (var dev in portNames)
			{
				lsit.Add($"Port: {dev}");
			}
			textBox1.Lines = portNames.ToArray();
			if (lsit.Count == 0)
			{
				MessageBox.Show("USB CDCデバイスが見つかりませんでした。");
			}
			else
			{
				MessageBox.Show($"{lsit.Count} 個のUSB CDCデバイスが見つかりました。");
			}
		}
	}
}
