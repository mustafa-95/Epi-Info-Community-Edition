﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Epi.Fields;
using EpiDashboard;
using EpiDashboard.Controls;
using EpiDashboard.Gadgets;
using EpiDashboard.Gadgets.Charting;

namespace EpiDashboard.Gadgets.Charting
{
    public class ScatterChartGadgetBase : ChartGadgetBase
    {
        public Controls.Charting.IChart SelectedChart { get; set; }

        protected object syncLockData = new object();

        protected delegate void SetChartDataDelegate(List<XYColumnChartData> dataList, Strata strata);

        protected virtual void SetChartData(List<XYColumnChartData> dataList, Strata strata)
        {
        }

        protected override void Construct()
        {
            //object element = this.FindName("txtLegendFontSize");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_PositiveIntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("txtHeight");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_PositiveIntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("txtWidth");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_PositiveIntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("txtTransTop");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_PositiveIntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("txtTransBottom");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_PositiveIntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("txtXAxisAngle");
            //if (element != null && element is TextBox)
            //{
            //    TextBox textBox = element as TextBox;
            //    textBox.PreviewKeyDown += new KeyEventHandler(txtInput_IntegerOnly_PreviewKeyDown);
            //}

            //element = this.FindName("expanderAdvancedOptions");
            //if (element != null && element is Expander)
            //{
            //    Expander expanderAdvancedOptions = element as Expander;
            //    expanderAdvancedOptions.Header = DashboardSharedStrings.GADGET_ADVANCED_OPTIONS;
            //}

            //element = this.FindName("expanderDisplayOptions");
            //if (element != null && element is Expander)
            //{
            //    Expander expanderDisplayOptions = element as Expander;
            //    expanderDisplayOptions.Header = DashboardSharedStrings.GADGET_DISPLAY_OPTIONS;
            //}

            base.Construct();
        }

        protected virtual bool GenerateScatterChartData(Dictionary<DataTable, List<DescriptiveStatistics>> stratifiedFrequencyTables, Strata strata = null)
        {
            lock (syncLockData)
            {
                ScatterChartParameters chtParameters = (ScatterChartParameters)Parameters;

                string second_y_var = string.Empty;

                Y2Type y2type = Y2Type.None;

                List<XYColumnChartData> dataList = new List<XYColumnChartData>();

                foreach (KeyValuePair<DataTable, List<DescriptiveStatistics>> tableKvp in stratifiedFrequencyTables)
                {
                    double count = 0;
                    foreach (DescriptiveStatistics ds in tableKvp.Value)
                    {
                        count = count + ds.observations;
                    }

                    // If there is only one table and the total for that table is zero, then no data can be displayed and thus no chart can be generated.
                    // Show a message to the user to this effect so they don't wonder why they're seeing a blank gadget.
                    
                    // Commented out for now because of scenarios where the "One chart for each value of" option is used, and we're unsure how to handle
                    // showing this message in that case.

                    //if (count == 0 && stratifiedFrequencyTables.Count == 1)
                    //{
                    //    this.Dispatcher.BeginInvoke(new RenderFinishWithErrorDelegate(RenderFinishWithError), DashboardSharedStrings.GADGET_MSG_NO_DATA);
                    //    return false;
                    //}

                    string strataValue = tableKvp.Key.TableName;
                    DataTable table = tableKvp.Key;

                    double cumulative_percent = 0;

                    foreach (DataRow row in table.Rows)
                    {
                        XYColumnChartData chartData = new XYColumnChartData();
                        chartData.X = strataValue;
                        chartData.Y = (double)row[1];

                        if (y2type != Y2Type.None)
                        {
                            foreach (DataRow dRow in DashboardHelper.DataSet.Tables[0].Rows)
                            {
                                //if (row[0].ToString().Equals(dRow[GadgetOptions.MainVariableName].ToString()) && (y2type == Y2Type.CumulativePercent || dRow[second_y_var] != DBNull.Value))
                                if (row[0].ToString().Equals(dRow[chtParameters.ColumnNames[0]].ToString()) && (y2type == Y2Type.CumulativePercent || dRow[second_y_var] != DBNull.Value))
                                {
                                    if (y2type == Y2Type.RatePer100kPop)
                                    {
                                        chartData.Y2 = chartData.Y / (Convert.ToDouble(dRow[second_y_var]) / 100000);
                                    }
                                    else if (y2type == Y2Type.CumulativePercent)
                                    {
                                        chartData.Y2 = cumulative_percent + (chartData.Y / count);
                                        cumulative_percent = chartData.Y2.Value;
                                    }
                                    else
                                    {
                                        chartData.Y2 = Convert.ToDouble(dRow[second_y_var]);
                                    }
                                    break;
                                }
                            }
                        }

                        else if (y2type == Y2Type.CumulativePercent)
                        {
                            foreach (DataRow dRow in DashboardHelper.DataSet.Tables[0].Rows)
                            {
                            }
                            
                        }

                        chartData.S = row[0];
                        if(chartData.S == null || string.IsNullOrEmpty(chartData.S.ToString().Trim())) 
                        {
                            chartData.S = Config.Settings.RepresentationOfMissing;
                        }
                        dataList.Add(chartData);
                    }
                }

                this.Dispatcher.BeginInvoke(new SetChartDataDelegate(SetChartData), dataList, strata);
            }

            return true;
        }

        //protected void ChartGadgetBase_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    SelectedChart = null;
        //    object panel = this.FindName("panelMain");
        //    if (panel is StackPanel)
        //    {
        //        foreach (UIElement element in (panel as StackPanel).Children)
        //        {
        //            if (element.IsMouseOver)
        //            {
        //                SelectedChart = element as Controls.Charting.IChart;
        //                break;
        //            }
        //        }
        //    }

        //    if (SelectedChart != null)
        //    {                
        //        object el = this.FindName("separatorCurrentChart");
        //        if (el is Separator) (el as Separator).Visibility = System.Windows.Visibility.Visible;
        //        el = this.FindName("mnuCurrentChart");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Visible;
        //        el = this.FindName("mnuSaveSelectedChartAsImage");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Visible;
        //        el = this.FindName("mnuCopySelectedChartImage");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Visible;
        //        el = this.FindName("mnuCopySelectedChartData");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else
        //    {
        //        object el = this.FindName("separatorCurrentChart");
        //        if (el is Separator) (el as Separator).Visibility = System.Windows.Visibility.Collapsed;
        //        el = this.FindName("mnuCurrentChart");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Collapsed;
        //        el = this.FindName("mnuSaveSelectedChartAsImage");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Collapsed;
        //        el = this.FindName("mnuCopySelectedChartImage");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Collapsed;
        //        el = this.FindName("mnuCopySelectedChartData");
        //        if (el is MenuItem) (el as MenuItem).Visibility = System.Windows.Visibility.Collapsed;
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