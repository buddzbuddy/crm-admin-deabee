using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServerSide
{
    public class SqlServerController
    {
        private Server m_server = null;

        public SqlServerController()
        {
            m_server = new Server();
            m_server.ConnectionContext.LoginSecure = false;
            m_server.ConnectionContext.Login = "sa";
            m_server.ConnectionContext.Password = "QQQwww123";
        }

        public void AttachDatabase(string database, StringCollection files, AttachOptions options)
        {
            m_server.AttachDatabase(database, files, options);

        }

        public void AddBackupDevice(string name)
        {
            BackupDevice device = new BackupDevice(m_server, name);
            m_server.BackupDevices.Add(device);

        }

        public string GetServerVersion(string serverName)
        {
            return m_server.PingSqlServerVersion(serverName).ToString();
        }

        public int CountActiveConnections(string database)
        {
            return m_server.GetActiveDBConnectionCount(database);
        }

        public void DeleteDatabase(string database)
        {
            m_server.KillDatabase(database);
        }

        public void DetachDatabse(string database, bool updatestatistics, bool removeFullTextIndex)
        {
            m_server.DetachDatabase(database, updatestatistics, removeFullTextIndex);
        }

        public void CreateDatabse(string database)
        {
            Database db = new Database(m_server, database);
            db.Create();
        }

        public void CreateTable(string database, string table, List<Column> ColumnList, List<Index> IndexList)
        {
            Database db = m_server.Databases[database];
            Table newTable = new Table(db, table);
            foreach (Column column in ColumnList)
            {
                column.Parent = newTable;
                newTable.Columns.Add(column);
            }

            if (IndexList != null)
            {
                foreach (Index index in IndexList)
                    newTable.Indexes.Add(index);
            }
            newTable.Create();
        }

        public Column CreateColumn(Table table, string name, DataType type, string @default, bool isIdentity, bool nullable)
        {
            Column column = new Column(table, name);
            column.DataType = type;
            column.Default = @default;
            column.Identity = isIdentity;
            column.Nullable = nullable;

            return column;
        }

        public Index CreateIndex(string name, bool isClustered, IndexKeyType type, string[] columnNameList)
        {
            Index index = new Index();

            index.Name = name;
            index.IndexKeyType = type;
            index.IsClustered = isClustered;

            foreach (string columnName in columnNameList)
                index.IndexedColumns.Add(new IndexedColumn(index, columnName));

            return index;
        }

        public void DropTable(string tableName)
        {
            Database db = m_server.Databases["Dynamic"];
            db.Tables[tableName].Drop();
        }

        //public object ShowTable(string database,string table)
        //{
        //    var allData = from 

        //}

    }
}
