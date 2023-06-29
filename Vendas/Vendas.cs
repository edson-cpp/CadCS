using Principal;
using Cadastros;
using System.IO;
using System;
using Newtonsoft.Json;

namespace Vendas;

public class Venda{
    public dynamic ListarRegistros(int id = 0, bool imp = true, int item = 0){
        string reg = "{}", listaReg = "";
        var lista = JsonConvert.DeserializeObject(reg);
        AcessoBD conn = new AcessoBD();
        string consulta;
        if(item == 0)
            consulta = "Select id, nome_clientes, dataven, totalven From vendas";
        else
            consulta = "Select id_produtos, descricao_produtos, preco, qtde, total_item From proven";

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
                    if(item == 0)
                        reg = $"{{'Código': {row[0]}, 'Nome do Cliente': {row[1]}, 'Data da Venda': {row[2]}, 'Total': {row[3]}}}";
                    else
                        reg = $"{{'Código do Produto': {row[0]}, 'Descrição': {row[1]}, 'Preço': {row[2]}, 'Qtde': {row[3]}, 'Total do Item': {row[4]}}}";
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

    public void MenuVendas(){
        var read = "";
        Uteis util = new Uteis();
        int op = 0;

        while(true){
            try{
                Console.WriteLine(util.PadCenter(40, " VENDAS ", '='));
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
        CadClientes cadcli = new CadClientes();
        CadProdutos cadpro = new CadProdutos();
        dynamic reg, proven = "", ret;
        int id = 0, idcli = 0, cod = 0;
        float qtde = 0, totven = 0, preco = 0;
        string nomecli = "", resp = "", desc = "";

        while (true){
            id = util.CampoInt("Informe o código do cliente [0 para listar os cadastros, -1 para cancelar a inclusão]: ");
            if(id == 0) cadcli.ListarRegistros();
            else if(id == -1) return;
            else{
                reg = cadcli.ListarRegistros(id, false);
                if(reg){
                    idcli = id;
                    nomecli = reg[0]["Nome"];
                    break;
                }
            }
        }

        while(true){
            id = util.CampoInt("Informe o código do produto [0 para listar os cadastros, -1 para cancelar a inclusão]: ");
            if(id == 0) cadpro.ListarRegistros();
            else if(id == -1) return;
            else{
                reg = cadpro.ListarRegistros(id, false);
                if(reg.Count()){
                    qtde = util.CampoFloat("Informe a quantidade vendida: ");
                    reg[0]["Qtde"] = qtde;
                    proven.Add(reg[0]);
                    while(true){
                        resp = util.CampoStr("Incluir outro produto? [S/N]: ").ToUpper();
                        if("SN".Contains(resp)) break;
                    }
                    if(resp == "N") break;
                }
            }
        }

        foreach(dynamic i in proven)
            totven += i["Preço"] * i["Qtde"];

        try{
            AcessoBD conn = new AcessoBD();
            conn.ExecutarInsercaoAtualizacaoExclusao("Insert Into vendas(id_clientes, nome_clientes, dataven, totalven) "
                                            + $"Values({idcli}, '{nomecli}', '{DateTime.Now.ToString("yyyy-MM-dd")}', {totven})");
            ret = conn.ExecutarConsulta("Select max(id) From vendas");
            foreach(dynamic i in proven){
                cod = i["Código"];
                desc = i["Descrição"];
                preco = i["Preço"];
                qtde = i["Qtde"];
                ret = conn.ExecutarInsercaoAtualizacaoExclusao(
                    "Insert Into proven(id_vendas, id_produtos, descricao_produtos, preco, qtde, total_item) "
                    + $"Values({ret[0]}, {cod}, '{desc}', {preco}, {qtde}, {preco * qtde})");
            }
            Console.WriteLine(new string('-', 40));
            Console.WriteLine("Registro incluído com sucesso.");
            Console.WriteLine(new string('-', 40));
        }catch (Exception ex){
            Console.WriteLine($"Ocorreu um erro ao inserir, por favor tente novamente.\n{ex.Message}");
        }
    }

    public void Alterar(){
        Console.WriteLine(new string('-', 40));
        Console.WriteLine("Função não implementada!");
        Console.WriteLine(new string('-', 40));
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
                    conn.ExecutarInsercaoAtualizacaoExclusao($"Delete From vendas Where id = {id}");
                    conn.ExecutarInsercaoAtualizacaoExclusao($"Delete From proven Where id_vendas = {id}");
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
        dynamic reg, regpro;
        string arq = "", dir = "", conteudo = "";

        int id = util.CampoInt("Informe o Código da Venda [0 = Todas]: ");
        reg = this.ListarRegistros(id, false);
        if(reg.Count() != 0){
            arq = util.CampoStr("Informe o nome do arquivo onde serão salvos os dados: ", 50);
            while(true){
                dir = util.CampoStr("Informe o caminho completo do arquivo [C = Cancelar]: ", 100);
                if("Cc".Contains(dir) || File.Exists(dir)) break;
                else Console.WriteLine("Diretório inexistente!");
            }

            if(!"Cc".Contains(dir)){
                try{
                    dir = dir.Replace('\\', '/');
                    if(dir.Substring(dir.Length - 1, 1) != "/") dir += "/";
                    if(arq.Substring(arq.Length - 4, 4) != ".txt") arq += ".txt";
                    StreamWriter a;
                    if(File.Exists(dir + arq))
                        a = File.AppendText(dir + arq);
                    else
                        a = File.CreateText(dir + arq);

                    foreach(dynamic i in reg){
                        conteudo += i.ToString() + "\n";
                        regpro = this.ListarRegistros(i["Código"], false, 1);
                        foreach(dynamic x in regpro)
                            conteudo += "    " + x.ToString() + "\n";
                        conteudo += "\n";
                    }
                    a.WriteLine(conteudo);
                    a.Close();
                    Console.WriteLine(new string('-', 40));
                    Console.WriteLine("Registro exportado com sucesso.");
                    Console.WriteLine(new string('-', 40));
                }catch (Exception ex){
                    Console.WriteLine($"Ocorreu um erro ao exportar os dados, por favor tente novamente.\n{ex.Message}");
                }
            }
        }
    }
}