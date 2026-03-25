Imports System.Data.SqlClient
Imports Pharmacy.GlobalFunctions

Public Class GlobalVariables




    'Τα ονόματα των DB
    Public Shared strDB1 As String = "Pharmacy2013C" ' το αντίγραφο του DB του Pharmakon που χρησιμοποίεί το πρόγραμμα μου
    Public Shared strDB1_Source As String = "Farnet_2024" ' το όνομα του παργματικού DB του Pharmakon 
    Public Shared strDB2 As String = "PharmacyCustomFiles" ' το DB με τα συνοδευτικά αρχεία του προγράμματος μου


    ' Το folder του SQL server που χρησιμοποιεί το πρόγραμμα μου ανάλογα με το που τρέχει (φαρμακείο pc1 και σπίτι)
    Public Shared strDBFolder As String = ""
    Public Shared strDBFolder_Home As String = "C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\"
    Public Shared strDBFolder_Home2 As String = "C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\"
    Public Shared strDBFolder_Farm As String = "C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\"
    Public Shared strDBFolder_Laptop As String = "C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\"
    Public Shared Pharmacy2013Server_Farm = "MSSQL$SQLEXPRESS" ' το SQL server του προγράμματος μου στο pc του φαρμακείου
    Public Shared Pharmacy2013Server_Home = "MSSQL$SQLEXPRESS" '  το SQL server του προγράμματος μου που χρησιμοποιώ στο σπίτι (services.msc --> properties)
    Public Shared strDBFolder2 As String = "Z:"
    Public Shared Pharmacy2013Folder As String ' Το folder τoυ DB του προγράμματος μου για χρήση σε όλο το πρόγραμμα
    Public Shared Pharmacy2013Server As String ' H μεταβλητή για το SQL server του προγράμματος μου για χρήση σε όλο το πρόγραμμα

    ' Το version του προγράμματος μου
