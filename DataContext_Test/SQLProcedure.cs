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
    public abstract class SQLProcedure
    {
        protected DataRowCollection source;
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

        public SQLProcedure(string name, string table, DataRow[] dt, DataTable source)
        {
            this.source = source.Rows;

            Name = name;
            Table = table;
            parameterList = new Dictionary<string, SQLParamName>();
            foreach (SQLParamName param in GenerateSQLParams(dt))
            {
                parameterList.Add(param.Name, param);
            }
        }

        public abstract bool RunProcedure();

        public abstract SQLParamName[] GenerateSQLParams(DataRow[] dt);

        protected abstract bool IsParamOptional(string paramName);

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
