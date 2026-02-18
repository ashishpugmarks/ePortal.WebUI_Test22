using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using ePortal.Persistence.Interface;

namespace ePortal.Persistence.Services
{
    public class ExportExcel : IExcelExport
    {
        //public enum ExportFormat { CSV = 1, Excel = 2 }
        private readonly string appType;

        public ExportExcel()
        {
            appType = "Web";
        }
        #region ExportDetails OverLoad : Type#3

        // Function  : ExportDetails 
        // Arguments : DetailsTable, ColumnList, Headers, FormatType, FileName	
        // Purpose	 : To get the specified column headers in the datatable and	
        //			   exorts in CSV / Excel format with specified columns and 
        //			   with specified headers

        //public FileContentResult ExportDetails(DataTable detailsTable, int[] columnList, string[] headers, ExportFormat formatType, string fileName)
        //{
        //    if (detailsTable.Rows.Count == 0)
        //        throw new Exception("No data to export");

        //    // Copy table into dataset
        //    DataSet dsExport = new DataSet("Export");
        //    DataTable dtExport = detailsTable.Copy();
        //    dtExport.TableName = "Values";
        //    dsExport.Tables.Add(dtExport);

        //    // Build field names
        //    string[] sFields = columnList.Select(i => ReplaceSpclChars(dtExport.Columns[i].ColumnName)).ToArray();

        //    // Transform dataset to CSV/Excel text
        //    string content = TransformData(dsExport, headers, sFields, formatType);

        //    string contentType = (formatType == ExportFormat.CSV) ? "text/csv" : "application/vnd.ms-excel";
        //    return new FileContentResult(Encoding.UTF8.GetBytes(content), contentType)
        //    {
        //        FileDownloadName = fileName
        //    };
        //}

        public FileContentResult ExportDetails(DataTable detailsTable, int[] columnList, string[] headers, ExportFormat formatType, string fileName)
        {
            if (detailsTable.Rows.Count == 0)
                throw new Exception("There are no details to export");

            if (columnList.Length != headers.Length)
                throw new Exception("ExportColumn List and Headers List should be of same length");

            if (columnList.Length > detailsTable.Columns.Count)
                throw new Exception("ExportColumn List should not exceed Total Columns");

            // Copy table into dataset
            DataSet dsExport = new DataSet("Export");
            DataTable dtExport = detailsTable.Copy();
            dtExport.TableName = "Values";
            dsExport.Tables.Add(dtExport);

            // Build field names
            string[] sFields = new string[columnList.Length];
            for (int i = 0; i < columnList.Length; i++)
            {
                if (columnList[i] < 0 || columnList[i] >= dtExport.Columns.Count)
                    throw new Exception("ExportColumn Number should not exceed Total Columns Range");

                sFields[i] = ReplaceSpclChars(dtExport.Columns[columnList[i]].ColumnName);
            }
            return Export_with_XSLT_Web(dsExport, headers, sFields, ExportFormat.Excel, fileName);
            // Transform dataset to CSV/Excel text
            //string content = TransformData(dsExport, headers, sFields, formatType);

            // Return as downloadable file
            //string contentType = (formatType == ExportFormat.CSV) ? "text/csv" : "application/vnd.ms-excel";
            //return new FileContentResult(Encoding.UTF8.GetBytes(content), contentType)
            //{
            //    FileDownloadName = fileName
            //};
        }
        #endregion // ExportDetails OverLoad : Type#3
        //private string TransformData(DataSet dsExport, string[] sHeaders, string[] sFields, ExportFormat formatType)
        //{
        //    using var stream = new MemoryStream();
        //    using var writer = new XmlTextWriter(stream, Encoding.UTF8);

        //    CreateStylesheet(writer, sHeaders, sFields, formatType);
        //    writer.Flush();
        //    stream.Seek(0, SeekOrigin.Begin);

        //    XmlDataDocument xmlDoc = new XmlDataDocument(dsExport);
        //    XslCompiledTransform xslTran = new XslCompiledTransform();
        //    xslTran.Load(new XmlTextReader(stream));

        //    using var sw = new StringWriter();
        //    xslTran.Transform(xmlDoc, null, sw);
        //    return sw.ToString();
        //}
        private FileContentResult Export_with_XSLT_Web(DataSet dsExport, string[] sHeaders, string[] sFileds, ExportFormat formatType,
        string fileName)
        {
            // XSLT to use for transforming this dataset
            using var stream = new MemoryStream();
            using var writer = new XmlTextWriter(stream, Encoding.UTF8);

            CreateStylesheet(writer, sHeaders, sFileds, formatType);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            XmlDataDocument xmlDoc = new XmlDataDocument(dsExport);
            var xslTran = new XslCompiledTransform();
            xslTran.Load(new XmlTextReader(stream));

            using var sw = new StringWriter();
            xslTran.Transform(xmlDoc, null, sw);

            // Build file content
            string content = sw.ToString();
            string contentType = (formatType == ExportFormat.CSV)
                ? "text/csv"
                : "application/vnd.ms-excel";

            //return new FileContentResult(Encoding.UTF8.GetBytes(content), contentType)
            //{
            //    FileDownloadName = fileName
            //};
            return new FileContentResult(Encoding.UTF8.GetBytes(content),
            "application/vnd.ms-excel")
            {
                FileDownloadName = fileName
            };
        }


        private void CreateStylesheet(XmlTextWriter writer, string[] sHeaders, string[] sFields, ExportFormat formatType)
        {
            string ns = "http://www.w3.org/1999/XSL/Transform";
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("xsl", "stylesheet", ns);
            writer.WriteAttributeString("version", "1.0");

            writer.WriteStartElement("xsl:output");
            writer.WriteAttributeString("method", "text");
            writer.WriteEndElement();

            writer.WriteStartElement("xsl:template");
            writer.WriteAttributeString("match", "/");

            // Headers
            for (int i = 0; i < sHeaders.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", "'" + sHeaders[i] + "'");
                writer.WriteEndElement();
                writer.WriteString("\"");
                if (i != sFields.Length - 1) writer.WriteString((formatType == ExportFormat.CSV) ? "," : "\t");
            }

            // Data rows
            writer.WriteStartElement("xsl:for-each");
            writer.WriteAttributeString("select", "Export/Values");
            writer.WriteString("\r\n");

            for (int i = 0; i < sFields.Length; i++)
            {
                writer.WriteString("\"");
                writer.WriteStartElement("xsl:value-of");
                writer.WriteAttributeString("select", sFields[i]);
                writer.WriteEndElement();
                writer.WriteString("\"");
                if (i != sFields.Length - 1) writer.WriteString((formatType == ExportFormat.CSV) ? "," : "\t");
            }

            writer.WriteEndElement(); // for-each
            writer.WriteEndElement(); // template
            writer.WriteEndElement(); // stylesheet
            writer.WriteEndDocument();
        }

        private string ReplaceSpclChars(string fieldName)
        {
            return fieldName
                .Replace(" ", "_x0020_")
                .Replace("%", "_x0025_")
                .Replace("#", "_x0023_")
                .Replace("&", "_x0026_")
                .Replace("/", "_x002F_");
        }
    }
}

