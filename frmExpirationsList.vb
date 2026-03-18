Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Windows.Forms.Timer
Imports System.Threading.Timer
Imports System.Globalization
Imports System.Threading


Public Class frmExpirationsList

    Private Sub frmExpirationsList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = New Point(frmCustomers.Location.X + 230, frmCustomers.Location.Y + 167)

        GetProductsList()
        GetExpirationsList()

    End Sub

    Private Sub txtSearchExpirations_TextChanged(sender As Object, e As EventArgs) Handles txtSearchExpirations.TextChanged
        GetProductsList()
        GetExpirationsList()
    End Sub

    Private Sub GetProductsList()
        Dim myTot As Integer

        'stringDTG = "SELECT distinct REPLACE([AP_DESCRIPTION],'''''','''') " & _
        '                   "FROM [dbo].[APOTIKH] " & _
        '                   "WHERE AP_DESCRIPTION like '%" & txtSearchExpirations.Text & "%' " & _
        '                   "ORDER BY REPLACE([AP_DESCRIPTION],'''''','''')"

        stringDTG = "SELECT distinct ProductName, Category, AP_ID, AP_CODE, ParadrugId " & _
                           "FROM PharmacyCustomFiles.[dbo].[Expirations] " & _
                           "WHERE ProductName like '%" & txtSearchExpirations.Text & "%' " & _
                           "ORDER BY ProductName"


        myTot = FillDatagrid(dgvProducts, bsExpProducts, {"Προιόν", "Κατηγορία"}, {150, 100}, {"", ""}, {})

        ' Ενημερωτικό μύνημα
        Select Case myTot
            Case 0

                rtxtExpirationsMessage.Text = "Δεν βρέθηκαν προιόντα"

            Case 1

                rtxtExpirationsMessage.Text = "Βρέθηκε 1 προιόν"

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό
                HightlightInRichTextBox(rtxtExpirationsMessage, {"1"})

            Case Is > 1

                rtxtExpirationsMessage.Text = "Βρέθηκαν " & myTot.ToString & " προιόντα"

                ' Αλλάζει με κόκκινο χρώμα τον αριθμό
                HightlightInRichTextBox(rtxtExpirationsMessage, {myTot.ToString})

        End Select

    End Sub

    Private Sub GetExpirationsList()
        Dim myTot As Integer
        Dim productName As String = ""

        Try
            productName = dgvProducts.SelectedRows(0).Cells(0).Value
        Catch ex As Exception
        End Try

        stringDTG = "SELECT Month, Year " & _
                           "FROM PharmacyCustomFiles.[dbo].[Expirations] " & _
                           "WHERE ProductName = '" & productName & "' and (Month is not null) and (Year is not null) " & _
                           "ORDER BY Year, Month"


        myTot = FillDatagrid(dgvExpirations, bsExpDates, {"Μήνας", "Έτος"}, {50, 50}, {"", ""}, {})

        'Πάει στο τελευταίο record του datagrid
        Try
            Dim myRow As Integer = Me.dgvExpirations.RowCount - 1
            Me.dgvExpirations.FirstDisplayedScrollingRowIndex = myRow
            Me.dgvExpirations.CurrentCell = Me.dgvExpirations.Rows(myRow).Cells(0)
        Catch ex As Exception
        End Try

    End Sub

    Private Sub dgvProducts_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProducts.CellClick
        GetExpirationsList()
        txtLastExpDate.Text = GetLastExpDate()
    End Sub

    Private Sub btnPairing_Click(sender As Object, e As EventArgs) Handles btnPairing.Click
        PairExpirationToParadrug()
        GetProductsList()
        Me.Close()
    End Sub

    Private Sub PairExpirationToParadrug()
        Dim category As String
        Dim OldProductName As String = dgvProducts.SelectedRows(0).Cells(0).Value

        If frmCustomers.rbParadrugs.Checked = True Then
            category = "ΠΑΡΑΦΑΡΜΑΚΑ"
        Else
            category = "ΦΑΡΜΑΚΑ"
        End If

        Dim insertData As String = "UPDATE PharmacyCustomFiles.[dbo].[Expirations] " & _
                                "SET [ProductName] = @NewProductName,  [Category] =@Category,  [AP_ID] = @AP_ID, [AP_CODE] = @AP_CODE ,[ParadrugId] = @ParadrugId " & _
                                "WHERE ProductName = @OldProductName"

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            'insertData = "INSERT INTO PharmacyCustomFiles.dbo.PricesParadrugs " & _
            '                                        "([Name] ,[Xondr] ,[Lian] ,[Notes] ,[AP_Code], [AP_ID], [Barcode]) " & _
            '                                        "VALUES (@Name, @Xondr, @Lian, @Notes, @AP_Code, @AP_ID, @Barcode )"

            Dim cmd As New SqlCommand(insertData, con)
            'MsgBox(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(6).Value & " - " & frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(4).Value & " - " & frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(5).Value)

            'If frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(6).Value Is DBNull.Value Then
            '    ap_id = 0
            'Else
            '    ap_id = CType(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(6).Value, Integer)
            'End If

            If frmCustomers.rbParadrugs.Checked = True Then
                cmd.Parameters.AddWithValue("@NewProductName", frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(0).Value)
                cmd.Parameters.AddWithValue("@Category", category)
                cmd.Parameters.AddWithValue("@AP_ID", If(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(6).Value Is DBNull.Value, DBNull.Value, CType(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(6).Value, Decimal)))
                cmd.Parameters.AddWithValue("@AP_CODE", If(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(4).Value Is DBNull.Value, DBNull.Value, CType(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(4).Value, Decimal)))
                cmd.Parameters.AddWithValue("@ParadrugId", frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(5).Value)
                cmd.Parameters.AddWithValue("@OldProductName", OldProductName)
            ElseIf frmCustomers.rbDrugs.Checked = True Then
                cmd.Parameters.AddWithValue("@NewProductName", frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(0).Value & " (" & frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(1).Value & ")")
                cmd.Parameters.AddWithValue("@Category", category)
                cmd.Parameters.AddWithValue("@AP_ID", If(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(5).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@AP_CODE", If(frmCustomers.dgvPricesParadrugs.SelectedRows(0).Cells(4).Value, DBNull.Value))
                cmd.Parameters.AddWithValue("@ParadrugId", DBNull.Value)
                cmd.Parameters.AddWithValue("@OldProductName", OldProductName)
            End If

            cmd.ExecuteNonQuery()

            'Ανανεώνει το Last Update
            'DisplayLastUpdate()

            '' Αν προσθέσαμε μια νέα έγγραφή ανανεώνει το datagrid
            'If ChangedOrNew_Paradrugs = "NewRow" Then
            '    'GetPriceParaDrugs()
            '    txtSearchPricesParadrugs.Text = dgvPricesParadrugs.Rows(i).Cells(0).Value
            'End If

            'End If

        End Using

    End Sub


    Private Function GetLastExpDate() As String

        Dim LastExpDate As String = ""

        Dim insertData As String = "SELECT concat([Month], '/', [Year]), datediff(d, concat([Year],'/',[Month], '/01'), getdate()) as mydatediff " & _
                                    "FROM [PharmacyCustomFiles].[dbo].[Expirations] " & _
                                    "WHERE (Month is not null) and (Year is not null) and ProductName = @ProductName " & _
                                    "ORDER BY mydatediff"

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            Dim cmd As New SqlCommand(insertData, con)
            cmd.Parameters.AddWithValue("@ProductName", dgvProducts.SelectedRows(0).Cells(0).Value)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                myReader.Read()
                LastExpDate = myReader(0)
            End If
        End Using

        Return LastExpDate

    End Function

    Private Sub dgvProducts_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProducts.CellContentClick

    End Sub

    Private Sub dgvProducts_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvProducts.KeyDown
        GetExpirationsList()
        txtLastExpDate.Text = GetLastExpDate()
    End Sub

    Private Sub dgvProducts_KeyUp(sender As Object, e As KeyEventArgs) Handles dgvProducts.KeyUp
        GetExpirationsList()
        txtLastExpDate.Text = GetLastExpDate()
    End Sub

    Private Sub dgvProducts_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProducts.RowEnter
       
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs)

    End Sub
End Class