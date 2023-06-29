using System.Data.SqlClient;
using System.Text;
//using System.Text.Json;
using Newtonsoft.Json;

namespace Principal;

public class AcessoBD{
    private SqlConnection Conectar(){
        string server = "localhost\\DEV";
        string database = "cadCS";
        string username = "UNX";
        string password = "1234";
        SqlConnection conn = new SqlConnection();
        conn.ConnectionString = "Server=" + server + ";Database=" + database + ";User Id=" + username + ";Password=" + password;
        return conn;
    }

    /// <summary>
    /// Executa comando SQL insert, update ou delete
    /// </summary>
    /// <param name="strSql">Comando a ser executado</param>
    /// <returns>Retorna a quantidade de registros afetados. Retorna -1 em caso de erro</returns>
    public int ExecutarInsercaoAtualizacaoExclusao(string strSql){
        SqlConnection conn = this.Conectar();
        SqlCommand comando = new SqlCommand(strSql, conn);
        try{
            conn.Open();
            return comando.ExecuteNonQuery();
        }catch(Exception ex){
            Console.WriteLine($"Ocorreu um erro na consulta dos dados, por favor tente novamente.\n{ex.Message}");
            return -1;
        }finally{
            conn.Close();
        }
    }

    /// <summary>
    /// Executa comando SQL select
    /// </summary>
    /// <param name="strSql">Comando a ser executado</param>
    /// <returns>Retorna um arquivo Json com o resultado da consulta</returns>
    public string ExecutarConsulta(string strSql){
        SqlConnection conn = this.Conectar();
        SqlCommand comando = null;
        SqlDataReader sqlReader = null;
        string retornoJson = "";
        try{
            conn.Open();
            comando = new SqlCommand(strSql, conn);
            sqlReader = comando.ExecuteReader();
            var items = new Dictionary<object, Dictionary<string, object>>();
            while (sqlReader.Read())
            {
                var item = new Dictionary<string, object>(sqlReader.FieldCount - 1);
                for (var i = 1; i < sqlReader.FieldCount; i++)
                {
                    item[sqlReader.GetName(i)] = sqlReader.GetValue(i);
                }
                items[sqlReader.GetValue(0)] = item;
            }
            retornoJson = JsonConvert.SerializeObject(items, Formatting.Indented);
        }catch(Exception ex){
            Console.WriteLine($"Ocorreu um erro na consulta dos dados, por favor tente novamente.\n{ex.Message}");
        }finally{
            if (sqlReader != null) sqlReader.Close();
            conn.Close();
        }
        Console.WriteLine(retornoJson);
        return retornoJson;
    }
}
