using BL.Constants;
using BL.Models.Common;
using BL.Models.FormPage;
using DAL.DBContext;
using DAL.Models.Form;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using PdfSharp.Charting;
using System.Data;

namespace BL.Services.Source
{
    public class SourceControlService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SourceControlService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public async Task<List<Values>> SourceValues(long ElementId)
        {
            NPF_FRM_Elements Elem = new NPF_FRM_Elements();
            NPF_FRM_Source Src = new NPF_FRM_Source();
            NPF_FRM_SourceType SrcT = new NPF_FRM_SourceType();
            List<NPF_FRM_Parameters> Pr = new List<NPF_FRM_Parameters>();
            List<Values> val = new List<Values>();
            try
            {
                Elem = context.NPF_FRM_Elements.Where(x => x.Id == ElementId).FirstOrDefault();
                if (Elem != null)
                {
                    if (Elem.Source != null)
                    {
                        Src = context.NPF_FRM_Source.Where(x => x.Id == Elem.Source.Id).FirstOrDefault();
                        SrcT = context.NPF_FRM_SourceType.Where(x => x.Id == Src.SourceTypeId).FirstOrDefault();
                        switch (SrcT.Name)
                        {
                            case "Query":
                                val = await Values(ElementId, Src.Name);
                                break;
                            //case "Views":
                            //    break;
                            //case "Procedure":
                            //    break;
                            default:
                                break;
                        }
                    }

                }



            }
            catch (Exception ex)
            {
            }
            return val;
        }
        public async Task<List<Values>> Values(long ElementId, string TableName)
        {
            List<NPF_FRM_Parameters> Pr = new List<NPF_FRM_Parameters>();
            List<Values> val = new List<Values>();
            try
            {
                Pr = context.NPF_FRM_Parameters.Where(x => x.ElementId == ElementId && x.DeleteStatus == false).ToList();
                string param = "";
                string AddiColumn = "";
                int count = 0;
                bool isComplex = false;
                if (Pr.Count > 0)
                {
                    foreach (NPF_FRM_Parameters p in Pr)
                    {
                        if(p.ParameterList.IsComplex != null)
                        {
                            isComplex = (bool)p.ParameterList.IsComplex;
                        }
                        if (!p.Value.ToLower().Contains("insert") &&
                            !p.Value.ToLower().Contains("update") &&
                            !p.Value.ToLower().Contains("delete ") &&
                            !p.Value.ToLower().Contains("drop") &&
                            !p.Value.ToLower().Contains("create") &&
                            !p.Value.ToLower().Contains("alter") &&
                            !p.Value.ToLower().Contains("truncate") &&
                            //!p.Value.ToLower().Contains("and") &&
                            //!p.Value.ToLower().Contains("or") &&
                            !p.Value.ToLower().Contains("into") &&
                            !p.Value.ToLower().Contains("database") &&
                            !p.Value.ToLower().Contains("table") &&
                            !p.Value.ToLower().Contains("index") )
                        {
                            if (count == 0)
                            {
                                if (isComplex)
                                {
                                    param = p.Value;
                                }
                                else
                                {
                                    //TODO:Add Id  p.ParameterList.Name 
                                    AddiColumn = p.ParameterList.Description;
                                    if (p.ParameterList.Name != string.Empty || p.ParameterList.Name != "" || p.ParameterList.Name != " ")
                                    {
                                        param += "WHERE a." + p.ParameterList.Name + " = " + p.Value + "";
                                    }
                                }
                                
                                count++;
                            }
                            else
                            {
                                param += " and a." + p.ParameterList.Name + " = " + p.Value + "";
                                count++;
                            }
                        }

                    }

                }
                try
                {
                    List<Values> MyList = new List<Values>();
                    if (isComplex)
                    {
                        MyList = await CollectionFromSql(param);
                    }
                    else
                    {
                        MyList = await CollectionFromSql("SELECT a.Id,a." + AddiColumn + " as text FROM " + TableName + " a " + param + ";");
                    }

                    if (MyList.Count > 0)
                    {
                        val.AddRange(MyList);
                    }
                }
                catch (Exception ex)
                {
                }


            }
            catch (Exception ex)
            {

            }
            return val;
        }
        public async Task<List<Values>> CollectionFromSql(string Sql)
        {
            NPFContext dbContext = new NPFContext();
            List<Values> v = new List<Values>();
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                using (var dataReader = cmd.ExecuteReader())
                {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                Values v31 = new Values()
                                {
                                    id = (long)dataReader["Id"],
                                    text = dataReader["text"].ToString()
                                };
                                v.Add(v31);
                            }
                            dataReader.NextResult();
                        }
                }

            }
            return v;
        }

        public async Task<List<CustomDictionary>> DetailsValues(long ElementId, long Id)
        {
            List<NPF_FRM_Parameters> Pr = new List<NPF_FRM_Parameters>();
            DataTable dt = new DataTable();

            List<CustomDictionary> val = new List<CustomDictionary>();
            try
            {
                Pr = context.NPF_FRM_Parameters.Where(x => x.ElementId == ElementId && x.DeleteStatus == false).ToList();
                string param = "";
                string additional = " and a.Id = " + Id + ";";
                if (Pr.Count > 0)
                {
                    foreach (NPF_FRM_Parameters p in Pr)
                    {
                        if (!p.Value.ToLower().Contains("insert") &&
                            !p.Value.ToLower().Contains("update") &&
                            !p.Value.ToLower().Contains("delete ") &&
                            !p.Value.ToLower().Contains("drop") &&
                            !p.Value.ToLower().Contains("create") &&
                            !p.Value.ToLower().Contains("alter") &&
                            !p.Value.ToLower().Contains("truncate") &&
                            //!p.Value.ToLower().Contains("and") &&
                            //!p.Value.ToLower().Contains("or") &&
                            !p.Value.ToLower().Contains("into") &&
                            !p.Value.ToLower().Contains("database") &&
                            !p.Value.ToLower().Contains("table") &&
                            !p.Value.ToLower().Contains("index"))
                        {
                            param = p.Value.Replace(";", additional);
                        }
                    }
                }
                try
                {
                    dt = await CustomCollectionFromSql(param);

                    Dictionary<string, string?> dictionary = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dt.Rows[0][col].ToString());
                    foreach (var d in dictionary)
                    {
                        CustomDictionary dict = new CustomDictionary() { 
                            key= d.Key, 
                            value = d.Value
                        };
                        val.Add(dict); 
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {

            }
            return val;
        }
        public async Task<DataTable> CustomCollectionFromSql(string Sql)
        {
            NPFContext dbContext = new NPFContext();
            DataTable v = new DataTable();
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        v.Load(dataReader);
                    }
                }

            }
            return v;
        }
    }
}
