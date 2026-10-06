using System.Text.Json;


string json = """
    {
       "vendas": [
            { "vendedor": "João Silva", "valor": 1200.50 },
    { "vendedor": "João Silva", "valor": 950.75 },
    { "vendedor": "João Silva", "valor": 1800.00 },
    { "vendedor": "João Silva", "valor": 1400.30 },
    { "vendedor": "João Silva", "valor": 1100.90 },
    { "vendedor": "João Silva", "valor": 1550.00 },
    { "vendedor": "João Silva", "valor": 1700.80 },
    { "vendedor": "João Silva", "valor": 250.30 },
    { "vendedor": "João Silva", "valor": 480.75 },
    { "vendedor": "João Silva", "valor": 320.40 },

    { "vendedor": "Maria Souza", "valor": 2100.40 },
    { "vendedor": "Maria Souza", "valor": 1350.60 },
    { "vendedor": "Maria Souza", "valor": 950.20 },
    { "vendedor": "Maria Souza", "valor": 1600.75 },
    { "vendedor": "Maria Souza", "valor": 1750.00 },
    { "vendedor": "Maria Souza", "valor": 1450.90 },
    { "vendedor": "Maria Souza", "valor": 400.50 },
    { "vendedor": "Maria Souza", "valor": 180.20 },
    { "vendedor": "Maria Souza", "valor": 90.75 },

    { "vendedor": "Carlos Oliveira", "valor": 800.50 },
    { "vendedor": "Carlos Oliveira", "valor": 1200.00 },

    { "vendedor": "Carlos Oliveira", "valor": 1950.30 },
    { "vendedor": "Carlos Oliveira", "valor": 1750.80 },
    { "vendedor": "Carlos Oliveira", "valor": 1300.60 },
    { "vendedor": "Carlos Oliveira", "valor": 300.40 },
    { "vendedor": "Carlos Oliveira", "valor": 500.00 },
    { "vendedor": "Carlos Oliveira", "valor": 125.75 },

    { "vendedor": "Ana Lima", "valor": 1000.00 },
    { "vendedor": "Ana Lima", "valor": 1100.50 },
    { "vendedor": "Ana Lima", "valor": 1250.75 },
    { "vendedor": "Ana Lima", "valor": 1400.20 },
    { "vendedor": "Ana Lima", "valor": 1550.90 },
    { "vendedor": "Ana Lima", "valor": 1650.00 },
    { "vendedor": "Ana Lima", "valor": 75.30 },
    { "vendedor": "Ana Lima", "valor": 420.90 },
    { "vendedor": "Ana Lima", "valor": 315.40 }
            ]
            }
    """;

string jsonEstoque = """
    {
        "estoque": [
            {
    "codigoProduto": 101,
    "descricaoProduto": "Caneta Azul",
    "estoque": 150

    },
    {
    "codigoProduto": 102,
    "descricaoProduto": "Caderno Universitário",
    "estoque": 75
    },
    {
    "codigoProduto": 103,
    "descricaoProduto": "Borracha Branca",
    "estoque": 200
    },
    {
    "codigoProduto": 104,
    "descricaoProduto": "Lápis Preto HB",
    "estoque": 320
    },
    {
    "codigoProduto": 105,
    "descricaoProduto": "Marcador de Texto Amarelo",
    "estoque": 90
    }
        ]
    }
    """;

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};
var dados = JsonSerializer.Deserialize<DadosVendas>(json, options);
List<Venda> vendas = dados?.Vendas ?? new List<Venda>();

var dadosEstoque = JsonSerializer.Deserialize<DadosEstoque>(jsonEstoque, options);
List<Produto> produtos = dadosEstoque?.Estoque ?? new();

List<MovimentacaoEstoque> movimentacoes = new();
int proximoId = 1;


