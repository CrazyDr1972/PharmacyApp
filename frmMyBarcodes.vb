Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Windows.Forms.Timer
Imports System.Threading.Timer
Imports System.Globalization
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Configuration
Imports Excel = Microsoft.Office.Interop.Excel

Public Class frmMyBarcodes

    Private Sub frmMyBarcodes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetBarcodesList()
    End Sub

    Public Sub GetBarcodesList()

        Dim sumCustomers As Integer = 0

        If rbByName.Checked = True Then
            stringDTG = "SELECT APOTIKH.AP_DESCRIPTION + ' ' + APOTIKH.AP_MORFI AS DESCRIPTION, count(APOTIKH.AP_DESCRIPTION) as Mycount  " & _
                   "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID inner JOIN " & _
                       "PharmacyCustomFiles.dbo.MyBarcodes ON APOTIKH_BARCODES.BRAP_AP_BARCODE = PharmacyCustomFiles.dbo.MyBarcodes.BarcodeDrug " & _
                    "WHERE APOTIKH.AP_DESCRIPTION + ' ' + APOTIKH.AP_MORFI LIKE '%" & txtSearchBarcode.Text & "%' AND PharmacyCustomFiles.dbo.MyBarcodes.DateOut IS NULL " & _
                    "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI " & _
                   "ORDER BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, count(APOTIKH.AP_DESCRIPTION) desc "
        ElseIf rbByBarcode.Checked = True Then

            stringDTG = "SELECT APOTIKH.AP_DESCRIPTION + ' ' + APOTIKH.AP_MORFI AS DESCRIPTION, count(APOTIKH.AP_DESCRIPTION) as Mycount  " & _
                       "FROM APOTIKH left JOIN APOTIKH_BARCODES ON APOTIKH.AP_ID = APOTIKH_BARCODES.BRAP_AP_ID inner JOIN " & _
                           "PharmacyCustomFiles.dbo.MyBarcodes ON APOTIKH_BARCODES.BRAP_AP_BARCODE = PharmacyCustomFiles.dbo.MyBarcodes.BarcodeDrug " & _
                        "WHERE PharmacyCustomFiles.dbo.MyBarcodes.BarcodeDrug LIKE '%" & txtSearchBarcode.Text & "%' AND PharmacyCustomFiles.dbo.MyBarcodes.DateOut IS NULL " & _
                        "GROUP BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI " & _
                      "ORDER BY APOTIKH.AP_DESCRIPTION, APOTIKH.AP_MORFI, count(APOTIKH.AP_DESCRIPTION) desc "
        End If

        sumCustomers = FillDatagrid(dgvBarcodes, bsBarcodes, {"Προιόν", "Τμχ", "Id"}, {280, 40, 30}, {"", "", ""}, {"Id"}, "", True)


        ' Γράφει μια ενημερωση στο Label κάτω από το ListBox
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν
        lblTotalDrugs.Text = "Σύνολο σκευασμάτων: " & sumCustomers

    End Sub

    Private Sub txtSearchBarcode_TextChanged(sender As Object, e As EventArgs) Handles txtSearchBarcode.TextChanged
        If IsNumeric(txtSearchBarcode.Text) = True Then
            rbByBarcode.Checked = True
        Else
            rbByName.Checked = True
        End If

        GetBarcodesList()

    End Sub

    Private Sub btnAddBarcode_Click(sender As Object, e As EventArgs) Handles btnAddBarcode.Click
        'Καταχωρεί την χρήση του barcode reader για καταχωρηση κουπονιών σε ασθενή
        UsingBarcodeForm = "MyBarcodesIn"

        ' Ανοίγει το form για να καταχωρήσουμε barcode
        frmAddDrugOnLoan.Show()
    End Sub

    Public Sub AddBarcode(ByVal BarcodeBox As String, ByVal BarcodeDrug As String, ByVal Price As Decimal)
        Dim insertData As String = ""
        Dim BarcodeExists As Integer = 0


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            BarcodeExists = CheckIfRecordExists(BarcodeDrug, BarcodeBox)

            If BarcodeExists = 0 Then

                insertData = "INSERT INTO PharmacyCustomFiles.dbo.MyBarcodes " & _
                            "([BarcodeDrug], [BarcodeBox], [Money], [DateIn]) VALUES (@BarcodeDrug, @BarcodeBox, @Price, @DateIn)"

                Dim cmd As New SqlCommand(insertData, con)

                cmd.Parameters.AddWithValue("@BarcodeDrug", BarcodeDrug)
                cmd.Parameters.AddWithValue("@BarcodeBox", BarcodeBox)
                cmd.Parameters.AddWithValue("@Price", Price)
                cmd.Parameters.AddWithValue("@DateIn", Now())
                'cmd.Parameters.AddWithValue("@DateOut", DateOut)

                cmd.ExecuteNonQuery()

            ElseIf BarcodeExists <> 0 Then

                MsgBox("Αυτο το κουπόνι το έχετε ήδη καταχωρήσει!")

            End If

            ''Ανανεώνει το Last Update
            '        DisplayLastUpdate()

            '        'Αν κάνουμε update στην τιμή
            '        If column = 1 Then
            '            Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
            '            ' Ανανεώνει τη λίστα των πελατων
            '            GetCustomersList()
            '            Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
            '            dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)

            '        End If

        End Using

    End Sub


    Public Sub UseBarcode(ByVal BarcodeBox As String, ByVal BarcodeDrug As String)
        Dim insertData As String = ""
        Dim BarcodeExists As Integer = 0


        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            BarcodeExists = CheckIfRecordExists(BarcodeDrug, BarcodeBox)

            If BarcodeExists <> 0 Then

                insertData = "UPDATE PharmacyCustomFiles.dbo.MyBarcodes " & _
                            "SET [DateOut] = @DateOut " & _
                            "WHERE Id = @Id"

                Dim cmd As New SqlCommand(insertData, con)
                cmd.Parameters.AddWithValue("@DateOut", Now())
                cmd.Parameters.AddWithValue("@Id", BarcodeExists)

                cmd.ExecuteNonQuery()

            ElseIf BarcodeExists = 0 Then

                MsgBox("Δεν υπάρχει τέτοιο κουπόνι!")

            End If

            ''Ανανεώνει το Last Update
            '        DisplayLastUpdate()

            '        'Αν κάνουμε update στην τιμή
            '        If column = 1 Then
            '            Dim SelectedCustomer As String = dgvCustomers.SelectedRows(0).Cells(0).Value
            '            ' Ανανεώνει τη λίστα των πελατων
            '            GetCustomersList()
            '            Dim rowIndex As Integer = SearchDatagrid(dgvCustomers, SelectedCustomer)
            '            dgvCustomers.CurrentCell = dgvCustomers.Rows(rowIndex).Cells(0)

            '        End If

        End Using

    End Sub


    Public Function CheckIfRecordExists(ByVal MyBarcDrug As String, ByVal MyBarcBox As String) As Integer
        Dim sql As String = ""
        Dim Id As Integer = 0

        Using con As SqlConnection = New System.Data.SqlClient.SqlConnection(connectionstring)
            con.Open()

            sql = "SELECT Id, BarcodeDrug, BarcodeBox FROM PharmacyCustomFiles.dbo.MyBarcodes " & _
                        "WHERE BarcodeDrug = @MyBarcDrug AND BarcodeBox = @MyBarcBox AND DateOut IS NULL"

            Dim cmd As New SqlCommand(sql, con)

            'Dim myId As Integer = dgvPricesParadrugs.Rows(0).Cells(5).Value

            'MsgBox(index & " -> " & dgvPricesParadrugs.Rows(index).Cells(1).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(2).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(3).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(4).Value & "-" & dgvPricesParadrugs.Rows(index).Cells(5).Value & "-")

            cmd.Parameters.AddWithValue("@MyBarcDrug", MyBarcDrug)
            cmd.Parameters.AddWithValue("@MyBarcBox", MyBarcBox)

            Dim myReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

            If myReader.HasRows Then
                Do While myReader.Read()
                    Id = myReader(0)
                    Return Id
                Loop
                Return True
            End If

            Return 0

        End Using

    End Function


    Private Sub btnUseBarcode_Click(sender As Object, e As EventArgs) Handles btnUseBarcode.Click
        'Καταχωρεί την χρήση του barcode reader για καταχωρηση κουπονιών σε ασθενή
        UsingBarcodeForm = "MyBarcodesOut"

        ' Ανοίγει το form για να καταχωρήσουμε barcode
        frmAddDrugOnLoan.Show()
    End Sub
End Class