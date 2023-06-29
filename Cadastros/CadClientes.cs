using Principal;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Cadastros;

public class CadClientes : Principal.Cadastros{
    public dynamic ListarRegistros(int id = 0, bool imp = true){
        string reg = "{}", listaReg = "";
        var lista = JsonConvert.DeserializeObject(reg);
        AcessoBD conn = new AcessoBD();
        string consulta = "Select id, nome, rua, num, cep, estado, cidade, bairro, fone, cpf, email From clientes";

        try{
            if(id != 0){
                consulta += $" Where id = {id}";
            }
            string row = conn.ExecutarConsulta(consulta);
            if(row == "{}"){
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Não há registros!");
                Console.WriteLine(new string('-', 40));
            }else{
                if(imp) {Console.WriteLine(new string(id == 0 ? '=' : '-', 40));}
                for (int i = 0; i < row.Count(); i++){
                    reg = $"{{'Código': {row[0]}, 'Nome': {row[1]}, 'Rua': {row[2]}, 'Número': {row[3]},";
                    reg += $"'CEP': {row[4]}, 'Estado': {row[5]}, 'Cidade': {row[6]}, 'Bairro': {row[7]},";
                    reg += $"'Fone': {row[8]}, 'CPF': {row[9]}, 'E-mail': {row[10]}}}";
                    if(imp){
                        Console.WriteLine(reg);
                        Console.WriteLine(new string('-', 40));
                    }else{
                        listaReg += reg;
                    }
                }
                if(id == 0 && imp){
                    Console.WriteLine(new string('=', 40));
                }
            }
        }catch (Exception ex){
            Console.WriteLine($"Ocorreu um erro na consulta dos dados, por favor tente novamente.\n{ex.Message}");
        }
        lista = JsonConvert.DeserializeObject(listaReg);
        return lista == null ? "" : lista;
    }

    public void MenuClientes(){
        Uteis util = new Uteis();
        var read = "";
        int op = 0;

        while(true){
            try{
                Console.WriteLine(util.PadCenter(40, " CADASTRO DE CLIENTES ", '='));
                util.MenuCad();
                Console.Write("Selecione uma opção: ");
                read = Console.ReadLine();
                op = int.Parse(read == null ? "" : read);
            }catch (Exception ex){
                Console.WriteLine($"Ocorreu um erro ao selecionar a opção, por favor tente novamente.\n{ex.Message}");
                continue;
            }

            if(op == 1) this.Cadastrar();
            else if(op == 2) this.Alterar();
            else if(op == 3) this.Excluir();
            else if(op == 4) this.Consultar();
            else if(op == 5) this.Listar();
            else if(op == 6) this.Exportar();
            else if(op == 7) break;
            else{
                Console.WriteLine("Opção inválida, por favor selecione um número de 1 a 7.");
                continue;
            }
            
        }
    }

    public void Cadastrar(){
        Uteis util = new Uteis();
        string nome = util.CampoStr("Nome ", 50);
        string rua = util.CampoStr("Nome da Rua ", 50);
        string num = util.CampoStr("Número ", 10);
        string cep = util.CampoStr("CEP ", 10);
        string estado = util.CampoStr("Estado ", 50);
        string cidade = util.CampoStr("Cidade ", 50);
        string bairro = util.CampoStr("Bairro ", 50);
        string fone = util.CampoStr("Fone ", 14);
        string cpf = util.CampoStr("CPF ", 18);
        string email = util.CampoStr("E-mail ", 50);
        try{
            AcessoBD conn = new AcessoBD();
            conn.ExecutarInsercaoAtualizacaoExclusao("Insert Into clientes(nome, rua, num, cep, estado, cidade, bairro, fone, cpf, email) "
                     + $"Values('{nome}', '{rua}', '{num}', '{cep}', '{estado}', '{cidade}', '{bairro}', '{fone}', '{cpf}', '{email}')");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("Registro incluído com sucesso.");
            Console.WriteLine(new string('-', 40));
        }catch (Exception ex){
            Console.WriteLine($"Ocorreu um erro ao inserir, por favor tente novamente.\n{ex.Message}");
        }
    }

    private void Alterar(){
        Uteis util = new Uteis();
        dynamic reg;
        int id = 0;
        while(true){
            id = util.CampoInt("Código [0 para listar]: ");
            reg = this.ListarRegistros(id);
            if(id != 0) break;
        }

        if(reg.Count() != 0){
            string nome = util.CampoStr("Nome ", 50, true, "Nome", reg["Nome"]);
            string rua = util.CampoStr("Nome da Rua ", 50, true, "Nome da Rua", reg["Rua"]);
            string num = util.CampoStr("Número ", 10, true, "Número", reg["Número"]);
            string cep = util.CampoStr("CEP ", 10, true, "CEP", reg["CEP"]);
            string estado = util.CampoStr("Estado ", 50, true, "Estado", reg["Estado"]);
            string cidade = util.CampoStr("Cidade ", 50, true, "Cidade", reg["Cidade"]);
            string bairro = util.CampoStr("Bairro ", 50, true, "Bairro", reg["Bairro"]);
            string fone = util.CampoStr("Fone ", 14, true, "Fone", reg["Fone"]);
            string cpf = util.CampoStr("CPF ", 18, true, "CPF", reg["CPF"]);
            string email = util.CampoStr("E-mail ", 50, true, "E-mail", reg["E-mail"]);

            try{
                AcessoBD conn = new AcessoBD();
                conn.ExecutarInsercaoAtualizacaoExclusao($"Update clientes Set nome = '{nome}', rua = '{rua}', num = '{num}', "
                        + $"cep = '{cep}', estado = '{estado}',  cidade = '{cidade}', bairro = '{bairro}', fone = '{fone}', "
                        + $"cpf = '{cpf}', email = '{email}' Where id = {id}");
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Registro alterado com sucesso.");
                Console.WriteLine(new string('-', 40));
            }catch (Exception ex){
                Console.WriteLine($"Ocorreu um erro ao alterar, por favor tente novamente.\n{ex.Message}");
            }
        }
    }

    private void Excluir(){
        dynamic reg;
        int id = 0;
        Uteis util = new Uteis();
        while(true){
            id = util.CampoInt("Código [0 para listar]: ");
            reg = this.ListarRegistros(id);
            if(id != 0) break;
        }

        if(reg.Count() != 0){
            try{
                var resp = "";
                while(true){
                    Console.Write("Tem certeza que deseja excluir o registro acima? [S/N]: ");
                    resp = Console.ReadLine();
                    if (resp == null) continue;
                    if ("SN".Contains(resp)) break;
                }

                Console.WriteLine(new string('-', 40));
                if(resp == "S"){
                    AcessoBD conn = new AcessoBD();
                    conn.ExecutarInsercaoAtualizacaoExclusao($"Delete From clientes Where id = {id}");
                    Console.WriteLine("Registro excluído com sucesso.");
                }else{
                    Console.WriteLine("O registro NÃO foi excluído.");
                }
                Console.WriteLine(new string('-', 40));
            }catch (Exception ex){
                Console.WriteLine($"Ocorreu um erro ao excluir, por favor tente novamente.\n{ex.Message}");
            }
        }
    }

    private void Consultar(){
        Uteis util = new Uteis();
        int id = util.CampoInt("Código: ");
        this.ListarRegistros(id);
    }

    private void Listar(){
        this.ListarRegistros();
    }

    private void Exportar(){
        Uteis util = new Uteis();
        util.ExportarArquivoTexto(this, "Cliente");
    }
}