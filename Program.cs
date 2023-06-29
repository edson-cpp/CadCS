using Principal;
using Cadastros;
using Vendas;

internal class Program{
    private static void Main(string[] args){
        Uteis util = new Uteis();
        var read = "";
        int op = 0;

        while(true){
            try{
                Console.WriteLine(util.PadCenter(40, " MENU PRINCIPAL ", '='));
                util.MenuPrin();
                Console.Write("Selecione uma opção: ");
                read = Console.ReadLine();
                op = int.Parse(read == null ? "" : read);
            }catch (Exception ex){
                Console.WriteLine($"Ocorreu um erro ao selecionar a opção, por favor tente novamente.\n{ex.Message}");
                continue;
            }

            if(op == 1){
                CadClientes cadcli = new CadClientes();
                cadcli.MenuClientes();
            }else if(op == 2){
                CadProdutos cadpro = new CadProdutos();
                cadpro.MenuProdutos();
            }else if(op == 3){
                CadContas cadcont = new CadContas();
                cadcont.MenuContas();
            }else if(op == 4){
                Venda ven = new Venda();
                ven.MenuVendas();
            }else if(op == 5){
                break;
            }else{
                Console.WriteLine("Opção inválida, por favor selecione uma opção do menu.");
                continue;
            }
        }
    }
}