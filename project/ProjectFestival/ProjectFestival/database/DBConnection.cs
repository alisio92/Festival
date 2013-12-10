using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.database
{
    public class DBConnection
    {
        public static ObservableCollection<T> GetDataOutDatabase<T>(string table)
        {
            var objecten = new ObservableCollection<T>();
            string sql = "SELECT * FROM " + table;
            DbDataReader reader = Database.GetData(sql);

            while (reader.Read())
            {
                objecten.Add(Create<T>(reader));
            }
            return objecten;
        }

        private static T Create<T>(IDataRecord record)
        {
            var properties = typeof(T).GetProperties();
            var returnVal = Activator.CreateInstance(typeof(T));

            properties.ToList().ForEach(item =>
            {
                string type = item.GetMethod.ReturnParameter.ParameterType.Name;

                if (type.StartsWith("ContactPersonType"))
                {
                    ContactPersonType c = new ContactPersonType();
                    c.Name = record[item.Name].ToString();
                }
                else if (type.StartsWith("ContactPersonTitle"))
                {
                    ContactPersonTitle c = new ContactPersonTitle();
                    c.Name = record[item.Name].ToString();
                }
                else if (type.StartsWith("TicketType"))
                {
                    TicketType t = new TicketType();
                    t.Name = record[item.Name].ToString();
                }
                else if (!type.StartsWith("ObservableCollection"))
                {
                    item.SetValue(returnVal, Convert.ChangeType(record[item.Name].ToString(), item.PropertyType));
                }
            });
            return (T)returnVal;
        }

        //private static T Create<T>(IDataRecord record)
        //{
        //    var properties = typeof(T).GetProperties();
        //    var returnVal = Activator.CreateInstance(typeof(T));

        //    properties.ToList().ForEach(item =>
        //    {
        //        string type = item.GetMethod.ReturnParameter.ParameterType.Name;
        //        Type type2 = item.GetMethod.ReturnType;
        //        //if (type.StartsWith("ContactPersonType"))
        //        //{
        //        //    Type lol = Type.GetType("ContactPersonType");
        //        //    object instance = Activator.CreateInstance(lol);
        //        //    instance = record;
        //        //}
        //        //else if (type.StartsWith("ContactPersonTitle"))
        //        //{
        //        //    Type lol = Type.GetType("ContactPersonTitle");
        //        //    object instance = Activator.CreateInstance(lol);
        //        //    instance = record;
        //        //}

        //        if (type.StartsWith("ContactPerson"))
        //        {

        //            //TypeConverter conv = TypeDescriptor.GetConverter(item);
        //            //T obj = (T)conv.ConvertFromInvariantString("test");

        //            //object value = "one";

        //            //var converter = TypeDescriptor.GetConverter(type);
        //            //if (converter.CanConvertFrom(value.GetType()))
        //            //{
        //            //    object newObject = converter.ConvertFrom(value);
        //            //    item.SetValue(returnVal,value);
        //            //}

        //            //Convert.ChangeType("bl", typeof(ContactPersonType));
        //            //item.SetValue(returnVal, Convert.ChangeType(record[item.Name], type2));

        //            ContactPersonType c = new ContactPersonType();
        //            c.Name = record[item.Name].ToString();
        //        }

        //        else if (!type.StartsWith("ObservableCollection"))
        //        {
        //            item.SetValue(returnVal, Convert.ChangeType(record[item.Name].ToString(), item.PropertyType));
        //        }
        //    });
        //    return (T)returnVal;
        //}

        //private static T Create<T>(IDataRecord record)
        //{
        //    var properties = typeof(T).GetProperties();
        //    var returnVal = Activator.CreateInstance(typeof(T));
        //    properties.ToList().ForEach(item =>
        //        {
        //            string type = item.GetMethod.ReturnParameter.ParameterType.Name;
        //            if (type.StartsWith("ContactPerson"))
        //            {
        //                //object[] parameters = { record };
        //                //var value = typeof(DBConnection).GetMethod("Create").MakeGenericMethod(item.PropertyType).Invoke(null, parameters);
        //                //item.SetValue(returnVal, value);

        //                //string n = item.PropertyType.ToString();

        //                //ContactPersonType test = (ContactPersonType)record[item.Name];

        //                //item.SetValue(returnVal, typeof(ContactPersonType));

        //                //Object b = type;

        //                //Type myType = type.ToString();

        //                //PropertyInfo[] myPropertyInfo = myType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //                //for (int i = 0; i < myPropertyInfo.Length; i++)
        //                //{
        //                //    PropertyInfo myPropInfo = (PropertyInfo)myPropertyInfo[i];
        //                //    String name = myPropInfo.Name;
        //                //    //Type type = myPropInfo.PropertyType;
        //                //}                 

        //                Type t = Type.GetType(item.GetMethod.ReturnParameter.ParameterType.ToString());
        //                //item.SetValue(returnVal, Convert.ChangeType(record[item.Name].ToString(), t));
        //                //string b= Convert.ChangeType(record[item.Name].ToString(), item.PropertyType.GetType()).ToString();
        //            }
        //            else if (!type.StartsWith("ObservableCollection"))
        //            {
        //                item.SetValue(returnVal, Convert.ChangeType(record[item.Name].ToString(), item.PropertyType));
        //            }
        //        });
        //    return (T)returnVal;
        //}

        //public static int EditItem(string table)
        //{
        //    DbTransaction trans = null;

        //    try
        //    {
        //        trans = Database.BeginTransaction();

        //        string sql = "UPDATE " + table + " SET Name=@Name,Company=@Company,Jobrole=@JobRole,Jobtitle=@JobTitle,City=@City,Email=@Email,Phone=@Phone,Cellphone=@Cellphone WHERE ID=@ID";
        //        DbParameter par1 = Database.AddParameter("@Name", contactPersoon.Name);
        //        DbParameter par2 = Database.AddParameter("@ID", contactPersoon.ID);
        //        DbParameter par3 = Database.AddParameter("@Company", contactPersoon.Company);
        //        DbParameter par4 = Database.AddParameter("@ContactPersonType", contactPersoon.JobRole.Name);
        //        DbParameter par5 = Database.AddParameter("@ContactPersonTitle", contactPersoon.JobTitle.Name);
        //        DbParameter par6 = Database.AddParameter("@City", contactPersoon.City);
        //        DbParameter par7 = Database.AddParameter("@Email", contactPersoon.Email);
        //        DbParameter par8 = Database.AddParameter("@Phone", contactPersoon.Phone);
        //        DbParameter par9 = Database.AddParameter("@Cellphone", contactPersoon.Cellphone);

        //        int rowsaffected = 0;
        //        rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

        //        trans.Commit();
        //        return rowsaffected;
        //    }
        //    catch (Exception)
        //    {
        //        trans.Rollback();
        //        return 0;
        //    }
        //}

        public static int DeleteItem(string table, int id)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM " + table + " WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", id);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }
    }
}
