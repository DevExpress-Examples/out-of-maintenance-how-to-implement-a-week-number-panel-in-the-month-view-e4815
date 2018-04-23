Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace DXSample
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
			schedulerControl1.Start = DateTime.Now
		End Sub
	End Class
End Namespace
