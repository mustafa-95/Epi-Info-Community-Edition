﻿using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Epi;
using Epi.Data;
using Epi.Fields;
using EpiDashboard;
using EpiDashboard.Gadgets.Charting;
using ComponentArt.Win.DataVisualization.Charting;

namespace EpiDashboard.Gadgets.Charting
{
    /// <summary>
    /// Interaction logic for ColumnChartGadget.xaml
    /// </summary>
    public partial class ColumnChartGadget : ChartGadgetBase
    {
        private bool IsDropDownList { get; set; }
        private bool IsCommentLegal { get; set; }
        private bool IsRecoded { get; set; }
        private bool IsOptionField { get; set; }



        public ColumnChartGadget()
        {
            InitializeComponent();
            Construct();
        }

        public ColumnChartGadget(DashboardHelper dashboardHelper)
        {
            InitializeComponent();
            this.DashboardHelper = dashboardHelper;
            Construct();
            FillComboboxes();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (LoadingCombos)
            {
                return;
            }

            RefreshResults();
        }

        private void FillComboboxes(bool update = false)
        {
            LoadingCombos = true;

            string prevField = string.Empty;
            string prevWeightField = string.Empty;
            string prevCrosstabField = string.Empty;
            string prev2ndYAxisField = string.Empty;
            List<string> prevStrataFields = new List<string>();

            if (update)
            {
                if (cmbField.SelectedIndex >= 0)
                {
                    prevField = cmbField.SelectedItem.ToString();
                }
                if (cmbFieldWeight.SelectedIndex >= 0)
                {
                    prevWeightField = cmbFieldWeight.SelectedItem.ToString();
                }
                if (cmbFieldCrosstab.SelectedIndex >= 0)
                {
                    prevCrosstabField = cmbFieldCrosstab.SelectedItem.ToString();
                }
                if (cmbSecondYAxisVariable.SelectedIndex >= 0)
                {
                    prev2ndYAxisField = cmbSecondYAxisVariable.SelectedItem.ToString();
                }
                foreach (string s in listboxFieldStrata.SelectedItems)
                {
                    prevStrataFields.Add(s);
                }
            }

            if (cmbLegendDock.Items.Count == 0)
            {
                cmbLegendDock.Items.Add(ChartingSharedStrings.LEGEND_DOCK_VALUE_LEFT);
                cmbLegendDock.Items.Add(ChartingSharedStrings.LEGEND_DOCK_VALUE_RIGHT);
                cmbLegendDock.Items.Add(ChartingSharedStrings.LEGEND_DOCK_VALUE_TOP);
                cmbLegendDock.Items.Add(ChartingSharedStrings.LEGEND_DOCK_VALUE_BOTTOM);
                cmbLegendDock.SelectedIndex = 1;
            }

            cmbField.ItemsSource = null;
            cmbField.Items.Clear();

            cmbFieldWeight.ItemsSource = null;
            cmbFieldWeight.Items.Clear();

            cmbFieldCrosstab.ItemsSource = null;
            cmbFieldCrosstab.Items.Clear();

            cmbSecondYAxisVariable.ItemsSource = null;
            cmbSecondYAxisVariable.Items.Clear();

            listboxFieldStrata.ItemsSource = null;
            listboxFieldStrata.Items.Clear();

            List<string> fieldNames = new List<string>();
            List<string> weightFieldNames = new List<string>();
            List<string> strataFieldNames = new List<string>();
            List<string> crosstabFieldNames = new List<string>();

            weightFieldNames.Add(string.Empty);
            crosstabFieldNames.Add(string.Empty);

            ColumnDataType columnDataType = ColumnDataType.Boolean | ColumnDataType.DateTime | ColumnDataType.Numeric | ColumnDataType.Text | ColumnDataType.UserDefined;
            fieldNames = DashboardHelper.GetFieldsAsList(columnDataType);

            columnDataType = ColumnDataType.Numeric | ColumnDataType.UserDefined;
            weightFieldNames.AddRange(DashboardHelper.GetFieldsAsList(columnDataType));

            columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text | ColumnDataType.UserDefined;
            strataFieldNames.AddRange(DashboardHelper.GetFieldsAsList(columnDataType));

            columnDataType = ColumnDataType.Numeric | ColumnDataType.Boolean | ColumnDataType.Text | ColumnDataType.UserDefined;
            crosstabFieldNames.AddRange(DashboardHelper.GetFieldsAsList(columnDataType));

            fieldNames.Sort();
            weightFieldNames.Sort();
            strataFieldNames.Sort();
            crosstabFieldNames.Sort();

            if (fieldNames.Contains("SYSTEMDATE"))
            {
                fieldNames.Remove("SYSTEMDATE");
            }

            if (DashboardHelper.IsUsingEpiProject)
            {
                if (fieldNames.Contains("RecStatus")) fieldNames.Remove("RecStatus");
                if (weightFieldNames.Contains("RecStatus")) weightFieldNames.Remove("RecStatus");

                if (strataFieldNames.Contains("RecStatus")) strataFieldNames.Remove("RecStatus");
                if (strataFieldNames.Contains("FKEY")) strataFieldNames.Remove("FKEY");
                if (strataFieldNames.Contains("GlobalRecordId")) strataFieldNames.Remove("GlobalRecordId");

                if (crosstabFieldNames.Contains("RecStatus")) crosstabFieldNames.Remove("RecStatus");
                if (crosstabFieldNames.Contains("FKEY")) crosstabFieldNames.Remove("FKEY");
                if (crosstabFieldNames.Contains("GlobalRecordId")) crosstabFieldNames.Remove("GlobalRecordId");
            }

            cmbField.ItemsSource = fieldNames;
            cmbFieldWeight.ItemsSource = weightFieldNames;
            cmbFieldCrosstab.ItemsSource = crosstabFieldNames;
            cmbSecondYAxisVariable.ItemsSource = weightFieldNames;
            listboxFieldStrata.ItemsSource = strataFieldNames;

            if (cmbField.Items.Count > 0)
            {
                cmbField.SelectedIndex = -1;
            }
            if (cmbFieldWeight.Items.Count > 0)
            {
                cmbFieldWeight.SelectedIndex = -1;
            }
            if (cmbFieldCrosstab.Items.Count > 0)
            {
                cmbFieldCrosstab.SelectedIndex = -1;
            }
            if (cmbSecondYAxisVariable.Items.Count > 0)
            {
                cmbSecondYAxisVariable.SelectedIndex = -1;
            }

            if (update)
            {
                cmbField.SelectedItem = prevField;
                cmbFieldWeight.SelectedItem = prevWeightField;
                cmbFieldCrosstab.SelectedItem = prevCrosstabField;
                cmbSecondYAxisVariable.SelectedItem = prev2ndYAxisField;

                foreach (string s in prevStrataFields)
                {
                    listboxFieldStrata.SelectedItems.Add(s);
                }
            }

            LoadingCombos = false;
        }

        public override void SetGadgetToProcessingState()
        {
            this.IsProcessing = true;

            this.panelAdvanced1.IsEnabled = false;
            this.panelAdvanced2.IsEnabled = false;
            this.panelAdvanced3.IsEnabled = false;
            this.panelDisplay1.IsEnabled = false;
            this.panelDisplay2.IsEnabled = false;
            this.panelDisplay3.IsEnabled = false;

            this.btnRun.IsEnabled = false;
        }

