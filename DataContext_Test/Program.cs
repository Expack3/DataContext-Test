using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.Linq;
using System.Data;
using System.Diagnostics;

namespace DataContext_Test
{
    class Program
    {
        const string tableColumnsQuery = "uspGetAllTablesAndColumns";
        const string tableProcsQuery = "uspGetAllTableProcs";
        const string procSourceQuery = "sp_helptext";

        static void Main(string[] args)
        {
            SqlConnection conn = new SqlConnection("Data Source=WIN-JVHVCOHCL3P;Database=testDatabase;Password=Cup86ego!;UID=sa");
            try
            {
                Console.WriteLine("Connecting to {0}", conn.DataSource);
                conn.Open();
                Console.WriteLine("Connection successful");

                DataTable tablesAndColumns = null;

                string[] tableNames = GetAllTableNames(conn, out tablesAndColumns);

                DataTable procedures = GetAllProcedures(tableNames, conn);

                //PrintProcedureResults(tablesAndColumns, procedures);

                SQLProcedure[] procObjs = GenerateProcedureObjects(procedures, conn);
                foreach (SQLProcedure po in procObjs)
                {
                    Console.WriteLine(po.ToString());
                }
            }
            catch (DataException e)
            {
                Console.WriteLine("An error occured when connecting to the database: {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured after connecting to the database: {0}", e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            if (Debugger.IsAttached)
            {
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void PrintProcedureResults(params DataTable[] dts)
        {
            foreach (DataTable dt in dts)
            {
                Console.WriteLine("Contents of {0}:", dt.TableName);
                StringBuilder builder = new StringBuilder();
                foreach (DataColumn dc in dt.Columns)
                {
                    builder.Append(dc.ColumnName.PadRight(25, ' '));
                }

                builder.Append('\n');

                foreach (DataRow dr in dt.Rows)
                {
                    foreach (object o in dr.ItemArray)
                        builder.Append(o.ToString().PadRight(25, ' '));
                    builder.Append('\n');
                }
                Console.WriteLine(builder.ToString());
            }
        }

        static string[] GetAllTableNames(SqlConnection conn, out DataTable dt)
        {
            dt = null;
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand(tableProcsQuery, conn);
                    da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    DataSet ds = new DataSet();
                    da.Fill(ds, "tablesAndColumns_T");
                    dt = ds.Tables["tablesAndColumns_T"];
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (!dt.TableName.Equals("tablesAndColumns_T"))
            {
                throw new ArgumentException("The data table is not named \"tablesAndColumns_T\".");
            }
            List<string> tableNames = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                if (!tableNames.Contains(dr.ItemArray[0]))
                    tableNames.Add((string)dr.ItemArray[0]);
            }
            return tableNames.ToArray();
        }

        //static SQLProcedure[] GetProcedureObjects(DataTable dt)
        //{
        //    if (!dt.TableName.Equals("procedures"))
        //        throw new ArgumentException("The provided data table was not named \"procedures\".");

        //}

        static DataTable GetAllProcedures(string[] listOfTables, SqlConnection conn)
        {
            DataTable final = null;
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand(tableProcsQuery, conn);
                    da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    DataSet ds = new DataSet();
                    da.Fill(ds, "procedures");

                    final = ds.Tables["procedures"];
                }
            }
            catch (Exception)
            {
                throw;
            }
            return final;
        }

        static SQLProcedure[] GenerateProcedureObjects(DataTable dt, SqlConnection conn)
        {
            if (dt == null)
            {
                throw new ArgumentNullException();
            }
            if (!dt.TableName.Equals("procedures"))
            {
                throw new ArgumentException("The data table is not named \"procedures\".");
            }

            List<string> tableNames = new List<string>();
            List<string> procedureNames = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                if (!tableNames.Contains(dr.ItemArray[0]))
                    tableNames.Add((string)dr.ItemArray[0]);
                if (!procedureNames.Contains(dr.ItemArray[1]))
                    procedureNames.Add((string)dr.ItemArray[1]);
            }
            List<DataRow> rows = new List<DataRow>();
            for (int i = 0; i < tableNames.Count; i++)
            {
                rows.AddRange(dt.Select(string.Format("TableName='{0}'", tableNames[i])));
            }
            tableNames.Clear();
            TSQLProcedure[] procedures = new TSQLProcedure[procedureNames.Count()];
                for (int i = 0; i < procedures.Length; i++)
                {
                    DataTable final = null;
                    try
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            SqlParameter param = new SqlParameter();
                            param.DbType = DbType.String;
                            param.Value = procedureNames[i];
                            param.ParameterName = "@objname";

                            da.SelectCommand = new SqlCommand(procSourceQuery, conn);
                            da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            da.SelectCommand.Parameters.Add(param);
                            DataSet ds = new DataSet();
                            da.Fill(ds, "source");

                            final = ds.Tables["source"];
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    procedures[i] = new TSQLProcedure(procedureNames[i], (string)rows.Where(x => x.ItemArray[1].Equals(procedureNames[i])).First().ItemArray[0], 
                        rows.Where(x => x.ItemArray[1].Equals(procedureNames[i])).ToArray(),final);
                }
            return procedures;
        }
    }
}
