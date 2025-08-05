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
	public class SerialItems
	{
		public ComInfo[] Items = new ComInfo[0];
		public int Count
		{
			get { return Items.Length; }
		}
		public SerialItems()
		{
			Get();
		}
		public SerialItems(string target = "")
		{
			Get(target);
		}
		public int Get(string target = "")
		{
			Items = new ComInfo[0];
			List<ComInfo> comInfoList = new List<ComInfo>();

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
					string des = name.Substring(0, idx0).Trim();
					string com = name.Substring(idx0 + 1, idx1 - idx0 - 1).Trim();
					;
					if ((target == "") || ((target != "") && (des.Contains(target) == true)))
					{
						comInfoList.Add(new ComInfo() { ComID = com, Description = des });
					}
				}
			}
			Items = comInfoList.ToArray();
			return Items.Length;
		}
		public int Find(string comID)
		{
			int idx = -1;
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i].ComID == comID)
				{
					idx = i;
					break;
				}
			}
			return idx;
		}
	}
}