Public Shared Version As String = "v7.9.7"
    ' Το folder του SQL server που χρησιμοποιεί το Pharmakon (το πραγματικό στο φαρμακείο, το δοκιμαστικό στο σπίτι)
    Public Shared strCSAfolder_Home As String = "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"  ' Home
    Public Shared strCSAfolder_Farm As String = "C:\FarmakoNet SQL\DATA\"  ' Pharmakeio
    Public Shared PharmaconServer_Farm = "MSSQL$CSASQL" ' το πραγματικό CSASQL server του Pharmakon στο pc του φαρμακείου (services.msc --> properties)
    Public Shared PharmaconServer_Home = "MSSQLSERVER" ' το δοκιμαστικό CSASQL server που χρησιμοποιώ στο σπίτι
    Public Shared FarNetFolder As String ' Το folder του SQL server που χρησιμοποιεί το Pharmakon για χρήση σε όλο το πρόγραμμα
    Public Shared PharmakonServer As String ' H μεταβλητή για το CSA SQL server για χρήση σε όλο το πρόγραμμα



    'Computer Names
    Public Shared strCompName_Farm1 As String = "DESKTOP-T7HMABG"
    Public Shared strCompName_Farm2 As String = "DESKTOP-T7HMABG"
    Public Shared strCompName_Home As String = "CRAZYDR"


    ' Δημιουργία του connection string για χρήση σε όλο το πρόγραμμα
    Public Shared connectionstring As String = ""
    Public Shared connectionstring_Home As String = "Data Source=CRAZYDR\SQLEXPRESS;Initial Catalog=" & strDB1 & ";User ID=farmakeio1;Password=niki"
    Public Shared connectionstring_Home2 As String = "Data Source=NIKOYLA-PC;Initial Catalog=" & strDB1 & ";User ID=sa;Password=teo"
    Public Shared connectionstring_Farm1 As String = "Data Source=DESKTOP-T7HMABG,1436\SQLEXPRESS;Initial Catalog=" & strDB1 & ";User ID=sa;Password=niki"
    Public Shared connectionstring_Farm As String = "Data Source=DESKTOP-T7HMABG\SQLEXPRESS;Initial Catalog=" & strDB1 & ";User ID=sa;Password=niki"
    Public Shared connectionstring_Laptop As String = "Data Source=CRAZYDR,1436\SQLEXPRESS;Initial Catalog=" & strDB1 & ";User ID=farmakeio1;Password=niki"
    Public Shared connectionstring_Farm2 As String = "Data Source=FARMAKEIO-PC\SQLEXPRESS;Initial Catalog=" & strDB1 & ";Integrated Security=True"
    Public Shared con As New SqlClient.SqlConnection(connectionstring)

    ' Forms
    Public Shared frmUFN As frmUpdateFarmNetDB

    ' Μεταβλητές αναγκαίες για να δουλέψει η FillDataGrid 
    Public Shared stringDTG As String = ""
    Public Shared strDTG_Desc, strDTG_Morf As String
    Public Shared stringDTG_Debts, stringDTG_DrugsOnLoan As String
    Public Shared cmdDTG As SqlCommand = Nothing
    Public Shared daDTG As SqlDataAdapter = Nothing
    Public Shared cbDTG As SqlCommandBuilder
    Public Shared dsDTG As DataSet = Nothing
    Public Shared dtDTG As DataTable = Nothing

    ' Ισοζύγια ανταλλαγών κατά ΦΠΑ
    Public Shared FPA65A, FPA13A, FPA23A, FPA0A As Decimal
    Public Shared FPA65B, FPA13B, FPA23B, FPA0B As Decimal
    Public Shared FPA65, FPA13, FPA23, FPA0 As Decimal
    Public Shared FPA65Prev, FPA13Prev, FPA23Prev, FPA0Prev As Decimal
    Public Shared FPA65Tot, FPA13Tot, FPA23Tot, FPA0Tot As Decimal



    ' Μεταβλητές BindingSource για τα DataGrid (τα ζητάει η FillDataGrid)
    Public Shared bsHairDies As New BindingSource  ' DataGrid HairDies του Customers Form 
    Public Shared bsDebts, bsDrugsOnLoan, bsPrescriptions As New BindingSource   ' DataGrid Debts του Customers Form 
    Public Shared bsCustomersEdit As New BindingSource    ' DataGrid Customers του CustomersEdit Form 
    Public Shared bsDrugs, bsPricesParadrugs, bsDrugs2 As New BindingSource    ' DataGrid Drugs του Drugs Form 
    Public Shared bsPrices As New BindingSource    ' DataGrid Prices του Drugs Form 
    Public Shared bsExpirations, bsExpProducts, bsExpDates, bsExpiringProducts, bsExpDates2, bsExpirationsNew As New BindingSource    ' DataGrid Expirations του Drugs Form 
    Public Shared bsEditDrugList As New BindingSource    ' DataGrid EditDrugsLst του DrugsLIstEdit Form 
    Public Shared bsProva As New BindingSource
    Public Shared bsZList As New BindingSource
    Public Shared bsDeliveriesList, bsExchangesGivenTo2 As New BindingSource
    Public Shared bsExchangeFrom2, bsTameiaAsked, bsPhones, bsTameiaGiven, bsCustomers, bsBarcodes, bsPrescriptionsExpired, bsEmporikes, bsAgoresSold, bsExchangesGivenTo, bsExchangesTakenFrom, bsExchangeTo2, bsDrugsExpir, bsDrugsPerDelivery, bsDrugReturns, bsDrugsNew, bsSuppliers, bsMorfesNew, bsExchangesTotal, bsExchangesFrom, bsExchangesTo As New BindingSource
    Public Shared selectedDrug_ApCode As String, selectedDrug_ApId As Integer = 0, selectedDrug_Xondr As Decimal
    Public Shared UsingBarcodeForm As String = ""
    Public Shared NewRowName, DatagridEdited As String
    Public Shared MultiplePrescriptionIndex As Integer = 1, MultiplePrescriptionCurrentIndex As Integer = 2


    Public Shared TG_MyDate As Date, TG_Description As String, TG_AmountPaid As Decimal, TG_CompletePayment As Boolean

    Public Shared varDebts1 As Date, varDebts2 As Decimal, varDebts3 As String
    Public Shared ChangedOrExists_Phones, ChangedOrNew_Exchanges, ChangedOrNew_Paradrugs As String
    Public Shared IsPrescriptionReadyToSave As Boolean = False
    Public Shared dtInitDate, dtEndDate As Nullable(Of DateTime)
    Public Shared PrescriptionInfoMode As String = ""
    Public Shared dirty As Boolean = False
    Public Shared DatabaseBackupTaken As Boolean = False
    Public Shared barcodeType As String = "barcode"
    Public Shared _loadingChooseFromCatalog As Boolean = False
    Public Shared _loadingParaDrugsList As Boolean = False
    Public Shared _loadingIncomplete As Boolean = False


    'Η μεταβλητή για την περίοδο σε μήνες που θα ερευνήσει για ληγμένα φάρμακα
    Public Shared months As Integer

    ' Καθορίζει το label που θα χρησιμοποιηθεί από το Timer
    Public Shared timerLabel As Label

    ' Καθορίζει μεταβλητές για να εναλλάσω τα Form
    'Public Shared formCustomerEdit As New frmCustomersEdit
    Public Shared ExchangesGivenOrTaken As String = ""

    ' Μεταβλητή που κρατάει την λίστα των κατηγοριών των Φαρμάκων
    Public Shared CategoryList() As String = GetDistinctContentsDBField("SELECT DISTINCT Category FROM PharmacyCustomFiles.dbo.Drugs WHERE Category is not null ORDER BY Category", "Category")

    Public Shared DrugListNew() As String = GetDistinctContentsDBField("SELECT distinct [AP_DESCRIPTION] FROM [APOTIKH] order by [AP_DESCRIPTION]", "AP_DESCRIPTION")

    Public Shared DrugList(), textPrevious As String
    Public Shared RowValuesOld(0 To 7), RowValuesNew(0 To 7) As String

    Public Shared previousValue As String
    Public Shared previousIndex As Integer
    'Public Shared ExpWithBarcode As Boolean

    'Version number
    Public Shared newValue, oldValue, myBarcode As String
    Public Shared expMonth, expYear, Qnt As Integer

    Public Shared lastRow, lastRowExpNew, lastColumn As Integer

    Public Shared InPerPharmacist, OutPerPharmacist, BalancePerPharmacist As Decimal

    Public Shared dtDrugsList As DataTable = FillDatatableWithComboBoxItems()

    Public Shared rowIndex, columnIndex, quantityExp As Integer
    Public Shared AreExchangedDrugsFrom As Boolean, errorTruncated As Integer = 0
    Public Shared valueSearched As String
    Public Shared changedIndex() As Array

    Public Shared SelectedDetailsApCode As String = "", SelectedDetailsDrugName As String = "", SelectedDetailsXondr As Decimal = 0
    Public Shared stringDTG2 As String
    Public Shared lstIndex As Double = -1, lstIndexService As Double = -1
    Public Shared newString, oldString As String
    Public Shared ParadrugCurrentRowChanged As Boolean
    Public Shared ParadrugRowEnter As Boolean = False




    'Function BuildConnString(ByVal stServer As String, ByVal strDatabase As String) As String

    '    Dim strTemp As String

    '    'strTemp = "Data Source=" & stServer & ";" & _
    '    '        "Initial Catalog='" & strDatabase & "';" & _
    '    '        "Integrated Security=True"

    '    strTemp = "Data Source=" & stServer & ";" & _
    '            "AttachDbFilename=" & strDatabase & ";" & _
    '    "Integrated Security=True;" & _
    '    "Connect Timeout=30;" & _
    '    "User Instance=True"

    '    Return strTemp




End Class









































































































































































































