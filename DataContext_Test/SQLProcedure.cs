using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;

namespace DataContext_Test
{
    class SQLProcedure
    {
        private Dictionary<string, SQLParamName> parameterList;
        public string Name { get; private set; }
        public string Table { get; private set; }
        public int NumberOfParams
        {
            get
            {
                return parameterList.Count;
            }
        }

        public SQLProcedure(string name, string table, DataRow[] dt)
        {
            Name = name;
            Table = table;
            parameterList = new Dictionary<string, SQLParamName>();
            foreach (SQLParamName param in GenerateSQLParams(dt))
            {
                parameterList.Add(param.Name, param);
            }
        }

        public bool RunProcedure(SqlConnection conn, params object[] parameters)
        {
            if (conn.State != ConnectionState.Open)
                throw new DataException("The given connection is not open.");

            if(parameters.Length != NumberOfParams)
            {
                throw new ArgumentOutOfRangeException(string.Format("The procedure {0} requires {1} parameters.",Name,NumberOfParams));
            }
            for(int i = 0; i < parameters.Length; i++)
            {
                if(!SQLServerDatamap.GetType(parameters[i].GetType()).Equals(parameterList.Values.ElementAt(i).ParamType))
                {
                    throw new ArgumentException(string.Format("Argument {0} is of type {1}; expected type is {2}", i,
                        parameters[i].GetType(), parameterList.Values.ElementAt(i).ParamType));
                }
            }
            SqlCommand procedure = new SqlCommand(string.Format("EXEC {0}",Name),conn);
            Parallel.For(0, parameters.Length, (i, loop) =>
            {
                var paramCopy = parameterList.Values.ElementAt(i);
                SqlParameter parameter = new SqlParameter(paramCopy.Name, paramCopy.ParamType);
                parameter.Scale = (byte)paramCopy.Scale;
                parameter.Precision = (byte)paramCopy.Precision;
                parameter.Value = parameters[i];
                if (paramCopy.Collation != null)
                    parameter.XmlSchemaCollectionName = paramCopy.Collation;
                procedure.Parameters.Add(parameter);
            });
            try
            {
                procedure.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                procedure.Dispose();
            }
            return true;
        }

        public SQLParamName[] GenerateSQLParams(DataRow[] dt)
        {
            SQLParamName[] SQLparameters = new SQLParamName[dt.Length];
            for (int i = 0; i < dt.Length; i++)
            {
                DataRow dr = dt[i];
                SQLParamName param = new SQLParamName();

                param.Name = (string)dr.ItemArray[2];
                param.Parent = this;
                param.ID = (int)dr.ItemArray[3];
                param.ParamType = GetSqlTypeFromString((string)dr.ItemArray[4]);
                param.MaximumLength = Convert.ToDouble(dr.ItemArray[5]);
                param.Precision = (int)dr.ItemArray[6];

                if (dr.ItemArray[7].GetType() == typeof(int))
                    param.Scale = (int)dr.ItemArray[7];
                else
                    param.Scale = -1;

                if(dr.ItemArray[8].GetType() == typeof(DBNull))
                    param.Collation = null;
                else
                    param.Collation = (string)dr.ItemArray[8];

                param.MakeFinal();
                SQLparameters[i] = param;
            }
            return SQLparameters;
        }

        //this should be an extension method for string
        private SqlDbType GetSqlTypeFromString (string s)
        {
            string[] types = Enum.GetNames(typeof(SqlDbType));

            //convert all enum names to lowercase
            for(int i = 0; i < types.Length; i++)
            {
                types[i] = types[i].ToLower();
            }

            //make sure s is also lowercase
            s = s.ToLower();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Equals(s))
                    return (SqlDbType)i;
            }
            throw new ArgumentException(string.Format("{0} is not a valid SQL datatype.", s));
        }

        public SQLParamName[] GetAllParamaters()
        {
            SQLParamName[] parameters = new SQLParamName[parameterList.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameterList.Values.ElementAt(i);
            }
            return parameters;
        }

        public SQLParamName[] GetParameters(params string[] parameterNames)
        {
            List<SQLParamName> parameters = new List<SQLParamName>();
            for (int i = 0; i < parameterNames.Length; i++)
            {
                if (parameterList.Keys.Contains(parameterNames[i]))
                {
                    parameters.Add(parameterList[parameterNames[i]]);
                }
            }
            return parameters.ToArray();
        }

        private int DetermineColumn(string columnName)
        {
            string lowercaseColumn = columnName.ToLower();
            if (lowercaseColumn.Contains("tablename"))
                return 0;
            if (lowercaseColumn.Contains("procname"))
                return 1;
            if (lowercaseColumn.Contains("paramname"))
                return 2;
            if (lowercaseColumn.Contains("parameter"))
                return 3;
            if (lowercaseColumn.Contains("datatype"))
                return 4;
            if (lowercaseColumn.Contains("max_len"))
                return 5;
            if (lowercaseColumn.Contains("prec"))
                return 6;
            if (lowercaseColumn.Contains("scale"))
                return 7;
            if (lowercaseColumn.Contains("collation"))
                return 8;

            return -1;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;

            SQLProcedure newObj = (SQLProcedure)obj;
            if (this.Name == newObj.Name && this.Table == newObj.Table && this.GetAllParamaters() == newObj.GetAllParamaters())
                return true;
            return false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Name: {0}", Name);
            builder.AppendFormat("Table: {0}", Table);
            builder.AppendLine("Procedures:");
            foreach (SQLParamName pn in parameterList.Values)
            {
                builder.AppendLine(pn.ToString());
            }
            return builder.ToString();
        }
    }
}
