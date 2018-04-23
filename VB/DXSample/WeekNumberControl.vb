Imports Microsoft.VisualBasic
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace DXSample
	Partial Public Class WeekNumberControl
		Inherits Control

		Private _Scheduler As SchedulerControl
		Public Property Scheduler() As SchedulerControl
			Get
				Return _Scheduler
			End Get
			Set(ByVal value As SchedulerControl)
				If _Scheduler Is value Then
					Return
				End If

				If _Scheduler IsNot Nothing Then
					RemoveHandler _Scheduler.VisibleIntervalChanged, AddressOf _Scheduler_VisibleIntervalChanged
					RemoveHandler _Scheduler.CustomDrawTimeCell, AddressOf _Scheduler_CustomDrawTimeCell
				End If
				_Scheduler = value
				AddHandler _Scheduler.VisibleIntervalChanged, AddressOf _Scheduler_VisibleIntervalChanged
				AddHandler _Scheduler.CustomDrawTimeCell, AddressOf _Scheduler_CustomDrawTimeCell
				Invalidate()
			End Set
		End Property

		Private privateStart As DateTime
		Private Property Start() As DateTime
			Get
				Return privateStart
			End Get
			Set(ByVal value As DateTime)
				privateStart = value
			End Set
		End Property
		Private privateWeekRectParameters As List(Of RectParameters)
		Private Property WeekRectParameters() As List(Of RectParameters)
			Get
				Return privateWeekRectParameters
			End Get
			Set(ByVal value As List(Of RectParameters))
				privateWeekRectParameters = value
			End Set
		End Property

		Public Sub New()
			InitializeComponent()
			WeekRectParameters = New List(Of RectParameters)()
			For i As Integer = 0 To 4
				WeekRectParameters.Add(New RectParameters())
			Next i
		End Sub

		Private Sub _Scheduler_CustomDrawTimeCell(ByVal sender As Object, ByVal e As CustomDrawObjectEventArgs)
			Dim cell As MonthSingleWeekCell = TryCast(e.ObjectInfo, MonthSingleWeekCell)
			If cell Is Nothing Then
				Return
			End If

			If cell.Interval.Start.DayOfWeek = DayOfWeek.Monday Then
				For i As Integer = 0 To 4
					If cell.Interval.Start = Start.AddDays(i * 7) Then
						WeekRectParameters(i) = New RectParameters() With {.Y = e.Bounds.Y, .Height = e.Bounds.Height}
					End If
				Next i
				Invalidate()
			End If
		End Sub

		Private Sub _Scheduler_VisibleIntervalChanged(ByVal sender As Object, ByVal e As EventArgs)
			Start = Scheduler.Start
		End Sub

		Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
			MyBase.OnPaint(pe)
			If Scheduler Is Nothing Then
				Return
			End If

			Dim pen As New Pen(Brushes.Black)
			Dim sf As New StringFormat()
			sf.Alignment = StringAlignment.Center
			sf.LineAlignment = StringAlignment.Center

			Dim monthRect As Rectangle
			For i As Integer = 0 To 4
				monthRect = New Rectangle(New Point(pe.ClipRectangle.Location.X, WeekRectParameters(i).Y), New Size(pe.ClipRectangle.Width - 1, WeekRectParameters(i).Height))
				pe.Graphics.DrawRectangle(pen, monthRect)

				Dim numberStart As Integer = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(Start.AddDays(7 * i), CalendarWeekRule.FirstDay, DayOfWeek.Monday)
				Dim numberEnd As Integer = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(Start.AddDays(7 * i + 6), CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday)

				Dim weekNumber As String = ""
				If numberStart = numberEnd Then
					weekNumber = numberStart.ToString()
				Else
					weekNumber = numberStart.ToString() & " / " & numberEnd.ToString()
				End If

				pe.Graphics.DrawString(weekNumber, Scheduler.Appearance.HeaderCaption.Font, Brushes.Black, monthRect, sf)
			Next i
		End Sub
	End Class

	Friend Class RectParameters
		Private privateY As Integer
		Public Property Y() As Integer
			Get
				Return privateY
			End Get
			Set(ByVal value As Integer)
				privateY = value
			End Set
		End Property
		Private privateHeight As Integer
		Public Property Height() As Integer
			Get
				Return privateHeight
			End Get
			Set(ByVal value As Integer)
				privateHeight = value
			End Set
		End Property
	End Class
End Namespace
