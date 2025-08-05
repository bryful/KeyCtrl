using System.IO.Ports;
using System;
using System.Management;

namespace PortTest
{
	public partial class Form1 : Form
	{
		public  string[] GetDeviceNames()
		{
			var deviceNameList = new System.Collections.ArrayList();
			
			var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

			ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
			ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

			//全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
			foreach (ManagementObject manageObj in manageObjCol)
			{
				//Nameプロパティを取得
				var namePropertyValue = manageObj.GetPropertyValue("Name");
				if (namePropertyValue == null)
				{
					continue;
				}

				//Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
				string name = namePropertyValue.ToString();
				if (check.IsMatch(name))
				{
					int idx0 = name.LastIndexOf('(');
					int idx1 = name.LastIndexOf(')');
					string des = name.Substring(0,idx0).Trim();
					string com = name.Substring(idx0 + 1, idx1 - idx0 - 1).Trim();
					deviceNameList.Add($"{des},{com}");
				}
			}

			//戻り値作成
			if (deviceNameList.Count > 0)
			{
				string[] deviceNames = new string[deviceNameList.Count];
				int index = 0;
				foreach (var name in deviceNameList)
				{
					deviceNames[index++] = name.ToString();
				}
				return deviceNames;
			}
			else
			{
				return null;
			}
		}
  
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{

			List<string> portNames = new List<string>();
			int i = 0;
			//"SELECT * FROM meta_class"
			//SELECT * FROM Win32_SerialPort
			foreach (var port in new ManagementObjectSearcher("SELECT * FROM meta_class").Get())
			{
				string name = port["Name"]?.ToString(); // 例: "USB-SERIAL CH340 (COM5)"
				string pnpId = port["PNPDeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string DeviceID = port["DeviceID"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				string Description = port["Description"]?.ToString(); // 例: "USB\\VID_303A&PID_1001\\..."
				portNames.Add($"Port: [{name}],   PNP ID: [{pnpId}] DID:{DeviceID} D:[{Description}]");
				i++;

			}
			textBox1.Lines = portNames.ToArray();
			MessageBox.Show($"{i} 個のUSB CDCデバイスが見つかりました。");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			
		}

		private void button3_Click(object sender, EventArgs e)
		{
			
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string[] portNames = GetDeviceNames();
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
