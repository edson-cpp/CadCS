using Principal;
using System.IO;
using Newtonsoft.Json;

namespace Cadastros;

public class CadProdutos : Principal.Cadastros{
    public dynamic ListarRegistros(int id = 0, bool imp = true){
        string reg = "{}", listaReg = "";
        var lista = JsonConvert.DeserializeObject(reg);
        AcessoBD conn = new AcessoBD();
        string consulta = "Select id, descricao, unidade, preco From produtos";

        try{
            if(id != 0){
                consulta+= $" Where id = {id}";
            }
            string row = conn.ExecutarConsulta(consulta);

            if(row == "{}"){
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Não há registros!");
                Console.WriteLine(new string('-', 40));
            }else{
                if(imp) {Console.WriteLine(new string(id == 0 ? '=' : '-', 40));}
                for (int i = 0; i < row.Count(); i++){
                    reg = $"{{'Código': {row[0]}, 'Descrição': {row[1]}, 'Unidade de Medida': {row[2]}, 'Preço': {row[3]}}}";
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

    public void MenuProdutos(){
        Uteis util = new Uteis();
        var read = "";
        int op = 0;

        while(true){
            try{
                Console.WriteLine(util.PadCenter(40, " CADASTRO DE PRODUTOS ", '='));
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
        string desc = util.CampoStr("Descrição ", 50);
        string und = util.CampoStr("Unidade de Medida  ", 10);
        float preco = util.CampoFloat("Preço ");
        try{
            AcessoBD conn = new AcessoBD();
            conn.ExecutarInsercaoAtualizacaoExclusao($"Insert Into produtos(descricao, unidade, preco) Values('{desc}', '{und}', {preco})");
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
            string desc = util.CampoStr("Descrição ", 50, true, "Descrição", reg["Descrição"]);
            string und = util.CampoStr("Unidade de Medida ", 10, true, "Unidade de Medida", reg["Unidade de Medida"]);
            float preco = util.CampoFloat("Preço ");

            try{
                AcessoBD conn = new AcessoBD();
                conn.ExecutarInsercaoAtualizacaoExclusao($"Update produtos Set descricao = '{desc}', "
                    + $"unidade = '{und}', preco = {preco} Where id = {id}");
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
                    conn.ExecutarInsercaoAtualizacaoExclusao($"Delete From produtos Where id = {id}");
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