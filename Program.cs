using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.Loja.Testes.ConsoleApp
{
    class Program
    {
        static void Main(String[] args)
        {
            using(var contexto = new LojaContext())
            {
                var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var cliente = contexto.Clientes.Include(c => c.EnderecoDeEntrega).FirstOrDefault();

                
                Console.WriteLine($"Endereço de entrega: {cliente.EnderecoDeEntrega.Logradouro}");

                var produto = contexto
                .Produtos
                .Include(c => c.Compras)
                .Where(p => p.Id == 2002)
                .FirstOrDefault();

                contexto.Entry(produto)
                    .Collection(p => p.Compras)
                    .Query()
                    .Where(c => c.Preco > 10)
                    .Load();

                Console.WriteLine($"Mostrando as compras do produto {produto.Nome}");

                foreach (var item in produto.Compras)
                {
                    Console.WriteLine(item);
                }

            }

        }

        private static void ExibeProdutosDaProducao()
        {
            using (var contexto2 = new LojaContext())
            {
                var serviceProvider = contexto2.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var promocao = contexto2.Promocoes.Include(p => p.Produtos).ThenInclude(pp => pp.Produto).FirstOrDefault();

                Console.WriteLine("\nMotrando os produtos da promoção...");
                foreach (var item in promocao.Produtos)
                {
                    Console.WriteLine(item.Produto);
                }
            }
        }

        private static void IncluirPromocao()
        {
            using (var contexto = new LojaContext())
            {
                var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var promocao = new Promocao();
                promocao.Descricao = "Queima Total 2017";
                promocao.DataInicio = new DateTime(2017, 1, 1);
                promocao.DataFim = new DateTime(2017, 1, 31);

                var produtos = contexto.Produtos.Where(p => p.Categoria == "Bebidas").ToList();

                foreach (var item in produtos)
                {
                    promocao.IncluiProduto(item);
                }
                contexto.Promocoes.Add(promocao);
                contexto.SaveChanges();

            }
        }

        private static void UmParaUm()
        {
            var fulano = new Cliente();
            fulano.Nome = "Fulaninho de tal";
            fulano.EnderecoDeEntrega = new Endereco()
            {
                Numero = 12,
                Logradouro = "Rua Urandi",
                Complemento = "Casa",
                Bairro = "Concordia",
                Cidade = "Belo Horizonte"
            };

            using (var contexto = new LojaContext())
            {
                var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                contexto.Clientes.Add(fulano);
                contexto.SaveChanges();
            }
        }

        private static void MuitosParaMuitos()
        {
            var p1 = new Produto() { Nome = "Suco de Laranja", Categoria = "Bebidas", PrecoUnitario = 8.79, Unidade = "Litros" };
            var p2 = new Produto() { Nome = "Café", Categoria = "Bebidas", PrecoUnitario = 12.45, Unidade = "Gramas" };
            var p3 = new Produto() { Nome = "Macarrão", Categoria = "Alimentos", PrecoUnitario = 4.23, Unidade = "Gramas" };


            var promocaoDePascoa = new Promocao();
            promocaoDePascoa.Descricao = "Pascoa";
            promocaoDePascoa.DataInicio = DateTime.Now;
            promocaoDePascoa.DataFim = DateTime.Now.AddMonths(3);

            promocaoDePascoa.IncluiProduto(p1);
            promocaoDePascoa.IncluiProduto(p2);
            promocaoDePascoa.IncluiProduto(p3);

            using (var contexto = new LojaContext())
            {
                var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                //contexto.Promocoes.Add(promocaoDePascoa);
                var promocao = contexto.Promocoes.Find(1);
                contexto.Promocoes.Remove(promocao);
                contexto.SaveChanges();

            }
        }

        private static void ExibeEntries(IEnumerable<EntityEntry> entries)
        {
            foreach (var e in entries)
            {
                Console.WriteLine(e.Entity.ToString() + " - " + e.State);
            }
        }


    }
}















/*
private static void AtualizarProduto()
{
    //incluir produto
    GravarUsandoEntity();
    RecuperarProdutos();

    //atualizar produto
    using (var contexto = new ProdutoDAOEntity())
    {
        Produto primeiro = contexto.Produtos().First();
        primeiro.Nome = "Warcraft";
        primeiro.Preco = 20.32;
        primeiro.Categoria = "Jogo";
        contexto.Atualizar(primeiro);

    }
        RecuperarProdutos();
}

private static void ExcluirProdutos()
{
    using (var contexto = new ProdutoDAOEntity())
    {
        IList<Produto> produtos = contexto.Produtos();
        foreach (var item in produtos)
        {
            contexto.Remover(item);

        }
        Console.WriteLine("PRODUTOS EXCLUIDOS");

    }
}

private static void RecuperarProdutos()
{
    using (var contexto = new ProdutoDAOEntity())
    {
        IList<Produto> produtos = contexto.Produtos();
        Console.WriteLine($"Foram encontrados {produtos.Count} produtos");
        foreach (var item in produtos)
        {
            Console.WriteLine(item.Nome);
        }
    }

}

private static void GravarUsandoEntity()
{
    Produto p = new Produto();
    p.Nome = "Livro";
    p.Categoria = "Livros";
    p.Preco = 19.89;

    using (var contexto = new ProdutoDAOEntity())
    {
        contexto.Adicionar(p);
    }
}

}
}
*/
