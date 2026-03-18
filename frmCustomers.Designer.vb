<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCustomers
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCustomers))
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.tmrFlashLabel = New System.Windows.Forms.Timer(Me.components)
        Me.tbcMain = New System.Windows.Forms.TabControl()
        Me.tbpExchanges = New System.Windows.Forms.TabPage()
        Me.btnAddManualTakenFrom = New System.Windows.Forms.Button()
        Me.btnAddTakenFrom = New System.Windows.Forms.Button()
        Me.btnAddManualGivenTo = New System.Windows.Forms.Button()
        Me.btnAddGivenTo = New System.Windows.Forms.Button()
        Me.rtxtPreviousFPA = New System.Windows.Forms.RichTextBox()
        Me.rtxtTakenFrom2 = New System.Windows.Forms.RichTextBox()
        Me.rtxtGivenTo2 = New System.Windows.Forms.RichTextBox()
        Me.GroupBox11 = New System.Windows.Forms.GroupBox()
        Me.lblFPAInfo = New System.Windows.Forms.Label()
        Me.btnExchangesBalancePerPharmacist = New System.Windows.Forms.Button()
        Me.chkAutoInsertName = New System.Windows.Forms.CheckBox()
        Me.Button12 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.btnMyBarcodes = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.rtxtTotalFPA = New System.Windows.Forms.RichTextBox()
        Me.lblExchangesBalance2 = New System.Windows.Forms.Label()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.lblPreviousBalance = New System.Windows.Forms.Label()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.rtxtCurrentFPA = New System.Windows.Forms.RichTextBox()
        Me.lblCurrFPA23 = New System.Windows.Forms.Label()
        Me.lblCurrFPA13 = New System.Windows.Forms.Label()
        Me.lblCurrFPA65 = New System.Windows.Forms.Label()
        Me.lblCurrentBalance = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.dtpFromDate = New System.Windows.Forms.DateTimePicker()
        Me.dtpToDate = New System.Windows.Forms.DateTimePicker()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.btnEditExchangers = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblLastUpdateExchanges = New System.Windows.Forms.Label()
        Me.lblPreviousBalanceTakenFrom = New System.Windows.Forms.Label()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.lblPreviousBalanceGivenTo = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.btnDeleteTakenFrom = New System.Windows.Forms.Button()
        Me.btnDeleteGivenTo = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cbExchangers = New System.Windows.Forms.ComboBox()
        Me.rtxtTakenFrom = New System.Windows.Forms.RichTextBox()
        Me.rtxtGivenTo = New System.Windows.Forms.RichTextBox()
        Me.dgvTakenFrom = New System.Windows.Forms.DataGridView()
        Me.dgvGivenTo = New System.Windows.Forms.DataGridView()
        Me.tbpCustomerDebts = New System.Windows.Forms.TabPage()
        Me.lblNewRow_Cust = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.lblNewRecord_Debts = New System.Windows.Forms.Label()
        Me.lblDirty_Debts = New System.Windows.Forms.Label()
        Me.lblTotalDebtPerCustomer = New System.Windows.Forms.Label()
        Me.lblLastUpdateCustomers = New System.Windows.Forms.Label()
        Me.cboSearchCustomers = New System.Windows.Forms.ComboBox()
        Me.grpDrugsOnLoan = New System.Windows.Forms.GroupBox()
        Me.btnAddDrug = New System.Windows.Forms.Button()
        Me.lblLastUpdateDrugsOnLoan = New System.Windows.Forms.Label()
        Me.lblSumDrugsOnLoan = New System.Windows.Forms.Label()
        Me.lblSumDrugsOnLoanLabel = New System.Windows.Forms.Label()
        Me.lblCustWithoutDrugsOnLoan = New System.Windows.Forms.Label()
        Me.btnDeleteDrugOnLoan = New System.Windows.Forms.Button()
        Me.dgvDrugsOnLoan = New System.Windows.Forms.DataGridView()
        Me.btnClearSearch = New System.Windows.Forms.Button()
        Me.grpCustHairDies = New System.Windows.Forms.GroupBox()
        Me.lblLastUpdateHairDies = New System.Windows.Forms.Label()
        Me.lblCustWithNoHairdies = New System.Windows.Forms.Label()
        Me.btnDeleteHairdies = New System.Windows.Forms.Button()
        Me.dgvHairdiesList = New System.Windows.Forms.DataGridView()
        Me.grpCustDebts = New System.Windows.Forms.GroupBox()
        Me.btnAddDebt = New System.Windows.Forms.Button()
        Me.lblScanHint = New System.Windows.Forms.Label()
        Me.btnPrintDebtsList = New System.Windows.Forms.Button()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.lblLastUpdateDebts = New System.Windows.Forms.Label()
        Me.lblCustWithNoDebts = New System.Windows.Forms.Label()
        Me.btnPayDebts = New System.Windows.Forms.Button()
        Me.lblTotalCustomerDebt = New System.Windows.Forms.Label()
        Me.lblTotalDebtLabel = New System.Windows.Forms.Label()
        Me.btnDeleteDebts = New System.Windows.Forms.Button()
        Me.dgvDebtsList = New System.Windows.Forms.DataGridView()
        Me.txtNoCustomers = New System.Windows.Forms.TextBox()
        Me.btnDeleteCustomer = New System.Windows.Forms.Button()
        Me.rtxtCustomersMessage = New System.Windows.Forms.RichTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtSearchCustomer = New System.Windows.Forms.TextBox()
        Me.dgvCustomers = New System.Windows.Forms.DataGridView()
        Me.grpPrescriptions = New System.Windows.Forms.GroupBox()
        Me.lblTotPrescriptions = New System.Windows.Forms.Label()
        Me.chkSelectAll = New System.Windows.Forms.CheckBox()
        Me.lblLastUpdatePrescriptions = New System.Windows.Forms.Label()
        Me.lblCustWithPrescriptions = New System.Windows.Forms.Label()
        Me.btnDeletePrescriptions = New System.Windows.Forms.Button()
        Me.dgvPrescriptions = New System.Windows.Forms.DataGridView()
        Me.tbpPricesParadrugs = New System.Windows.Forms.TabPage()
        Me.btnImportExcel = New System.Windows.Forms.Button()
        Me.Button11 = New System.Windows.Forms.Button()
        Me.lblNewRecordAdded = New System.Windows.Forms.Label()
        Me.txtRowChanged3 = New System.Windows.Forms.TextBox()
        Me.txtRowChanged2 = New System.Windows.Forms.TextBox()
        Me.Button10 = New System.Windows.Forms.Button()
        Me.Button9 = New System.Windows.Forms.Button()
        Me.txtRowChanged = New System.Windows.Forms.TextBox()
        Me.chkManualBarcode = New System.Windows.Forms.CheckBox()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.chkPairing = New System.Windows.Forms.CheckBox()
        Me.btnExpiringDrugs = New System.Windows.Forms.Button()
        Me.txtSearchPricesParadrugs = New System.Windows.Forms.TextBox()
        Me.grpExpirationList = New System.Windows.Forms.GroupBox()
        Me.lblParadrugName = New System.Windows.Forms.Label()
        Me.lblNewRecord_Exp = New System.Windows.Forms.Label()
        Me.lblDirtyState_Exp = New System.Windows.Forms.Label()
        Me.btnDeleteExpiration = New System.Windows.Forms.Button()
        Me.txtNoExpirations = New System.Windows.Forms.TextBox()
        Me.dgvExpirations = New System.Windows.Forms.DataGridView()
        Me.btnExpirations = New System.Windows.Forms.Button()
        Me.grpDrugsOrParadrugs = New System.Windows.Forms.GroupBox()
        Me.rbParadrugs = New System.Windows.Forms.RadioButton()
        Me.rbDrugs = New System.Windows.Forms.RadioButton()
        Me.grpCalculateLianiki = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cboFPA_Paradrugs = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.txtProfit_Paradrugs = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtTotalPrice_Paradrugs = New System.Windows.Forms.TextBox()
        Me.grpLastUpdateParadrugs = New System.Windows.Forms.GroupBox()
        Me.lblLastUpdateParadrugs = New System.Windows.Forms.Label()
        Me.grpSearchParadrugOptions = New System.Windows.Forms.GroupBox()
        Me.rbByQRcode = New System.Windows.Forms.RadioButton()
        Me.rbByBarcode = New System.Windows.Forms.RadioButton()
        Me.rbByName = New System.Windows.Forms.RadioButton()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.btnDeletePriceParadrugs = New System.Windows.Forms.Button()
        Me.rtxtPricesParadrugs = New System.Windows.Forms.RichTextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dgvPricesParadrugs = New System.Windows.Forms.DataGridView()
        Me.tbpPhones = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.lblLastUpdatePhones = New System.Windows.Forms.Label()
        Me.cboPhoneCatalog = New System.Windows.Forms.ComboBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtSearchPhones = New System.Windows.Forms.TextBox()
        Me.btnDeletePhones = New System.Windows.Forms.Button()
        Me.rtxtPhones = New System.Windows.Forms.RichTextBox()
        Me.dgvPhones = New System.Windows.Forms.DataGridView()
        Me.tbpBackup = New System.Windows.Forms.TabPage()
        Me.lblAdminInfo = New System.Windows.Forms.Label()
        Me.Button15 = New System.Windows.Forms.Button()
        Me.Button14 = New System.Windows.Forms.Button()
        Me.Button13 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnStopService = New System.Windows.Forms.Button()
        Me.btnStartService = New System.Windows.Forms.Button()
        Me.txtServiceName = New System.Windows.Forms.TextBox()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.btnCoppyAppStation1 = New System.Windows.Forms.Button()
        Me.lblPCName = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblLastUpdated = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbOnlyVisualBasic = New System.Windows.Forms.RadioButton()
        Me.rbEverything = New System.Windows.Forms.RadioButton()
        Me.rbOnlyDatabases = New System.Windows.Forms.RadioButton()
        Me.btnUpdatePharmacy2013C = New System.Windows.Forms.Button()
        Me.lblLastBuilded = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.rbWhereNikoyla = New System.Windows.Forms.RadioButton()
        Me.rbWhereSaloni = New System.Windows.Forms.RadioButton()
        Me.rbWhereFarm2 = New System.Windows.Forms.RadioButton()
        Me.rbWhereLaptop = New System.Windows.Forms.RadioButton()
        Me.rbWhereSpiti = New System.Windows.Forms.RadioButton()
        Me.rbWhereFarm1 = New System.Windows.Forms.RadioButton()
        Me.lstMessage = New System.Windows.Forms.ListBox()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.btnBackupRestore = New System.Windows.Forms.Button()
        Me.grpBackupDestination = New System.Windows.Forms.GroupBox()
        Me.rbPC2Usb = New System.Windows.Forms.RadioButton()
        Me.rbUsb2PC = New System.Windows.Forms.RadioButton()
        Me.grpBackupSource = New System.Windows.Forms.GroupBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txtSQLServer_Pharmacy2013 = New System.Windows.Forms.TextBox()
        Me.txtSQLServer_Pharmakon = New System.Windows.Forms.TextBox()
        Me.txtSourceFarmnet_mdf = New System.Windows.Forms.TextBox()
        Me.txtPCName = New System.Windows.Forms.TextBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.rbDBStation2 = New System.Windows.Forms.RadioButton()
        Me.rbDBStation1 = New System.Windows.Forms.RadioButton()
        Me.txtConnectionString = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblLastUpdatedDB2 = New System.Windows.Forms.Label()
        Me.lblLastUpdatedDB1 = New System.Windows.Forms.Label()
        Me.lblLastMod_DB2 = New System.Windows.Forms.Label()
        Me.lblLabel = New System.Windows.Forms.Label()
        Me.txtSourceFarmnetDB = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.btnOpenDestinationFolder = New System.Windows.Forms.Button()
        Me.btnOpenFolderVS = New System.Windows.Forms.Button()
        Me.txtDestinationDrive = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtSourceFolderVS = New System.Windows.Forms.TextBox()
        Me.btnOpenFolderDB = New System.Windows.Forms.Button()
        Me.txtDB2 = New System.Windows.Forms.TextBox()
        Me.txtSourceDB = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtDB1 = New System.Windows.Forms.TextBox()
        Me.tbpAgoresSold = New System.Windows.Forms.TabPage()
        Me.rtxtAgoresSoldMessage = New System.Windows.Forms.RichTextBox()
        Me.cbAgoresOrSold = New System.Windows.Forms.ComboBox()
        Me.btnAgoresSoldDeleteRecord = New System.Windows.Forms.Button()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.txtAgoresSoldSearch = New System.Windows.Forms.TextBox()
        Me.dgvAgoresSold = New System.Windows.Forms.DataGridView()
        Me.tbpTameia = New System.Windows.Forms.TabPage()
        Me.cboTameia = New System.Windows.Forms.ComboBox()
        Me.rtxtTameiaAsked2 = New System.Windows.Forms.RichTextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtSearchTameia = New System.Windows.Forms.TextBox()
        Me.btnDeleteTameiaGiven = New System.Windows.Forms.Button()
        Me.rtxtTameiaGiven = New System.Windows.Forms.RichTextBox()
        Me.dgvTameiaGiven = New System.Windows.Forms.DataGridView()
        Me.btnDeleteTameiaAsked = New System.Windows.Forms.Button()
        Me.rtxtTameiaAsked = New System.Windows.Forms.RichTextBox()
        Me.dgvTameiaAsked = New System.Windows.Forms.DataGridView()
        Me.tmrRerunDatagridV = New System.Windows.Forms.Timer(Me.components)
        Me.tmrExpirations = New System.Windows.Forms.Timer(Me.components)
        Me.dgvExchangeFrom2 = New System.Windows.Forms.DataGridView()
        Me.rtxtExchangeFrom2 = New System.Windows.Forms.RichTextBox()
        Me.btnEditExchangeFrom = New System.Windows.Forms.Button()
        Me.btnSaveExchangeFrom = New System.Windows.Forms.Button()
        Me.btnDeleteExchangeFrom = New System.Windows.Forms.Button()
        Me.cboMyPharmacist = New System.Windows.Forms.ComboBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cboIntervall = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtBalance = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.dtpExchangesNew = New System.Windows.Forms.DateTimePicker()
        Me.btnEditDrugList2 = New System.Windows.Forms.Button()
        Me.chkWithExpir = New System.Windows.Forms.CheckBox()
        Me.dgvExchangeTo2 = New System.Windows.Forms.DataGridView()
        Me.rtxtExchangeTo2 = New System.Windows.Forms.RichTextBox()
        Me.btnEditExchangeTo = New System.Windows.Forms.Button()
        Me.btnSaveExchangeTo = New System.Windows.Forms.Button()
        Me.btnDeleteExchangeTo = New System.Windows.Forms.Button()
        Me.tmrSearchCustomers = New System.Windows.Forms.Timer(Me.components)
        Me.tmrExpirationKeystrokes = New System.Windows.Forms.Timer(Me.components)
        Me.tbcMain.SuspendLayout()
        Me.tbpExchanges.SuspendLayout()
        Me.GroupBox11.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.dgvTakenFrom, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvGivenTo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpCustomerDebts.SuspendLayout()
        Me.grpDrugsOnLoan.SuspendLayout()
        CType(Me.dgvDrugsOnLoan, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpCustHairDies.SuspendLayout()
        CType(Me.dgvHairdiesList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpCustDebts.SuspendLayout()
        CType(Me.dgvDebtsList, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvCustomers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpPrescriptions.SuspendLayout()
        CType(Me.dgvPrescriptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpPricesParadrugs.SuspendLayout()
        Me.grpExpirationList.SuspendLayout()
        CType(Me.dgvExpirations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpDrugsOrParadrugs.SuspendLayout()
        Me.grpCalculateLianiki.SuspendLayout()
        Me.grpLastUpdateParadrugs.SuspendLayout()
        Me.grpSearchParadrugOptions.SuspendLayout()
        CType(Me.dgvPricesParadrugs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpPhones.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.dgvPhones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpBackup.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.grpBackupDestination.SuspendLayout()
        Me.grpBackupSource.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.tbpAgoresSold.SuspendLayout()
        CType(Me.dgvAgoresSold, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpTameia.SuspendLayout()
        CType(Me.dgvTameiaGiven, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvTameiaAsked, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvExchangeFrom2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvExchangeTo2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrFlashLabel
        '
        Me.tmrFlashLabel.Interval = 1000
        '
        'tbcMain
        '
        Me.tbcMain.Controls.Add(Me.tbpExchanges)
        Me.tbcMain.Controls.Add(Me.tbpCustomerDebts)
        Me.tbcMain.Controls.Add(Me.tbpPricesParadrugs)
        Me.tbcMain.Controls.Add(Me.tbpPhones)
        Me.tbcMain.Controls.Add(Me.tbpBackup)
        Me.tbcMain.Controls.Add(Me.tbpAgoresSold)
        Me.tbcMain.Controls.Add(Me.tbpTameia)
        Me.tbcMain.Location = New System.Drawing.Point(3, 3)
        Me.tbcMain.Name = "tbcMain"
        Me.tbcMain.SelectedIndex = 1
        Me.tbcMain.Size = New System.Drawing.Size(1004, 722)
        Me.tbcMain.TabIndex = 22
        '
        'tbpExchanges
        '
        Me.tbpExchanges.Controls.Add(Me.btnAddManualTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.btnAddTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.btnAddManualGivenTo)
        Me.tbpExchanges.Controls.Add(Me.btnAddGivenTo)
        Me.tbpExchanges.Controls.Add(Me.rtxtPreviousFPA)
        Me.tbpExchanges.Controls.Add(Me.rtxtTakenFrom2)
        Me.tbpExchanges.Controls.Add(Me.rtxtGivenTo2)
        Me.tbpExchanges.Controls.Add(Me.GroupBox11)
        Me.tbpExchanges.Controls.Add(Me.GroupBox9)
        Me.tbpExchanges.Controls.Add(Me.GroupBox8)
        Me.tbpExchanges.Controls.Add(Me.GroupBox7)
        Me.tbpExchanges.Controls.Add(Me.btnEditExchangers)
        Me.tbpExchanges.Controls.Add(Me.GroupBox2)
        Me.tbpExchanges.Controls.Add(Me.lblPreviousBalanceTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.Label29)
        Me.tbpExchanges.Controls.Add(Me.lblPreviousBalanceGivenTo)
        Me.tbpExchanges.Controls.Add(Me.Label27)
        Me.tbpExchanges.Controls.Add(Me.btnDeleteTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.btnDeleteGivenTo)
        Me.tbpExchanges.Controls.Add(Me.Label10)
        Me.tbpExchanges.Controls.Add(Me.cbExchangers)
        Me.tbpExchanges.Controls.Add(Me.rtxtTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.rtxtGivenTo)
        Me.tbpExchanges.Controls.Add(Me.dgvTakenFrom)
        Me.tbpExchanges.Controls.Add(Me.dgvGivenTo)
        Me.tbpExchanges.Location = New System.Drawing.Point(4, 22)
        Me.tbpExchanges.Name = "tbpExchanges"
        Me.tbpExchanges.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpExchanges.Size = New System.Drawing.Size(996, 696)
        Me.tbpExchanges.TabIndex = 9
        Me.tbpExchanges.Text = "Ανταλλαγές"
        Me.tbpExchanges.UseVisualStyleBackColor = True
        '
        'btnAddManualTakenFrom
        '
        Me.btnAddManualTakenFrom.Location = New System.Drawing.Point(803, 579)
        Me.btnAddManualTakenFrom.Name = "btnAddManualTakenFrom"
        Me.btnAddManualTakenFrom.Size = New System.Drawing.Size(39, 23)
        Me.btnAddManualTakenFrom.TabIndex = 80
        Me.btnAddManualTakenFrom.Text = "+ (M)"
        Me.btnAddManualTakenFrom.UseVisualStyleBackColor = True
        '
        'btnAddTakenFrom
        '
        Me.btnAddTakenFrom.Location = New System.Drawing.Point(766, 579)
        Me.btnAddTakenFrom.Name = "btnAddTakenFrom"
        Me.btnAddTakenFrom.Size = New System.Drawing.Size(31, 23)
        Me.btnAddTakenFrom.TabIndex = 81
        Me.btnAddTakenFrom.Text = "+"
        Me.btnAddTakenFrom.UseVisualStyleBackColor = True
        '
        'btnAddManualGivenTo
        '
        Me.btnAddManualGivenTo.Location = New System.Drawing.Point(331, 577)
        Me.btnAddManualGivenTo.Name = "btnAddManualGivenTo"
        Me.btnAddManualGivenTo.Size = New System.Drawing.Size(39, 23)
        Me.btnAddManualGivenTo.TabIndex = 74
        Me.btnAddManualGivenTo.Text = "+ (M)"
        Me.btnAddManualGivenTo.UseVisualStyleBackColor = True
        '
        'btnAddGivenTo
        '
        Me.btnAddGivenTo.Location = New System.Drawing.Point(294, 577)
        Me.btnAddGivenTo.Name = "btnAddGivenTo"
        Me.btnAddGivenTo.Size = New System.Drawing.Size(31, 23)
        Me.btnAddGivenTo.TabIndex = 79
        Me.btnAddGivenTo.Text = "+"
        Me.btnAddGivenTo.UseVisualStyleBackColor = True
        '
        'rtxtPreviousFPA
        '
        Me.rtxtPreviousFPA.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtPreviousFPA.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtPreviousFPA.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtPreviousFPA.Location = New System.Drawing.Point(412, 67)
        Me.rtxtPreviousFPA.Multiline = False
        Me.rtxtPreviousFPA.Name = "rtxtPreviousFPA"
        Me.rtxtPreviousFPA.ReadOnly = True
        Me.rtxtPreviousFPA.Size = New System.Drawing.Size(240, 18)
        Me.rtxtPreviousFPA.TabIndex = 78
        Me.rtxtPreviousFPA.Text = ""
        Me.rtxtPreviousFPA.WordWrap = False
        '
        'rtxtTakenFrom2
        '
        Me.rtxtTakenFrom2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTakenFrom2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTakenFrom2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTakenFrom2.Location = New System.Drawing.Point(488, 134)
        Me.rtxtTakenFrom2.Name = "rtxtTakenFrom2"
        Me.rtxtTakenFrom2.ReadOnly = True
        Me.rtxtTakenFrom2.Size = New System.Drawing.Size(425, 22)
        Me.rtxtTakenFrom2.TabIndex = 76
        Me.rtxtTakenFrom2.Text = ""
        '
        'rtxtGivenTo2
        '
        Me.rtxtGivenTo2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtGivenTo2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtGivenTo2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtGivenTo2.Location = New System.Drawing.Point(15, 134)
        Me.rtxtGivenTo2.Name = "rtxtGivenTo2"
        Me.rtxtGivenTo2.ReadOnly = True
        Me.rtxtGivenTo2.Size = New System.Drawing.Size(425, 22)
        Me.rtxtGivenTo2.TabIndex = 75
        Me.rtxtGivenTo2.Text = ""
        '
        'GroupBox11
        '
        Me.GroupBox11.Controls.Add(Me.lblFPAInfo)
        Me.GroupBox11.Controls.Add(Me.btnExchangesBalancePerPharmacist)
        Me.GroupBox11.Controls.Add(Me.chkAutoInsertName)
        Me.GroupBox11.Controls.Add(Me.Button12)
        Me.GroupBox11.Controls.Add(Me.Button6)
        Me.GroupBox11.Controls.Add(Me.btnMyBarcodes)
        Me.GroupBox11.Controls.Add(Me.Button7)
        Me.GroupBox11.Location = New System.Drawing.Point(203, 611)
        Me.GroupBox11.Name = "GroupBox11"
        Me.GroupBox11.Size = New System.Drawing.Size(609, 82)
        Me.GroupBox11.TabIndex = 74
        Me.GroupBox11.TabStop = False
        '
        'lblFPAInfo
        '
        Me.lblFPAInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblFPAInfo.ForeColor = System.Drawing.Color.Red
        Me.lblFPAInfo.Location = New System.Drawing.Point(494, 53)
        Me.lblFPAInfo.Name = "lblFPAInfo"
        Me.lblFPAInfo.Size = New System.Drawing.Size(96, 12)
        Me.lblFPAInfo.TabIndex = 73
        Me.lblFPAInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnExchangesBalancePerPharmacist
        '
        Me.btnExchangesBalancePerPharmacist.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnExchangesBalancePerPharmacist.Location = New System.Drawing.Point(15, 19)
        Me.btnExchangesBalancePerPharmacist.Name = "btnExchangesBalancePerPharmacist"
        Me.btnExchangesBalancePerPharmacist.Size = New System.Drawing.Size(95, 23)
        Me.btnExchangesBalancePerPharmacist.TabIndex = 50
        Me.btnExchangesBalancePerPharmacist.Text = "Συγκεντρωτική"
        Me.btnExchangesBalancePerPharmacist.UseVisualStyleBackColor = True
        '
        'chkAutoInsertName
        '
        Me.chkAutoInsertName.Checked = True
        Me.chkAutoInsertName.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAutoInsertName.Location = New System.Drawing.Point(16, 44)
        Me.chkAutoInsertName.Name = "chkAutoInsertName"
        Me.chkAutoInsertName.Size = New System.Drawing.Size(94, 30)
        Me.chkAutoInsertName.TabIndex = 45
        Me.chkAutoInsertName.Text = "Αυτόματη  Καταχώρηση"
        Me.chkAutoInsertName.UseVisualStyleBackColor = True
        '
        'Button12
        '
        Me.Button12.Enabled = False
        Me.Button12.Location = New System.Drawing.Point(491, 12)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(99, 36)
        Me.Button12.TabIndex = 72
        Me.Button12.Text = "ΦΠΑ στις ανταλλαγές"
        Me.Button12.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(136, 19)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(75, 36)
        Me.Button6.TabIndex = 65
        Me.Button6.Text = "Δημιουργία Excel"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'btnMyBarcodes
        '
        Me.btnMyBarcodes.Location = New System.Drawing.Point(241, 19)
        Me.btnMyBarcodes.Name = "btnMyBarcodes"
        Me.btnMyBarcodes.Size = New System.Drawing.Size(95, 23)
        Me.btnMyBarcodes.TabIndex = 66
        Me.btnMyBarcodes.Text = "My Precious!"
        Me.btnMyBarcodes.UseVisualStyleBackColor = True
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(367, 19)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(95, 38)
        Me.Button7.TabIndex = 67
        Me.Button7.Text = "Υπολογισμός Χονδρικής ΙΦΕΤ"
        Me.Button7.UseVisualStyleBackColor = True
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.rtxtTotalFPA)
        Me.GroupBox9.Controls.Add(Me.lblExchangesBalance2)
        Me.GroupBox9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.GroupBox9.Location = New System.Drawing.Point(670, 11)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(257, 79)
        Me.GroupBox9.TabIndex = 71
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "ΙΣΟΖΥΓΙΟ"
        '
        'rtxtTotalFPA
        '
        Me.rtxtTotalFPA.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTotalFPA.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTotalFPA.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTotalFPA.Location = New System.Drawing.Point(6, 56)
        Me.rtxtTotalFPA.Multiline = False
        Me.rtxtTotalFPA.Name = "rtxtTotalFPA"
        Me.rtxtTotalFPA.ReadOnly = True
        Me.rtxtTotalFPA.Size = New System.Drawing.Size(240, 18)
        Me.rtxtTotalFPA.TabIndex = 79
        Me.rtxtTotalFPA.Text = ""
        Me.rtxtTotalFPA.WordWrap = False
        '
        'lblExchangesBalance2
        '
        Me.lblExchangesBalance2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblExchangesBalance2.Location = New System.Drawing.Point(6, 16)
        Me.lblExchangesBalance2.Name = "lblExchangesBalance2"
        Me.lblExchangesBalance2.Size = New System.Drawing.Size(240, 37)
        Me.lblExchangesBalance2.TabIndex = 49
        Me.lblExchangesBalance2.Text = "0,00"
        Me.lblExchangesBalance2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.lblPreviousBalance)
        Me.GroupBox8.Location = New System.Drawing.Point(406, 11)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(252, 79)
        Me.GroupBox8.TabIndex = 70
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Προηγούμενο Υπόλοιπο"
        '
        'lblPreviousBalance
        '
        Me.lblPreviousBalance.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblPreviousBalance.Location = New System.Drawing.Point(6, 20)
        Me.lblPreviousBalance.Name = "lblPreviousBalance"
        Me.lblPreviousBalance.Size = New System.Drawing.Size(236, 33)
        Me.lblPreviousBalance.TabIndex = 53
        Me.lblPreviousBalance.Text = "0,00"
        Me.lblPreviousBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.rtxtCurrentFPA)
        Me.GroupBox7.Controls.Add(Me.lblCurrFPA23)
        Me.GroupBox7.Controls.Add(Me.lblCurrFPA13)
        Me.GroupBox7.Controls.Add(Me.lblCurrFPA65)
        Me.GroupBox7.Controls.Add(Me.lblCurrentBalance)
        Me.GroupBox7.Controls.Add(Me.Label17)
        Me.GroupBox7.Controls.Add(Me.dtpFromDate)
        Me.GroupBox7.Controls.Add(Me.dtpToDate)
        Me.GroupBox7.Controls.Add(Me.Label15)
        Me.GroupBox7.Location = New System.Drawing.Point(16, 11)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(378, 79)
        Me.GroupBox7.TabIndex = 69
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Σύνολο Τρέχουσας Περιόδου"
        '
        'rtxtCurrentFPA
        '
        Me.rtxtCurrentFPA.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtCurrentFPA.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtCurrentFPA.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtCurrentFPA.Location = New System.Drawing.Point(131, 52)
        Me.rtxtCurrentFPA.Multiline = False
        Me.rtxtCurrentFPA.Name = "rtxtCurrentFPA"
        Me.rtxtCurrentFPA.ReadOnly = True
        Me.rtxtCurrentFPA.Size = New System.Drawing.Size(240, 18)
        Me.rtxtCurrentFPA.TabIndex = 77
        Me.rtxtCurrentFPA.Text = ""
        Me.rtxtCurrentFPA.WordWrap = False
        '
        'lblCurrFPA23
        '
        Me.lblCurrFPA23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCurrFPA23.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lblCurrFPA23.Location = New System.Drawing.Point(321, 55)
        Me.lblCurrFPA23.Name = "lblCurrFPA23"
        Me.lblCurrFPA23.Size = New System.Drawing.Size(78, 15)
        Me.lblCurrFPA23.TabIndex = 67
        Me.lblCurrFPA23.Text = "0,00"
        Me.lblCurrFPA23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCurrFPA13
        '
        Me.lblCurrFPA13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCurrFPA13.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lblCurrFPA13.Location = New System.Drawing.Point(227, 55)
        Me.lblCurrFPA13.Name = "lblCurrFPA13"
        Me.lblCurrFPA13.Size = New System.Drawing.Size(88, 15)
        Me.lblCurrFPA13.TabIndex = 66
        Me.lblCurrFPA13.Text = "0,00"
        Me.lblCurrFPA13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCurrFPA65
        '
        Me.lblCurrFPA65.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCurrFPA65.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.lblCurrFPA65.Location = New System.Drawing.Point(128, 55)
        Me.lblCurrFPA65.Name = "lblCurrFPA65"
        Me.lblCurrFPA65.Size = New System.Drawing.Size(93, 15)
        Me.lblCurrFPA65.TabIndex = 65
        Me.lblCurrFPA65.Text = "0,00"
        Me.lblCurrFPA65.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCurrentBalance
        '
        Me.lblCurrentBalance.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCurrentBalance.Location = New System.Drawing.Point(131, 20)
        Me.lblCurrentBalance.Name = "lblCurrentBalance"
        Me.lblCurrentBalance.Size = New System.Drawing.Size(240, 24)
        Me.lblCurrentBalance.TabIndex = 53
        Me.lblCurrentBalance.Text = "0,00"
        Me.lblCurrentBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label17.Location = New System.Drawing.Point(8, 26)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(26, 13)
        Me.Label17.TabIndex = 64
        Me.Label17.Text = "Από"
        '
        'dtpFromDate
        '
        Me.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpFromDate.Location = New System.Drawing.Point(34, 24)
        Me.dtpFromDate.Name = "dtpFromDate"
        Me.dtpFromDate.Size = New System.Drawing.Size(88, 20)
        Me.dtpFromDate.TabIndex = 62
        Me.dtpFromDate.Value = New Date(2013, 10, 3, 0, 0, 0, 0)
        '
        'dtpToDate
        '
        Me.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpToDate.Location = New System.Drawing.Point(34, 49)
        Me.dtpToDate.Name = "dtpToDate"
        Me.dtpToDate.Size = New System.Drawing.Size(88, 20)
        Me.dtpToDate.TabIndex = 63
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label15.Location = New System.Drawing.Point(8, 52)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(27, 13)
        Me.Label15.TabIndex = 61
        Me.Label15.Text = "εως"
        '
        'btnEditExchangers
        '
        Me.btnEditExchangers.Location = New System.Drawing.Point(137, 636)
        Me.btnEditExchangers.Name = "btnEditExchangers"
        Me.btnEditExchangers.Size = New System.Drawing.Size(35, 25)
        Me.btnEditExchangers.TabIndex = 68
        Me.btnEditExchangers.Text = "Edit"
        Me.btnEditExchangers.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblLastUpdateExchanges)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(826, 663)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(105, 39)
        Me.GroupBox2.TabIndex = 58
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Τελ. Ανανέωση"
        '
        'lblLastUpdateExchanges
        '
        Me.lblLastUpdateExchanges.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateExchanges.Location = New System.Drawing.Point(6, 18)
        Me.lblLastUpdateExchanges.Name = "lblLastUpdateExchanges"
        Me.lblLastUpdateExchanges.Size = New System.Drawing.Size(96, 12)
        Me.lblLastUpdateExchanges.TabIndex = 55
        Me.lblLastUpdateExchanges.Text = "??"
        Me.lblLastUpdateExchanges.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPreviousBalanceTakenFrom
        '
        Me.lblPreviousBalanceTakenFrom.AutoSize = True
        Me.lblPreviousBalanceTakenFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblPreviousBalanceTakenFrom.Location = New System.Drawing.Point(626, 577)
        Me.lblPreviousBalanceTakenFrom.Name = "lblPreviousBalanceTakenFrom"
        Me.lblPreviousBalanceTakenFrom.Size = New System.Drawing.Size(36, 17)
        Me.lblPreviousBalanceTakenFrom.TabIndex = 57
        Me.lblPreviousBalanceTakenFrom.Text = "0,00"
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label29.Location = New System.Drawing.Point(497, 579)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(132, 13)
        Me.Label29.TabIndex = 56
        Me.Label29.Text = "Σύνολο Προηγ.Περιόδου: "
        '
        'lblPreviousBalanceGivenTo
        '
        Me.lblPreviousBalanceGivenTo.AutoSize = True
        Me.lblPreviousBalanceGivenTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblPreviousBalanceGivenTo.Location = New System.Drawing.Point(150, 577)
        Me.lblPreviousBalanceGivenTo.Name = "lblPreviousBalanceGivenTo"
        Me.lblPreviousBalanceGivenTo.Size = New System.Drawing.Size(36, 17)
        Me.lblPreviousBalanceGivenTo.TabIndex = 55
        Me.lblPreviousBalanceGivenTo.Text = "0,00"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label27.Location = New System.Drawing.Point(20, 579)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(132, 13)
        Me.Label27.TabIndex = 54
        Me.Label27.Text = "Σύνολο Προηγ.Περιόδου: "
        '
        'btnDeleteTakenFrom
        '
        Me.btnDeleteTakenFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteTakenFrom.Location = New System.Drawing.Point(848, 579)
        Me.btnDeleteTakenFrom.Name = "btnDeleteTakenFrom"
        Me.btnDeleteTakenFrom.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteTakenFrom.TabIndex = 47
        Me.btnDeleteTakenFrom.Text = "Διαγραφή"
        Me.btnDeleteTakenFrom.UseVisualStyleBackColor = True
        '
        'btnDeleteGivenTo
        '
        Me.btnDeleteGivenTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteGivenTo.Location = New System.Drawing.Point(376, 577)
        Me.btnDeleteGivenTo.Name = "btnDeleteGivenTo"
        Me.btnDeleteGivenTo.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteGivenTo.TabIndex = 46
        Me.btnDeleteGivenTo.Text = "Διαγραφή"
        Me.btnDeleteGivenTo.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label10.Location = New System.Drawing.Point(20, 621)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(73, 13)
        Me.Label10.TabIndex = 44
        Me.Label10.Text = "Συνάδελφος:"
        '
        'cbExchangers
        '
        Me.cbExchangers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cbExchangers.FormattingEnabled = True
        Me.cbExchangers.Location = New System.Drawing.Point(23, 637)
        Me.cbExchangers.Name = "cbExchangers"
        Me.cbExchangers.Size = New System.Drawing.Size(109, 23)
        Me.cbExchangers.TabIndex = 43
        '
        'rtxtTakenFrom
        '
        Me.rtxtTakenFrom.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTakenFrom.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTakenFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTakenFrom.Location = New System.Drawing.Point(488, 109)
        Me.rtxtTakenFrom.Name = "rtxtTakenFrom"
        Me.rtxtTakenFrom.ReadOnly = True
        Me.rtxtTakenFrom.Size = New System.Drawing.Size(425, 22)
        Me.rtxtTakenFrom.TabIndex = 42
        Me.rtxtTakenFrom.Text = ""
        '
        'rtxtGivenTo
        '
        Me.rtxtGivenTo.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtGivenTo.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtGivenTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtGivenTo.Location = New System.Drawing.Point(16, 109)
        Me.rtxtGivenTo.Name = "rtxtGivenTo"
        Me.rtxtGivenTo.ReadOnly = True
        Me.rtxtGivenTo.Size = New System.Drawing.Size(425, 22)
        Me.rtxtGivenTo.TabIndex = 41
        Me.rtxtGivenTo.Text = ""
        '
        'dgvTakenFrom
        '
        Me.dgvTakenFrom.AllowUserToDeleteRows = False
        Me.dgvTakenFrom.AllowUserToResizeColumns = False
        Me.dgvTakenFrom.AllowUserToResizeRows = False
        Me.dgvTakenFrom.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTakenFrom.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvTakenFrom.Location = New System.Drawing.Point(488, 162)
        Me.dgvTakenFrom.Name = "dgvTakenFrom"
        Me.dgvTakenFrom.RowHeadersVisible = False
        Me.dgvTakenFrom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvTakenFrom.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTakenFrom.ShowEditingIcon = False
        Me.dgvTakenFrom.Size = New System.Drawing.Size(425, 411)
        Me.dgvTakenFrom.TabIndex = 40
        '
        'dgvGivenTo
        '
        Me.dgvGivenTo.AllowUserToDeleteRows = False
        Me.dgvGivenTo.AllowUserToResizeColumns = False
        Me.dgvGivenTo.AllowUserToResizeRows = False
        Me.dgvGivenTo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvGivenTo.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvGivenTo.Location = New System.Drawing.Point(16, 162)
        Me.dgvGivenTo.Name = "dgvGivenTo"
        Me.dgvGivenTo.RowHeadersVisible = False
        Me.dgvGivenTo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvGivenTo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvGivenTo.ShowEditingIcon = False
        Me.dgvGivenTo.Size = New System.Drawing.Size(425, 411)
        Me.dgvGivenTo.TabIndex = 39
        '
        'tbpCustomerDebts
        '
        Me.tbpCustomerDebts.Controls.Add(Me.lblNewRow_Cust)
        Me.tbpCustomerDebts.Controls.Add(Me.Button1)
        Me.tbpCustomerDebts.Controls.Add(Me.lblNewRecord_Debts)
        Me.tbpCustomerDebts.Controls.Add(Me.lblDirty_Debts)
        Me.tbpCustomerDebts.Controls.Add(Me.lblTotalDebtPerCustomer)
        Me.tbpCustomerDebts.Controls.Add(Me.lblLastUpdateCustomers)
        Me.tbpCustomerDebts.Controls.Add(Me.cboSearchCustomers)
        Me.tbpCustomerDebts.Controls.Add(Me.grpDrugsOnLoan)
        Me.tbpCustomerDebts.Controls.Add(Me.btnClearSearch)
        Me.tbpCustomerDebts.Controls.Add(Me.grpCustHairDies)
        Me.tbpCustomerDebts.Controls.Add(Me.grpCustDebts)
        Me.tbpCustomerDebts.Controls.Add(Me.btnDeleteCustomer)
        Me.tbpCustomerDebts.Controls.Add(Me.rtxtCustomersMessage)
        Me.tbpCustomerDebts.Controls.Add(Me.Label3)
        Me.tbpCustomerDebts.Controls.Add(Me.txtSearchCustomer)
        Me.tbpCustomerDebts.Controls.Add(Me.dgvCustomers)
        Me.tbpCustomerDebts.Controls.Add(Me.grpPrescriptions)
        Me.tbpCustomerDebts.Location = New System.Drawing.Point(4, 22)
        Me.tbpCustomerDebts.Name = "tbpCustomerDebts"
        Me.tbpCustomerDebts.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpCustomerDebts.Size = New System.Drawing.Size(996, 696)
        Me.tbpCustomerDebts.TabIndex = 11
        Me.tbpCustomerDebts.Text = "Χρέη - Βαφές"
        Me.tbpCustomerDebts.UseVisualStyleBackColor = True
        '
        'lblNewRow_Cust
        '
        Me.lblNewRow_Cust.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblNewRow_Cust.ForeColor = System.Drawing.Color.Red
        Me.lblNewRow_Cust.Location = New System.Drawing.Point(21, 598)
        Me.lblNewRow_Cust.Name = "lblNewRow_Cust"
        Me.lblNewRow_Cust.Size = New System.Drawing.Size(67, 12)
        Me.lblNewRow_Cust.TabIndex = 89
        Me.lblNewRow_Cust.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button1.Location = New System.Drawing.Point(747, 343)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(190, 33)
        Me.Button1.TabIndex = 88
        Me.Button1.Text = "Λίστα ληξιπροθεσμων συνταγών"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'lblNewRecord_Debts
        '
        Me.lblNewRecord_Debts.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblNewRecord_Debts.ForeColor = System.Drawing.Color.Red
        Me.lblNewRecord_Debts.Location = New System.Drawing.Point(340, 3)
        Me.lblNewRecord_Debts.Name = "lblNewRecord_Debts"
        Me.lblNewRecord_Debts.Size = New System.Drawing.Size(67, 12)
        Me.lblNewRecord_Debts.TabIndex = 64
        Me.lblNewRecord_Debts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDirty_Debts
        '
        Me.lblDirty_Debts.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblDirty_Debts.Location = New System.Drawing.Point(423, 3)
        Me.lblDirty_Debts.Name = "lblDirty_Debts"
        Me.lblDirty_Debts.Size = New System.Drawing.Size(67, 12)
        Me.lblDirty_Debts.TabIndex = 63
        Me.lblDirty_Debts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTotalDebtPerCustomer
        '
        Me.lblTotalDebtPerCustomer.AutoSize = True
        Me.lblTotalDebtPerCustomer.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblTotalDebtPerCustomer.Location = New System.Drawing.Point(174, 32)
        Me.lblTotalDebtPerCustomer.Name = "lblTotalDebtPerCustomer"
        Me.lblTotalDebtPerCustomer.Size = New System.Drawing.Size(17, 17)
        Me.lblTotalDebtPerCustomer.TabIndex = 73
        Me.lblTotalDebtPerCustomer.Text = "0"
        '
        'lblLastUpdateCustomers
        '
        Me.lblLastUpdateCustomers.AutoSize = True
        Me.lblLastUpdateCustomers.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateCustomers.Location = New System.Drawing.Point(18, 580)
        Me.lblLastUpdateCustomers.Name = "lblLastUpdateCustomers"
        Me.lblLastUpdateCustomers.Size = New System.Drawing.Size(53, 12)
        Me.lblLastUpdateCustomers.TabIndex = 71
        Me.lblLastUpdateCustomers.Text = "Τελ.Αναν.:"
        '
        'cboSearchCustomers
        '
        Me.cboSearchCustomers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cboSearchCustomers.FormattingEnabled = True
        Me.cboSearchCustomers.Items.AddRange(New Object() {"Όλους", "Χρέη από συναλλαγές", "Οφειλές σε πελάτες", "Χρέη από φάρμακα", "Συνταγές προς εκτέλεση", "Βαφές"})
        Me.cboSearchCustomers.Location = New System.Drawing.Point(15, 55)
        Me.cboSearchCustomers.Name = "cboSearchCustomers"
        Me.cboSearchCustomers.Size = New System.Drawing.Size(187, 23)
        Me.cboSearchCustomers.TabIndex = 70
        Me.cboSearchCustomers.Text = "Όλους"
        '
        'grpDrugsOnLoan
        '
        Me.grpDrugsOnLoan.Controls.Add(Me.btnAddDrug)
        Me.grpDrugsOnLoan.Controls.Add(Me.lblLastUpdateDrugsOnLoan)
        Me.grpDrugsOnLoan.Controls.Add(Me.lblSumDrugsOnLoan)
        Me.grpDrugsOnLoan.Controls.Add(Me.lblSumDrugsOnLoanLabel)
        Me.grpDrugsOnLoan.Controls.Add(Me.lblCustWithoutDrugsOnLoan)
        Me.grpDrugsOnLoan.Controls.Add(Me.btnDeleteDrugOnLoan)
        Me.grpDrugsOnLoan.Controls.Add(Me.dgvDrugsOnLoan)
        Me.grpDrugsOnLoan.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpDrugsOnLoan.Location = New System.Drawing.Point(259, 343)
        Me.grpDrugsOnLoan.Name = "grpDrugsOnLoan"
        Me.grpDrugsOnLoan.Size = New System.Drawing.Size(361, 260)
        Me.grpDrugsOnLoan.TabIndex = 69
        Me.grpDrugsOnLoan.TabStop = False
        '
        'btnAddDrug
        '
        Me.btnAddDrug.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnAddDrug.Location = New System.Drawing.Point(247, 232)
        Me.btnAddDrug.Name = "btnAddDrug"
        Me.btnAddDrug.Size = New System.Drawing.Size(23, 23)
        Me.btnAddDrug.TabIndex = 69
        Me.btnAddDrug.Text = "+"
        Me.btnAddDrug.UseVisualStyleBackColor = True
        '
        'lblLastUpdateDrugsOnLoan
        '
        Me.lblLastUpdateDrugsOnLoan.AutoSize = True
        Me.lblLastUpdateDrugsOnLoan.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateDrugsOnLoan.Location = New System.Drawing.Point(12, 229)
        Me.lblLastUpdateDrugsOnLoan.Name = "lblLastUpdateDrugsOnLoan"
        Me.lblLastUpdateDrugsOnLoan.Size = New System.Drawing.Size(53, 12)
        Me.lblLastUpdateDrugsOnLoan.TabIndex = 57
        Me.lblLastUpdateDrugsOnLoan.Text = "Τελ.Αναν.:"
        '
        'lblSumDrugsOnLoan
        '
        Me.lblSumDrugsOnLoan.AutoSize = True
        Me.lblSumDrugsOnLoan.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblSumDrugsOnLoan.Location = New System.Drawing.Point(131, 30)
        Me.lblSumDrugsOnLoan.Name = "lblSumDrugsOnLoan"
        Me.lblSumDrugsOnLoan.Size = New System.Drawing.Size(17, 17)
        Me.lblSumDrugsOnLoan.TabIndex = 56
        Me.lblSumDrugsOnLoan.Text = "0"
        '
        'lblSumDrugsOnLoanLabel
        '
        Me.lblSumDrugsOnLoanLabel.AutoSize = True
        Me.lblSumDrugsOnLoanLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblSumDrugsOnLoanLabel.Location = New System.Drawing.Point(6, 32)
        Me.lblSumDrugsOnLoanLabel.Name = "lblSumDrugsOnLoanLabel"
        Me.lblSumDrugsOnLoanLabel.Size = New System.Drawing.Size(129, 13)
        Me.lblSumDrugsOnLoanLabel.TabIndex = 55
        Me.lblSumDrugsOnLoanLabel.Text = "ΧΡΕH απο 25 Φάρμακα: "
        '
        'lblCustWithoutDrugsOnLoan
        '
        Me.lblCustWithoutDrugsOnLoan.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCustWithoutDrugsOnLoan.Location = New System.Drawing.Point(2, 106)
        Me.lblCustWithoutDrugsOnLoan.Name = "lblCustWithoutDrugsOnLoan"
        Me.lblCustWithoutDrugsOnLoan.Size = New System.Drawing.Size(356, 47)
        Me.lblCustWithoutDrugsOnLoan.TabIndex = 54
        Me.lblCustWithoutDrugsOnLoan.Text = "Ο επιλεγμένος πελάτης δεν χρωστάει φάρμακα"
        Me.lblCustWithoutDrugsOnLoan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblCustWithoutDrugsOnLoan.Visible = False
        '
        'btnDeleteDrugOnLoan
        '
        Me.btnDeleteDrugOnLoan.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteDrugOnLoan.Location = New System.Drawing.Point(276, 232)
        Me.btnDeleteDrugOnLoan.Name = "btnDeleteDrugOnLoan"
        Me.btnDeleteDrugOnLoan.Size = New System.Drawing.Size(75, 23)
        Me.btnDeleteDrugOnLoan.TabIndex = 45
        Me.btnDeleteDrugOnLoan.Text = "Διαγραφή"
        Me.btnDeleteDrugOnLoan.UseVisualStyleBackColor = True
        '
        'dgvDrugsOnLoan
        '
        Me.dgvDrugsOnLoan.AllowUserToAddRows = False
        Me.dgvDrugsOnLoan.AllowUserToResizeColumns = False
        Me.dgvDrugsOnLoan.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvDrugsOnLoan.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvDrugsOnLoan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvDrugsOnLoan.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgvDrugsOnLoan.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvDrugsOnLoan.Location = New System.Drawing.Point(9, 58)
        Me.dgvDrugsOnLoan.Name = "dgvDrugsOnLoan"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvDrugsOnLoan.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.dgvDrugsOnLoan.RowHeadersVisible = False
        Me.dgvDrugsOnLoan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvDrugsOnLoan.Size = New System.Drawing.Size(342, 171)
        Me.dgvDrugsOnLoan.TabIndex = 11
        Me.dgvDrugsOnLoan.Visible = False
        '
        'btnClearSearch
        '
        Me.btnClearSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnClearSearch.Image = CType(resources.GetObject("btnClearSearch.Image"), System.Drawing.Image)
        Me.btnClearSearch.Location = New System.Drawing.Point(145, 29)
        Me.btnClearSearch.Name = "btnClearSearch"
        Me.btnClearSearch.Size = New System.Drawing.Size(23, 23)
        Me.btnClearSearch.TabIndex = 68
        Me.btnClearSearch.UseVisualStyleBackColor = True
        '
        'grpCustHairDies
        '
        Me.grpCustHairDies.Controls.Add(Me.lblLastUpdateHairDies)
        Me.grpCustHairDies.Controls.Add(Me.lblCustWithNoHairdies)
        Me.grpCustHairDies.Controls.Add(Me.btnDeleteHairdies)
        Me.grpCustHairDies.Controls.Add(Me.dgvHairdiesList)
        Me.grpCustHairDies.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpCustHairDies.Location = New System.Drawing.Point(730, 413)
        Me.grpCustHairDies.Name = "grpCustHairDies"
        Me.grpCustHairDies.Size = New System.Drawing.Size(210, 190)
        Me.grpCustHairDies.TabIndex = 46
        Me.grpCustHairDies.TabStop = False
        '
        'lblLastUpdateHairDies
        '
        Me.lblLastUpdateHairDies.AutoSize = True
        Me.lblLastUpdateHairDies.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateHairDies.Location = New System.Drawing.Point(15, 154)
        Me.lblLastUpdateHairDies.Name = "lblLastUpdateHairDies"
        Me.lblLastUpdateHairDies.Size = New System.Drawing.Size(53, 12)
        Me.lblLastUpdateHairDies.TabIndex = 59
        Me.lblLastUpdateHairDies.Text = "Τελ.Αναν.:"
        '
        'lblCustWithNoHairdies
        '
        Me.lblCustWithNoHairdies.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCustWithNoHairdies.Location = New System.Drawing.Point(9, 64)
        Me.lblCustWithNoHairdies.Name = "lblCustWithNoHairdies"
        Me.lblCustWithNoHairdies.Size = New System.Drawing.Size(198, 47)
        Me.lblCustWithNoHairdies.TabIndex = 54
        Me.lblCustWithNoHairdies.Text = "Ο επιλεγμένος πελάτης δεν έχει  βαφές"
        Me.lblCustWithNoHairdies.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblCustWithNoHairdies.Visible = False
        '
        'btnDeleteHairdies
        '
        Me.btnDeleteHairdies.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteHairdies.Location = New System.Drawing.Point(121, 160)
        Me.btnDeleteHairdies.Name = "btnDeleteHairdies"
        Me.btnDeleteHairdies.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteHairdies.TabIndex = 45
        Me.btnDeleteHairdies.Text = "Διαγραφή"
        Me.btnDeleteHairdies.UseVisualStyleBackColor = True
        '
        'dgvHairdiesList
        '
        Me.dgvHairdiesList.AllowUserToResizeColumns = False
        Me.dgvHairdiesList.AllowUserToResizeRows = False
        Me.dgvHairdiesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvHairdiesList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvHairdiesList.Location = New System.Drawing.Point(11, 29)
        Me.dgvHairdiesList.Name = "dgvHairdiesList"
        Me.dgvHairdiesList.RowHeadersVisible = False
        Me.dgvHairdiesList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvHairdiesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvHairdiesList.Size = New System.Drawing.Size(189, 125)
        Me.dgvHairdiesList.TabIndex = 11
        Me.dgvHairdiesList.Visible = False
        '
        'grpCustDebts
        '
        Me.grpCustDebts.Controls.Add(Me.btnAddDebt)
        Me.grpCustDebts.Controls.Add(Me.lblScanHint)
        Me.grpCustDebts.Controls.Add(Me.btnPrintDebtsList)
        Me.grpCustDebts.Controls.Add(Me.Label25)
        Me.grpCustDebts.Controls.Add(Me.lblLastUpdateDebts)
        Me.grpCustDebts.Controls.Add(Me.lblCustWithNoDebts)
        Me.grpCustDebts.Controls.Add(Me.btnPayDebts)
        Me.grpCustDebts.Controls.Add(Me.lblTotalCustomerDebt)
        Me.grpCustDebts.Controls.Add(Me.lblTotalDebtLabel)
        Me.grpCustDebts.Controls.Add(Me.btnDeleteDebts)
        Me.grpCustDebts.Controls.Add(Me.dgvDebtsList)
        Me.grpCustDebts.Controls.Add(Me.txtNoCustomers)
        Me.grpCustDebts.Location = New System.Drawing.Point(259, 15)
        Me.grpCustDebts.Name = "grpCustDebts"
        Me.grpCustDebts.Size = New System.Drawing.Size(394, 315)
        Me.grpCustDebts.TabIndex = 44
        Me.grpCustDebts.TabStop = False
        Me.grpCustDebts.Text = "-"
        '
        'btnAddDebt
        '
        Me.btnAddDebt.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnAddDebt.Location = New System.Drawing.Point(189, 273)
        Me.btnAddDebt.Name = "btnAddDebt"
        Me.btnAddDebt.Size = New System.Drawing.Size(23, 23)
        Me.btnAddDebt.TabIndex = 68
        Me.btnAddDebt.Text = "+"
        Me.btnAddDebt.UseVisualStyleBackColor = True
        '
        'lblScanHint
        '
        Me.lblScanHint.AutoSize = True
        Me.lblScanHint.ForeColor = System.Drawing.Color.OrangeRed
        Me.lblScanHint.Location = New System.Drawing.Point(23, 269)
        Me.lblScanHint.Name = "lblScanHint"
        Me.lblScanHint.Size = New System.Drawing.Size(45, 13)
        Me.lblScanHint.TabIndex = 67
        Me.lblScanHint.Text = "Label28"
        Me.lblScanHint.Visible = False
        '
        'btnPrintDebtsList
        '
        Me.btnPrintDebtsList.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnPrintDebtsList.Location = New System.Drawing.Point(218, 273)
        Me.btnPrintDebtsList.Name = "btnPrintDebtsList"
        Me.btnPrintDebtsList.Size = New System.Drawing.Size(75, 23)
        Me.btnPrintDebtsList.TabIndex = 66
        Me.btnPrintDebtsList.Text = "Εκτύπωση"
        Me.btnPrintDebtsList.UseVisualStyleBackColor = True
        '
        'Label25
        '
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label25.ForeColor = System.Drawing.Color.Red
        Me.Label25.Location = New System.Drawing.Point(-51, 48)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(45, 12)
        Me.Label25.TabIndex = 65
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblLastUpdateDebts
        '
        Me.lblLastUpdateDebts.AutoSize = True
        Me.lblLastUpdateDebts.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateDebts.Location = New System.Drawing.Point(12, 294)
        Me.lblLastUpdateDebts.Name = "lblLastUpdateDebts"
        Me.lblLastUpdateDebts.Size = New System.Drawing.Size(53, 12)
        Me.lblLastUpdateDebts.TabIndex = 58
        Me.lblLastUpdateDebts.Text = "Τελ.Αναν.:"
        '
        'lblCustWithNoDebts
        '
        Me.lblCustWithNoDebts.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCustWithNoDebts.Location = New System.Drawing.Point(8, 134)
        Me.lblCustWithNoDebts.Name = "lblCustWithNoDebts"
        Me.lblCustWithNoDebts.Size = New System.Drawing.Size(367, 63)
        Me.lblCustWithNoDebts.TabIndex = 53
        Me.lblCustWithNoDebts.Text = "Ο επιλεγμένος πελάτης δεν έχει χρέη"
        Me.lblCustWithNoDebts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblCustWithNoDebts.Visible = False
        '
        'btnPayDebts
        '
        Me.btnPayDebts.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnPayDebts.Location = New System.Drawing.Point(299, 23)
        Me.btnPayDebts.Name = "btnPayDebts"
        Me.btnPayDebts.Size = New System.Drawing.Size(76, 23)
        Me.btnPayDebts.TabIndex = 52
        Me.btnPayDebts.Text = "Αποπληρωμή"
        Me.btnPayDebts.UseVisualStyleBackColor = True
        '
        'lblTotalCustomerDebt
        '
        Me.lblTotalCustomerDebt.AutoSize = True
        Me.lblTotalCustomerDebt.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblTotalCustomerDebt.Location = New System.Drawing.Point(163, 23)
        Me.lblTotalCustomerDebt.Name = "lblTotalCustomerDebt"
        Me.lblTotalCustomerDebt.Size = New System.Drawing.Size(77, 17)
        Me.lblTotalCustomerDebt.TabIndex = 51
        Me.lblTotalCustomerDebt.Text = "4.222,25 "
        Me.lblTotalCustomerDebt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTotalDebtLabel
        '
        Me.lblTotalDebtLabel.AutoSize = True
        Me.lblTotalDebtLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblTotalDebtLabel.Location = New System.Drawing.Point(7, 26)
        Me.lblTotalDebtLabel.Name = "lblTotalDebtLabel"
        Me.lblTotalDebtLabel.Size = New System.Drawing.Size(159, 13)
        Me.lblTotalDebtLabel.TabIndex = 50
        Me.lblTotalDebtLabel.Text = "ΟΦΕΙΛΕΣ απο 22 συναλλαγές: "
        '
        'btnDeleteDebts
        '
        Me.btnDeleteDebts.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteDebts.Location = New System.Drawing.Point(299, 273)
        Me.btnDeleteDebts.Name = "btnDeleteDebts"
        Me.btnDeleteDebts.Size = New System.Drawing.Size(75, 23)
        Me.btnDeleteDebts.TabIndex = 44
        Me.btnDeleteDebts.Text = "Διαγραφή"
        Me.btnDeleteDebts.UseVisualStyleBackColor = True
        '
        'dgvDebtsList
        '
        Me.dgvDebtsList.AllowUserToAddRows = False
        Me.dgvDebtsList.AllowUserToResizeColumns = False
        Me.dgvDebtsList.AllowUserToResizeRows = False
        Me.dgvDebtsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvDebtsList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvDebtsList.Location = New System.Drawing.Point(12, 51)
        Me.dgvDebtsList.Name = "dgvDebtsList"
        Me.dgvDebtsList.RowHeadersVisible = False
        Me.dgvDebtsList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvDebtsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvDebtsList.ShowEditingIcon = False
        Me.dgvDebtsList.Size = New System.Drawing.Size(363, 216)
        Me.dgvDebtsList.TabIndex = 37
        Me.dgvDebtsList.Visible = False
        '
        'txtNoCustomers
        '
        Me.txtNoCustomers.Font = New System.Drawing.Font("Microsoft Sans Serif", 28.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtNoCustomers.Location = New System.Drawing.Point(26, 277)
        Me.txtNoCustomers.Name = "txtNoCustomers"
        Me.txtNoCustomers.Size = New System.Drawing.Size(607, 50)
        Me.txtNoCustomers.TabIndex = 59
        Me.txtNoCustomers.Text = "Δεν βρέθηκαν πελάτες"
        Me.txtNoCustomers.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtNoCustomers.Visible = False
        '
        'btnDeleteCustomer
        '
        Me.btnDeleteCustomer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteCustomer.Location = New System.Drawing.Point(148, 586)
        Me.btnDeleteCustomer.Name = "btnDeleteCustomer"
        Me.btnDeleteCustomer.Size = New System.Drawing.Size(75, 23)
        Me.btnDeleteCustomer.TabIndex = 43
        Me.btnDeleteCustomer.Text = "Διαγραφή"
        Me.btnDeleteCustomer.UseVisualStyleBackColor = True
        '
        'rtxtCustomersMessage
        '
        Me.rtxtCustomersMessage.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtCustomersMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtCustomersMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtCustomersMessage.Location = New System.Drawing.Point(15, 84)
        Me.rtxtCustomersMessage.Name = "rtxtCustomersMessage"
        Me.rtxtCustomersMessage.ReadOnly = True
        Me.rtxtCustomersMessage.Size = New System.Drawing.Size(232, 30)
        Me.rtxtCustomersMessage.TabIndex = 42
        Me.rtxtCustomersMessage.Text = ""
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 15)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(102, 13)
        Me.Label3.TabIndex = 38
        Me.Label3.Text = "Αναζήτηση πελάτη"
        '
        'txtSearchCustomer
        '
        Me.txtSearchCustomer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSearchCustomer.Location = New System.Drawing.Point(15, 31)
        Me.txtSearchCustomer.Name = "txtSearchCustomer"
        Me.txtSearchCustomer.Size = New System.Drawing.Size(124, 20)
        Me.txtSearchCustomer.TabIndex = 37
        '
        'dgvCustomers
        '
        Me.dgvCustomers.AllowUserToOrderColumns = True
        Me.dgvCustomers.AllowUserToResizeColumns = False
        Me.dgvCustomers.AllowUserToResizeRows = False
        Me.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCustomers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvCustomers.Location = New System.Drawing.Point(15, 118)
        Me.dgvCustomers.MultiSelect = False
        Me.dgvCustomers.Name = "dgvCustomers"
        Me.dgvCustomers.RowHeadersVisible = False
        Me.dgvCustomers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvCustomers.ShowEditingIcon = False
        Me.dgvCustomers.Size = New System.Drawing.Size(232, 462)
        Me.dgvCustomers.TabIndex = 36
        '
        'grpPrescriptions
        '
        Me.grpPrescriptions.Controls.Add(Me.lblTotPrescriptions)
        Me.grpPrescriptions.Controls.Add(Me.chkSelectAll)
        Me.grpPrescriptions.Controls.Add(Me.lblLastUpdatePrescriptions)
        Me.grpPrescriptions.Controls.Add(Me.lblCustWithPrescriptions)
        Me.grpPrescriptions.Controls.Add(Me.btnDeletePrescriptions)
        Me.grpPrescriptions.Controls.Add(Me.dgvPrescriptions)
        Me.grpPrescriptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpPrescriptions.Location = New System.Drawing.Point(671, 15)
        Me.grpPrescriptions.Name = "grpPrescriptions"
        Me.grpPrescriptions.Size = New System.Drawing.Size(301, 315)
        Me.grpPrescriptions.TabIndex = 72
        Me.grpPrescriptions.TabStop = False
        '
        'lblTotPrescriptions
        '
        Me.lblTotPrescriptions.AutoSize = True
        Me.lblTotPrescriptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblTotPrescriptions.Location = New System.Drawing.Point(25, 22)
        Me.lblTotPrescriptions.Name = "lblTotPrescriptions"
        Me.lblTotPrescriptions.Size = New System.Drawing.Size(19, 13)
        Me.lblTotPrescriptions.TabIndex = 59
        Me.lblTotPrescriptions.Text = "??"
        '
        'chkSelectAll
        '
        Me.chkSelectAll.Location = New System.Drawing.Point(117, 17)
        Me.chkSelectAll.Name = "chkSelectAll"
        Me.chkSelectAll.Size = New System.Drawing.Size(104, 25)
        Me.chkSelectAll.TabIndex = 58
        Me.chkSelectAll.Text = "Εμφάνιση όλων"
        Me.chkSelectAll.UseVisualStyleBackColor = True
        '
        'lblLastUpdatePrescriptions
        '
        Me.lblLastUpdatePrescriptions.AutoSize = True
        Me.lblLastUpdatePrescriptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdatePrescriptions.Location = New System.Drawing.Point(16, 276)
        Me.lblLastUpdatePrescriptions.Name = "lblLastUpdatePrescriptions"
        Me.lblLastUpdatePrescriptions.Size = New System.Drawing.Size(53, 12)
        Me.lblLastUpdatePrescriptions.TabIndex = 57
        Me.lblLastUpdatePrescriptions.Text = "Τελ.Αναν.:"
        '
        'lblCustWithPrescriptions
        '
        Me.lblCustWithPrescriptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblCustWithPrescriptions.Location = New System.Drawing.Point(5, 143)
        Me.lblCustWithPrescriptions.Name = "lblCustWithPrescriptions"
        Me.lblCustWithPrescriptions.Size = New System.Drawing.Size(284, 47)
        Me.lblCustWithPrescriptions.TabIndex = 54
        Me.lblCustWithPrescriptions.Text = "Ο επιλεγμένος πελάτης δεν έχει συνταγές προς εκτέλεση"
        Me.lblCustWithPrescriptions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblCustWithPrescriptions.Visible = False
        '
        'btnDeletePrescriptions
        '
        Me.btnDeletePrescriptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeletePrescriptions.Location = New System.Drawing.Point(214, 276)
        Me.btnDeletePrescriptions.Name = "btnDeletePrescriptions"
        Me.btnDeletePrescriptions.Size = New System.Drawing.Size(75, 23)
        Me.btnDeletePrescriptions.TabIndex = 45
        Me.btnDeletePrescriptions.Text = "Διαγραφή"
        Me.btnDeletePrescriptions.UseVisualStyleBackColor = True
        '
        'dgvPrescriptions
        '
        Me.dgvPrescriptions.AllowUserToResizeColumns = False
        Me.dgvPrescriptions.AllowUserToResizeRows = False
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvPrescriptions.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.dgvPrescriptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvPrescriptions.DefaultCellStyle = DataGridViewCellStyle5
        Me.dgvPrescriptions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvPrescriptions.Location = New System.Drawing.Point(9, 51)
        Me.dgvPrescriptions.Name = "dgvPrescriptions"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvPrescriptions.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.dgvPrescriptions.RowHeadersVisible = False
        Me.dgvPrescriptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPrescriptions.Size = New System.Drawing.Size(280, 220)
        Me.dgvPrescriptions.TabIndex = 11
        Me.dgvPrescriptions.Visible = False
        '
        'tbpPricesParadrugs
        '
        Me.tbpPricesParadrugs.Controls.Add(Me.btnImportExcel)
        Me.tbpPricesParadrugs.Controls.Add(Me.Button11)
        Me.tbpPricesParadrugs.Controls.Add(Me.lblNewRecordAdded)
        Me.tbpPricesParadrugs.Controls.Add(Me.txtRowChanged3)
        Me.tbpPricesParadrugs.Controls.Add(Me.txtRowChanged2)
        Me.tbpPricesParadrugs.Controls.Add(Me.Button10)
        Me.tbpPricesParadrugs.Controls.Add(Me.Button9)
        Me.tbpPricesParadrugs.Controls.Add(Me.txtRowChanged)
        Me.tbpPricesParadrugs.Controls.Add(Me.chkManualBarcode)
        Me.tbpPricesParadrugs.Controls.Add(Me.Button5)
        Me.tbpPricesParadrugs.Controls.Add(Me.chkPairing)
        Me.tbpPricesParadrugs.Controls.Add(Me.btnExpiringDrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.txtSearchPricesParadrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.grpExpirationList)
        Me.tbpPricesParadrugs.Controls.Add(Me.grpDrugsOrParadrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.grpCalculateLianiki)
        Me.tbpPricesParadrugs.Controls.Add(Me.grpLastUpdateParadrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.grpSearchParadrugOptions)
        Me.tbpPricesParadrugs.Controls.Add(Me.Button3)
        Me.tbpPricesParadrugs.Controls.Add(Me.btnDeletePriceParadrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.rtxtPricesParadrugs)
        Me.tbpPricesParadrugs.Controls.Add(Me.Label6)
        Me.tbpPricesParadrugs.Controls.Add(Me.dgvPricesParadrugs)
        Me.tbpPricesParadrugs.Location = New System.Drawing.Point(4, 22)
        Me.tbpPricesParadrugs.Name = "tbpPricesParadrugs"
        Me.tbpPricesParadrugs.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpPricesParadrugs.Size = New System.Drawing.Size(996, 696)
        Me.tbpPricesParadrugs.TabIndex = 7
        Me.tbpPricesParadrugs.Text = "Τιμές & Λήξεις"
        Me.tbpPricesParadrugs.UseVisualStyleBackColor = True
        '
        'btnImportExcel
        '
        Me.btnImportExcel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnImportExcel.Location = New System.Drawing.Point(25, 647)
        Me.btnImportExcel.Name = "btnImportExcel"
        Me.btnImportExcel.Size = New System.Drawing.Size(96, 23)
        Me.btnImportExcel.TabIndex = 70
        Me.btnImportExcel.Text = "Import Excel"
        Me.btnImportExcel.UseVisualStyleBackColor = True
        '
        'Button11
        '
        Me.Button11.Location = New System.Drawing.Point(784, 25)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(27, 23)
        Me.Button11.TabIndex = 69
        Me.Button11.Text = "Button11"
        Me.Button11.UseVisualStyleBackColor = True
        '
        'lblNewRecordAdded
        '
        Me.lblNewRecordAdded.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblNewRecordAdded.ForeColor = System.Drawing.Color.Red
        Me.lblNewRecordAdded.Location = New System.Drawing.Point(520, 621)
        Me.lblNewRecordAdded.Name = "lblNewRecordAdded"
        Me.lblNewRecordAdded.Size = New System.Drawing.Size(94, 24)
        Me.lblNewRecordAdded.TabIndex = 68
        Me.lblNewRecordAdded.Text = "??"
        Me.lblNewRecordAdded.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtRowChanged3
        '
        Me.txtRowChanged3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtRowChanged3.Location = New System.Drawing.Point(793, 554)
        Me.txtRowChanged3.Name = "txtRowChanged3"
        Me.txtRowChanged3.Size = New System.Drawing.Size(166, 21)
        Me.txtRowChanged3.TabIndex = 67
        Me.txtRowChanged3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtRowChanged3.Visible = False
        '
        'txtRowChanged2
        '
        Me.txtRowChanged2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtRowChanged2.Location = New System.Drawing.Point(793, 528)
        Me.txtRowChanged2.Name = "txtRowChanged2"
        Me.txtRowChanged2.Size = New System.Drawing.Size(166, 21)
        Me.txtRowChanged2.TabIndex = 66
        Me.txtRowChanged2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtRowChanged2.Visible = False
        '
        'Button10
        '
        Me.Button10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button10.Location = New System.Drawing.Point(877, 337)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(65, 23)
        Me.Button10.TabIndex = 65
        Me.Button10.Text = "Edited"
        Me.Button10.UseVisualStyleBackColor = True
        Me.Button10.Visible = False
        '
        'Button9
        '
        Me.Button9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button9.Location = New System.Drawing.Point(877, 308)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(65, 23)
        Me.Button9.TabIndex = 64
        Me.Button9.Text = "Value"
        Me.Button9.UseVisualStyleBackColor = True
        Me.Button9.Visible = False
        '
        'txtRowChanged
        '
        Me.txtRowChanged.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtRowChanged.Location = New System.Drawing.Point(793, 363)
        Me.txtRowChanged.Multiline = True
        Me.txtRowChanged.Name = "txtRowChanged"
        Me.txtRowChanged.Size = New System.Drawing.Size(166, 161)
        Me.txtRowChanged.TabIndex = 63
        Me.txtRowChanged.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'chkManualBarcode
        '
        Me.chkManualBarcode.AutoSize = True
        Me.chkManualBarcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.chkManualBarcode.Location = New System.Drawing.Point(241, 56)
        Me.chkManualBarcode.Name = "chkManualBarcode"
        Me.chkManualBarcode.Size = New System.Drawing.Size(102, 17)
        Me.chkManualBarcode.TabIndex = 62
        Me.chkManualBarcode.Text = "Manual barcode"
        Me.chkManualBarcode.UseVisualStyleBackColor = True
        Me.chkManualBarcode.Visible = False
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(260, 23)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(27, 23)
        Me.Button5.TabIndex = 46
        Me.Button5.Text = "Button5"
        Me.Button5.UseVisualStyleBackColor = True
        Me.Button5.Visible = False
        '
        'chkPairing
        '
        Me.chkPairing.Location = New System.Drawing.Point(898, 270)
        Me.chkPairing.Name = "chkPairing"
        Me.chkPairing.Size = New System.Drawing.Size(22, 25)
        Me.chkPairing.TabIndex = 61
        Me.chkPairing.UseVisualStyleBackColor = True
        Me.chkPairing.Visible = False
        '
        'btnExpiringDrugs
        '
        Me.btnExpiringDrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnExpiringDrugs.Location = New System.Drawing.Point(793, 308)
        Me.btnExpiringDrugs.Name = "btnExpiringDrugs"
        Me.btnExpiringDrugs.Size = New System.Drawing.Size(78, 36)
        Me.btnExpiringDrugs.TabIndex = 61
        Me.btnExpiringDrugs.Text = "Προσεχείς Λήξεις"
        Me.btnExpiringDrugs.UseVisualStyleBackColor = True
        '
        'txtSearchPricesParadrugs
        '
        Me.txtSearchPricesParadrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSearchPricesParadrugs.Location = New System.Drawing.Point(20, 25)
        Me.txtSearchPricesParadrugs.Name = "txtSearchPricesParadrugs"
        Me.txtSearchPricesParadrugs.Size = New System.Drawing.Size(205, 20)
        Me.txtSearchPricesParadrugs.TabIndex = 34
        '
        'grpExpirationList
        '
        Me.grpExpirationList.Controls.Add(Me.lblParadrugName)
        Me.grpExpirationList.Controls.Add(Me.lblNewRecord_Exp)
        Me.grpExpirationList.Controls.Add(Me.lblDirtyState_Exp)
        Me.grpExpirationList.Controls.Add(Me.btnDeleteExpiration)
        Me.grpExpirationList.Controls.Add(Me.txtNoExpirations)
        Me.grpExpirationList.Controls.Add(Me.dgvExpirations)
        Me.grpExpirationList.Controls.Add(Me.btnExpirations)
        Me.grpExpirationList.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpExpirationList.Location = New System.Drawing.Point(793, 80)
        Me.grpExpirationList.Name = "grpExpirationList"
        Me.grpExpirationList.Size = New System.Drawing.Size(144, 224)
        Me.grpExpirationList.TabIndex = 60
        Me.grpExpirationList.TabStop = False
        Me.grpExpirationList.Text = "Λήξεις"
        '
        'lblParadrugName
        '
        Me.lblParadrugName.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblParadrugName.Location = New System.Drawing.Point(-2, -15)
        Me.lblParadrugName.Name = "lblParadrugName"
        Me.lblParadrugName.Size = New System.Drawing.Size(146, 12)
        Me.lblParadrugName.TabIndex = 69
        Me.lblParadrugName.Text = "???"
        Me.lblParadrugName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblNewRecord_Exp
        '
        Me.lblNewRecord_Exp.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblNewRecord_Exp.ForeColor = System.Drawing.Color.Red
        Me.lblNewRecord_Exp.Location = New System.Drawing.Point(77, 15)
        Me.lblNewRecord_Exp.Name = "lblNewRecord_Exp"
        Me.lblNewRecord_Exp.Size = New System.Drawing.Size(67, 12)
        Me.lblNewRecord_Exp.TabIndex = 62
        Me.lblNewRecord_Exp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDirtyState_Exp
        '
        Me.lblDirtyState_Exp.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblDirtyState_Exp.Location = New System.Drawing.Point(11, 15)
        Me.lblDirtyState_Exp.Name = "lblDirtyState_Exp"
        Me.lblDirtyState_Exp.Size = New System.Drawing.Size(67, 12)
        Me.lblDirtyState_Exp.TabIndex = 61
        Me.lblDirtyState_Exp.Text = "??"
        Me.lblDirtyState_Exp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnDeleteExpiration
        '
        Me.btnDeleteExpiration.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteExpiration.Location = New System.Drawing.Point(13, 165)
        Me.btnDeleteExpiration.Name = "btnDeleteExpiration"
        Me.btnDeleteExpiration.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteExpiration.TabIndex = 60
        Me.btnDeleteExpiration.Text = "Διαγραφή"
        Me.btnDeleteExpiration.UseVisualStyleBackColor = True
        '
        'txtNoExpirations
        '
        Me.txtNoExpirations.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtNoExpirations.Location = New System.Drawing.Point(6, 77)
        Me.txtNoExpirations.Name = "txtNoExpirations"
        Me.txtNoExpirations.ReadOnly = True
        Me.txtNoExpirations.Size = New System.Drawing.Size(135, 21)
        Me.txtNoExpirations.TabIndex = 59
        Me.txtNoExpirations.Text = "Χώρις λήξεις"
        Me.txtNoExpirations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtNoExpirations.Visible = False
        '
        'dgvExpirations
        '
        Me.dgvExpirations.AllowUserToDeleteRows = False
        Me.dgvExpirations.AllowUserToResizeColumns = False
        Me.dgvExpirations.AllowUserToResizeRows = False
        Me.dgvExpirations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvExpirations.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvExpirations.Location = New System.Drawing.Point(13, 30)
        Me.dgvExpirations.Name = "dgvExpirations"
        Me.dgvExpirations.RowHeadersVisible = False
        Me.dgvExpirations.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvExpirations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvExpirations.ShowEditingIcon = False
        Me.dgvExpirations.Size = New System.Drawing.Size(120, 129)
        Me.dgvExpirations.TabIndex = 58
        '
        'btnExpirations
        '
        Me.btnExpirations.Enabled = False
        Me.btnExpirations.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnExpirations.Location = New System.Drawing.Point(13, 190)
        Me.btnExpirations.Name = "btnExpirations"
        Me.btnExpirations.Size = New System.Drawing.Size(86, 23)
        Me.btnExpirations.TabIndex = 56
        Me.btnExpirations.Text = "Αντιστοίχηση"
        Me.btnExpirations.UseVisualStyleBackColor = True
        Me.btnExpirations.Visible = False
        '
        'grpDrugsOrParadrugs
        '
        Me.grpDrugsOrParadrugs.Controls.Add(Me.rbParadrugs)
        Me.grpDrugsOrParadrugs.Controls.Add(Me.rbDrugs)
        Me.grpDrugsOrParadrugs.Location = New System.Drawing.Point(349, 9)
        Me.grpDrugsOrParadrugs.Name = "grpDrugsOrParadrugs"
        Me.grpDrugsOrParadrugs.Size = New System.Drawing.Size(102, 59)
        Me.grpDrugsOrParadrugs.TabIndex = 57
        Me.grpDrugsOrParadrugs.TabStop = False
        '
        'rbParadrugs
        '
        Me.rbParadrugs.AutoSize = True
        Me.rbParadrugs.Checked = True
        Me.rbParadrugs.Location = New System.Drawing.Point(5, 34)
        Me.rbParadrugs.Name = "rbParadrugs"
        Me.rbParadrugs.Size = New System.Drawing.Size(97, 17)
        Me.rbParadrugs.TabIndex = 45
        Me.rbParadrugs.TabStop = True
        Me.rbParadrugs.Text = "Custom αρχείο"
        Me.rbParadrugs.UseVisualStyleBackColor = True
        '
        'rbDrugs
        '
        Me.rbDrugs.AutoSize = True
        Me.rbDrugs.Location = New System.Drawing.Point(5, 11)
        Me.rbDrugs.Name = "rbDrugs"
        Me.rbDrugs.Size = New System.Drawing.Size(79, 17)
        Me.rbDrugs.TabIndex = 44
        Me.rbDrugs.Text = "Pharmakon"
        Me.rbDrugs.UseVisualStyleBackColor = True
        '
        'grpCalculateLianiki
        '
        Me.grpCalculateLianiki.Controls.Add(Me.Label13)
        Me.grpCalculateLianiki.Controls.Add(Me.Label12)
        Me.grpCalculateLianiki.Controls.Add(Me.Label11)
        Me.grpCalculateLianiki.Controls.Add(Me.Label5)
        Me.grpCalculateLianiki.Controls.Add(Me.cboFPA_Paradrugs)
        Me.grpCalculateLianiki.Controls.Add(Me.Label23)
        Me.grpCalculateLianiki.Controls.Add(Me.txtProfit_Paradrugs)
        Me.grpCalculateLianiki.Controls.Add(Me.Label21)
        Me.grpCalculateLianiki.Controls.Add(Me.txtTotalPrice_Paradrugs)
        Me.grpCalculateLianiki.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpCalculateLianiki.Location = New System.Drawing.Point(493, 6)
        Me.grpCalculateLianiki.Name = "grpCalculateLianiki"
        Me.grpCalculateLianiki.Size = New System.Drawing.Size(205, 65)
        Me.grpCalculateLianiki.TabIndex = 55
        Me.grpCalculateLianiki.TabStop = False
        Me.grpCalculateLianiki.Text = "Υπολογισμός Λιανικής Τιμής"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label13.Location = New System.Drawing.Point(183, 38)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(13, 13)
        Me.Label13.TabIndex = 56
        Me.Label13.Text = "€"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label12.Location = New System.Drawing.Point(126, 21)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(31, 9)
        Me.Label12.TabIndex = 55
        Me.Label12.Text = "Λιανική"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label11.Location = New System.Drawing.Point(77, 21)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(31, 9)
        Me.Label11.TabIndex = 54
        Me.Label11.Text = "Κέρδος"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label5.Location = New System.Drawing.Point(14, 21)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(22, 9)
        Me.Label5.TabIndex = 53
        Me.Label5.Text = "ΦΠΑ"
        '
        'cboFPA_Paradrugs
        '
        Me.cboFPA_Paradrugs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cboFPA_Paradrugs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboFPA_Paradrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cboFPA_Paradrugs.FormattingEnabled = True
        Me.cboFPA_Paradrugs.Items.AddRange(New Object() {"6", "13", "23"})
        Me.cboFPA_Paradrugs.Location = New System.Drawing.Point(15, 32)
        Me.cboFPA_Paradrugs.Name = "cboFPA_Paradrugs"
        Me.cboFPA_Paradrugs.Size = New System.Drawing.Size(44, 23)
        Me.cboFPA_Paradrugs.TabIndex = 52
        Me.cboFPA_Paradrugs.Text = "6"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label23.Location = New System.Drawing.Point(64, 36)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(12, 13)
        Me.Label23.TabIndex = 48
        Me.Label23.Text = "x"
        '
        'txtProfit_Paradrugs
        '
        Me.txtProfit_Paradrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtProfit_Paradrugs.Location = New System.Drawing.Point(78, 33)
        Me.txtProfit_Paradrugs.Name = "txtProfit_Paradrugs"
        Me.txtProfit_Paradrugs.Size = New System.Drawing.Size(29, 21)
        Me.txtProfit_Paradrugs.TabIndex = 49
        Me.txtProfit_Paradrugs.Text = "35"
        Me.txtProfit_Paradrugs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label21.Location = New System.Drawing.Point(110, 36)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(13, 13)
        Me.Label21.TabIndex = 50
        Me.Label21.Text = "="
        '
        'txtTotalPrice_Paradrugs
        '
        Me.txtTotalPrice_Paradrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtTotalPrice_Paradrugs.Location = New System.Drawing.Point(126, 33)
        Me.txtTotalPrice_Paradrugs.Name = "txtTotalPrice_Paradrugs"
        Me.txtTotalPrice_Paradrugs.Size = New System.Drawing.Size(56, 21)
        Me.txtTotalPrice_Paradrugs.TabIndex = 51
        Me.txtTotalPrice_Paradrugs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'grpLastUpdateParadrugs
        '
        Me.grpLastUpdateParadrugs.Controls.Add(Me.lblLastUpdateParadrugs)
        Me.grpLastUpdateParadrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.grpLastUpdateParadrugs.Location = New System.Drawing.Point(821, 581)
        Me.grpLastUpdateParadrugs.Name = "grpLastUpdateParadrugs"
        Me.grpLastUpdateParadrugs.Size = New System.Drawing.Size(105, 39)
        Me.grpLastUpdateParadrugs.TabIndex = 54
        Me.grpLastUpdateParadrugs.TabStop = False
        Me.grpLastUpdateParadrugs.Text = "Τελ. Ανανέωση"
        '
        'lblLastUpdateParadrugs
        '
        Me.lblLastUpdateParadrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdateParadrugs.Location = New System.Drawing.Point(6, 18)
        Me.lblLastUpdateParadrugs.Name = "lblLastUpdateParadrugs"
        Me.lblLastUpdateParadrugs.Size = New System.Drawing.Size(93, 12)
        Me.lblLastUpdateParadrugs.TabIndex = 55
        Me.lblLastUpdateParadrugs.Text = "??"
        Me.lblLastUpdateParadrugs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpSearchParadrugOptions
        '
        Me.grpSearchParadrugOptions.Controls.Add(Me.rbByQRcode)
        Me.grpSearchParadrugOptions.Controls.Add(Me.rbByBarcode)
        Me.grpSearchParadrugOptions.Controls.Add(Me.rbByName)
        Me.grpSearchParadrugOptions.Location = New System.Drawing.Point(20, 43)
        Me.grpSearchParadrugOptions.Name = "grpSearchParadrugOptions"
        Me.grpSearchParadrugOptions.Size = New System.Drawing.Size(215, 31)
        Me.grpSearchParadrugOptions.TabIndex = 53
        Me.grpSearchParadrugOptions.TabStop = False
        '
        'rbByQRcode
        '
        Me.rbByQRcode.AutoSize = True
        Me.rbByQRcode.Location = New System.Drawing.Point(141, 11)
        Me.rbByQRcode.Name = "rbByQRcode"
        Me.rbByQRcode.Size = New System.Drawing.Size(65, 17)
        Me.rbByQRcode.TabIndex = 46
        Me.rbByQRcode.Text = "QRcode"
        Me.rbByQRcode.UseVisualStyleBackColor = True
        '
        'rbByBarcode
        '
        Me.rbByBarcode.AutoSize = True
        Me.rbByBarcode.Location = New System.Drawing.Point(70, 11)
        Me.rbByBarcode.Name = "rbByBarcode"
        Me.rbByBarcode.Size = New System.Drawing.Size(65, 17)
        Me.rbByBarcode.TabIndex = 45
        Me.rbByBarcode.Text = "Barcode"
        Me.rbByBarcode.UseVisualStyleBackColor = True
        '
        'rbByName
        '
        Me.rbByName.AutoSize = True
        Me.rbByName.Checked = True
        Me.rbByName.Location = New System.Drawing.Point(5, 11)
        Me.rbByName.Name = "rbByName"
        Me.rbByName.Size = New System.Drawing.Size(59, 17)
        Me.rbByName.TabIndex = 44
        Me.rbByName.TabStop = True
        Me.rbByName.Text = "Όνομα"
        Me.rbByName.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button3.Image = CType(resources.GetObject("Button3.Image"), System.Drawing.Image)
        Me.Button3.Location = New System.Drawing.Point(231, 23)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(23, 23)
        Me.Button3.TabIndex = 38
        Me.Button3.UseVisualStyleBackColor = True
        '
        'btnDeletePriceParadrugs
        '
        Me.btnDeletePriceParadrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeletePriceParadrugs.Location = New System.Drawing.Point(683, 593)
        Me.btnDeletePriceParadrugs.Name = "btnDeletePriceParadrugs"
        Me.btnDeletePriceParadrugs.Size = New System.Drawing.Size(75, 23)
        Me.btnDeletePriceParadrugs.TabIndex = 37
        Me.btnDeletePriceParadrugs.Text = "Διαγραφή"
        Me.btnDeletePriceParadrugs.UseVisualStyleBackColor = True
        '
        'rtxtPricesParadrugs
        '
        Me.rtxtPricesParadrugs.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtPricesParadrugs.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtPricesParadrugs.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtPricesParadrugs.Location = New System.Drawing.Point(16, 593)
        Me.rtxtPricesParadrugs.Name = "rtxtPricesParadrugs"
        Me.rtxtPricesParadrugs.Size = New System.Drawing.Size(659, 24)
        Me.rtxtPricesParadrugs.TabIndex = 36
        Me.rtxtPricesParadrugs.Text = ""
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label6.Location = New System.Drawing.Point(14, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(63, 13)
        Me.Label6.TabIndex = 35
        Me.Label6.Text = "Αναζήτηση"
        '
        'dgvPricesParadrugs
        '
        Me.dgvPricesParadrugs.AllowUserToResizeColumns = False
        Me.dgvPricesParadrugs.AllowUserToResizeRows = False
        Me.dgvPricesParadrugs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPricesParadrugs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.dgvPricesParadrugs.Location = New System.Drawing.Point(16, 80)
        Me.dgvPricesParadrugs.Name = "dgvPricesParadrugs"
        Me.dgvPricesParadrugs.RowHeadersVisible = False
        Me.dgvPricesParadrugs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPricesParadrugs.ShowEditingIcon = False
        Me.dgvPricesParadrugs.Size = New System.Drawing.Size(742, 507)
        Me.dgvPricesParadrugs.TabIndex = 33
        '
        'tbpPhones
        '
        Me.tbpPhones.Controls.Add(Me.GroupBox3)
        Me.tbpPhones.Controls.Add(Me.cboPhoneCatalog)
        Me.tbpPhones.Controls.Add(Me.Button4)
        Me.tbpPhones.Controls.Add(Me.Label2)
        Me.tbpPhones.Controls.Add(Me.txtSearchPhones)
        Me.tbpPhones.Controls.Add(Me.btnDeletePhones)
        Me.tbpPhones.Controls.Add(Me.rtxtPhones)
        Me.tbpPhones.Controls.Add(Me.dgvPhones)
        Me.tbpPhones.Location = New System.Drawing.Point(4, 22)
        Me.tbpPhones.Name = "tbpPhones"
        Me.tbpPhones.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpPhones.Size = New System.Drawing.Size(996, 696)
        Me.tbpPhones.TabIndex = 13
        Me.tbpPhones.Text = "Τηλέφωνα"
        Me.tbpPhones.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.lblLastUpdatePhones)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(767, 44)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(105, 39)
        Me.GroupBox3.TabIndex = 70
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Τελ. Ανανέωση"
        '
        'lblLastUpdatePhones
        '
        Me.lblLastUpdatePhones.AutoSize = True
        Me.lblLastUpdatePhones.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdatePhones.Location = New System.Drawing.Point(6, 18)
        Me.lblLastUpdatePhones.Name = "lblLastUpdatePhones"
        Me.lblLastUpdatePhones.Size = New System.Drawing.Size(15, 12)
        Me.lblLastUpdatePhones.TabIndex = 55
        Me.lblLastUpdatePhones.Text = "??"
        '
        'cboPhoneCatalog
        '
        Me.cboPhoneCatalog.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cboPhoneCatalog.FormattingEnabled = True
        Me.cboPhoneCatalog.Items.AddRange(New Object() {"Η ΑΤΖΕΝΤΑ ΜΟΥ", "ΑΣΘΕΝΕΙΣ - PHARM", "ΙΑΤΡΟΙ", ""})
        Me.cboPhoneCatalog.Location = New System.Drawing.Point(294, 14)
        Me.cboPhoneCatalog.Name = "cboPhoneCatalog"
        Me.cboPhoneCatalog.Size = New System.Drawing.Size(146, 23)
        Me.cboPhoneCatalog.TabIndex = 69
        Me.cboPhoneCatalog.Text = "Η ΑΤΖΕΝΤΑ ΜΟΥ"
        '
        'Button4
        '
        Me.Button4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button4.Image = CType(resources.GetObject("Button4.Image"), System.Drawing.Image)
        Me.Button4.Location = New System.Drawing.Point(227, 14)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(23, 23)
        Me.Button4.TabIndex = 67
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label2.Location = New System.Drawing.Point(15, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 66
        Me.Label2.Text = "Αναζήτηση:"
        '
        'txtSearchPhones
        '
        Me.txtSearchPhones.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSearchPhones.Location = New System.Drawing.Point(87, 16)
        Me.txtSearchPhones.Name = "txtSearchPhones"
        Me.txtSearchPhones.Size = New System.Drawing.Size(134, 20)
        Me.txtSearchPhones.TabIndex = 65
        '
        'btnDeletePhones
        '
        Me.btnDeletePhones.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeletePhones.Location = New System.Drawing.Point(699, 582)
        Me.btnDeletePhones.Name = "btnDeletePhones"
        Me.btnDeletePhones.Size = New System.Drawing.Size(65, 23)
        Me.btnDeletePhones.TabIndex = 64
        Me.btnDeletePhones.Text = "Διαγραφή"
        Me.btnDeletePhones.UseVisualStyleBackColor = True
        '
        'rtxtPhones
        '
        Me.rtxtPhones.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtPhones.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtPhones.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtPhones.Location = New System.Drawing.Point(10, 581)
        Me.rtxtPhones.Name = "rtxtPhones"
        Me.rtxtPhones.ReadOnly = True
        Me.rtxtPhones.Size = New System.Drawing.Size(683, 24)
        Me.rtxtPhones.TabIndex = 63
        Me.rtxtPhones.Text = ""
        '
        'dgvPhones
        '
        Me.dgvPhones.AllowUserToResizeColumns = False
        Me.dgvPhones.AllowUserToResizeRows = False
        Me.dgvPhones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPhones.Location = New System.Drawing.Point(10, 44)
        Me.dgvPhones.MultiSelect = False
        Me.dgvPhones.Name = "dgvPhones"
        Me.dgvPhones.RowHeadersVisible = False
        Me.dgvPhones.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvPhones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPhones.ShowEditingIcon = False
        Me.dgvPhones.Size = New System.Drawing.Size(754, 528)
        Me.dgvPhones.TabIndex = 62
        '
        'tbpBackup
        '
        Me.tbpBackup.AutoScroll = True
        Me.tbpBackup.Controls.Add(Me.lblAdminInfo)
        Me.tbpBackup.Controls.Add(Me.Button15)
        Me.tbpBackup.Controls.Add(Me.Button14)
        Me.tbpBackup.Controls.Add(Me.Button13)
        Me.tbpBackup.Controls.Add(Me.Label1)
        Me.tbpBackup.Controls.Add(Me.btnStopService)
        Me.tbpBackup.Controls.Add(Me.btnStartService)
        Me.tbpBackup.Controls.Add(Me.txtServiceName)
        Me.tbpBackup.Controls.Add(Me.Button8)
        Me.tbpBackup.Controls.Add(Me.btnCoppyAppStation1)
        Me.tbpBackup.Controls.Add(Me.lblPCName)
        Me.tbpBackup.Controls.Add(Me.Label26)
        Me.tbpBackup.Controls.Add(Me.GroupBox4)
        Me.tbpBackup.Controls.Add(Me.GroupBox1)
        Me.tbpBackup.Controls.Add(Me.btnUpdatePharmacy2013C)
        Me.tbpBackup.Controls.Add(Me.lblLastBuilded)
        Me.tbpBackup.Controls.Add(Me.Label20)
        Me.tbpBackup.Controls.Add(Me.GroupBox5)
        Me.tbpBackup.Controls.Add(Me.lstMessage)
        Me.tbpBackup.Controls.Add(Me.lblMessage)
        Me.tbpBackup.Controls.Add(Me.btnBackupRestore)
        Me.tbpBackup.Controls.Add(Me.grpBackupDestination)
        Me.tbpBackup.Controls.Add(Me.grpBackupSource)
        Me.tbpBackup.ForeColor = System.Drawing.Color.Black
        Me.tbpBackup.Location = New System.Drawing.Point(4, 22)
        Me.tbpBackup.Name = "tbpBackup"
        Me.tbpBackup.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpBackup.Size = New System.Drawing.Size(996, 696)
        Me.tbpBackup.TabIndex = 8
        Me.tbpBackup.Text = "Backup"
        Me.tbpBackup.UseVisualStyleBackColor = True
        '
        'lblAdminInfo
        '
        Me.lblAdminInfo.AutoSize = True
        Me.lblAdminInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAdminInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblAdminInfo.ForeColor = System.Drawing.Color.Red
        Me.lblAdminInfo.Location = New System.Drawing.Point(845, 676)
        Me.lblAdminInfo.Name = "lblAdminInfo"
        Me.lblAdminInfo.Size = New System.Drawing.Size(86, 27)
        Me.lblAdminInfo.TabIndex = 68
        Me.lblAdminInfo.Text = "ADMIN"
        Me.lblAdminInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblAdminInfo.Visible = False
        '
        'Button15
        '
        Me.Button15.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button15.Location = New System.Drawing.Point(438, 560)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(138, 36)
        Me.Button15.TabIndex = 66
        Me.Button15.Text = "Δοκιμή Update"
        Me.Button15.UseVisualStyleBackColor = True
        Me.Button15.Visible = False
        '
        'Button14
        '
        Me.Button14.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button14.Location = New System.Drawing.Point(438, 606)
        Me.Button14.Name = "Button14"
        Me.Button14.Size = New System.Drawing.Size(169, 47)
        Me.Button14.TabIndex = 65
        Me.Button14.Text = "Update Databases"
        Me.Button14.UseVisualStyleBackColor = True
        Me.Button14.Visible = False
        '
        'Button13
        '
        Me.Button13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button13.Location = New System.Drawing.Point(215, 625)
        Me.Button13.Name = "Button13"
        Me.Button13.Size = New System.Drawing.Size(137, 37)
        Me.Button13.TabIndex = 64
        Me.Button13.Text = "SQL Server Management"
        Me.Button13.UseVisualStyleBackColor = True
        Me.Button13.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label1.Location = New System.Drawing.Point(212, 583)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 13)
        Me.Label1.TabIndex = 63
        Me.Label1.Text = "Start/Stop Service"
        Me.Label1.Visible = False
        '
        'btnStopService
        '
        Me.btnStopService.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnStopService.Location = New System.Drawing.Point(370, 597)
        Me.btnStopService.Name = "btnStopService"
        Me.btnStopService.Size = New System.Drawing.Size(42, 23)
        Me.btnStopService.TabIndex = 62
        Me.btnStopService.Text = "Stop"
        Me.btnStopService.UseVisualStyleBackColor = True
        Me.btnStopService.Visible = False
        '
        'btnStartService
        '
        Me.btnStartService.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnStartService.Location = New System.Drawing.Point(322, 597)
        Me.btnStartService.Name = "btnStartService"
        Me.btnStartService.Size = New System.Drawing.Size(42, 23)
        Me.btnStartService.TabIndex = 61
        Me.btnStartService.Text = "Start"
        Me.btnStartService.UseVisualStyleBackColor = True
        Me.btnStartService.Visible = False
        '
        'txtServiceName
        '
        Me.txtServiceName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtServiceName.Location = New System.Drawing.Point(215, 599)
        Me.txtServiceName.Name = "txtServiceName"
        Me.txtServiceName.Size = New System.Drawing.Size(101, 20)
        Me.txtServiceName.TabIndex = 60
        Me.txtServiceName.Text = "MSSQL$CSASQL"
        Me.txtServiceName.Visible = False
        '
        'Button8
        '
        Me.Button8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Button8.Location = New System.Drawing.Point(18, 546)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(87, 22)
        Me.Button8.TabIndex = 58
        Me.Button8.Text = "Compare DBs"
        Me.Button8.UseVisualStyleBackColor = True
        Me.Button8.Visible = False
        '
        'btnCoppyAppStation1
        '
        Me.btnCoppyAppStation1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnCoppyAppStation1.Location = New System.Drawing.Point(326, 494)
        Me.btnCoppyAppStation1.Name = "btnCoppyAppStation1"
        Me.btnCoppyAppStation1.Size = New System.Drawing.Size(75, 41)
        Me.btnCoppyAppStation1.TabIndex = 57
        Me.btnCoppyAppStation1.Text = "Copy App to Station1"
        Me.btnCoppyAppStation1.UseVisualStyleBackColor = True
        '
        'lblPCName
        '
        Me.lblPCName.AutoSize = True
        Me.lblPCName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblPCName.Location = New System.Drawing.Point(75, 583)
        Me.lblPCName.Name = "lblPCName"
        Me.lblPCName.Size = New System.Drawing.Size(19, 13)
        Me.lblPCName.TabIndex = 56
        Me.lblPCName.Text = "??"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label26.Location = New System.Drawing.Point(6, 583)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(64, 13)
        Me.Label26.TabIndex = 55
        Me.Label26.Text = "Τερματικό :"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblLastUpdated)
        Me.GroupBox4.Location = New System.Drawing.Point(160, 492)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(142, 39)
        Me.GroupBox4.TabIndex = 54
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Τελευταία ανανέωση"
        '
        'lblLastUpdated
        '
        Me.lblLastUpdated.AutoSize = True
        Me.lblLastUpdated.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdated.Location = New System.Drawing.Point(10, 17)
        Me.lblLastUpdated.Name = "lblLastUpdated"
        Me.lblLastUpdated.Size = New System.Drawing.Size(15, 12)
        Me.lblLastUpdated.TabIndex = 55
        Me.lblLastUpdated.Text = "??"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbOnlyVisualBasic)
        Me.GroupBox1.Controls.Add(Me.rbEverything)
        Me.GroupBox1.Controls.Add(Me.rbOnlyDatabases)
        Me.GroupBox1.Location = New System.Drawing.Point(290, 339)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(142, 92)
        Me.GroupBox1.TabIndex = 53
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Content"
        '
        'rbOnlyVisualBasic
        '
        Me.rbOnlyVisualBasic.AutoSize = True
        Me.rbOnlyVisualBasic.Location = New System.Drawing.Point(11, 67)
        Me.rbOnlyVisualBasic.Name = "rbOnlyVisualBasic"
        Me.rbOnlyVisualBasic.Size = New System.Drawing.Size(106, 17)
        Me.rbOnlyVisualBasic.TabIndex = 2
        Me.rbOnlyVisualBasic.Text = "Only Visual Basic"
        Me.rbOnlyVisualBasic.UseVisualStyleBackColor = True
        '
        'rbEverything
        '
        Me.rbEverything.AutoSize = True
        Me.rbEverything.Location = New System.Drawing.Point(11, 26)
        Me.rbEverything.Name = "rbEverything"
        Me.rbEverything.Size = New System.Drawing.Size(75, 17)
        Me.rbEverything.TabIndex = 1
        Me.rbEverything.Text = "Everything"
        Me.rbEverything.UseVisualStyleBackColor = True
        '
        'rbOnlyDatabases
        '
        Me.rbOnlyDatabases.AutoSize = True
        Me.rbOnlyDatabases.Checked = True
        Me.rbOnlyDatabases.Location = New System.Drawing.Point(11, 46)
        Me.rbOnlyDatabases.Name = "rbOnlyDatabases"
        Me.rbOnlyDatabases.Size = New System.Drawing.Size(100, 17)
        Me.rbOnlyDatabases.TabIndex = 0
        Me.rbOnlyDatabases.TabStop = True
        Me.rbOnlyDatabases.Text = "Only Databases"
        Me.rbOnlyDatabases.UseVisualStyleBackColor = True
        '
        'btnUpdatePharmacy2013C
        '
        Me.btnUpdatePharmacy2013C.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnUpdatePharmacy2013C.Location = New System.Drawing.Point(17, 492)
        Me.btnUpdatePharmacy2013C.Name = "btnUpdatePharmacy2013C"
        Me.btnUpdatePharmacy2013C.Size = New System.Drawing.Size(136, 47)
        Me.btnUpdatePharmacy2013C.TabIndex = 52
        Me.btnUpdatePharmacy2013C.Text = "Update Pharmacy2013C"
        Me.btnUpdatePharmacy2013C.UseVisualStyleBackColor = True
        '
        'lblLastBuilded
        '
        Me.lblLastBuilded.AutoSize = True
        Me.lblLastBuilded.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastBuilded.Location = New System.Drawing.Point(75, 606)
        Me.lblLastBuilded.Name = "lblLastBuilded"
        Me.lblLastBuilded.Size = New System.Drawing.Size(19, 13)
        Me.lblLastBuilded.TabIndex = 51
        Me.lblLastBuilded.Text = "??"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label20.Location = New System.Drawing.Point(6, 606)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(63, 13)
        Me.Label20.TabIndex = 50
        Me.Label20.Text = "Builded on :"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.rbWhereNikoyla)
        Me.GroupBox5.Controls.Add(Me.rbWhereSaloni)
        Me.GroupBox5.Controls.Add(Me.rbWhereFarm2)
        Me.GroupBox5.Controls.Add(Me.rbWhereLaptop)
        Me.GroupBox5.Controls.Add(Me.rbWhereSpiti)
        Me.GroupBox5.Controls.Add(Me.rbWhereFarm1)
        Me.GroupBox5.Location = New System.Drawing.Point(18, 339)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(118, 143)
        Me.GroupBox5.TabIndex = 49
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Location"
        '
        'rbWhereNikoyla
        '
        Me.rbWhereNikoyla.AutoSize = True
        Me.rbWhereNikoyla.Location = New System.Drawing.Point(12, 116)
        Me.rbWhereNikoyla.Name = "rbWhereNikoyla"
        Me.rbWhereNikoyla.Size = New System.Drawing.Size(64, 17)
        Me.rbWhereNikoyla.TabIndex = 53
        Me.rbWhereNikoyla.Text = "Νικούλα"
        Me.rbWhereNikoyla.UseVisualStyleBackColor = True
        '
        'rbWhereSaloni
        '
        Me.rbWhereSaloni.AutoSize = True
        Me.rbWhereSaloni.Location = New System.Drawing.Point(12, 98)
        Me.rbWhereSaloni.Name = "rbWhereSaloni"
        Me.rbWhereSaloni.Size = New System.Drawing.Size(59, 17)
        Me.rbWhereSaloni.TabIndex = 52
        Me.rbWhereSaloni.Text = "Σαλόνι"
        Me.rbWhereSaloni.UseVisualStyleBackColor = True
        '
        'rbWhereFarm2
        '
        Me.rbWhereFarm2.AutoSize = True
        Me.rbWhereFarm2.Checked = True
        Me.rbWhereFarm2.Location = New System.Drawing.Point(11, 42)
        Me.rbWhereFarm2.Name = "rbWhereFarm2"
        Me.rbWhereFarm2.Size = New System.Drawing.Size(90, 17)
        Me.rbWhereFarm2.TabIndex = 51
        Me.rbWhereFarm2.TabStop = True
        Me.rbWhereFarm2.Text = "Φαρμακείο 2"
        Me.rbWhereFarm2.UseVisualStyleBackColor = True
        '
        'rbWhereLaptop
        '
        Me.rbWhereLaptop.AutoSize = True
        Me.rbWhereLaptop.Location = New System.Drawing.Point(11, 80)
        Me.rbWhereLaptop.Name = "rbWhereLaptop"
        Me.rbWhereLaptop.Size = New System.Drawing.Size(58, 17)
        Me.rbWhereLaptop.TabIndex = 50
        Me.rbWhereLaptop.Text = "Laptop"
        Me.rbWhereLaptop.UseVisualStyleBackColor = True
        '
        'rbWhereSpiti
        '
        Me.rbWhereSpiti.AutoSize = True
        Me.rbWhereSpiti.Location = New System.Drawing.Point(11, 62)
        Me.rbWhereSpiti.Name = "rbWhereSpiti"
        Me.rbWhereSpiti.Size = New System.Drawing.Size(50, 17)
        Me.rbWhereSpiti.TabIndex = 49
        Me.rbWhereSpiti.Text = "Σπίτι"
        Me.rbWhereSpiti.UseVisualStyleBackColor = True
        '
        'rbWhereFarm1
        '
        Me.rbWhereFarm1.AutoSize = True
        Me.rbWhereFarm1.Checked = True
        Me.rbWhereFarm1.Location = New System.Drawing.Point(11, 22)
        Me.rbWhereFarm1.Name = "rbWhereFarm1"
        Me.rbWhereFarm1.Size = New System.Drawing.Size(90, 17)
        Me.rbWhereFarm1.TabIndex = 1
        Me.rbWhereFarm1.TabStop = True
        Me.rbWhereFarm1.Text = "Φαρμακείο 1"
        Me.rbWhereFarm1.UseVisualStyleBackColor = True
        '
        'lstMessage
        '
        Me.lstMessage.FormattingEnabled = True
        Me.lstMessage.Location = New System.Drawing.Point(438, 339)
        Me.lstMessage.Name = "lstMessage"
        Me.lstMessage.Size = New System.Drawing.Size(345, 212)
        Me.lstMessage.TabIndex = 48
        '
        'lblMessage
        '
        Me.lblMessage.AutoSize = True
        Me.lblMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblMessage.Location = New System.Drawing.Point(647, 483)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(16, 13)
        Me.lblMessage.TabIndex = 47
        Me.lblMessage.Text = "..."
        '
        'btnBackupRestore
        '
        Me.btnBackupRestore.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnBackupRestore.Location = New System.Drawing.Point(158, 425)
        Me.btnBackupRestore.Name = "btnBackupRestore"
        Me.btnBackupRestore.Size = New System.Drawing.Size(111, 25)
        Me.btnBackupRestore.TabIndex = 46
        Me.btnBackupRestore.Text = "Backup Everything"
        Me.btnBackupRestore.UseVisualStyleBackColor = True
        '
        'grpBackupDestination
        '
        Me.grpBackupDestination.Controls.Add(Me.rbPC2Usb)
        Me.grpBackupDestination.Controls.Add(Me.rbUsb2PC)
        Me.grpBackupDestination.Location = New System.Drawing.Point(142, 339)
        Me.grpBackupDestination.Name = "grpBackupDestination"
        Me.grpBackupDestination.Size = New System.Drawing.Size(142, 80)
        Me.grpBackupDestination.TabIndex = 43
        Me.grpBackupDestination.TabStop = False
        Me.grpBackupDestination.Text = "Modality"
        '
        'rbPC2Usb
        '
        Me.rbPC2Usb.AutoSize = True
        Me.rbPC2Usb.Checked = True
        Me.rbPC2Usb.Location = New System.Drawing.Point(11, 26)
        Me.rbPC2Usb.Name = "rbPC2Usb"
        Me.rbPC2Usb.Size = New System.Drawing.Size(127, 17)
        Me.rbPC2Usb.TabIndex = 1
        Me.rbPC2Usb.TabStop = True
        Me.rbPC2Usb.Text = "PC → USB  (Backup)"
        Me.rbPC2Usb.UseVisualStyleBackColor = True
        '
        'rbUsb2PC
        '
        Me.rbUsb2PC.AutoSize = True
        Me.rbUsb2PC.Location = New System.Drawing.Point(11, 46)
        Me.rbUsb2PC.Name = "rbUsb2PC"
        Me.rbUsb2PC.Size = New System.Drawing.Size(127, 17)
        Me.rbUsb2PC.TabIndex = 0
        Me.rbUsb2PC.Text = "USB → PC  (Restore)"
        Me.rbUsb2PC.UseVisualStyleBackColor = True
        '
        'grpBackupSource
        '
        Me.grpBackupSource.Controls.Add(Me.Label24)
        Me.grpBackupSource.Controls.Add(Me.txtSQLServer_Pharmacy2013)
        Me.grpBackupSource.Controls.Add(Me.txtSQLServer_Pharmakon)
        Me.grpBackupSource.Controls.Add(Me.txtSourceFarmnet_mdf)
        Me.grpBackupSource.Controls.Add(Me.txtPCName)
        Me.grpBackupSource.Controls.Add(Me.GroupBox6)
        Me.grpBackupSource.Controls.Add(Me.txtConnectionString)
        Me.grpBackupSource.Controls.Add(Me.Label14)
        Me.grpBackupSource.Controls.Add(Me.btnClose)
        Me.grpBackupSource.Controls.Add(Me.lblLastUpdatedDB2)
        Me.grpBackupSource.Controls.Add(Me.lblLastUpdatedDB1)
        Me.grpBackupSource.Controls.Add(Me.lblLastMod_DB2)
        Me.grpBackupSource.Controls.Add(Me.lblLabel)
        Me.grpBackupSource.Controls.Add(Me.txtSourceFarmnetDB)
        Me.grpBackupSource.Controls.Add(Me.Label18)
        Me.grpBackupSource.Controls.Add(Me.btnOpenDestinationFolder)
        Me.grpBackupSource.Controls.Add(Me.btnOpenFolderVS)
        Me.grpBackupSource.Controls.Add(Me.txtDestinationDrive)
        Me.grpBackupSource.Controls.Add(Me.Label19)
        Me.grpBackupSource.Controls.Add(Me.txtSourceFolderVS)
        Me.grpBackupSource.Controls.Add(Me.btnOpenFolderDB)
        Me.grpBackupSource.Controls.Add(Me.txtDB2)
        Me.grpBackupSource.Controls.Add(Me.txtSourceDB)
        Me.grpBackupSource.Controls.Add(Me.Label7)
        Me.grpBackupSource.Controls.Add(Me.txtDB1)
        Me.grpBackupSource.Location = New System.Drawing.Point(18, 16)
        Me.grpBackupSource.Name = "grpBackupSource"
        Me.grpBackupSource.Size = New System.Drawing.Size(804, 303)
        Me.grpBackupSource.TabIndex = 42
        Me.grpBackupSource.TabStop = False
        Me.grpBackupSource.Text = "Directories"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label24.Location = New System.Drawing.Point(639, 118)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(65, 13)
        Me.Label24.TabIndex = 63
        Me.Label24.Text = "SQL servers"
        '
        'txtSQLServer_Pharmacy2013
        '
        Me.txtSQLServer_Pharmacy2013.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSQLServer_Pharmacy2013.Location = New System.Drawing.Point(642, 160)
        Me.txtSQLServer_Pharmacy2013.Name = "txtSQLServer_Pharmacy2013"
        Me.txtSQLServer_Pharmacy2013.Size = New System.Drawing.Size(134, 20)
        Me.txtSQLServer_Pharmacy2013.TabIndex = 62
        Me.txtSQLServer_Pharmacy2013.Text = "..."
        '
        'txtSQLServer_Pharmakon
        '
        Me.txtSQLServer_Pharmakon.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSQLServer_Pharmakon.Location = New System.Drawing.Point(642, 134)
        Me.txtSQLServer_Pharmakon.Name = "txtSQLServer_Pharmakon"
        Me.txtSQLServer_Pharmakon.Size = New System.Drawing.Size(134, 20)
        Me.txtSQLServer_Pharmakon.TabIndex = 61
        Me.txtSQLServer_Pharmakon.Text = "..."
        '
        'txtSourceFarmnet_mdf
        '
        Me.txtSourceFarmnet_mdf.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSourceFarmnet_mdf.Location = New System.Drawing.Point(180, 81)
        Me.txtSourceFarmnet_mdf.Name = "txtSourceFarmnet_mdf"
        Me.txtSourceFarmnet_mdf.Size = New System.Drawing.Size(104, 20)
        Me.txtSourceFarmnet_mdf.TabIndex = 60
        Me.txtSourceFarmnet_mdf.Text = "???"
        '
        'txtPCName
        '
        Me.txtPCName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtPCName.Location = New System.Drawing.Point(445, 196)
        Me.txtPCName.Name = "txtPCName"
        Me.txtPCName.Size = New System.Drawing.Size(169, 20)
        Me.txtPCName.TabIndex = 59
        Me.txtPCName.Text = "???"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.rbDBStation2)
        Me.GroupBox6.Controls.Add(Me.rbDBStation1)
        Me.GroupBox6.Location = New System.Drawing.Point(649, 204)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(80, 77)
        Me.GroupBox6.TabIndex = 58
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Location DB"
        '
        'rbDBStation2
        '
        Me.rbDBStation2.AutoSize = True
        Me.rbDBStation2.Checked = True
        Me.rbDBStation2.Location = New System.Drawing.Point(12, 47)
        Me.rbDBStation2.Name = "rbDBStation2"
        Me.rbDBStation2.Size = New System.Drawing.Size(43, 17)
        Me.rbDBStation2.TabIndex = 45
        Me.rbDBStation2.TabStop = True
        Me.rbDBStation2.Text = "Aux"
        Me.rbDBStation2.UseVisualStyleBackColor = True
        '
        'rbDBStation1
        '
        Me.rbDBStation1.AutoSize = True
        Me.rbDBStation1.Location = New System.Drawing.Point(12, 24)
        Me.rbDBStation1.Name = "rbDBStation1"
        Me.rbDBStation1.Size = New System.Drawing.Size(48, 17)
        Me.rbDBStation1.TabIndex = 44
        Me.rbDBStation1.Text = "Main"
        Me.rbDBStation1.UseVisualStyleBackColor = True
        '
        'txtConnectionString
        '
        Me.txtConnectionString.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtConnectionString.Location = New System.Drawing.Point(6, 134)
        Me.txtConnectionString.Name = "txtConnectionString"
        Me.txtConnectionString.Size = New System.Drawing.Size(608, 20)
        Me.txtConnectionString.TabIndex = 56
        Me.txtConnectionString.Text = "???"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label14.Location = New System.Drawing.Point(6, 118)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(91, 13)
        Me.Label14.TabIndex = 57
        Me.Label14.Text = "Connection String"
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnClose.Image = CType(resources.GetObject("btnClose.Image"), System.Drawing.Image)
        Me.btnClose.Location = New System.Drawing.Point(735, 245)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(54, 52)
        Me.btnClose.TabIndex = 54
        Me.btnClose.UseVisualStyleBackColor = True
        Me.btnClose.Visible = False
        '
        'lblLastUpdatedDB2
        '
        Me.lblLastUpdatedDB2.AutoSize = True
        Me.lblLastUpdatedDB2.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdatedDB2.Location = New System.Drawing.Point(625, 73)
        Me.lblLastUpdatedDB2.Name = "lblLastUpdatedDB2"
        Me.lblLastUpdatedDB2.Size = New System.Drawing.Size(15, 12)
        Me.lblLastUpdatedDB2.TabIndex = 55
        Me.lblLastUpdatedDB2.Text = "??"
        '
        'lblLastUpdatedDB1
        '
        Me.lblLastUpdatedDB1.AutoSize = True
        Me.lblLastUpdatedDB1.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastUpdatedDB1.Location = New System.Drawing.Point(625, 47)
        Me.lblLastUpdatedDB1.Name = "lblLastUpdatedDB1"
        Me.lblLastUpdatedDB1.Size = New System.Drawing.Size(15, 12)
        Me.lblLastUpdatedDB1.TabIndex = 54
        Me.lblLastUpdatedDB1.Text = "??"
        '
        'lblLastMod_DB2
        '
        Me.lblLastMod_DB2.AutoSize = True
        Me.lblLastMod_DB2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLastMod_DB2.Location = New System.Drawing.Point(664, 72)
        Me.lblLastMod_DB2.Name = "lblLastMod_DB2"
        Me.lblLastMod_DB2.Size = New System.Drawing.Size(0, 13)
        Me.lblLastMod_DB2.TabIndex = 53
        '
        'lblLabel
        '
        Me.lblLabel.AutoSize = True
        Me.lblLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.lblLabel.Location = New System.Drawing.Point(640, 28)
        Me.lblLabel.Name = "lblLabel"
        Me.lblLabel.Size = New System.Drawing.Size(63, 12)
        Me.lblLabel.TabIndex = 52
        Me.lblLabel.Text = "Database Info"
        '
        'txtSourceFarmnetDB
        '
        Me.txtSourceFarmnetDB.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSourceFarmnetDB.Location = New System.Drawing.Point(6, 81)
        Me.txtSourceFarmnetDB.Name = "txtSourceFarmnetDB"
        Me.txtSourceFarmnetDB.Size = New System.Drawing.Size(168, 20)
        Me.txtSourceFarmnetDB.TabIndex = 46
        Me.txtSourceFarmnetDB.Text = "???"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label18.Location = New System.Drawing.Point(6, 245)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(57, 13)
        Me.Label18.TabIndex = 41
        Me.Label18.Text = "USB Drive"
        '
        'btnOpenDestinationFolder
        '
        Me.btnOpenDestinationFolder.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnOpenDestinationFolder.Location = New System.Drawing.Point(132, 258)
        Me.btnOpenDestinationFolder.Name = "btnOpenDestinationFolder"
        Me.btnOpenDestinationFolder.Size = New System.Drawing.Size(42, 23)
        Me.btnOpenDestinationFolder.TabIndex = 43
        Me.btnOpenDestinationFolder.Text = "Open"
        Me.btnOpenDestinationFolder.UseVisualStyleBackColor = True
        '
        'btnOpenFolderVS
        '
        Me.btnOpenFolderVS.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnOpenFolderVS.Location = New System.Drawing.Point(364, 193)
        Me.btnOpenFolderVS.Name = "btnOpenFolderVS"
        Me.btnOpenFolderVS.Size = New System.Drawing.Size(42, 23)
        Me.btnOpenFolderVS.TabIndex = 45
        Me.btnOpenFolderVS.Text = "Open"
        Me.btnOpenFolderVS.UseVisualStyleBackColor = True
        '
        'txtDestinationDrive
        '
        Me.txtDestinationDrive.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtDestinationDrive.Location = New System.Drawing.Point(8, 261)
        Me.txtDestinationDrive.Name = "txtDestinationDrive"
        Me.txtDestinationDrive.Size = New System.Drawing.Size(118, 20)
        Me.txtDestinationDrive.TabIndex = 40
        Me.txtDestinationDrive.Text = "D:\PharmacyBackup"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label19.Location = New System.Drawing.Point(9, 180)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(92, 13)
        Me.Label19.TabIndex = 44
        Me.Label19.Text = "Visual Studio Files"
        '
        'txtSourceFolderVS
        '
        Me.txtSourceFolderVS.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSourceFolderVS.Location = New System.Drawing.Point(11, 196)
        Me.txtSourceFolderVS.Name = "txtSourceFolderVS"
        Me.txtSourceFolderVS.Size = New System.Drawing.Size(347, 20)
        Me.txtSourceFolderVS.TabIndex = 43
        Me.txtSourceFolderVS.Text = "???"
        '
        'btnOpenFolderDB
        '
        Me.btnOpenFolderDB.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnOpenFolderDB.Location = New System.Drawing.Point(426, 41)
        Me.btnOpenFolderDB.Name = "btnOpenFolderDB"
        Me.btnOpenFolderDB.Size = New System.Drawing.Size(42, 23)
        Me.btnOpenFolderDB.TabIndex = 42
        Me.btnOpenFolderDB.Text = "Open"
        Me.btnOpenFolderDB.UseVisualStyleBackColor = True
        '
        'txtDB2
        '
        Me.txtDB2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtDB2.Location = New System.Drawing.Point(485, 69)
        Me.txtDB2.Name = "txtDB2"
        Me.txtDB2.Size = New System.Drawing.Size(134, 20)
        Me.txtDB2.TabIndex = 40
        Me.txtDB2.Text = "..."
        '
        'txtSourceDB
        '
        Me.txtSourceDB.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSourceDB.Location = New System.Drawing.Point(6, 43)
        Me.txtSourceDB.Name = "txtSourceDB"
        Me.txtSourceDB.Size = New System.Drawing.Size(414, 20)
        Me.txtSourceDB.TabIndex = 36
        Me.txtSourceDB.Text = "???"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label7.Location = New System.Drawing.Point(6, 27)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(58, 13)
        Me.Label7.TabIndex = 37
        Me.Label7.Text = "Databases"
        '
        'txtDB1
        '
        Me.txtDB1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtDB1.Location = New System.Drawing.Point(485, 43)
        Me.txtDB1.Name = "txtDB1"
        Me.txtDB1.Size = New System.Drawing.Size(134, 20)
        Me.txtDB1.TabIndex = 38
        Me.txtDB1.Text = "..."
        '
        'tbpAgoresSold
        '
        Me.tbpAgoresSold.Controls.Add(Me.rtxtAgoresSoldMessage)
        Me.tbpAgoresSold.Controls.Add(Me.cbAgoresOrSold)
        Me.tbpAgoresSold.Controls.Add(Me.btnAgoresSoldDeleteRecord)
        Me.tbpAgoresSold.Controls.Add(Me.Label22)
        Me.tbpAgoresSold.Controls.Add(Me.txtAgoresSoldSearch)
        Me.tbpAgoresSold.Controls.Add(Me.dgvAgoresSold)
        Me.tbpAgoresSold.Location = New System.Drawing.Point(4, 22)
        Me.tbpAgoresSold.Name = "tbpAgoresSold"
        Me.tbpAgoresSold.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpAgoresSold.Size = New System.Drawing.Size(996, 696)
        Me.tbpAgoresSold.TabIndex = 10
        Me.tbpAgoresSold.Text = "Έσοδα-Έξοδα"
        Me.tbpAgoresSold.UseVisualStyleBackColor = True
        '
        'rtxtAgoresSoldMessage
        '
        Me.rtxtAgoresSoldMessage.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtAgoresSoldMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtAgoresSoldMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtAgoresSoldMessage.Location = New System.Drawing.Point(12, 582)
        Me.rtxtAgoresSoldMessage.Name = "rtxtAgoresSoldMessage"
        Me.rtxtAgoresSoldMessage.ReadOnly = True
        Me.rtxtAgoresSoldMessage.Size = New System.Drawing.Size(670, 24)
        Me.rtxtAgoresSoldMessage.TabIndex = 45
        Me.rtxtAgoresSoldMessage.Text = ""
        '
        'cbAgoresOrSold
        '
        Me.cbAgoresOrSold.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cbAgoresOrSold.FormattingEnabled = True
        Me.cbAgoresOrSold.Items.AddRange(New Object() {"Έσοδα (Πωλήσεις)", "Έξοδα (Δαπάνες)"})
        Me.cbAgoresOrSold.Location = New System.Drawing.Point(202, 14)
        Me.cbAgoresOrSold.Name = "cbAgoresOrSold"
        Me.cbAgoresOrSold.Size = New System.Drawing.Size(127, 23)
        Me.cbAgoresOrSold.TabIndex = 44
        Me.cbAgoresOrSold.Text = "Έσοδα (Πωλήσεις)"
        '
        'btnAgoresSoldDeleteRecord
        '
        Me.btnAgoresSoldDeleteRecord.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnAgoresSoldDeleteRecord.Location = New System.Drawing.Point(688, 581)
        Me.btnAgoresSoldDeleteRecord.Name = "btnAgoresSoldDeleteRecord"
        Me.btnAgoresSoldDeleteRecord.Size = New System.Drawing.Size(75, 23)
        Me.btnAgoresSoldDeleteRecord.TabIndex = 41
        Me.btnAgoresSoldDeleteRecord.Text = "Διαγραφή"
        Me.btnAgoresSoldDeleteRecord.UseVisualStyleBackColor = True
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label22.Location = New System.Drawing.Point(12, 19)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(63, 13)
        Me.Label22.TabIndex = 40
        Me.Label22.Text = "Αναζήτηση"
        '
        'txtAgoresSoldSearch
        '
        Me.txtAgoresSoldSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtAgoresSoldSearch.Location = New System.Drawing.Point(81, 16)
        Me.txtAgoresSoldSearch.Name = "txtAgoresSoldSearch"
        Me.txtAgoresSoldSearch.Size = New System.Drawing.Size(115, 20)
        Me.txtAgoresSoldSearch.TabIndex = 39
        '
        'dgvAgoresSold
        '
        Me.dgvAgoresSold.AllowUserToResizeColumns = False
        Me.dgvAgoresSold.AllowUserToResizeRows = False
        Me.dgvAgoresSold.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAgoresSold.Location = New System.Drawing.Point(12, 49)
        Me.dgvAgoresSold.Name = "dgvAgoresSold"
        Me.dgvAgoresSold.RowHeadersVisible = False
        Me.dgvAgoresSold.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvAgoresSold.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvAgoresSold.ShowEditingIcon = False
        Me.dgvAgoresSold.Size = New System.Drawing.Size(751, 526)
        Me.dgvAgoresSold.TabIndex = 38
        '
        'tbpTameia
        '
        Me.tbpTameia.Controls.Add(Me.cboTameia)
        Me.tbpTameia.Controls.Add(Me.rtxtTameiaAsked2)
        Me.tbpTameia.Controls.Add(Me.Label4)
        Me.tbpTameia.Controls.Add(Me.txtSearchTameia)
        Me.tbpTameia.Controls.Add(Me.btnDeleteTameiaGiven)
        Me.tbpTameia.Controls.Add(Me.rtxtTameiaGiven)
        Me.tbpTameia.Controls.Add(Me.dgvTameiaGiven)
        Me.tbpTameia.Controls.Add(Me.btnDeleteTameiaAsked)
        Me.tbpTameia.Controls.Add(Me.rtxtTameiaAsked)
        Me.tbpTameia.Controls.Add(Me.dgvTameiaAsked)
        Me.tbpTameia.Location = New System.Drawing.Point(4, 22)
        Me.tbpTameia.Name = "tbpTameia"
        Me.tbpTameia.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpTameia.Size = New System.Drawing.Size(996, 696)
        Me.tbpTameia.TabIndex = 12
        Me.tbpTameia.Text = "Ταμεία"
        Me.tbpTameia.UseVisualStyleBackColor = True
        '
        'cboTameia
        '
        Me.cboTameia.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.cboTameia.FormattingEnabled = True
        Me.cboTameia.Items.AddRange(New Object() {"ΟΛΑ", "ΕΟΠΥΥ", "ΕΔΟΕΑΠ", "ΕΥΔΑΠ", "ΤΥΠΕΤ", "ΤΕΑΠΑΣΑ"})
        Me.cboTameia.Location = New System.Drawing.Point(240, 7)
        Me.cboTameia.Name = "cboTameia"
        Me.cboTameia.Size = New System.Drawing.Size(109, 23)
        Me.cboTameia.TabIndex = 68
        Me.cboTameia.Text = "ΟΛΑ"
        '
        'rtxtTameiaAsked2
        '
        Me.rtxtTameiaAsked2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTameiaAsked2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTameiaAsked2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTameiaAsked2.Location = New System.Drawing.Point(9, 366)
        Me.rtxtTameiaAsked2.Name = "rtxtTameiaAsked2"
        Me.rtxtTameiaAsked2.ReadOnly = True
        Me.rtxtTameiaAsked2.Size = New System.Drawing.Size(683, 24)
        Me.rtxtTameiaAsked2.TabIndex = 62
        Me.rtxtTameiaAsked2.Text = ""
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label4.Location = New System.Drawing.Point(14, 12)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(66, 13)
        Me.Label4.TabIndex = 61
        Me.Label4.Text = "Αναζήτηση:"
        '
        'txtSearchTameia
        '
        Me.txtSearchTameia.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtSearchTameia.Location = New System.Drawing.Point(86, 9)
        Me.txtSearchTameia.Name = "txtSearchTameia"
        Me.txtSearchTameia.Size = New System.Drawing.Size(134, 20)
        Me.txtSearchTameia.TabIndex = 60
        '
        'btnDeleteTameiaGiven
        '
        Me.btnDeleteTameiaGiven.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteTameiaGiven.Location = New System.Drawing.Point(698, 583)
        Me.btnDeleteTameiaGiven.Name = "btnDeleteTameiaGiven"
        Me.btnDeleteTameiaGiven.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteTameiaGiven.TabIndex = 59
        Me.btnDeleteTameiaGiven.Text = "Διαγραφή"
        Me.btnDeleteTameiaGiven.UseVisualStyleBackColor = True
        '
        'rtxtTameiaGiven
        '
        Me.rtxtTameiaGiven.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTameiaGiven.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTameiaGiven.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTameiaGiven.Location = New System.Drawing.Point(9, 582)
        Me.rtxtTameiaGiven.Name = "rtxtTameiaGiven"
        Me.rtxtTameiaGiven.ReadOnly = True
        Me.rtxtTameiaGiven.Size = New System.Drawing.Size(683, 24)
        Me.rtxtTameiaGiven.TabIndex = 58
        Me.rtxtTameiaGiven.Text = ""
        '
        'dgvTameiaGiven
        '
        Me.dgvTameiaGiven.AllowUserToResizeColumns = False
        Me.dgvTameiaGiven.AllowUserToResizeRows = False
        Me.dgvTameiaGiven.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTameiaGiven.Location = New System.Drawing.Point(9, 405)
        Me.dgvTameiaGiven.MultiSelect = False
        Me.dgvTameiaGiven.Name = "dgvTameiaGiven"
        Me.dgvTameiaGiven.RowHeadersVisible = False
        Me.dgvTameiaGiven.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvTameiaGiven.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTameiaGiven.ShowEditingIcon = False
        Me.dgvTameiaGiven.Size = New System.Drawing.Size(754, 169)
        Me.dgvTameiaGiven.TabIndex = 57
        '
        'btnDeleteTameiaAsked
        '
        Me.btnDeleteTameiaAsked.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteTameiaAsked.Location = New System.Drawing.Point(698, 341)
        Me.btnDeleteTameiaAsked.Name = "btnDeleteTameiaAsked"
        Me.btnDeleteTameiaAsked.Size = New System.Drawing.Size(65, 23)
        Me.btnDeleteTameiaAsked.TabIndex = 54
        Me.btnDeleteTameiaAsked.Text = "Διαγραφή"
        Me.btnDeleteTameiaAsked.UseVisualStyleBackColor = True
        '
        'rtxtTameiaAsked
        '
        Me.rtxtTameiaAsked.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtTameiaAsked.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtTameiaAsked.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtTameiaAsked.Location = New System.Drawing.Point(9, 340)
        Me.rtxtTameiaAsked.Name = "rtxtTameiaAsked"
        Me.rtxtTameiaAsked.ReadOnly = True
        Me.rtxtTameiaAsked.Size = New System.Drawing.Size(683, 24)
        Me.rtxtTameiaAsked.TabIndex = 51
        Me.rtxtTameiaAsked.Text = ""
        '
        'dgvTameiaAsked
        '
        Me.dgvTameiaAsked.AllowUserToResizeColumns = False
        Me.dgvTameiaAsked.AllowUserToResizeRows = False
        Me.dgvTameiaAsked.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTameiaAsked.Location = New System.Drawing.Point(9, 37)
        Me.dgvTameiaAsked.MultiSelect = False
        Me.dgvTameiaAsked.Name = "dgvTameiaAsked"
        Me.dgvTameiaAsked.RowHeadersVisible = False
        Me.dgvTameiaAsked.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvTameiaAsked.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTameiaAsked.ShowEditingIcon = False
        Me.dgvTameiaAsked.Size = New System.Drawing.Size(754, 299)
        Me.dgvTameiaAsked.TabIndex = 50
        '
        'tmrRerunDatagridV
        '
        Me.tmrRerunDatagridV.Interval = 200
        '
        'tmrExpirations
        '
        Me.tmrExpirations.Interval = 3000
        '
        'dgvExchangeFrom2
        '
        Me.dgvExchangeFrom2.AllowUserToAddRows = False
        Me.dgvExchangeFrom2.AllowUserToDeleteRows = False
        Me.dgvExchangeFrom2.AllowUserToResizeColumns = False
        Me.dgvExchangeFrom2.AllowUserToResizeRows = False
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvExchangeFrom2.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.dgvExchangeFrom2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvExchangeFrom2.DefaultCellStyle = DataGridViewCellStyle8
        Me.dgvExchangeFrom2.Location = New System.Drawing.Point(20, 19)
        Me.dgvExchangeFrom2.Name = "dgvExchangeFrom2"
        Me.dgvExchangeFrom2.ReadOnly = True
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvExchangeFrom2.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.dgvExchangeFrom2.RowHeadersVisible = False
        Me.dgvExchangeFrom2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvExchangeFrom2.ShowEditingIcon = False
        Me.dgvExchangeFrom2.Size = New System.Drawing.Size(730, 176)
        Me.dgvExchangeFrom2.TabIndex = 7
        '
        'rtxtExchangeFrom2
        '
        Me.rtxtExchangeFrom2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtExchangeFrom2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtExchangeFrom2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtExchangeFrom2.Location = New System.Drawing.Point(15, 205)
        Me.rtxtExchangeFrom2.Name = "rtxtExchangeFrom2"
        Me.rtxtExchangeFrom2.Size = New System.Drawing.Size(491, 26)
        Me.rtxtExchangeFrom2.TabIndex = 23
        Me.rtxtExchangeFrom2.Text = ""
        '
        'btnEditExchangeFrom
        '
        Me.btnEditExchangeFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnEditExchangeFrom.Location = New System.Drawing.Point(598, 205)
        Me.btnEditExchangeFrom.Name = "btnEditExchangeFrom"
        Me.btnEditExchangeFrom.Size = New System.Drawing.Size(75, 23)
        Me.btnEditExchangeFrom.TabIndex = 28
        Me.btnEditExchangeFrom.Text = "Edit"
        Me.btnEditExchangeFrom.UseVisualStyleBackColor = True
        '
        'btnSaveExchangeFrom
        '
        Me.btnSaveExchangeFrom.Enabled = False
        Me.btnSaveExchangeFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnSaveExchangeFrom.Location = New System.Drawing.Point(517, 205)
        Me.btnSaveExchangeFrom.Name = "btnSaveExchangeFrom"
        Me.btnSaveExchangeFrom.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveExchangeFrom.TabIndex = 27
        Me.btnSaveExchangeFrom.Text = "Save"
        Me.btnSaveExchangeFrom.UseVisualStyleBackColor = True
        '
        'btnDeleteExchangeFrom
        '
        Me.btnDeleteExchangeFrom.Enabled = False
        Me.btnDeleteExchangeFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteExchangeFrom.Location = New System.Drawing.Point(679, 205)
        Me.btnDeleteExchangeFrom.Name = "btnDeleteExchangeFrom"
        Me.btnDeleteExchangeFrom.Size = New System.Drawing.Size(75, 23)
        Me.btnDeleteExchangeFrom.TabIndex = 29
        Me.btnDeleteExchangeFrom.Text = "Delete"
        Me.btnDeleteExchangeFrom.UseVisualStyleBackColor = True
        '
        'cboMyPharmacist
        '
        Me.cboMyPharmacist.FormattingEnabled = True
        Me.cboMyPharmacist.Location = New System.Drawing.Point(168, 13)
        Me.cboMyPharmacist.Name = "cboMyPharmacist"
        Me.cboMyPharmacist.Size = New System.Drawing.Size(102, 21)
        Me.cboMyPharmacist.TabIndex = 25
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(17, 16)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(145, 13)
        Me.Label16.TabIndex = 26
        '
        'cboIntervall
        '
        Me.cboIntervall.Items.AddRange(New Object() {"Όλες", "Τρέχων μήνας", "Χθες", "Σήμερα"})
        Me.cboIntervall.Location = New System.Drawing.Point(122, 40)
        Me.cboIntervall.Name = "cboIntervall"
        Me.cboIntervall.Size = New System.Drawing.Size(110, 21)
        Me.cboIntervall.TabIndex = 24
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(19, 43)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(97, 13)
        Me.Label9.TabIndex = 27
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(313, 21)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(106, 13)
        Me.Label8.TabIndex = 29
        '
        'txtBalance
        '
        Me.txtBalance.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtBalance.Location = New System.Drawing.Point(425, 13)
        Me.txtBalance.Name = "txtBalance"
        Me.txtBalance.Size = New System.Drawing.Size(73, 26)
        Me.txtBalance.TabIndex = 30
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(693, 13)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 31
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'dtpExchangesNew
        '
        Me.dtpExchangesNew.Enabled = False
        Me.dtpExchangesNew.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.dtpExchangesNew.Location = New System.Drawing.Point(274, 45)
        Me.dtpExchangesNew.Name = "dtpExchangesNew"
        Me.dtpExchangesNew.Size = New System.Drawing.Size(244, 20)
        Me.dtpExchangesNew.TabIndex = 35
        '
        'btnEditDrugList2
        '
        Me.btnEditDrugList2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnEditDrugList2.Location = New System.Drawing.Point(664, 45)
        Me.btnEditDrugList2.Name = "btnEditDrugList2"
        Me.btnEditDrugList2.Size = New System.Drawing.Size(104, 23)
        Me.btnEditDrugList2.TabIndex = 40
        Me.btnEditDrugList2.Text = "Λίστα προιόντων"
        Me.btnEditDrugList2.UseVisualStyleBackColor = True
        '
        'chkWithExpir
        '
        Me.chkWithExpir.AutoSize = True
        Me.chkWithExpir.Location = New System.Drawing.Point(570, 17)
        Me.chkWithExpir.Name = "chkWithExpir"
        Me.chkWithExpir.Size = New System.Drawing.Size(74, 17)
        Me.chkWithExpir.TabIndex = 41
        Me.chkWithExpir.Text = "Με λήξεις"
        Me.chkWithExpir.UseVisualStyleBackColor = True
        '
        'dgvExchangeTo2
        '
        Me.dgvExchangeTo2.AllowUserToAddRows = False
        Me.dgvExchangeTo2.AllowUserToDeleteRows = False
        Me.dgvExchangeTo2.AllowUserToResizeColumns = False
        Me.dgvExchangeTo2.AllowUserToResizeRows = False
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvExchangeTo2.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle10
        Me.dgvExchangeTo2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvExchangeTo2.DefaultCellStyle = DataGridViewCellStyle11
        Me.dgvExchangeTo2.Location = New System.Drawing.Point(20, 18)
        Me.dgvExchangeTo2.Name = "dgvExchangeTo2"
        Me.dgvExchangeTo2.ReadOnly = True
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvExchangeTo2.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.dgvExchangeTo2.RowHeadersVisible = False
        Me.dgvExchangeTo2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvExchangeTo2.ShowEditingIcon = False
        Me.dgvExchangeTo2.Size = New System.Drawing.Size(730, 176)
        Me.dgvExchangeTo2.TabIndex = 7
        '
        'rtxtExchangeTo2
        '
        Me.rtxtExchangeTo2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.rtxtExchangeTo2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxtExchangeTo2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.rtxtExchangeTo2.Location = New System.Drawing.Point(15, 205)
        Me.rtxtExchangeTo2.Name = "rtxtExchangeTo2"
        Me.rtxtExchangeTo2.Size = New System.Drawing.Size(491, 27)
        Me.rtxtExchangeTo2.TabIndex = 23
        Me.rtxtExchangeTo2.Text = ""
        '
        'btnEditExchangeTo
        '
        Me.btnEditExchangeTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnEditExchangeTo.Location = New System.Drawing.Point(598, 205)
        Me.btnEditExchangeTo.Name = "btnEditExchangeTo"
        Me.btnEditExchangeTo.Size = New System.Drawing.Size(75, 23)
        Me.btnEditExchangeTo.TabIndex = 31
        Me.btnEditExchangeTo.Text = "Edit"
        Me.btnEditExchangeTo.UseVisualStyleBackColor = True
        '
        'btnSaveExchangeTo
        '
        Me.btnSaveExchangeTo.Enabled = False
        Me.btnSaveExchangeTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnSaveExchangeTo.Location = New System.Drawing.Point(517, 205)
        Me.btnSaveExchangeTo.Name = "btnSaveExchangeTo"
        Me.btnSaveExchangeTo.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveExchangeTo.TabIndex = 30
        Me.btnSaveExchangeTo.Text = "Save"
        Me.btnSaveExchangeTo.UseVisualStyleBackColor = True
        '
        'btnDeleteExchangeTo
        '
        Me.btnDeleteExchangeTo.Enabled = False
        Me.btnDeleteExchangeTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.btnDeleteExchangeTo.Location = New System.Drawing.Point(679, 205)
        Me.btnDeleteExchangeTo.Name = "btnDeleteExchangeTo"
        Me.btnDeleteExchangeTo.Size = New System.Drawing.Size(75, 23)
        Me.btnDeleteExchangeTo.TabIndex = 32
        Me.btnDeleteExchangeTo.Text = "Delete"
        Me.btnDeleteExchangeTo.UseVisualStyleBackColor = True
        '
        'tmrSearchCustomers
        '
        Me.tmrSearchCustomers.Interval = 500
        '
        'tmrExpirationKeystrokes
        '
        Me.tmrExpirationKeystrokes.Interval = 1000
        '
        'frmCustomers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1009, 731)
        Me.Controls.Add(Me.tbcMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmCustomers"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Διαχείρηση Φαρμακείου"
        Me.tbcMain.ResumeLayout(False)
        Me.tbpExchanges.ResumeLayout(False)
        Me.tbpExchanges.PerformLayout()
        Me.GroupBox11.ResumeLayout(False)
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        CType(Me.dgvTakenFrom, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvGivenTo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpCustomerDebts.ResumeLayout(False)
        Me.tbpCustomerDebts.PerformLayout()
        Me.grpDrugsOnLoan.ResumeLayout(False)
        Me.grpDrugsOnLoan.PerformLayout()
        CType(Me.dgvDrugsOnLoan, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpCustHairDies.ResumeLayout(False)
        Me.grpCustHairDies.PerformLayout()
        CType(Me.dgvHairdiesList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpCustDebts.ResumeLayout(False)
        Me.grpCustDebts.PerformLayout()
        CType(Me.dgvDebtsList, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvCustomers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpPrescriptions.ResumeLayout(False)
        Me.grpPrescriptions.PerformLayout()
        CType(Me.dgvPrescriptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpPricesParadrugs.ResumeLayout(False)
        Me.tbpPricesParadrugs.PerformLayout()
        Me.grpExpirationList.ResumeLayout(False)
        Me.grpExpirationList.PerformLayout()
        CType(Me.dgvExpirations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpDrugsOrParadrugs.ResumeLayout(False)
        Me.grpDrugsOrParadrugs.PerformLayout()
        Me.grpCalculateLianiki.ResumeLayout(False)
        Me.grpCalculateLianiki.PerformLayout()
        Me.grpLastUpdateParadrugs.ResumeLayout(False)
        Me.grpSearchParadrugOptions.ResumeLayout(False)
        Me.grpSearchParadrugOptions.PerformLayout()
        CType(Me.dgvPricesParadrugs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpPhones.ResumeLayout(False)
        Me.tbpPhones.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.dgvPhones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpBackup.ResumeLayout(False)
        Me.tbpBackup.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.grpBackupDestination.ResumeLayout(False)
        Me.grpBackupDestination.PerformLayout()
        Me.grpBackupSource.ResumeLayout(False)
        Me.grpBackupSource.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.tbpAgoresSold.ResumeLayout(False)
        Me.tbpAgoresSold.PerformLayout()
        CType(Me.dgvAgoresSold, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpTameia.ResumeLayout(False)
        Me.tbpTameia.PerformLayout()
        CType(Me.dgvTameiaGiven, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvTameiaAsked, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvExchangeFrom2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvExchangeTo2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrFlashLabel As System.Windows.Forms.Timer
    Friend WithEvents tbcMain As System.Windows.Forms.TabControl
    Friend WithEvents tmrRerunDatagridV As System.Windows.Forms.Timer
    Friend WithEvents tmrExpirations As System.Windows.Forms.Timer
    Friend WithEvents tbpPricesParadrugs As System.Windows.Forms.TabPage
    Friend WithEvents btnDeletePriceParadrugs As System.Windows.Forms.Button
    Friend WithEvents rtxtPricesParadrugs As System.Windows.Forms.RichTextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtSearchPricesParadrugs As System.Windows.Forms.TextBox
    Friend WithEvents dgvPricesParadrugs As System.Windows.Forms.DataGridView
    Friend WithEvents tbpBackup As System.Windows.Forms.TabPage
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtSourceDB As System.Windows.Forms.TextBox
    Friend WithEvents grpBackupDestination As System.Windows.Forms.GroupBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents txtDestinationDrive As System.Windows.Forms.TextBox
    Friend WithEvents grpBackupSource As System.Windows.Forms.GroupBox
    Friend WithEvents btnOpenFolderDB As System.Windows.Forms.Button
    Friend WithEvents txtDB2 As System.Windows.Forms.TextBox
    Friend WithEvents txtDB1 As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txtSourceFolderVS As System.Windows.Forms.TextBox
    Friend WithEvents btnOpenFolderVS As System.Windows.Forms.Button
    Friend WithEvents btnOpenDestinationFolder As System.Windows.Forms.Button
    Friend WithEvents btnBackupRestore As System.Windows.Forms.Button
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents rbPC2Usb As System.Windows.Forms.RadioButton
    Friend WithEvents rbUsb2PC As System.Windows.Forms.RadioButton
    Friend WithEvents lstMessage As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents rbWhereSpiti As System.Windows.Forms.RadioButton
    Friend WithEvents rbWhereFarm1 As System.Windows.Forms.RadioButton
    Friend WithEvents tbpExchanges As System.Windows.Forms.TabPage
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents cbExchangers As System.Windows.Forms.ComboBox
    Friend WithEvents rtxtTakenFrom As System.Windows.Forms.RichTextBox
    Friend WithEvents rtxtGivenTo As System.Windows.Forms.RichTextBox
    Friend WithEvents dgvTakenFrom As System.Windows.Forms.DataGridView
    Friend WithEvents dgvGivenTo As System.Windows.Forms.DataGridView
    Friend WithEvents chkAutoInsertName As System.Windows.Forms.CheckBox
    Friend WithEvents btnDeleteGivenTo As System.Windows.Forms.Button
    Friend WithEvents btnDeleteTakenFrom As System.Windows.Forms.Button
    Friend WithEvents lblExchangesBalance2 As System.Windows.Forms.Label
    Friend WithEvents btnExchangesBalancePerPharmacist As System.Windows.Forms.Button
    Friend WithEvents tbpAgoresSold As System.Windows.Forms.TabPage
    Friend WithEvents btnAgoresSoldDeleteRecord As System.Windows.Forms.Button
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents txtAgoresSoldSearch As System.Windows.Forms.TextBox
    Friend WithEvents dgvAgoresSold As System.Windows.Forms.DataGridView
    Friend WithEvents cbAgoresOrSold As System.Windows.Forms.ComboBox
    Friend WithEvents rtxtAgoresSoldMessage As System.Windows.Forms.RichTextBox
    Friend WithEvents dgvExchangeFrom2 As System.Windows.Forms.DataGridView
    Friend WithEvents rtxtExchangeFrom2 As System.Windows.Forms.RichTextBox
    Friend WithEvents btnEditExchangeFrom As System.Windows.Forms.Button
    Friend WithEvents btnSaveExchangeFrom As System.Windows.Forms.Button
    Friend WithEvents btnDeleteExchangeFrom As System.Windows.Forms.Button
    Friend WithEvents cboMyPharmacist As System.Windows.Forms.ComboBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents cboIntervall As System.Windows.Forms.ComboBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtBalance As System.Windows.Forms.TextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents dtpExchangesNew As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnEditDrugList2 As System.Windows.Forms.Button
    Friend WithEvents chkWithExpir As System.Windows.Forms.CheckBox
    Friend WithEvents dgvExchangeTo2 As System.Windows.Forms.DataGridView
    Friend WithEvents rtxtExchangeTo2 As System.Windows.Forms.RichTextBox
    Friend WithEvents btnEditExchangeTo As System.Windows.Forms.Button
    Friend WithEvents btnSaveExchangeTo As System.Windows.Forms.Button
    Friend WithEvents btnDeleteExchangeTo As System.Windows.Forms.Button
    Friend WithEvents tbpCustomerDebts As System.Windows.Forms.TabPage
    Friend WithEvents grpCustDebts As System.Windows.Forms.GroupBox
    Friend WithEvents btnDeleteDebts As System.Windows.Forms.Button
    Friend WithEvents dgvDebtsList As System.Windows.Forms.DataGridView
    Friend WithEvents btnDeleteCustomer As System.Windows.Forms.Button
    Friend WithEvents rtxtCustomersMessage As System.Windows.Forms.RichTextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtSearchCustomer As System.Windows.Forms.TextBox
    Friend WithEvents dgvCustomers As System.Windows.Forms.DataGridView
    Friend WithEvents btnPayDebts As System.Windows.Forms.Button
    Friend WithEvents lblTotalCustomerDebt As System.Windows.Forms.Label
    Friend WithEvents lblTotalDebtLabel As System.Windows.Forms.Label
    Friend WithEvents grpCustHairDies As System.Windows.Forms.GroupBox
    Friend WithEvents btnDeleteHairdies As System.Windows.Forms.Button
    Friend WithEvents dgvHairdiesList As System.Windows.Forms.DataGridView
    Friend WithEvents lblCustWithNoHairdies As System.Windows.Forms.Label
    Friend WithEvents lblCustWithNoDebts As System.Windows.Forms.Label
    Friend WithEvents tbpTameia As System.Windows.Forms.TabPage
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtSearchTameia As System.Windows.Forms.TextBox
    Friend WithEvents btnDeleteTameiaGiven As System.Windows.Forms.Button
    Friend WithEvents rtxtTameiaGiven As System.Windows.Forms.RichTextBox
    Friend WithEvents dgvTameiaGiven As System.Windows.Forms.DataGridView
    Friend WithEvents btnDeleteTameiaAsked As System.Windows.Forms.Button
    Friend WithEvents rtxtTameiaAsked As System.Windows.Forms.RichTextBox
    Friend WithEvents dgvTameiaAsked As System.Windows.Forms.DataGridView
    Friend WithEvents rtxtTameiaAsked2 As System.Windows.Forms.RichTextBox
    Friend WithEvents rbWhereLaptop As System.Windows.Forms.RadioButton
    Friend WithEvents tbpPhones As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtSearchPhones As System.Windows.Forms.TextBox
    Friend WithEvents btnDeletePhones As System.Windows.Forms.Button
    Friend WithEvents rtxtPhones As System.Windows.Forms.RichTextBox
    Friend WithEvents dgvPhones As System.Windows.Forms.DataGridView
    Friend WithEvents cboTameia As System.Windows.Forms.ComboBox
    Friend WithEvents lblLastBuilded As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents cboFPA_Paradrugs As System.Windows.Forms.ComboBox
    Friend WithEvents txtTotalPrice_Paradrugs As System.Windows.Forms.TextBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents txtProfit_Paradrugs As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents btnClearSearch As System.Windows.Forms.Button
    Friend WithEvents lblPreviousBalanceTakenFrom As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents lblPreviousBalanceGivenTo As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents lblPreviousBalance As System.Windows.Forms.Label
    Friend WithEvents cboPhoneCatalog As System.Windows.Forms.ComboBox
    Friend WithEvents txtSourceFarmnetDB As System.Windows.Forms.TextBox
    Friend WithEvents btnUpdatePharmacy2013C As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rbOnlyVisualBasic As System.Windows.Forms.RadioButton
    Friend WithEvents rbEverything As System.Windows.Forms.RadioButton
    Friend WithEvents rbOnlyDatabases As System.Windows.Forms.RadioButton
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents grpSearchParadrugOptions As System.Windows.Forms.GroupBox
    Friend WithEvents rbByBarcode As System.Windows.Forms.RadioButton
    Friend WithEvents rbByName As System.Windows.Forms.RadioButton
    Friend WithEvents lblLastMod_DB2 As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdatedDB2 As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdatedDB1 As System.Windows.Forms.Label
    Friend WithEvents lblLabel As System.Windows.Forms.Label
    Friend WithEvents grpLastUpdateParadrugs As System.Windows.Forms.GroupBox
    Friend WithEvents lblLastUpdateParadrugs As System.Windows.Forms.Label
    Friend WithEvents grpDrugsOnLoan As System.Windows.Forms.GroupBox
    Friend WithEvents lblCustWithoutDrugsOnLoan As System.Windows.Forms.Label
    Friend WithEvents btnDeleteDrugOnLoan As System.Windows.Forms.Button
    Friend WithEvents dgvDrugsOnLoan As System.Windows.Forms.DataGridView
    Friend WithEvents lblSumDrugsOnLoan As System.Windows.Forms.Label
    Friend WithEvents lblSumDrugsOnLoanLabel As System.Windows.Forms.Label
    Friend WithEvents cboSearchCustomers As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents lblLastUpdateExchanges As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdateDrugsOnLoan As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdateCustomers As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdateDebts As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents lblLastUpdatePhones As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdateHairDies As System.Windows.Forms.Label
    Friend WithEvents grpCalculateLianiki As System.Windows.Forms.GroupBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents grpPrescriptions As System.Windows.Forms.GroupBox
    Friend WithEvents lblLastUpdatePrescriptions As System.Windows.Forms.Label
    Friend WithEvents lblCustWithPrescriptions As System.Windows.Forms.Label
    Friend WithEvents btnDeletePrescriptions As System.Windows.Forms.Button
    Friend WithEvents dgvPrescriptions As System.Windows.Forms.DataGridView
    Friend WithEvents tmrSearchCustomers As System.Windows.Forms.Timer
    Friend WithEvents lblTotalDebtPerCustomer As System.Windows.Forms.Label
    Friend WithEvents chkSelectAll As System.Windows.Forms.CheckBox
    Friend WithEvents lblTotPrescriptions As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtNoCustomers As System.Windows.Forms.TextBox
    Friend WithEvents btnExpirations As System.Windows.Forms.Button
    Friend WithEvents grpDrugsOrParadrugs As System.Windows.Forms.GroupBox
    Friend WithEvents rbParadrugs As System.Windows.Forms.RadioButton
    Friend WithEvents rbDrugs As System.Windows.Forms.RadioButton
    Friend WithEvents grpExpirationList As System.Windows.Forms.GroupBox
    Friend WithEvents txtNoExpirations As System.Windows.Forms.TextBox
    Friend WithEvents dgvExpirations As System.Windows.Forms.DataGridView
    Friend WithEvents btnDeleteExpiration As System.Windows.Forms.Button
    Friend WithEvents btnExpiringDrugs As System.Windows.Forms.Button
    Friend WithEvents chkPairing As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents lblLastUpdated As System.Windows.Forms.Label
    Friend WithEvents txtConnectionString As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents dtpToDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpFromDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents chkManualBarcode As System.Windows.Forms.CheckBox
    Friend WithEvents tmrExpirationKeystrokes As System.Windows.Forms.Timer
    Friend WithEvents btnMyBarcodes As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents rbWhereFarm2 As System.Windows.Forms.RadioButton
    Friend WithEvents lblPCName As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents btnCoppyAppStation1 As System.Windows.Forms.Button
    Friend WithEvents txtRowChanged As System.Windows.Forms.TextBox
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents txtRowChanged2 As System.Windows.Forms.TextBox
    Friend WithEvents txtRowChanged3 As System.Windows.Forms.TextBox
    Friend WithEvents lblNewRecordAdded As System.Windows.Forms.Label
    Friend WithEvents lblNewRecord_Exp As System.Windows.Forms.Label
    Friend WithEvents lblDirtyState_Exp As System.Windows.Forms.Label
    Friend WithEvents lblNewRecord_Debts As System.Windows.Forms.Label
    Friend WithEvents lblDirty_Debts As System.Windows.Forms.Label
    Friend WithEvents lblNewRow_Cust As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents rbDBStation2 As System.Windows.Forms.RadioButton
    Friend WithEvents rbDBStation1 As System.Windows.Forms.RadioButton
    Friend WithEvents lblParadrugName As System.Windows.Forms.Label
    Friend WithEvents rbWhereNikoyla As System.Windows.Forms.RadioButton
    Friend WithEvents rbWhereSaloni As System.Windows.Forms.RadioButton
    Friend WithEvents btnPrintDebtsList As System.Windows.Forms.Button
    Friend WithEvents txtPCName As System.Windows.Forms.TextBox
    Friend WithEvents btnEditExchangers As System.Windows.Forms.Button
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents GroupBox9 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents lblCurrentBalance As System.Windows.Forms.Label
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents GroupBox11 As System.Windows.Forms.GroupBox
    Friend WithEvents rtxtTakenFrom2 As System.Windows.Forms.RichTextBox
    Friend WithEvents rtxtGivenTo2 As System.Windows.Forms.RichTextBox
    Friend WithEvents lblCurrFPA23 As System.Windows.Forms.Label
    Friend WithEvents lblCurrFPA13 As System.Windows.Forms.Label
    Friend WithEvents lblCurrFPA65 As System.Windows.Forms.Label
    Friend WithEvents rtxtCurrentFPA As System.Windows.Forms.RichTextBox
    Friend WithEvents rtxtPreviousFPA As System.Windows.Forms.RichTextBox
    Friend WithEvents rtxtTotalFPA As System.Windows.Forms.RichTextBox
    Friend WithEvents lblFPAInfo As System.Windows.Forms.Label
    Friend WithEvents btnImportExcel As System.Windows.Forms.Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnStopService As Button
    Friend WithEvents btnStartService As Button
    Friend WithEvents txtServiceName As TextBox
    Friend WithEvents Button13 As Button
    Friend WithEvents txtSourceFarmnet_mdf As TextBox
    Friend WithEvents Button14 As Button
    Friend WithEvents rbByQRcode As RadioButton
    Friend WithEvents Button15 As Button
    Friend WithEvents Label24 As Label
    Friend WithEvents txtSQLServer_Pharmacy2013 As TextBox
    Friend WithEvents txtSQLServer_Pharmakon As TextBox
    Friend WithEvents lblAdminInfo As Label
    Friend WithEvents btnAddManualGivenTo As Button
    Friend WithEvents btnAddGivenTo As Button
    Friend WithEvents btnAddManualTakenFrom As Button
    Friend WithEvents btnAddTakenFrom As Button
    Friend WithEvents lblScanHint As Label
    Friend WithEvents btnAddDebt As Button
    Friend WithEvents btnAddDrug As Button
End Class
