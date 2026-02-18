using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection;
namespace ePortal.Persistence
{
    public class DataTableConverter
    {
        public List<T> TableToList<T>(DataTable dt) where T : new()
        {
            var modelList = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                T obj = new T();
                foreach (DataColumn column in dt.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop != null && row[column] != DBNull.Value)
                    {
                        try
                        {
                            object value = Convert.ChangeType(row[column], prop.PropertyType);
                            prop.SetValue(obj, value, null);
                        }
                        catch
                        {

                        }
                    }
                }
                modelList.Add(obj);
            }

            return modelList;
        }


        public DataTable ArrayListToDataTable<T>(ArrayList list) where T : class
        {
            var dt = new DataTable(typeof(T).Name);

            // Get writable public properties
            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            // Create columns (handle Nullable<T>)
            foreach (var p in props)
            {
                var colType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                dt.Columns.Add(p.Name, colType);
            }

            // Fill rows
            foreach (var item in list)
            {
                if (item is T obj)
                {
                    var row = dt.NewRow();
                    foreach (var p in props)
                    {
                        var val = p.GetValue(obj, null) ?? DBNull.Value;
                        row[p.Name] = val;
                    }
                    dt.Rows.Add(row);
                }
                else if (item != null)
                {
                    // Optional: throw or ignore
                    // throw new InvalidOperationException($"Item of type {item.GetType().Name} is not {typeof(T).Name}.");
                }
            }

            return dt;
        }
    }
}
