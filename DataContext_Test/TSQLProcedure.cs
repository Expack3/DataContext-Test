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
    public class TSQLProcedure : SQLProcedure
    {
        public TSQLProcedure(string name, string table, DataRow[] dt, DataTable source) : base(name,table,dt,source)
        {

        }

        public override SQLParamName[] GenerateSQLParams(DataRow[] dt)
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

                if (dr.ItemArray[8].GetType() == typeof(DBNull))
                    param.Collation = null;
                else
                    param.Collation = (string)dr.ItemArray[8];

                param.Optional = IsParamOptional(param.Name);

                param.MakeFinal();
                SQLparameters[i] = param;
            }
            return SQLparameters;
        }

        //this should be an extension method for string
        private SqlDbType GetSqlTypeFromString(string s)
        {
            string[] types = Enum.GetNames(typeof(SqlDbType));

            //convert all enum names to lowercase
            for (int i = 0; i < types.Length; i++)
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

        public override bool RunProcedure()
        {
            throw new NotImplementedException();
        }

        protected override bool IsParamOptional(string paramName)
        {
            foreach (DataRow dr in source)
            {
                string contents = ((string)dr.ItemArray[0]);
                if (contents.Contains(paramName) && contents.Contains("null"))
                    return true;
            }
            return false;
        }
    }
}
