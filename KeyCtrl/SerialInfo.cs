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
		public ComInfo comInfo = new ComInfo();
		private string _result = "";
		public string Result
		{
			get { return _result; }
		}
		public SerialInfo()
		{
			Clear();
		}
		public SerialInfo(ComInfo ci)
		{
			Clear();
			comInfo = ci;
		}
		public void Clear()
		{
			comInfo.Clear();
			ClosePort();
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
		public bool OpenPort()
		{
			bool ret = false;
			Debug.WriteLine($"Open Serial Port1: {comInfo.ComID}");
			if (serialPort != null)
			{
				if (serialPort.IsOpen) return true; // 既に開いている場合はtrueを返す
			}
			Debug.WriteLine($"Open Serial Port2: {comInfo.ComID}");
			if (comInfo.ComID == "")
			{
				Debug.WriteLine($"Open Serial Port2err: {comInfo.ComID}");
				return ret;
			}
			Debug.WriteLine($"Open Serial Port: {comInfo.ComID}");
			serialPort = new SerialPort(comInfo.ComID, 115200); // ボーレートは適宜変更
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
				Debug.WriteLine($"Open Serial Port3: {comInfo.ComID}");
				serialPort.Open();
				
				ret = true; // 書き込み成功
				Debug.WriteLine($"Open Serial Port4: {comInfo.ComID}");
			}
			catch (Exception ex)
			{
				Clear(); // エラーが発生したらポート情報をクリア
				Debug.WriteLine($"エラー: {ex.Message}");
				ret = false;
			}
			Debug.WriteLine($"Open Serial Port5: {comInfo.ComID}");

			return ret;
		}
		public bool Send(string s)
		{
			bool ret = false;
			OpenPort();
			if (serialPort != null && serialPort.IsOpen)
			{
				try
				{
					_result = "";
					Debug.WriteLine($"送信: {s}");
					serialPort.WriteLine(s);
					serialPort.ReadTimeout = 2000; // 読み取りタイムアウトを設定
					Thread.Sleep(500); // 少し待つ
					string str = serialPort.ReadExisting().Trim();
					ret = true; // 書き込み成功
					if (str != "")
					{
						_result = str;
						Debug.WriteLine($"受信: {str}");
					}
				}
				catch (Exception ex)
				{
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
}