        public override void SetGadgetToFinishedState()
        {
            this.IsProcessing = false;

            this.panelAdvanced1.IsEnabled = true;
            this.panelAdvanced2.IsEnabled = true;
            this.panelAdvanced3.IsEnabled = true;
            this.panelDisplay1.IsEnabled = true;
            this.panelDisplay2.IsEnabled = true;
            this.panelDisplay3.IsEnabled = true;

            this.btnRun.IsEnabled = true;

            if (IsCommentLegal || IsOptionField)
            {
                this.checkboxCommentLegalLabels.IsEnabled = true;
            }

            EnableDisableY2Fields();

            base.SetGadgetToFinishedState();
        }

        public override void RefreshResults()
        {
            if (!LoadingCombos && GadgetOptions != null && cmbField.SelectedIndex > -1)
            {
                CreateInputVariableList();
                infoPanel.Visibility = System.Windows.Visibility.Collapsed;
                waitPanel.Visibility = System.Windows.Visibility.Visible;
                messagePanel.MessagePanelType = EpiDashboard.Controls.MessagePanelType.StatusPanel;
                descriptionPanel.PanelMode = EpiDashboard.Controls.GadgetDescriptionPanel.DescriptionPanelMode.Collapsed;
                baseWorker = new BackgroundWorker();
                baseWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(Execute);
                baseWorker.RunWorkerAsync();
                base.RefreshResults();
            }
            else if (!LoadingCombos && cmbField.SelectedIndex == -1)
            {
                ClearResults();
                waitPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void CreateInputVariableList()
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>();

            GadgetOptions.MainVariableName = string.Empty;
            GadgetOptions.WeightVariableName = string.Empty;
            GadgetOptions.StrataVariableNames = new List<string>();
            GadgetOptions.CrosstabVariableName = string.Empty;
            GadgetOptions.ColumnNames = new List<string>();

            int xStart;
            int xEnd;
            bool success = int.TryParse(txtStartValue.Text, out xStart);
            if (success)
            {
                XAxisStart = xStart;
            }
            success = int.TryParse(txtEndValue.Text, out xEnd);
            if (success)
            {
                XAxisEnd = xEnd;
            }

            if (cmbField.SelectedIndex > -1 && !string.IsNullOrEmpty(cmbField.SelectedItem.ToString()))
            {
                inputVariableList.Add("freqvar", cmbField.SelectedItem.ToString());
                GadgetOptions.MainVariableName = cmbField.SelectedItem.ToString();
            }
            else
            {
                return;
            }

            if (cmbFieldWeight.SelectedIndex > -1 && !string.IsNullOrEmpty(cmbFieldWeight.SelectedItem.ToString()))
            {
                inputVariableList.Add("weightvar", cmbFieldWeight.SelectedItem.ToString());
                GadgetOptions.WeightVariableName = cmbFieldWeight.SelectedItem.ToString();
            }

            if (cmbFieldCrosstab.SelectedIndex > -1 && !string.IsNullOrEmpty(cmbFieldCrosstab.SelectedItem.ToString()))
            {
                inputVariableList.Add("crosstabvar", cmbFieldCrosstab.SelectedItem.ToString());
                GadgetOptions.CrosstabVariableName = cmbFieldCrosstab.SelectedItem.ToString();
            }

            if (cmbSecondYAxisVariable.SelectedIndex > -1 && !string.IsNullOrEmpty(cmbSecondYAxisVariable.SelectedItem.ToString()))
            {
                inputVariableList.Add("second_y_var", cmbSecondYAxisVariable.SelectedItem.ToString());
            }

            if (cmbSecondYAxis.SelectedIndex == 1)
            {
                inputVariableList.Add("second_y_var_type", "single");
            }
            if (cmbSecondYAxis.SelectedIndex == 2)
            {
                inputVariableList.Add("second_y_var_type", "rate_per_100k");
            }
            if (cmbSecondYAxis.SelectedIndex == 3)
            {
                inputVariableList.Add("second_y_var_type", "cumulative_percent");
            }

            if (listboxFieldStrata.SelectedItems.Count > 0)
            {
                GadgetOptions.StrataVariableNames = new List<string>();
                foreach (string s in listboxFieldStrata.SelectedItems)
                {
                    GadgetOptions.StrataVariableNames.Add(s);
                }
            }

            if (checkboxAllValues.IsChecked == true)
            {
                inputVariableList.Add("allvalues", "true");
                GadgetOptions.ShouldUseAllPossibleValues = true;
            }
            else
            {
                inputVariableList.Add("allvalues", "false");
                GadgetOptions.ShouldUseAllPossibleValues = false;
            }

            if (checkboxCommentLegalLabels.IsChecked == true)
            {
                GadgetOptions.ShouldShowCommentLegalLabels = true;
            }
            else
            {
                GadgetOptions.ShouldShowCommentLegalLabels = false;
            }

            if (checkboxSortHighLow.IsChecked == true)
            {
                inputVariableList.Add("sort", "highlow");
                GadgetOptions.ShouldSortHighToLow = true;
            }
            else
            {
                GadgetOptions.ShouldSortHighToLow = false;
            }

            if (checkboxIncludeMissing.IsChecked == true)
            {
                inputVariableList.Add("includemissing", "true");
                GadgetOptions.ShouldIncludeMissing = true;
            }
            else
            {
                inputVariableList.Add("includemissing", "false");
                GadgetOptions.ShouldIncludeMissing = false;
            }

            GadgetOptions.ShouldIncludeFullSummaryStatistics = false;
            GadgetOptions.InputVariableList = inputVariableList;
        }

        protected override void Construct()
        {
            if (!string.IsNullOrEmpty(CustomOutputHeading) && !CustomOutputHeading.Equals("(none)"))
            {
                headerPanel.Text = CustomOutputHeading;
            }

            StrataGridList = new List<Grid>();
            StrataExpanderList = new List<Expander>();

            mnuCopy.Click += new RoutedEventHandler(mnuCopy_Click);
            mnuSendDataToHTML.Click += new RoutedEventHandler(mnuSendDataToHTML_Click);

            //#if LINUX_BUILD
            //            mnuSendDataToExcel.Visibility = Visibility.Collapsed;
            //#else
            //            mnuSendDataToExcel.Visibility = Visibility.Visible;
            //            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot; Microsoft.Win32.RegistryKey excelKey = key.OpenSubKey("Excel.Application"); bool excelInstalled = excelKey == null ? false : true; key = Microsoft.Win32.Registry.ClassesRoot;
            //            excelKey = key.OpenSubKey("Excel.Application");
            //            excelInstalled = excelKey == null ? false : true;

            //            if (!excelInstalled)
            //            {
            //                mnuSendDataToExcel.Visibility = Visibility.Collapsed;
            //            }
            //            else
            //            {
            //                mnuSendDataToExcel.Click += new RoutedEventHandler(mnuSendDataToExcel_Click);
            //            }
            //#endif

            mnuSendToBack.Click += new RoutedEventHandler(mnuSendToBack_Click);
            mnuClose.Click += new RoutedEventHandler(mnuClose_Click);
            this.IsProcessing = false;
            this.GadgetStatusUpdate += new GadgetStatusUpdateHandler(RequestUpdateStatusMessage);
            this.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(IsCancelled);

            #region Translation

            tblockWidth.Text = DashboardSharedStrings.GADGET_WIDTH;
            tblockHeight.Text = DashboardSharedStrings.GADGET_HEIGHT;
            tblockMainVariable.Text = DashboardSharedStrings.GADGET_MAIN_VARIABLE;
            tblockStrataVariable.Text = DashboardSharedStrings.GADGET_STRATA_VARIABLE;
            tblockWeightVariable.Text = DashboardSharedStrings.GADGET_WEIGHT_VARIABLE;
            tblockCrosstabVariable.Text = ChartingSharedStrings.GRAPH_FOR_EACH_VALUE_OF_VARIABLE;

            checkboxAllValues.Content = DashboardSharedStrings.GADGET_ALL_LIST_VALUES;
            checkboxCommentLegalLabels.Content = DashboardSharedStrings.GADGET_LIST_LABELS;
            checkboxIncludeMissing.Content = DashboardSharedStrings.GADGET_INCLUDE_MISSING;
            checkboxSortHighLow.Content = DashboardSharedStrings.GADGET_SORT_HI_LOW;

            tblockSecondYAxis.Text = ChartingSharedStrings.SECOND_Y_AXIS_TYPE;
            tblockSecondYAxisVariable.Text = ChartingSharedStrings.SECOND_Y_AXIS_VARIABLE;

            expanderAdvancedOptions.Header = DashboardSharedStrings.GADGET_ADVANCED_OPTIONS;
            expanderDisplayOptions.Header = DashboardSharedStrings.GADGET_DISPLAY_OPTIONS;

            lblColorsStyles.Content = ChartingSharedStrings.PANEL_COLORS_AND_STYLES;
            lblLabels.Content = ChartingSharedStrings.PANEL_LABELS;
            lblLegend.Content = ChartingSharedStrings.PANEL_LEGEND;

            checkboxUseRefValues.Content = ChartingSharedStrings.USE_REFERENCE_VALUES;
            checkboxUseDiffColors.Content = ChartingSharedStrings.DIFF_BAR_COLORS;
            checkboxAnnotations.Content = ChartingSharedStrings.SHOW_ANNOTATIONS;
            checkboxGridLines.Content = ChartingSharedStrings.SHOW_GRID_LINES;
            tblockBarSpacing.Text = ChartingSharedStrings.SPACE_BETWEEN_BARS;
            tblockPalette.Text = ChartingSharedStrings.COLOR_PALETTE;
            tblockBarType.Text = ChartingSharedStrings.BAR_TYPE;
            tblockComposition.Text = ChartingSharedStrings.COMPOSITION;
            tblockOrientation.Text = ChartingSharedStrings.ORIENTATION;

            tblockLineTypeY2.Text = ChartingSharedStrings.LINE_TYPE_Y2;
            tblockLineDashTypeY2.Text = ChartingSharedStrings.LINE_DASH_STYLE_Y2;
            tblockLineThicknessY2.Text = ChartingSharedStrings.LINE_THICKNESS_Y2;

            tblockYAxisLabelValue.Text = ChartingSharedStrings.Y_AXIS_LABEL;
            tblockYAxisFormatString.Text = ChartingSharedStrings.Y_AXIS_FORMAT;

            tblockY2AxisLabelValue.Text = ChartingSharedStrings.Y2_AXIS_LABEL;
            tblockY2AxisLegendTitle.Text = ChartingSharedStrings.Y2_AXIS_LEGEND_TITLE;
            tblockY2AxisFormatString.Text = ChartingSharedStrings.Y2_AXIS_FORMAT;

            tblockXAxisLabelType.Text = ChartingSharedStrings.X_AXIS_LABEL_TYPE;
            tblockXAxisLabelValue.Text = ChartingSharedStrings.X_AXIS_LABEL;
            tblockXAxisAngle.Text = ChartingSharedStrings.X_AXIS_ANGLE;
            tblockChartTitleValue.Text = ChartingSharedStrings.CHART_TITLE;
            tblockChartSubTitleValue.Text = ChartingSharedStrings.CHART_SUBTITLE;

            checkboxShowLegend.Content = ChartingSharedStrings.SHOW_LEGEND;
            checkboxShowLegendBorder.Content = ChartingSharedStrings.SHOW_LEGEND_BORDER;
            checkboxShowVarName.Content = ChartingSharedStrings.SHOW_VAR_NAMES;
            tblockLegendFontSize.Text = ChartingSharedStrings.LEGEND_FONT_SIZE;
            tblockLegendDock.Text = ChartingSharedStrings.LEGEND_PLACEMENT;

            btnRun.Content = DashboardSharedStrings.GADGET_RUN_BUTTON;

            #endregion // Translation

            base.Construct();
        }

        private void ClearResults()
        {
            messagePanel.Visibility = System.Windows.Visibility.Collapsed;
            messagePanel.Text = string.Empty;
            descriptionPanel.PanelMode = EpiDashboard.Controls.GadgetDescriptionPanel.DescriptionPanelMode.Collapsed;

            tblockXAxisLabel.Visibility = System.Windows.Visibility.Collapsed;
            tblockYAxisLabel.Visibility = System.Windows.Visibility.Collapsed;
            tblockChartTitle.Visibility = System.Windows.Visibility.Collapsed;

            panelMain.Children.Clear();

            StrataGridList.Clear();
            StrataExpanderList.Clear();
        }

        //private void ShowLabels()
        //{
        //    if (string.IsNullOrEmpty(tblockXAxisLabel.Text.Trim()))
        //    {
        //        tblockXAxisLabel.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        tblockXAxisLabel.Visibility = System.Windows.Visibility.Visible;
        //    }

        //    if (string.IsNullOrEmpty(tblockYAxisLabel.Text.Trim()))
        //    {
        //        tblockYAxisLabel.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        tblockYAxisLabel.Visibility = System.Windows.Visibility.Visible;
        //    }

        //    if (string.IsNullOrEmpty(tblockChartTitle.Text.Trim()))
        //    {
        //        tblockChartTitle.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        tblockChartTitle.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}

        private void RenderFinish()
        {
            waitPanel.Visibility = System.Windows.Visibility.Collapsed;

            messagePanel.MessagePanelType = EpiDashboard.Controls.MessagePanelType.StatusPanel;
            messagePanel.Text = string.Empty;
            messagePanel.Visibility = System.Windows.Visibility.Collapsed;

            //ShowLabels();
            HideConfigPanel();
            CheckAndSetPosition();
        }

        private void RenderFinishWithWarning(string errorMessage)
        {
            waitPanel.Visibility = System.Windows.Visibility.Collapsed; //waitCursor.Visibility = Visibility.Hidden;

            messagePanel.MessagePanelType = EpiDashboard.Controls.MessagePanelType.WarningPanel;
            messagePanel.Text = errorMessage;
            messagePanel.Visibility = System.Windows.Visibility.Visible;

            //ShowLabels();
            HideConfigPanel();
            CheckAndSetPosition();
        }

        private void RenderFinishWithError(string errorMessage)
        {
            waitPanel.Visibility = System.Windows.Visibility.Collapsed;

            messagePanel.MessagePanelType = EpiDashboard.Controls.MessagePanelType.ErrorPanel;
            messagePanel.Text = errorMessage;
            messagePanel.Visibility = System.Windows.Visibility.Visible;

            panelMain.Children.Clear();

            HideConfigPanel();
            CheckAndSetPosition();
        }

        public void CreateFromLegacyXml(XmlElement element)
        {
            bool isStacked = false;

            foreach (XmlElement child in element.ChildNodes)
            {
                switch (child.Name.ToLower())
                {
                    case "casestatusvariable":
                        if (!isStacked) listboxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                        break;
                    case "datevariable":
                        if (!isStacked && !string.IsNullOrEmpty(child.InnerText.Trim()))
                        {
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                        }
                        break;
                    case "yaxisvariable":
                        listboxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                        break;
                    case "xaxisvariable":
                        if (!string.IsNullOrEmpty(child.InnerText.Trim()))
                        {
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                        }
                        break;
                    //case "yaxisscattervariable":
                    //    cbxScatterYAxisField.Text = child.InnerText.Replace("&lt;", "<");
                    //    break;
                    //case "xaxisscattervariable":
                    //    cbxScatterXAxisField.Text = child.InnerText.Replace("&lt;", "<");
                    //    break;
                    case "allvalues":
                        if (child.InnerText.ToLower().Equals("true"))
                        {
                            checkboxAllValues.IsChecked = true;
                        }
                        else
                        {
                            checkboxAllValues.IsChecked = false;
                        }
                        break;
                    //case "horizontalgridlines":
                    //    if (child.InnerText.ToLower().Equals("true"))
                    //    {
                    //        checkboxShowHorizontalGridLines.IsChecked = true;
                    //    }
                    //    else
                    //    {
                    //        checkboxShowHorizontalGridLines.IsChecked = false;
                    //    }
                    //    break;
                    case "singlevariable":
                        if (!isStacked)
                        {
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                        }
                        break;
                    case "weightvariable":
                        if (!string.IsNullOrEmpty(child.InnerText.Trim()))
                        {
                            cmbFieldWeight.Text = child.InnerText.Replace("&lt;", "<");
                            cmbFieldWeight.Text = child.InnerText.Replace("&lt;", "<");
                        }
                        break;
                    case "stratavariable":
                        if (!isStacked) listboxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                        break;
                    case "columnaggregatefunction":
                        if (child.InnerText == "Count")
                        {
                            cmbComposition.SelectedIndex = 1;
                        }
                        else
                        {
                            cmbComposition.SelectedIndex = 2;
                        }
                        break;
                    case "charttype":
                        if (child.InnerText == "Stacked Column")
                        {
                            isStacked = true;
                        }
                        else if (child.InnerText == "Bar")
                        {
                            cmbOrientation.SelectedIndex = 1;
                        }
                        //cbxChartType.Text = child.InnerText;
                        break;
                    case "customheading":
                        this.CustomOutputHeading = child.InnerText;
                        break;
                    case "customdescription":
                        this.CustomOutputDescription = child.InnerText;
                        break;
                    case "customcaption":
                        this.CustomOutputCaption = child.InnerText;
                        break;
                    case "chartsize":
                        switch (child.InnerText)
                        {
                            case "Small":
                                txtWidth.Text = "400";
                                txtHeight.Text = "250";
                                break;
                            case "Medium":
                                txtWidth.Text = "533";
                                txtHeight.Text = "333";
                                break;
                            case "Large":
                                txtWidth.Text = "800";
                                txtHeight.Text = "500";
                                break;
                            case "Custom":
                                break;
                        }
                        break;
                    case "chartwidth":
                        if (!string.IsNullOrEmpty(child.InnerText))
                        {
                            txtWidth.Text = child.InnerText;
                        }
                        break;
                    case "chartheight":
                        if (!string.IsNullOrEmpty(child.InnerText))
                        {
                            txtHeight.Text = child.InnerText;
                        }
                        break;
                    case "charttitle":
                        txtChartTitle.Text = child.InnerText;
                        break;
                    case "chartlegendtitle":
                        //LegendTitle = child.InnerText;
                        break;
                    case "xaxislabel":
                        txtXAxisLabelValue.Text = child.InnerText;
                        cmbXAxisLabelType.SelectedIndex = 3;
                        break;
                    case "yaxislabel":
                        txtYAxisLabelValue.Text = child.InnerText;
                        break;
                    //case "xaxisstartvalue":
                    //    txtXAxisStartValue.Text = child.InnerText;
                    //    break;
                    //case "xaxisendvalue":
                    //    txtXAxisEndValue.Text = child.InnerText;
                    //    break;
                    case "xaxisrotation":
                        switch (child.InnerText)
                        {
                            case "0":
                                txtXAxisAngle.Text = "0";
                                break;
                            case "45":
                                txtXAxisAngle.Text = "-45";
                                break;
                            case "90":
                                txtXAxisAngle.Text = "-90";
                                break;
                            default:
                                txtXAxisAngle.Text = "0";
                                break;
                        }
                        break;
                }
            }
        }

        public override void CreateFromXml(XmlElement element)
        {
            this.LoadingCombos = true;
            HideConfigPanel();
            infoPanel.Visibility = System.Windows.Visibility.Collapsed;
            messagePanel.Visibility = System.Windows.Visibility.Collapsed;

            if (element.Name.Equals("chartGadget") || element.Name.Equals("ChartControl"))
            {
                CreateFromLegacyXml(element);
            }
            else
            {
                foreach (XmlElement child in element.ChildNodes)
                {
                    switch (child.Name.ToLower())
                    {
                        case "mainvariable":
                            cmbField.Text = child.InnerText.Replace("&lt;", "<");
                            break;
                        case "stratavariable":
                            listboxFieldStrata.SelectedItems.Add(child.InnerText.Replace("&lt;", "<"));
                            break;
                        case "stratavariables":
                            foreach (XmlElement field in child.ChildNodes)
                            {
                                List<string> fields = new List<string>();
                                if (field.Name.ToLower().Equals("stratavariable"))
                                {
                                    listboxFieldStrata.SelectedItems.Add(field.InnerText.Replace("&lt;", "<"));
                                }
                            }
                            break;
                        case "weightvariable":
                            cmbFieldWeight.Text = child.InnerText.Replace("&lt;", "<");
                            break;
                        case "crosstabvariable":
                            cmbFieldCrosstab.Text = child.InnerText.Replace("&lt;", "<");
                            break;
                        case "secondyvar":
                            cmbSecondYAxisVariable.Text = child.InnerText.Replace("&lt;", "<");
                            cmbSecondYAxisVariable.Visibility = System.Windows.Visibility.Visible;
                            //cmbSecondYAxis.SelectedIndex = 1;
                            break;
                        case "secondyvartype":
                            Y2Type y2Type = ((Y2Type)Int32.Parse(child.InnerText));
                            switch (y2Type)
                            {
                                case Y2Type.None:
                                    cmbSecondYAxis.SelectedIndex = 0;
                                    tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                                    break;
                                case Y2Type.SingleField:
                                    cmbSecondYAxis.SelectedIndex = 1;
                                    tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Visible;
                                    break;
                                case Y2Type.RatePer100kPop:
                                    cmbSecondYAxis.SelectedIndex = 2;
                                    tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Visible;
                                    break;
                                case Y2Type.CumulativePercent:
                                    cmbSecondYAxis.SelectedIndex = 3;
                                    tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                                    break;
                            }
                            break;
                        case "sort":
                            if (child.InnerText.ToLower().Equals("highlow"))
                            {
                                checkboxSortHighLow.IsChecked = true;
                            }
                            break;
                        case "allvalues":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxAllValues.IsChecked = true; }
                            else { checkboxAllValues.IsChecked = false; }
                            break;
                        case "showlistlabels":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxCommentLegalLabels.IsChecked = true; }
                            else { checkboxCommentLegalLabels.IsChecked = false; }
                            break;
                        case "includemissing":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxIncludeMissing.IsChecked = true; }
                            else { checkboxIncludeMissing.IsChecked = false; }
                            break;
                        case "customheading":
                            if (!string.IsNullOrEmpty(child.InnerText) && !child.InnerText.Equals("(none)"))
                            {
                                this.CustomOutputHeading = child.InnerText.Replace("&lt;", "<"); ;
                            }
                            else
                            {
                                this.CustomOutputHeading = string.Empty;
                            }
                            break;
                        case "customdescription":
                            if (!string.IsNullOrEmpty(child.InnerText) && !child.InnerText.Equals("(none)"))
                            {
                                this.CustomOutputDescription = child.InnerText.Replace("&lt;", "<");
                                if (!string.IsNullOrEmpty(CustomOutputDescription) && !CustomOutputHeading.Equals("(none)"))
                                {
                                    descriptionPanel.Text = CustomOutputDescription;
                                    descriptionPanel.PanelMode = EpiDashboard.Controls.GadgetDescriptionPanel.DescriptionPanelMode.DisplayMode;
                                }
                                else
                                {
                                    descriptionPanel.PanelMode = EpiDashboard.Controls.GadgetDescriptionPanel.DescriptionPanelMode.Collapsed;
                                }
                            }
                            break;
                        case "customcaption":
                            this.CustomOutputCaption = child.InnerText;
                            break;
                        case "datafilters":
                            this.DataFilters = new DataFilters(this.DashboardHelper);
                            this.DataFilters.CreateFromXml(child);
                            break;
                        case "usediffbarcolors":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxUseDiffColors.IsChecked = true; }
                            else { checkboxUseDiffColors.IsChecked = false; }
                            break;
                        case "userefvalues":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxUseRefValues.IsChecked = true; }
                            else { checkboxUseRefValues.IsChecked = false; }
                            break;
                        case "showannotations":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxAnnotations.IsChecked = true; }
                            else { checkboxAnnotations.IsChecked = false; }
                            break;
                        case "y2showannotations":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxAnnotationsY2.IsChecked = true; }
                            else { checkboxAnnotationsY2.IsChecked = false; }
                            break;
                        case "showgridlines":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxGridLines.IsChecked = true; }
                            else { checkboxGridLines.IsChecked = false; }
                            break;
                        case "composition":
                            cmbComposition.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "barspace":
                            cmbBarSpacing.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "orientation":
                            cmbOrientation.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "palette":
                            cmbPalette.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "bartype":
                            cmbBarType.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "legenddock":
                            cmbLegendDock.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "y2linetype":
                            cmbLineTypeY2.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "y2linedashstyle":
                            cmbLineDashTypeY2.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "y2linethickness":
                            cmbLineThicknessY2.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "yaxislabel":
                            txtYAxisLabelValue.Text = child.InnerText;
                            break;
                        case "yaxisformatstring":
                            txtYAxisFormatString.Text = child.InnerText;
                            break;
                        case "y2axisformatstring":
                            txtY2AxisFormatString.Text = child.InnerText;
                            break;
                        case "y2axislabel":
                            txtY2AxisLabelValue.Text = child.InnerText;
                            break;
                        case "y2axislegendtitle":
                            txtY2AxisLegendTitle.Text = child.InnerText;
                            break;
                        case "xaxislabeltype":
                            cmbXAxisLabelType.SelectedIndex = int.Parse(child.InnerText);
                            break;
                        case "xaxislabel":
                            txtXAxisLabelValue.Text = child.InnerText;
                            break;
                        case "xaxisangle":
                            txtXAxisAngle.Text = child.InnerText;
                            break;
                        case "charttitle":
                            txtChartTitle.Text = child.InnerText;
                            break;
                        case "chartsubtitle":
                            txtChartSubTitle.Text = child.InnerText;
                            break;
                        case "showlegend":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxShowLegend.IsChecked = true; }
                            else { checkboxShowLegend.IsChecked = false; }
                            break;
                        case "showlegendborder":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxShowLegendBorder.IsChecked = true; }
                            else { checkboxShowLegendBorder.IsChecked = false; }
                            break;
                        case "showlegendvarnames":
                            if (child.InnerText.ToLower().Equals("true")) { checkboxShowVarName.IsChecked = true; }
                            else { checkboxShowVarName.IsChecked = false; }
                            break;
                        case "legendfontsize":
                            txtLegendFontSize.Text = child.InnerText;
                            break;
                        case "height":
                            txtHeight.Text = child.InnerText;
                            break;
                        case "width":
                            txtWidth.Text = child.InnerText;
                            break;
                        case "yaxisfrom":
                            txtFromValue.Text = child.InnerText;
                            break;
                        case "yaxisto":
                            txtToValue.Text = child.InnerText;
                            break;
                        case "yaxisstep":
                            txtStepValue.Text = child.InnerText;
                            break;
                        case "xaxisstartvalue":
                            txtStartValue.Text = child.InnerText;
                            break;
                        case "xaxisendvalue":
                            txtEndValue.Text = child.InnerText;
                            break;
                    }
                }
            }

            base.CreateFromXml(element);

            this.LoadingCombos = false;
            RefreshResults();
            HideConfigPanel();
            SetXAxisLabelControls();
        }

        /// <summary>
        /// Serializes the gadget into Xml
        /// </summary>
        /// <param name="doc">The Xml docment</param>
        /// <returns>XmlNode</returns>
        public override XmlNode Serialize(XmlDocument doc)
        {
            CreateInputVariableList();

            Dictionary<string, string> inputVariableList = GadgetOptions.InputVariableList;

            string mainVar = string.Empty;
            string strataVar = string.Empty;
            string crosstabVar = string.Empty;
            string weightVar = string.Empty;
            string second_y_var = string.Empty;
            string sort = string.Empty;
            bool allValues = false;
            bool showConfLimits = true;
            bool showCumulativePercent = true;
            bool includeMissing = false;

            int height = 600;
            int width = 800;

            int.TryParse(txtHeight.Text, out height);
            int.TryParse(txtWidth.Text, out width);

            crosstabVar = GadgetOptions.CrosstabVariableName.Replace("<", "&lt;");

            if (inputVariableList.ContainsKey("freqvar"))
            {
                mainVar = inputVariableList["freqvar"].Replace("<", "&lt;");
            }
            if (inputVariableList.ContainsKey("weightvar"))
            {
                weightVar = inputVariableList["weightvar"].Replace("<", "&lt;");
            }
            if (inputVariableList.ContainsKey("second_y_var"))
            {
                second_y_var = inputVariableList["second_y_var"].Replace("<", "&lt;");
            }

            Y2Type second_y_var_type = Y2Type.None;

            if (inputVariableList.ContainsKey("second_y_var_type"))
            {
                switch (inputVariableList["second_y_var_type"])
                {
                    case "rate_per_100k":
                        second_y_var_type = Y2Type.RatePer100kPop;
                        break;
                    case "single":
                        second_y_var_type = Y2Type.SingleField;
                        break;
                    case "cumulative_percent":
                        second_y_var_type = Y2Type.CumulativePercent;
                        break;
                }
            }

            if (inputVariableList.ContainsKey("sort"))
            {
                sort = inputVariableList["sort"];
            }
            if (inputVariableList.ContainsKey("allvalues"))
            {
                allValues = bool.Parse(inputVariableList["allvalues"]);
            }
            if (inputVariableList.ContainsKey("showconflimits"))
            {
                showConfLimits = bool.Parse(inputVariableList["showconflimits"]);
            }
            if (inputVariableList.ContainsKey("showcumulativepercent"))
            {
                showCumulativePercent = bool.Parse(inputVariableList["showcumulativepercent"]);
            }
            if (inputVariableList.ContainsKey("includemissing"))
            {
                includeMissing = bool.Parse(inputVariableList["includemissing"]);
            }

            CustomOutputHeading = headerPanel.Text;
            CustomOutputDescription = descriptionPanel.Text;

            string xmlString =
            "<mainVariable>" + mainVar + "</mainVariable>";

            if (GadgetOptions.StrataVariableNames.Count == 1)
            {
                xmlString = xmlString + "<strataVariable>" + GadgetOptions.StrataVariableNames[0].Replace("<", "&lt;") + "</strataVariable>";
            }
            else if (GadgetOptions.StrataVariableNames.Count > 1)
            {
                xmlString = xmlString + "<strataVariables>";

                foreach (string strataVariable in this.GadgetOptions.StrataVariableNames)
                {
                    xmlString = xmlString + "<strataVariable>" + strataVariable.Replace("<", "&lt;") + "</strataVariable>";
                }

                xmlString = xmlString + "</strataVariables>";
            }

            xmlString = xmlString + "<weightVariable>" + weightVar + "</weightVariable>" +
                "<crosstabVariable>" + crosstabVar + "</crosstabVariable>" +
                "<secondYVarType>" + ((int)second_y_var_type).ToString() + "</secondYVarType>" +
                "<secondYVar>" + second_y_var + "</secondYVar>" +
                "<height>" + height + "</height>" +
                "<width>" + width + "</width>" +

            "<sort>" + sort + "</sort>" +
            "<allValues>" + allValues + "</allValues>" +
            "<showListLabels>" + checkboxCommentLegalLabels.IsChecked + "</showListLabels>" +
            "<includeMissing>" + includeMissing + "</includeMissing>" +
            "<customHeading>" + CustomOutputHeading.Replace("<", "&lt;") + "</customHeading>" +
            "<customDescription>" + CustomOutputDescription.Replace("<", "&lt;") + "</customDescription>" +
            "<customCaption>" + CustomOutputCaption + "</customCaption>";

            xmlString += "<useDiffBarColors>" + checkboxUseDiffColors.IsChecked.Value + "</useDiffBarColors>";
            xmlString += "<useRefValues>" + checkboxUseRefValues.IsChecked.Value + "</useRefValues>";
            xmlString += "<showAnnotations>" + checkboxAnnotations.IsChecked.Value + "</showAnnotations>";
            xmlString += "<y2showAnnotations>" + checkboxAnnotationsY2.IsChecked.Value + "</y2showAnnotations>";
            xmlString += "<showGridLines>" + checkboxGridLines.IsChecked.Value + "</showGridLines>";

            xmlString += "<composition>" + cmbComposition.SelectedIndex + "</composition>";
            xmlString += "<barSpace>" + cmbBarSpacing.SelectedIndex + "</barSpace>";
            xmlString += "<orientation>" + cmbOrientation.SelectedIndex + "</orientation>";
            xmlString += "<palette>" + cmbPalette.SelectedIndex + "</palette>";
            xmlString += "<barType>" + cmbBarType.SelectedIndex + "</barType>";

            xmlString += "<y2LineType>" + cmbLineTypeY2.SelectedIndex + "</y2LineType>";
            xmlString += "<y2LineDashStyle>" + cmbLineDashTypeY2.SelectedIndex + "</y2LineDashStyle>";
            xmlString += "<y2LineThickness>" + cmbLineThicknessY2.SelectedIndex + "</y2LineThickness>";

            xmlString += "<yAxisFrom>" + txtFromValue.Text + "</yAxisFrom>";
            xmlString += "<yAxisTo>" + txtToValue.Text + "</yAxisTo>";
            xmlString += "<yAxisStep>" + txtStepValue.Text + "</yAxisStep>";

            xmlString += "<xAxisStartValue>" + txtStartValue.Text + "</xAxisStartValue>";
            xmlString += "<xAxisEndValue>" + txtEndValue.Text + "</xAxisEndValue>";

            xmlString += "<yAxisLabel>" + txtYAxisLabelValue.Text + "</yAxisLabel>";
            xmlString += "<yAxisFormatString>" + txtYAxisFormatString.Text + "</yAxisFormatString>";
            xmlString += "<y2AxisLabel>" + txtY2AxisLabelValue.Text + "</y2AxisLabel>";
            xmlString += "<y2AxisLegendTitle>" + txtY2AxisLegendTitle.Text + "</y2AxisLegendTitle>";
            xmlString += "<y2AxisFormatString>" + txtY2AxisFormatString.Text + "</y2AxisFormatString>";
            xmlString += "<xAxisLabelType>" + cmbXAxisLabelType.SelectedIndex + "</xAxisLabelType>";
            xmlString += "<xAxisLabel>" + txtXAxisLabelValue.Text + "</xAxisLabel>";
            xmlString += "<xAxisAngle>" + txtXAxisAngle.Text + "</xAxisAngle>";
            xmlString += "<chartTitle>" + txtChartTitle.Text + "</chartTitle>";
            xmlString += "<chartSubTitle>" + txtChartSubTitle.Text + "</chartSubTitle>";

            xmlString += "<showLegend>" + checkboxShowLegend.IsChecked.Value + "</showLegend>";
            xmlString += "<showLegendBorder>" + checkboxShowLegendBorder.IsChecked.Value + "</showLegendBorder>";
            xmlString += "<showLegendVarNames>" + checkboxShowVarName.IsChecked.Value + "</showLegendVarNames>";
            xmlString += "<legendFontSize>" + txtLegendFontSize.Text + "</legendFontSize>";

            xmlString += "<legendDock>" + cmbLegendDock.SelectedIndex + "</legendDock>";

            xmlString = xmlString + SerializeAnchors();

            System.Xml.XmlElement element = doc.CreateElement("columnChartGadget");
            element.InnerXml = xmlString;
            element.AppendChild(SerializeFilters(doc));

            System.Xml.XmlAttribute id = doc.CreateAttribute("id");
            System.Xml.XmlAttribute locationY = doc.CreateAttribute("top");
            System.Xml.XmlAttribute locationX = doc.CreateAttribute("left");
            System.Xml.XmlAttribute collapsed = doc.CreateAttribute("collapsed");
            System.Xml.XmlAttribute type = doc.CreateAttribute("gadgetType");

            id.Value = this.UniqueIdentifier.ToString();
            locationY.Value = Canvas.GetTop(this).ToString("F0");
            locationX.Value = Canvas.GetLeft(this).ToString("F0");
            collapsed.Value = "false"; // currently no way to collapse the gadget, so leave this 'false' for now
            type.Value = "EpiDashboard.Gadgets.Charting.ColumnChartGadget";

            element.Attributes.Append(locationY);
            element.Attributes.Append(locationX);
            element.Attributes.Append(collapsed);
            element.Attributes.Append(type);
            element.Attributes.Append(id);

            return element;
        }

        protected override void SetChartData(List<XYColumnChartData> dataList, Strata strata)
        {
            #region Input Validation
            if (dataList == null)
            {
                throw new ArgumentNullException("dataList");
            }
            #endregion // Input Validation

            if (dataList.Count > 0)
            {
                ColumnChartSettings chartSettings = new ColumnChartSettings();
                chartSettings.ChartTitle = txtChartTitle.Text;
                chartSettings.ChartSubTitle = txtChartSubTitle.Text;
                if (strata != null)
                {
                    chartSettings.ChartStrataTitle = strata.Filter;
                }
                chartSettings.ChartWidth = int.Parse(txtWidth.Text);
                chartSettings.ChartHeight = int.Parse(txtHeight.Text);
                chartSettings.ShowDefaultGridLines = (bool)checkboxGridLines.IsChecked;

                double from;
                bool success = Double.TryParse(txtFromValue.Text, out from);
                if (success) { chartSettings.YAxisFrom = from; }

                double to;
                success = Double.TryParse(txtToValue.Text, out to);
                if (success) { chartSettings.YAxisTo = to; }

                double step;
                success = Double.TryParse(txtStepValue.Text, out step);
                if (success) { chartSettings.YAxisStep = step; }

                switch (cmbOrientation.SelectedIndex)
                {
                    case 1:
                        chartSettings.Orientation = Orientation.Horizontal;
                        break;
                    case 0:
                    default:
                        chartSettings.Orientation = Orientation.Vertical;
                        break;
                }

                switch (cmbBarType.SelectedIndex)
                {
                    case 1:
                        chartSettings.BarKind = BarKind.Cylinder;
                        break;
                    case 2:
                        chartSettings.BarKind = BarKind.Rectangle;
                        break;
                    case 3:
                        chartSettings.BarKind = BarKind.RoundedBlock;
                        break;
                    default:
                    case 0:
                        chartSettings.BarKind = BarKind.Block;
                        break;
                }

                switch (cmbBarSpacing.SelectedIndex)
                {
                    case 1:
                        chartSettings.BarSpacing = BarSpacing.None;
                        break;
                    case 2:
                        chartSettings.BarSpacing = BarSpacing.Small;
                        break;
                    case 3:
                        chartSettings.BarSpacing = BarSpacing.Medium;
                        break;
                    case 4:
                        chartSettings.BarSpacing = BarSpacing.Large;
                        break;
                    case 0:
                    default:
                        chartSettings.BarSpacing = BarSpacing.Default;
                        break;
                }

                switch (cmbComposition.SelectedIndex)
                {
                    case 0:
                        chartSettings.Composition = CompositionKind.SideBySide;
                        break;
                    case 1:
                        chartSettings.Composition = CompositionKind.Stacked;
                        break;
                    case 2:
                        chartSettings.Composition = CompositionKind.Stacked100;
                        break;
                }

                chartSettings.LegendDock = ComponentArt.Win.DataVisualization.Charting.Dock.Right;

                if (cmbLegendDock.SelectedItem.ToString().Equals(ChartingSharedStrings.LEGEND_DOCK_VALUE_LEFT))
                {
                    chartSettings.LegendDock = ComponentArt.Win.DataVisualization.Charting.Dock.Left;
                }
                else if (cmbLegendDock.SelectedItem.ToString().Equals(ChartingSharedStrings.LEGEND_DOCK_VALUE_RIGHT))
                {
                    chartSettings.LegendDock = ComponentArt.Win.DataVisualization.Charting.Dock.Right;
                }
                else if (cmbLegendDock.SelectedItem.ToString().Equals(ChartingSharedStrings.LEGEND_DOCK_VALUE_TOP))
                {
                    chartSettings.LegendDock = ComponentArt.Win.DataVisualization.Charting.Dock.Top;
                }
                else if (cmbLegendDock.SelectedItem.ToString().Equals(ChartingSharedStrings.LEGEND_DOCK_VALUE_BOTTOM))
                {
                    chartSettings.LegendDock = ComponentArt.Win.DataVisualization.Charting.Dock.Bottom;
                }

                chartSettings.LegendFontSize = double.Parse(txtLegendFontSize.Text);

                ComboBoxItem cbi = cmbPalette.SelectedItem as ComboBoxItem;
                ComponentArt.Win.DataVisualization.Palette palette = ComponentArt.Win.DataVisualization.Palette.GetPalette(cbi.Content.ToString());
                chartSettings.Palette = palette;

                chartSettings.ShowAnnotations = (bool)checkboxAnnotations.IsChecked;
                chartSettings.ShowAnnotationsY2 = (bool)checkboxAnnotationsY2.IsChecked;
                chartSettings.ShowLegend = (bool)checkboxShowLegend.IsChecked;
                chartSettings.ShowLegendBorder = (bool)checkboxShowLegendBorder.IsChecked;
                chartSettings.ShowLegendVarNames = (bool)checkboxShowVarName.IsChecked;
                chartSettings.UseDiffColors = (bool)checkboxUseDiffColors.IsChecked;
                chartSettings.UseRefValues = (bool)checkboxUseRefValues.IsChecked;
                chartSettings.XAxisLabel = txtXAxisLabelValue.Text;
                chartSettings.XAxisLabelRotation = int.Parse(txtXAxisAngle.Text);
                chartSettings.YAxisLabel = txtYAxisLabelValue.Text;
                chartSettings.YAxisFormattingString = txtYAxisFormatString.Text;
                chartSettings.Y2AxisFormattingString = txtY2AxisFormatString.Text;
                chartSettings.Y2AxisLabel = txtY2AxisLabelValue.Text;
                chartSettings.Y2AxisLegendTitle = txtY2AxisLegendTitle.Text;

                if (cmbSecondYAxis.SelectedIndex == 3)
                {
                    chartSettings.Y2AxisFormattingString = "P0";
                    chartSettings.Y2IsCumulativePercent = true;
                }

                switch (cmbXAxisLabelType.SelectedIndex)
                {
                    case 3:
                        chartSettings.XAxisLabelType = XAxisLabelType.Custom;
                        break;
                    case 1:
                        chartSettings.XAxisLabelType = XAxisLabelType.FieldPrompt;
                        Field field = DashboardHelper.GetAssociatedField(GadgetOptions.MainVariableName);
                        if (field == null)
                        {
                            chartSettings.XAxisLabel = GadgetOptions.MainVariableName;
                        }
                        else
                        {
                            chartSettings.XAxisLabel = ((IDataField)field).PromptText;
                        }
                        break;
                    case 2:
                        chartSettings.XAxisLabelType = XAxisLabelType.None;
                        break;
                    default:
                        chartSettings.XAxisLabelType = XAxisLabelType.Automatic;
                        chartSettings.XAxisLabel = GadgetOptions.MainVariableName;
                        break;
                }

                switch (cmbLineTypeY2.SelectedIndex)
                {
                    case 1:
                        chartSettings.LineKindY2 = LineKind.Polygon;
                        break;
                    case 2:
                        chartSettings.LineKindY2 = LineKind.Smooth;
                        break;
                    case 3:
                        chartSettings.LineKindY2 = LineKind.Step;
                        break;
                    default:
                    case 0:
                        chartSettings.LineKindY2 = LineKind.Auto;
                        break;
                }

                switch (cmbLineDashTypeY2.SelectedIndex)
                {
                    case 0:
                        chartSettings.LineDashStyleY2 = LineDashStyle.Dash;
                        break;
                    case 1:
                        chartSettings.LineDashStyleY2 = LineDashStyle.DashDot;
                        break;
                    case 2:
                        chartSettings.LineDashStyleY2 = LineDashStyle.DashDotDot;
                        break;
                    case 3:
                        chartSettings.LineDashStyleY2 = LineDashStyle.Dot;
                        break;
                    default:
                        chartSettings.LineDashStyleY2 = LineDashStyle.Solid;
                        break;
                }

                int lineThickness = cmbLineThicknessY2.SelectedIndex + 1;
                chartSettings.Y2LineThickness = (double)lineThickness;

                EpiDashboard.Controls.Charting.ColumnChart columnChart = new EpiDashboard.Controls.Charting.ColumnChart(DashboardHelper, GadgetOptions, chartSettings, dataList);
                columnChart.Margin = new Thickness(0, 0, 0, 16);
                columnChart.MouseEnter += new MouseEventHandler(chart_MouseEnter);
                columnChart.MouseLeave += new MouseEventHandler(chart_MouseLeave);
                panelMain.Children.Add(columnChart);
            }
        }

        /// <summary>
        /// Updates the variable names available in the gadget's properties
        /// </summary>
        public override void UpdateVariableNames()
        {
            FillComboboxes(true);
        }

        protected override void worker_WorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (DashboardHelper.IsAutoClosing)
            {
                System.Threading.Thread.Sleep(2200);
            }
            else
            {
                System.Threading.Thread.Sleep(300);
            }
            this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToFinishedState));
        }

        protected override void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            lock (syncLock)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                this.Dispatcher.BeginInvoke(new SimpleCallback(SetGadgetToProcessingState));
                this.Dispatcher.BeginInvoke(new SimpleCallback(ClearResults));

                string freqVar = GadgetOptions.MainVariableName;
                string weightVar = GadgetOptions.WeightVariableName;
                string crosstabVar = GadgetOptions.CrosstabVariableName;

                bool includeMissing = GadgetOptions.ShouldIncludeMissing;
                List<string> stratas = new List<string>();

                try
                {
                    RequestUpdateStatusDelegate requestUpdateStatus = new RequestUpdateStatusDelegate(RequestUpdateStatusMessage);
                    CheckForCancellationDelegate checkForCancellation = new CheckForCancellationDelegate(IsCancelled);

                    GadgetOptions.GadgetStatusUpdate += new GadgetStatusUpdateHandler(requestUpdateStatus);
                    GadgetOptions.GadgetCheckForCancellation += new GadgetCheckForCancellationHandler(checkForCancellation);

                    if (this.DataFilters != null && this.DataFilters.Count > 0)
                    {
                        GadgetOptions.CustomFilter = this.DataFilters.GenerateDataFilterString(false);
                    }
                    else
                    {
                        GadgetOptions.CustomFilter = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(crosstabVar.Trim()))
                    {
                        List<string> crosstabVarList = new List<string>();
                        crosstabVarList.Add(crosstabVar);

                        foreach (Strata strata in DashboardHelper.GetStrataValuesAsDictionary(crosstabVarList, false, false))
                        {
                            GadgetParameters parameters = new GadgetParameters(GadgetOptions);

                            if (!string.IsNullOrEmpty(GadgetOptions.CustomFilter))
                            {
                                parameters.CustomFilter = "(" + parameters.CustomFilter + ") AND " + strata.SafeFilter;
                            }
                            else
                            {
                                parameters.CustomFilter = strata.SafeFilter;
                            }
                            parameters.CrosstabVariableName = string.Empty;
                            Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables = DashboardHelper.GenerateFrequencyTable(parameters);
                            GenerateChartData(stratifiedFrequencyTables, strata);
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables = DashboardHelper.GenerateFrequencyTable(GadgetOptions);
                        GenerateChartData(stratifiedFrequencyTables);
                    }

                    this.Dispatcher.BeginInvoke(new SimpleCallback(RenderFinish));

                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), ex.Message);
                }
                finally
                {
                    stopwatch.Stop();
                    Debug.Print("Column chart gadget took " + stopwatch.Elapsed.ToString() + " seconds to complete.");
                    Debug.Print(DashboardHelper.DataFilters.GenerateDataFilterString());
                }
            }
        }

        /// <summary>
        /// Converts the gadget's output to Html
        /// </summary>
        /// <returns></returns>
        public override string ToHTML(string htmlFileName = "", int count = 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<h2>" + this.txtChartTitle.Text + "</h2>");
            sb.AppendLine("<h3>" + this.txtChartSubTitle.Text + "</h3>");

            foreach (UIElement element in panelMain.Children)
            {
                if (element is EpiDashboard.Controls.Charting.ColumnChart)
                {
                    sb.AppendLine(((EpiDashboard.Controls.Charting.ColumnChart)element).ToHTML(htmlFileName, count, true, false));
                }
            }

            return sb.ToString();
        }

        private void EnableDisableY2Fields()
        {
            if (cmbSecondYAxis.SelectedIndex == 0)
            {
                tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                cmbSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                cmbSecondYAxisVariable.SelectedIndex = -1;
                checkboxAnnotationsY2.IsEnabled = false;
                checkboxAnnotationsY2.IsChecked = false;
                cmbLineDashTypeY2.IsEnabled = false;
                cmbLineThicknessY2.IsEnabled = false;
                cmbLineTypeY2.IsEnabled = false;
                txtY2AxisLabelValue.IsEnabled = false;
                txtY2AxisFormatString.IsEnabled = false;
            }
            else if (cmbSecondYAxis.SelectedIndex == 3)
            {
                tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                cmbSecondYAxisVariable.Visibility = System.Windows.Visibility.Collapsed;
                cmbSecondYAxisVariable.SelectedIndex = -1;
                checkboxAnnotationsY2.IsEnabled = true;
                cmbLineDashTypeY2.IsEnabled = true;
                cmbLineThicknessY2.IsEnabled = true;
                cmbLineTypeY2.IsEnabled = true;
                txtY2AxisLabelValue.IsEnabled = true;
                txtY2AxisFormatString.IsEnabled = false;
            }
            else
            {
                tblockSecondYAxisVariable.Visibility = System.Windows.Visibility.Visible;
                cmbSecondYAxisVariable.Visibility = System.Windows.Visibility.Visible;
                checkboxAnnotationsY2.IsEnabled = true;
                cmbLineDashTypeY2.IsEnabled = true;
                cmbLineThicknessY2.IsEnabled = true;
                cmbLineTypeY2.IsEnabled = true;
                txtY2AxisLabelValue.IsEnabled = true;
                txtY2AxisFormatString.IsEnabled = true;

                if (cmbSecondYAxis.SelectedIndex == 1)
                {
                    tblockSecondYAxisVariable.Text = "Second y-axis variable:";
                }
                else if (cmbSecondYAxis.SelectedIndex == 2)
                {
                    tblockSecondYAxisVariable.Text = "Population variable:";
                }
            }
        }

        private void cmbSecondYAxis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadingCombos || tblockSecondYAxisVariable == null || cmbSecondYAxisVariable == null) return;

            EnableDisableY2Fields();
        }

        //private new void ChartGadgetBase_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{               
        //    if (SelectedChart != null)
        //    {
        //        separatorCurrentChart.Visibility = System.Windows.Visibility.Visible;
        //        mnuCurrentChart.Visibility = System.Windows.Visibility.Visible;
        //        mnuSaveSelectedChartAsImage.Visibility = System.Windows.Visibility.Visible;
        //        mnuCopySelectedChartImage.Visibility = System.Windows.Visibility.Visible;
        //        mnuCopySelectedChartData.Visibility = System.Windows.Visibility.Visible;                
        //    }
        //    else
        //    {
        //        separatorCurrentChart.Visibility = System.Windows.Visibility.Collapsed;
        //        mnuCurrentChart.Visibility = System.Windows.Visibility.Collapsed;
        //        mnuSaveSelectedChartAsImage.Visibility = System.Windows.Visibility.Collapsed;
        //        mnuCopySelectedChartImage.Visibility = System.Windows.Visibility.Collapsed;
        //        mnuCopySelectedChartData.Visibility = System.Windows.Visibility.Collapsed;                
        //    }
        //}

        //protected virtual void mnuSaveSelectedChartAsImage_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedChart != null)
        //    {
        //        SelectedChart.SaveImageToFile();
        //    }
        //}

        //protected virtual void mnuCopySelectedChartImage_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedChart != null)
        //    {
        //        SelectedChart.CopyImageToClipboard();
        //    } 
        //}

        //protected virtual void mnuCopySelectedChartData_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedChart != null)
        //    {
        //        SelectedChart.CopyAllDataToClipboard();
        //    }
        //}
    }
}
