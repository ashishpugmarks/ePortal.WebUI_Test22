using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.Persistence
{
    public static class DataTableExtensions
    {
        //public static IEnumerable<SelectListItem> AsSelectList(this DataTable dt, string valueField, string textField)
        //{
        //    return dt.AsEnumerable().Select(r => new SelectListItem
        //    {
        //        Value = r[valueField].ToString(),
        //        Text = r[textField].ToString()
        //    });
        //}

        public static IEnumerable<SelectListItem> AsSelectList(this DataTable dt, string valueField, string textField, bool addDefault = true, string defaultText = "Select", string defaultValue = "0")
        {
            var list = new List<SelectListItem>();

            if (addDefault)
            {
                list.Add(new SelectListItem
                {
                    Text = defaultText,
                    Value = defaultValue
                });
            }

            if (dt != null)
            {
                list.AddRange(dt.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r[valueField]?.ToString(),
                    Text = r[textField]?.ToString()
                }));
            }

            return list;
        }


        //public static IEnumerable<SelectListItem> AsSelectList_DS(this DataSet ds, string valueField, string textField)
        //{           

        //    return ds.Tables[0].AsEnumerable().Select(r => new SelectListItem
        //    {
        //        Value = r[valueField].ToString(),
        //        Text = r[textField].ToString()
        //    });
        //}


        //public static IEnumerable<SelectListItem> AsSelectList_DS(this DataSet ds, string valueField, string textField, bool addDefault = true, string defaultText = "Select", string defaultValue = "0")
        //{
        //    var list = new List<SelectListItem>();

        //    if (addDefault)
        //    {
        //        list.Add(new SelectListItem
        //        {
        //            Text = defaultText,
        //            Value = defaultValue
        //        });
        //    }

        //    if (ds != null && ds.Tables[0].Rows.Count>0)
        //    {
        //        list.AddRange(ds.Tables[0].AsEnumerable().Select(r => new SelectListItem
        //        {
        //            Value = r[valueField]?.ToString(),
        //            Text = r[textField]?.ToString()
        //        }));
        //    }

        //    return list;
        //}

        public static IEnumerable<SelectListItem> AsSelectList_DS(this DataSet ds, string valueField, string textField, bool addDefault = true, string defaultText = "Select", string defaultValue = "0", bool addOther = false, string otherText = "Other", string otherValue = "")
        {
            var list = new List<SelectListItem>();

            if (addDefault)
            {
                list.Add(new SelectListItem
                {
                    Text = defaultText,
                    Value = defaultValue
                });
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                list.AddRange(ds.Tables[0].AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r[valueField]?.ToString(),
                    Text = r[textField]?.ToString()
                }));
            }

            if (addOther)
            {
                list.Add(new SelectListItem
                {
                    Text = otherText,
                    Value = otherValue
                });
            }

            return list;
        }


        public static string ComposeAllIds(this DataTable dt, string columnName, bool distinct = true)
        {
            // null or empty table
            if (dt == null || dt.Rows.Count == 0) return string.Empty;

            // column existence check (prevents ArgumentException)
            if (!dt.Columns.Contains(columnName)) return string.Empty;

            // AsEnumerable requires System.Data.DataSetExtensions
            var values = dt.AsEnumerable()
                            .Select(r => r[columnName]?.ToString())
                            .Where(s => !string.IsNullOrWhiteSpace(s));

            if (distinct) values = (EnumerableRowCollection<string?>)values.Distinct();

            // Join with commas; trim to be safe
            var joined = string.Join(",", values).Trim();

            return joined; // may be "" if nothing valid
        }

        public static string ComposeAllIds(this IEnumerable<SelectListItem> list, string defaultValue = "0")
        {
            if (list == null) return string.Empty;

            var ids = list
                .Where(i => i != null && i.Value != defaultValue && !string.IsNullOrWhiteSpace(i.Value))
                .Select(i => i.Value)
                .Distinct()
                .ToArray();

            return string.Join(",", ids);
        }
    }

}
