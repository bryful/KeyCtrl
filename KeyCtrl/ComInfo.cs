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
	public class ComInfo
	{
		public string ComID { get; set; } = "";
		public string Description { get; set; } = "";
		public ComInfo()
		{
			Clear();
		}
		public ComInfo(string comID = "", string description = "")
		{
			ComID = comID;
			Description = description;
		}
		public void Clear()
		{
			ComID = "";
			Description = "";
		}
	}
}
