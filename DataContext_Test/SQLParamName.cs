using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data;

namespace DataContext_Test
{
    class SQLParamName
    {
        private string name;
        private SQLProcedure parent;
        private int id;
        private SqlDbType paramType;
        private double maximumLength;
        private int precision;
        private int scale;
        private string collation;
        private bool finalized;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!finalized)
                    name = value;
            }
        }

        public SQLProcedure Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (!finalized)
                    parent = value;
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if (!finalized)
                    id = value;
            }
        }

        public SqlDbType ParamType
        {
            get
            {
                return paramType;
            }
            set
            {
                if (!finalized)
                    paramType = value;
            }
        }

        public double MaximumLength
        {
            get
            {
                return maximumLength;
            }
            set
            {
                if (!finalized)
                    maximumLength = value;
            }
        }

        public int Precision
        {
            get
            {
                return precision;
            }
            set
            {
                if (!finalized)
                    precision = value;
            }
        }

        public int Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (!finalized)
                    scale = value;
            }
        }

        public string Collation
        {
            get
            {
                return collation;
            }
            set
            {
                if (!finalized)
                    collation = value;
            }
        }

        public SQLParamName()
        {

        }

        public SQLParamName(string name, SQLProcedure procedure, int id, SqlDbType paramType, double maxLength, int precision, int scale, string collation)
        {
            Name = name;
            Parent = procedure;
            ID = id;
            ParamType = paramType;
            MaximumLength = maxLength;
            Precision = precision;
            Scale = scale;
            Collation = collation;

            if (procedure != null)
                finalized = true;
        }

        public void MakeFinal()
        {
            finalized = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (obj.GetType() != typeof(SQLParamName))
                return false;
            SQLParamName comp = (SQLParamName)obj;

            if (this.Name != comp.Name)
                return false;
            if (this.Parent != comp.Parent)
                return false;
            if (this.ID != comp.ID)
                return false;
            if (this.ParamType != comp.ParamType)
                return false;
            if (this.MaximumLength != comp.MaximumLength)
                return false;
            if (this.Precision != comp.Precision)
                return false;
            if (this.Scale != comp.Scale)
                return false;
            if (this.Collation != comp.Collation)
                return false;

            return true;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}\nParent: {1}\nID: {2}\nType: {3}\nMax Length: {4}\nPrecision: {5}\nScale: {6}\nCollation: {7}\nFinalized: {8}",
                name, parent.Name, id, paramType, maximumLength, precision, scale, collation, finalized);
        }
    }
}
