Imports PrtgAPI
Imports PrtgAPI.Parameters
Imports PrtgAPI.Request
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Private prtgClient As PrtgClient
    Public cancellationToken As CancellationToken = CancellationToken.None

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
                    If sensor.Status = PrtgAPI.Status.Down Then
                        downCount += 1
                    ElseIf sensor.Status = PrtgAPI.Status.Warning Then
                        warningCount += 1
                    ElseIf sensor.Status = PrtgAPI.Status.Up Then
                        upCount += 1
                    End If
                Next
            Next

            Label2.Text = String.Format("Sensors: {0} Up, {1} Warning, {2} Down", upCount, warningCount, downCount)
        Catch ex As Exception
            ' Code to execute if there is an exception
            Console.WriteLine("Error connecting to PRTG server: " & ex.Message)
        End Try
    End Sub

    Private Sub label1_Click(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Private Sub label2_Click(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Private Sub Label1_Click_1(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class
