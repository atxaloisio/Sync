using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Sync
{
    public class SyncDatabase
    {
        /// <summary>
        /// String de conexão com banco de dados local.
        /// </summary>
        string strConDbLocal = string.Empty;
        /// <summary>
        /// String de conexão com banco de dados remoto.
        /// </summary>
        string strConDbRemoto = string.Empty;
        public SyncDatabase()
        {
            strConDbLocal = ConfigurationManager.ConnectionStrings["MySQLEntities_local"].ConnectionString;
            strConDbRemoto = ConfigurationManager.ConnectionStrings["MySQLEntities_local"].ConnectionString;
        }

        public void SyncClientes()
        {
            MySqlConnection conDbLocal = new MySqlConnection(strConDbLocal);
            MySqlConnection conDbRemoto = new MySqlConnection(strConDbRemoto);


        }
    }
}
