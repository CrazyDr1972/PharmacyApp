Imports Pharmacy.GlobalFunctions
Imports Pharmacy.GlobalVariables
Imports System.Data.SqlClient
Imports System.Windows.Forms.Timer
Imports System.Threading.Timer
Imports System.Globalization
Imports System.Threading

Public Class frmExchangesTotalsPerPharmacist

    Private Sub frmExchangesTotalsPerPharmacist_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        frmCustomers.Enabled = True
    End Sub

    Private Sub frmExchangesTotalsPerPharmacist_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim Pharmacists() As String = {"Λίντα", "Έφη", "Μαριέτα", "Κωνσταντίνος", "Αθηνα-Ηρώ", "Στέλλα", "Πόπη", "Μάγδα"}
        Dim index As Integer = 0, total As Decimal = 0, masterBalance As Decimal = 0

        For Each cntrl As Control In Me.Controls
            If TypeOf cntrl Is Label Then
                'MsgBox(cntrl.Name)
                Dim partName As String = cntrl.Name.Substring(0, 6)
                If partName = "lblIso" And cntrl.Name <> "lblIsoTot" Then
                    index = cntrl.Name.Substring(6, 1)
                    'total = GetExchangesTotalBalancePerPharmacist(Pharmacists(index - 1)).ToString("F2")
                    total = GetExchangesTotalBalancePerPharmacist(Pharmacists(index - 1)).ToString("###,###.00")
                    cntrl.Text = Math.Abs(total)
                    If total > 0 Then
                        cntrl.ForeColor = Color.Green
                    ElseIf total < 0 Then
                        cntrl.ForeColor = Color.Red
                    Else
                        cntrl.ForeColor = Color.Black
                    End If

                    masterBalance += total

                End If
            End If
        Next

        lblIsoTot.Text = Math.Abs(masterBalance).ToString("c")
        If masterBalance > 0 Then
            lblIsoTot.ForeColor = Color.Green
        ElseIf masterBalance < 0 Then
            lblIsoTot.ForeColor = Color.Red
        Else
            lblIsoTot.ForeColor = Color.Black
        End If

    End Sub

    Private Function GetExchangesTotalBalancePerPharmacist(ByVal pharmacist As String) As Decimal
        Dim FromDate, ToDate, stringGiven, stringTaken As String

        FromDate = frmCustomers.dtpFromDate.Value
        ToDate = frmCustomers.dtpToDate.Value

        stringGiven = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " & _
                                    "WHERE Exch ='" & pharmacist & "' AND FromTo=0 " 

        stringTaken = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " & _
                                   "WHERE Exch ='" & pharmacist & "' AND FromTo=1 " 

        'stringGiven = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " & _
        '                            "WHERE Exch ='" & pharmacist & "' AND FromTo=0 AND " & _
        '                                "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') <=0 AND " & _
        '                                "Datediff(day, MyDate, '" & CType(ToDate, Date).ToString("yyyy-MM-dd") & "') >=0 "

        'stringTaken = "SELECT Qnt, Xondr From PharmacyCustomFiles.dbo.Exchanges " & _
        '                           "WHERE Exch ='" & pharmacist & "' AND FromTo=1 AND " & _
        '                                "Datediff(day, MyDate, '" & CType(FromDate, Date).ToString("yyyy-MM-dd") & "') <=0 AND " & _
        '                                "Datediff(day, MyDate, '" & CType(ToDate, Date).ToString("yyyy-MM-dd") & "') >=0 "


        ' Γράφει μια ενημερωση στο Label κάτω από το datagrid
        ' ανάλογα με τον αριθμό των πελατών που βρέθηκαν

        Dim totalItemsGiven As Integer = CalculateSums(stringGiven, "Qnt"), totalItemsTaken As Integer = CalculateSums(stringTaken, "Qnt")
        Dim totalSumGiven As Decimal = CalculateSums(stringGiven, "Xondr"), totalSumTaken As Decimal = CalculateSums(stringTaken, "Xondr")

        Dim OutTot As Decimal = totalSumGiven
        Dim InTot As Decimal = totalSumTaken

        Return (OutTot - InTot)

    End Function

End Class