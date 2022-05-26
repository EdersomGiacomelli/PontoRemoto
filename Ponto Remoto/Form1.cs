using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Ponto_Remoto
{
    public partial class Form1 : Form
    {
        private MySqlConnection conexao;
        public Form1()
        {
            InitializeComponent();
            this.conexao = new ConnectionFactory().GetConnection();

        }

        #region Conexão com o BD

       //conexão com o banco de dados
        public static MySqlConnection Conexao;

        //função para execução das querys
        public static MySqlCommand Comando;

        //responsável por inserir dados em uma datatable
        public static MySqlDataAdapter Adaptador;

        //liga banco com propriedades datasource        
        public static DataTable datTabela;

        public static void conectar()
        {
            //cria string de conexão ao BD
            string strCon = @"server=127.0.0.1;user id=root;Pwd=masterkey;persistsecurityinfo=True;";


            //estabelece parametros de conexão ao banco
            Conexao = new MySqlConnection(strCon);

            //Abre conexão com bd
            Conexao.Open();

            //autoriza alteração sem primary key
            Comando = new MySqlCommand("SET SQL_SAFE_UPDATES=0;", Conexao);

            Comando.ExecuteNonQuery();

            Conexao.Close();
            
        }


        #endregion



        #region Atualização de horários e tab automático

        //tab automático no textbox entra/sai
        private void txt_EntraSai_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.Length == 1)
            {
                SendKeys.Send("{TAB}");

            }
        }


        //tab automático no textbox senha e seleciona o usuário e puxa os dados
        private void txt_Senha_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.Length == 4)
            {
                SendKeys.Send("{TAB}");
                try
                {
                    
                    //define comando sql
                    string consulta = @"select nome, cpf, id from tb_usuarios where senha=@senha";

                    //organização do comando e parametros
                    MySqlCommand executaCmd = new MySqlCommand(consulta, conexao);
                    executaCmd.Parameters.AddWithValue("@senha", txt_SenhaPonto.Text);

                    //abre a conexao e executa o comando
                    conexao.Open();
                    executaCmd.ExecuteNonQuery();

                    //recebe o conteúdo do select do banco
                    MySqlDataReader dr;
                    dr = executaCmd.ExecuteReader();
                    dr.Read();

                    txt_Nome.Text = dr.GetString(0);
                    txt_Cpf.Text = dr.GetString(1);
                    txt_Id.Text = dr.GetString(2);


                    //fecha a conexão
                    conexao.Close();
                    
                    //if(txt_EntraSai.Text == "s" || txt_EntraSai.Text == "S") {

                    //}
                }
                catch (Exception erro)
                {

                    MessageBox.Show("Erro ao buscar: " + erro);
                }

            }
        }



       #endregion

        private void txt_SenhaPonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            //permite apenas números
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {

                e.Handled = true;

            }
        }

        
        private void txt_Data_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        //data e hora capturados automaticamente do sistema
        private void timer1_Tick(object sender, EventArgs e)
        {
            txt_Horario.Text = DateTime.Now.ToString("HH:mm:ss");

            txt_Data.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        //clique no salvar - insere os dados no banco
        private void btn_Salvar_Click_1(object sender, EventArgs e)
        {
            if (txt_EntraSai.Text == "e" || txt_EntraSai.Text == "E")
            {

                string cpf, nome, data, entrada, saida;
                int senha, Fk_id;

                cpf = txt_Cpf.Text;
                nome = txt_Nome.Text;
                data = txt_Data.Text;
                entrada = txt_Horario.Text;
                saida = txt_Horario.Text;
                senha = int.Parse(txt_SenhaPonto.Text);
                Fk_id = int.Parse(txt_Id.Text);


                
                //vai fazer um insert na tabela de horários com valores obtidos e saida como mesmo horário da entrada
                
                string sqlinsert = @"insert into tb_horarios
                                    (Cpf_Reg, Nome_Reg, Data_Reg, Entrada, Saida, Senha_Reg, Fk_Id)
                                    SELECT @Cpf_Reg, @Nome_Reg, @Data_Reg, @Entrada, @Saida, @Senha_Reg, @Fk_Id 
                                    FROM dual
                                    WHERE NOT EXISTS (SELECT * FROM tb_horarios
                                    WHERE Senha_Reg = @Senha_Reg 
                                    AND Data_Reg = @Data_Reg);";

                MySqlCommand executaCmd = new MySqlCommand(sqlinsert, conexao);

                //Recebe dados da tela e envia para o BD

                executaCmd.Parameters.AddWithValue("@Cpf_Reg", cpf);
                executaCmd.Parameters.AddWithValue("@Nome_Reg", nome);
                executaCmd.Parameters.AddWithValue("@Data_Reg", data);
                executaCmd.Parameters.AddWithValue("@Entrada", entrada);
                executaCmd.Parameters.AddWithValue("@Saida", saida);
                executaCmd.Parameters.AddWithValue("@Senha_Reg", senha);
                executaCmd.Parameters.AddWithValue("@Fk_Id", Fk_id);


                //Envia para o Banco de dados

                //abre a conexão, executa o SQL
                conexao.Open();

                executaCmd.ExecuteNonQuery();

                //fecha a conexão
                conexao.Close();

                //mostra a mensagem
                MessageBox.Show("Entrada registrada!");

                

            }
            else if (txt_EntraSai.Text == "s" || txt_EntraSai.Text == "S")
            {

                try
                {
                    
                    //define comando sql
                    string consulta = @"select Id_Reg from tb_horarios where Data_Reg =@Data_Reg and Senha_Reg =@Senha_Reg;";
                    //organização do comando e parametros
                    MySqlCommand executacmd = new MySqlCommand(consulta, conexao);
                    executacmd.Parameters.AddWithValue("@Data_Reg", txt_Data.Text);
                    executacmd.Parameters.AddWithValue("@Senha_Reg", int.Parse(txt_SenhaPonto.Text));

                    //abre a conexao e executa o comando
                    conexao.Open();
                    executacmd.ExecuteNonQuery();

                    //recebe o conteúdo do select do banco
                    MySqlDataReader dr;
                    dr = executacmd.ExecuteReader();
                    dr.Read();

                    txt_registro.Text = dr.GetString(0);

                    //fecha a conexão
                    conexao.Close();
                    

                    
                }
                catch (Exception erro)
                {

                    MessageBox.Show("Erro:" + erro);
                }


                
                 
                string Saida = txt_Horario.Text;
                int Id = int.Parse(txt_registro.Text);
                
               //definir o comando a ser executado SQL
                string sqlup = @"update tb_horarios set Saida=@Saida Where Id_Reg=@Id";

                //Recebe dados da tela e envia para o BD
                MySqlCommand executaCmd = new MySqlCommand(sqlup, conexao);
                executaCmd.Parameters.AddWithValue("@Id", Id);
                executaCmd.Parameters.AddWithValue("@Saida", Saida);

                //Envia para o Banco de dados

                //abre a conexão, executa o SQL
                conexao.Open();

                executaCmd.ExecuteNonQuery();

                //fecha a conexão
                conexao.Close();

                //mostra a mensagem
                MessageBox.Show("Saída registrada!");

                

            }
            else
            {
                MessageBox.Show("Digite apenas E para entrada ou S para saída!");
            }
            txt_EntraSai.Text = string.Empty;
            txt_SenhaPonto.Text = string.Empty;
            txt_Id.Text = string.Empty;
            txt_Nome.Text = string.Empty;
            txt_Cpf.Text = string.Empty;
            txt_registro.Text = string.Empty;
            txt_EntraSai.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conectar();
        }
    }
}
