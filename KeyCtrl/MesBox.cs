using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyCtrl
{
	public partial class MesBox : Form
	{
		public new string Text 
		{ 
			get { return textBox1.Text; } 
			set { textBox1.Text = value; } 
		}
		public MesBox()
		{
			InitializeComponent();
		}
	}
}
