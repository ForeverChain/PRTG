Imports PrtgAPI
Imports PrtgAPI.Parameters
Imports PrtgAPI.Request
Imports System.Threading
Imports System.Drawing
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Private prtgClient As PrtgClient
    Public cancellationToken As CancellationToken = CancellationToken.None

    Private WithEvents dgvDevices As DataGridView

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Load, AddressOf Form1_Load
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
        ' Code executed when the form is closing
        ' Disconnect from the PRTG server and clean up resources
        prtgClient = Nothing
    End Sub

    Private Async Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim prtgServerUrl As String = "http://127.0.0.1/"
        Dim username As String = "prtgadmin"
        Dim password As String = "prtgadmin"

        prtgClient = New PrtgClient(prtgServerUrl, username, password)

        Try
            Dim devices = Await prtgClient.GetDevicesAsync()
            Dim deviceCount As Integer = devices.Count
            Label1.Text = "Total devices: " & deviceCount

            Dim downCount As Integer = 0
            Dim warningCount As Integer = 0
            Dim upCount As Integer = 0

            For Each device In devices
                Dim sensors = Await prtgClient.GetSensorsAsync([Property].ParentId, device.Id)
                For Each sensor In sensors
                    Dim statusText As String = GetStatusText(sensor.Status)

                    Dim statusColor As Color
                    Select Case sensor.Status
                        Case PrtgAPI.Status.Down
                            statusColor = Color.Red
                        Case PrtgAPI.Status.Warning
                            statusColor = Color.Yellow
                        Case PrtgAPI.Status.Up
                            statusColor = Color.Green
                        Case Else
                            statusColor = Color.Gray
                    End Select

                    Dim row As DataGridViewRow = New DataGridViewRow()
                    row.CreateCells(DataGridView1, statusText, device.Name, device.Host, sensor.Message)
                    row.Cells(0).Style.BackColor = statusColor
                    DataGridView1.Rows.Add(row)
                Next

                ' Count the statuses
                If sensors.Any(Function(s) s.Status = PrtgAPI.Status.Down) Then
                    downCount += 1
                ElseIf sensors.Any(Function(s) s.Status = PrtgAPI.Status.Warning) Then
                    warningCount += 1
                ElseIf sensors.All(Function(s) s.Status = PrtgAPI.Status.Up) Then
                    upCount += 1
                End If
            Next

            Label2.Text = String.Format("Sensors: {0} Up, {1} Warning, {2} Down", upCount, warningCount, downCount)
        Catch ex As Exception
            ' Code to execute if there is an exception
            Console.WriteLine("Error connecting to PRTG server: " & ex.Message)
        End Try

        AddHandler DataGridView1.CellPainting, AddressOf DataGridView1_CellPainting
    End Sub
    Private Function GetStatusText(status As PrtgAPI.Status) As String
        Select Case status
            Case PrtgAPI.Status.Down
                Return "Down"
            Case PrtgAPI.Status.Warning
                Return "Warning"
            Case PrtgAPI.Status.Up
                Return "Up"
            Case Else
                Return "Unknown"
        End Select
    End Function

    Private Sub DataGridView1_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs)
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then ' Assuming the leftmost column index is 0
            Dim statusCell As DataGridViewCell = DataGridView1.Rows(e.RowIndex).Cells(0)
            If statusCell.Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(statusCell.Value.ToString()) Then
                Dim statusText As String = statusCell.Value.ToString().ToLower()
                If statusText = "warning" Then
                    e.CellStyle.ForeColor = Color.Black
                Else
                    e.CellStyle.Font = New Font(DataGridView1.Font, FontStyle.Bold)
                    e.CellStyle.ForeColor = Color.White
                End If
            End If
        End If
    End Sub


    Private Sub label1_Click(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Private Sub label2_Click(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Private Sub Label1_Click_1(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Form1_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


End Class
