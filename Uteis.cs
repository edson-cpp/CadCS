using System;
using System.Linq;
using System.IO;
using System.Text;

namespace Principal;

public interface Cadastros{
    public dynamic ListarRegistros(int id, bool imp);
}

public class Uteis{

    /// <summary>
    /// Exibe menu da tela principal
    /// </summary>
    public void MenuPrin(){
        Console.WriteLine("[ 1 ] - Cadastro de Clientes");
        Console.WriteLine("[ 2 ] - Cadastro de Produtos");
        Console.WriteLine("[ 3 ] - Cadastro de Contas");
        Console.WriteLine("[ 4 ] - Vendas");
        Console.WriteLine("[ 5 ] - Sair");
    }

    /// <summary>
    /// Exibe menu das telas de cadastro
    /// </summary>
    public void MenuCad(){
        Console.WriteLine("[ 1 ] - Cadastrar");
        Console.WriteLine("[ 2 ] - Alterar");
        Console.WriteLine("[ 3 ] - Excluir");
        Console.WriteLine("[ 4 ] - Consultar");
        Console.WriteLine("[ 5 ] - Listar");
        Console.WriteLine("[ 6 ] - Exportar");
        Console.WriteLine("[ 7 ] - Menu");
    }

    /// <summary>
    /// Controla possíveis erros de digitação ou de tipos incorretos passados pelo usuário: Formatação Int
    /// </summary>
    /// <param name="msg">Mensagem a ser passada antes do ReadLine - Type(str)</param>
    /// <returns>Retorna o valor digitado pelo usuário</returns>
    public int CampoInt(string msg){
        var valor = "";
        while (true){
            try{
                Console.Write(msg);
                valor = Console.ReadLine();
                break;
            }catch (Exception ex){
                Console.WriteLine($"Tipo de dado incorreto, por favor informe um valor inteiro.\n{ex.Message}");
                continue;
            }
        }
        return valor == null ? 0 : int.Parse(valor);
    }

    /// <summary>
    /// Controla possíveis erros de digitação ou de tipos incorretos passados pelo usuário: Formatação Float
    /// </summary>
    /// <param name="msg">Mensagem a ser passada antes do ReadLine - Type(str)</param>
    /// <returns>Retorna o valor digitado pelo usuário</returns>
    public float CampoFloat(string msg){
        var valor = "";
        while (true){
            try{
                Console.Write(msg);
                valor = Console.ReadLine();
                valor = valor == null ? "" : valor.Replace(',', '.');
                if(valor.Count(f => (f == '.')) > 1){
                    Console.WriteLine("Formato inválido, não informe pontuação na milhar. Use o formato: 1200,25");
                    continue;
                }
                break;
            }catch (Exception ex){
                Console.WriteLine($"Tipo de dado incorreto, por favor informe um valor numérico.\n{ex.Message}");
                continue;
            }
        }
        return float.Parse(valor);
    }

    /// <summary>
    /// Controla possíveis erros de digitação ou de tipos incorretos passados pelo usuário: Formatação String
    /// </summary>
    /// <param name="msg">Mensagem a ser passada no input - Type(str)</param>
    /// <param name="comp">comprimento máximo do campo - Type(int)</param>
    /// <param name="alt">Define se irá perguntar ao cliente se campo será alterado</param>
    /// <param name="campo">Informa nome do campo que será exibido para o cliente perguntando se quer alterar</param>
    /// <param name="valor">Valor que se encontra atualmente na base de dados</param>
    /// <returns>Retorna o valor digitado pelo usuário</returns>
    public string CampoStr(string msg, int comp=0, bool alt=false, string campo="", string valor=""){
        var alterar = "S";
        while (true){
            try{
                if(alt){
                    while (true){
                        Console.Write($"Alterar campo {campo}? Valor atual {valor}: [S/N]: ");
                        alterar = Console.ReadLine();
                        alterar = alterar == null ? "" : alterar.Substring(0, 1).ToUpper();
                        if("SN".Contains(alterar)) break;
                    }
                }
                if(alterar == "S"){
                    if(comp == 0){
                        Console.Write(msg);
                        alterar = Console.ReadLine();
                    }else{
                        Console.Write(msg + $"[ Max {comp} ]: ");
                        alterar = Console.ReadLine();
                    }
                }
            }catch(Exception ex){
                Console.WriteLine($"Tipo de dado incorreto, por favor tente novamente.\n{ex.Message}");
                continue;
            }
            return alterar == null ? "" : alterar;
        }
    }

    /// <summary>
    /// Exporta dados para arquivo texto
    /// </summary>
    /// <param name="cls">Classe que irá exportar os dados</param>
    /// <param name="orig">Origem dos dados a serem exportados (informar capitalizado)</param>
    public void ExportarArquivoTexto(Cadastros cls, string orig){
        string dir = "";
        int id = CampoInt($"Informe o Código do {orig} [0 = Todos]: ");
        string reg = cls.ListarRegistros(id, false);
        if(reg == ""){
            return;
        }
        string arq = CampoStr("Informe o nome do arquivo onde serão salvos os dados: ", 50);
        while(true){
            dir = CampoStr("Informe o caminho completo do arquivo [C = Cancelar]: ", 100);
            if("Cc".Contains(dir) || Path.Exists(dir)){
                break;
            }else{
                Console.WriteLine("Diretório inexistente!");
            }
        }
        if(!"Cc".Contains(dir)){
            try{
                dir = dir.Replace("\\", "/");
                if(dir.Substring(dir.Length - 1, 1) != "/"){
                    dir += "/";
                }
                if(arq.Substring(arq.Length - 4, 4) != ".txt"){
                    arq += ".txt";
                }
                StreamWriter a = new StreamWriter(dir + arq);
                string conteudo = "";
                foreach(int i in reg){
                    conteudo += i.ToString() + "\n";
                }
                a.Write(conteudo);
                a.Close();
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Registro exportado com sucesso.");
                Console.WriteLine(new string('-', 40));
            }catch(Exception ex){
                Console.WriteLine($"Ocorreu um erro ao exportar os dados, por favor tente novamente.\n{ex.Message}");
            }
        }
    }

    /// <summary>
    /// Retorna uma repetição de caracteres
    /// </summary>
    /// <param name="c">Caracter que deve ser repetido na string</param>
    /// <param name="qtde">Quantidade de vezes que o caracter deve ser repetido</param>
    /// <returns>Retorna uma string com a repetição do caracter informado em c na quantidade informada em qtde</returns>
    public string Print(string c, int qtde){
        return new StringBuilder(qtde).Insert(0, c, qtde).ToString();
    }

    /// <summary>
    /// Faz o padding centralizando a string passada como parâmetro
    /// </summary>
    /// <param name="compTotal">Quantidade de caracteres totais retornados</param>
    /// <param name="expressao">String que será centralizada pelo caracter informado</param>
    /// <param name="padChar">Caracter que será impresso em ambos os lados para centralizar a string passada</param>
    /// <returns>Retorna string com caracteres envolvendo a string passada à direita e à esquerda</returns>
    public string PadCenter(int compTotal, string expressao, char padChar){
        return expressao.PadLeft(((compTotal - expressao.Length) / 2) + expressao.Length, padChar).PadRight(compTotal, padChar);
    }
}