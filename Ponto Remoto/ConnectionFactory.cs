using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ponto_Remoto
{
    class ConnectionFactory
    {
        //criação de método de conexão ao BD
        public MySqlConnection GetConnection()
        {
            //busca a string no App.config
            string conexao = ConfigurationManager.ConnectionStrings["bd_controle"].ConnectionString;


            return new MySqlConnection(conexao);
        }
    }
}