string continuar;
do
{
    Console.Write("Digite o código do produto: ");
int codigo = Convert.ToInt32(Console.ReadLine());

Produto? produtoEncontrado = produtos.Find(p => p.CodigoProduto == codigo);

if (produtoEncontrado == null)
{
    Console.WriteLine("Produto não encontrado.");
}
else
{
    Console.WriteLine($"Encontrado: {produtoEncontrado.DescricaoProduto}");
    Console.WriteLine($"Estoque atual: {produtoEncontrado.Estoque}");
    Console.Write("Tipo (E = entrada, S = saída): ");
    string tipo = (Console.ReadLine() ?? "").Trim().ToUpper();

    Console.Write("Quantidade: ");
    int quantidade = Convert.ToInt32(Console.ReadLine());

    Console.Write("Descrição: ");
    string descricao = Console.ReadLine() ?? "";

    bool aplicada = false;

    if (quantidade <= 0)
    {
        Console.WriteLine("A quantidade deve ser maior que zero.");
    }
    else if (tipo == "E")
    {
        produtoEncontrado.Estoque += quantidade;
        aplicada = true;
    }
    else if (tipo == "S" && quantidade <= produtoEncontrado.Estoque)
    {
        produtoEncontrado.Estoque -= quantidade;
        aplicada = true;
    }
    else if (tipo == "S")
    {
        Console.WriteLine("Estoque insuficiente para essa saída.");
    }
    else
    {
        Console.WriteLine("Tipo inválido. Digite E ou S.");
    }

    if (aplicada)
    {
        var movimentacao = new MovimentacaoEstoque
        {
            Id = proximoId++,
            CodigoProduto = produtoEncontrado.CodigoProduto,
            Tipo = tipo == "E" ? "Entrada" : "Saída",
            Quantidade = quantidade,
            Descricao = descricao
        };

        movimentacoes.Add(movimentacao);
        Console.WriteLine($"Movimentação #{movimentacao.Id} registrada.");
        Console.WriteLine($"Estoque final: {produtoEncontrado.Estoque}");
    }
}
    Console.Write("Registrar outra movimentação? (S/N): ");
    continuar = (Console.ReadLine() ?? "").Trim().ToUpper();

} while (continuar == "S");

Console.Write("Valor original: R$");
decimal valor = Convert.ToDecimal(Console.ReadLine());

Console.Write("Data de vencimento (dd/MM/aaaa): ");
DateTime vencimento = DateTime.Parse(Console.ReadLine() ?? "");

int diasAtraso = (DateTime.Today - vencimento.Date).Days;

if (diasAtraso < 0)
{
    diasAtraso = 0;
}

decimal juros = valor * 0.025m * diasAtraso;
decimal total = valor + juros;

Console.WriteLine($"Dias de atraso: {diasAtraso}");
Console.WriteLine($"Juros: R${juros:F2}");
Console.WriteLine($"Total com juros: R${total:F2}");

Dictionary<string, decimal> totaisComissao = new();

foreach (Venda venda in vendas)
{
    decimal comissaoVenda= CalcularComissao(venda.Valor);


    Console.WriteLine($"Você fez uma venda de: R${venda.Valor:F2} \n Sua comissão é: R${comissaoVenda:F2}");
    Console.WriteLine(venda.Vendedor);
    Console.WriteLine(venda.Valor);
}

decimal CalcularComissao(decimal valor)
{
    if (valor < 100)
    {
        return 0;
    }
    else if (valor < 500)
    {
        return valor * 0.01m;

    }
    return valor * 0.05m;
}


foreach (Produto produto in produtos)
{
    Console.WriteLine($"Código do Produto: {produto.CodigoProduto}");
    Console.WriteLine($"Descrição do Produto: {produto.DescricaoProduto}");
    Console.WriteLine($"Quantidade em Estoque: {produto.Estoque}");
    Console.WriteLine();
}

Console.WriteLine("Aperte Enter pra fechar...");
Console.ReadLine();
public class Venda
{
    public string Vendedor { get; set; }= "";
    public decimal Valor { get; set; }
}
public class DadosVendas
{
    public List<Venda> Vendas { get; set; } = new();
}

public class Produto
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = "";
    public int Estoque { get; set; }
}

public class DadosEstoque
{
    public List<Produto> Estoque { get; set; } = new();
}

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int CodigoProduto { get; set; }
    public string Tipo { get; set; } = "";
    public int Quantidade { get; set; }
    public string Descricao { get; set; } = "";
}