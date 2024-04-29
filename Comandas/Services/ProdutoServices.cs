using Comandas.Data;
using Comandas.Data.Models;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace Comandas.Services
{
    public class ProdutoServices : IProdutoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly ICategoriaServices _categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProdutoServices(ApplicationDbContext context, CurrentUserService user, ICategoriaServices categories, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _user = user;
            _categories = categories;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task AddProdutoAsync(Produto produto)
        {
#pragma warning disable CS8629 // O tipo de valor de nulidade pode ser nulo.
            var categoria = await _categories.GetCategoriasByIdAsync((Guid)produto.ID_categoria);
#pragma warning restore CS8629 // O tipo de valor de nulidade pode ser nulo.

            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var conferirProdutos = await GetAllProdutosAsync();
            produto.ApplicationUserId = userId;
            produto.Categoria = categoria;
            produto.NomeDaCategoria = categoria.Nome;

            if (produto.Quantidade == null) { produto.Quantidade = 0; }
            if (produto.Valor == null) { produto.Valor = 0; }
            if (produto.ValorDeCusto == null) { produto.ValorDeCusto = 0; }
            if (produto.MargemLucro == null) {produto.MargemLucro = 0;}
            if (produto.PrecoAutomatico) 
            {
                if (produto.MargemLucro == 0)
                {  
                    if(produto.ValorDeCusto > 0)
                    { produto.MargemLucro = (double?)((produto.Valor - produto.ValorDeCusto) / produto.ValorDeCusto); }
                    
                }
                else 
                {
                    decimal? margemDeLucro = (decimal?)produto.MargemLucro;
                    produto.Valor = produto.ValorDeCusto * (margemDeLucro + 1);
                }
            }

            //Se o codigo do produto for igual a 0 ou nulo ele adicionara um codigo maior que o maior codigo cadastrado.
            if (produto.Codigo == null || produto.Codigo == 0)
            {
                var maiorCodigo = 0;
                foreach (var item in conferirProdutos)
                {
                    if (item.Codigo > maiorCodigo) { maiorCodigo = item.Codigo;}
                }
                produto.Codigo = maiorCodigo +=1;
            }

            if (produto.IsVolume)
            {
                var produtoParent = await GetProdutosByIdAsync((Guid)produto.CodigoDoProdutoVolume);
                if (produto.Quantidade > 0)
                {
                    produtoParent.Quantidade = produtoParent.Quantidade % produto.QuantidadeVolume;
                    produtoParent.Quantidade = produtoParent.Quantidade + (produto.Quantidade * produto.QuantidadeVolume);
                    await UpdateProdutoAsync(produtoParent);
                }
                else
                {
                    var produtoVolume = await GetProdutosByIdAsync((Guid)produto.CodigoDoProdutoVolume);
                    if (produtoVolume.Quantidade < produto.QuantidadeVolume) produto.Quantidade = 0;
                    else produto.Quantidade = produtoVolume.Quantidade / produto.QuantidadeVolume;
                }
            }
            _context.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ClonarProdutos(string doador, string receptor)
        {
            try
            {
                var Produtos = await _context.Produtos.Where(x => x.ApplicationUserId == doador).ToListAsync();
                var Categorias = await _context.Categorias.Where(x => x.ApplicationUserId == doador).ToListAsync();
                List<Produto> novosProdutos = new List<Produto>();
                List<Categoria> novasCategorias = new List<Categoria>();
                foreach (var item in Categorias)
                {
                    Categoria nova = new();
                    
                    nova.CategoriaId = Guid.NewGuid();
                    nova.ApplicationUserId = receptor;
                    nova.Descricao = item.Descricao;
                    nova.Nome = item.Nome;
                    novasCategorias.Add(nova);

                    var produtosCategoria = Produtos.Where(x => x.Categoria == item).ToList();
                    foreach (var produto in produtosCategoria)
                    {
                            Produto novo = new();
                            novo.Id = Guid.NewGuid();
                            novo.ApplicationUserId = receptor;
                            novo.Categoria = nova;
                            novo.ID_categoria = nova.CategoriaId;
                            novo.Codigo = produto.Codigo;
                            novo.Nome = produto.Nome;
                            novo.Valor = produto.Valor;
                            novo.ValorDeCusto = produto.ValorDeCusto;
                            novo.IsActive = produto.IsActive;
                            novo.IsControled = produto.IsControled;
                            novo.IsVolume = produto.IsVolume;
                            novo.MargemLucro = produto.MargemLucro;
                            novo.PrecoAutomatico = produto.PrecoAutomatico;
                            novo.Quantidade = produto.Quantidade;
                            novo.CodigoDoProdutoVolume = produto.CodigoDoProdutoVolume;
                            novo.QuantidadeVolume = produto.QuantidadeVolume;
                            novo.NomeDaCategoria = produto.NomeDaCategoria;
                            novosProdutos.Add(novo);
                    }
                }
                List<Produto> produtosVolume = novosProdutos.Where(x => x.IsVolume == true).ToList();
                foreach (var item in produtosVolume)
                {
                    var prod = Produtos.FirstOrDefault(x => x.Id == item.CodigoDoProdutoVolume);
                    prod = novosProdutos.FirstOrDefault(x => x.Nome == prod.Nome);
                    item.CodigoDoProdutoVolume = prod.Id;
                }

                _context.AddRange(novasCategorias);
                _context.AddRange(novosProdutos);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            { 

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task DeleteProdutoAsync(Guid id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                List<Produto> produtosDependentes = await _context.Produtos.Where(x => x.CodigoDoProdutoVolume == produto.Id).ToListAsync();
                if (produtosDependentes.Count > 0)
                {
                    foreach (var item in produtosDependentes)
                    {
                        _context.Produtos.Remove(item);
                    }
                }
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<byte[]> GerarCodeDeBarras(Produto produto)
        {
            var Freport = new FastReport.Report();
            List<RelatorioProduto> produtosRelatorios = new List<RelatorioProduto>();
            RelatorioProduto novoProduto = new();
            novoProduto.Codigo = produto.Codigo;
            novoProduto.Nome = produto.Nome;
            novoProduto.Quantidade = produto.Quantidade;
            novoProduto.Valor = produto.Valor;
            novoProduto.ValorDeCusto = produto.ValorDeCusto;
            novoProduto.MargemLucro = produto.MargemLucro;
            novoProduto.NomeDaCategoria = produto.NomeDaCategoria;
            produtosRelatorios.Add(novoProduto);

            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\codigo_de_barras_individual.frx");
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(produtosRelatorios, "produtos", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }

        public async Task<byte[]> GerarRelatorio(List<Produto> produtos, int tipo = 0)
        {
            List<RelatorioProduto> produtosRelatorios = new List<RelatorioProduto>();
            foreach (var item in produtos)
            {
                RelatorioProduto novoProduto = new();
                novoProduto.Codigo = item.Codigo;
                novoProduto.Nome = item.Nome;
                novoProduto.Quantidade = item.Quantidade;
                novoProduto.Valor = item.Valor;
                novoProduto.ValorDeCusto = item.ValorDeCusto;
                novoProduto.MargemLucro = item.MargemLucro;
                novoProduto.NomeDaCategoria = item.NomeDaCategoria;
                produtosRelatorios.Add(novoProduto);
            }

            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\produtoRelatorio.frx");

            if (tipo == 1)
            {
                reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\codigo_de_barras.frx");
            }

            var Freport = new FastReport.Report();
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(produtosRelatorios, "produtos", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }
            
        public async Task<List<LogPrecoProduto>> GetAllLogs()
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            List<LogPrecoProduto> logs = await _context.logPrecoProdutos.Where(x  => x.IdUsuario == userId).ToListAsync();
            return logs;
        }

        public async Task<List<LogPrecoProduto>> GetAllLogsPorPeriodo(DateTime? inicial, DateTime? final)
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            return await _context.logPrecoProdutos.Where(x => x.Data.Date >= inicial && x.Data.Date <= final && x.IdUsuario == userId).ToListAsync();
        }

        public async Task<List<Produto>> GetAllProdutosAsync()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var produtos = await _context.Produtos.Where(p => p.ApplicationUserId == userId).ToListAsync();
            return produtos;
        }

        public async Task<Produto> GetProdutosByCodigoAsync(int codigo)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;


            // Filtra diretamente no banco de dados
            var produto = await _context.Produtos
                                        .FirstOrDefaultAsync(p => p.ApplicationUserId == userId && p.Codigo == codigo);

            // Retorna null se nenhum produto for encontrado com o código especificado
            return produto;
        }

        public async Task<Produto> GetProdutosByIdAsync(Guid id)
        {
             return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
            
        }

        public async Task GravaLogProduto(LogPrecoProduto log)
        {

            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;


            var produto = await GetProdutosByIdAsync(log.IdProduto);
            if (log.Tag == "compra")
            {
                log.Lucro = (produto.ValorDeCusto - log.Custo) * log.Quantidade;
                produto.ValorDeCusto = ((produto.ValorDeCusto * produto.Quantidade) - log.Lucro) / produto.Quantidade;
            }
            else log.Lucro = 0;

            log.IdUsuario = userId;
            log.Data = DateTime.Now;

            _context.Add(log);
            await _context.SaveChangesAsync();

            await UpdateProdutoAsync(produto,produto.IsVolume,true);


        }

        public async Task UpdateProdutoAsync(Produto produto, bool volumeQuantidade = false, bool compra = false)
        {
            List<Produto> produtosDependentes = await _context.Produtos.Where(x => x.CodigoDoProdutoVolume == produto.Id).ToListAsync();
            var conferirProdutos = await GetAllProdutosAsync();
            var categoria = await _categories.GetCategoriasByIdAsync((Guid)produto.ID_categoria);
            produto.NomeDaCategoria = categoria.Nome;
            if (produto.Codigo == null || produto.Codigo == 0)
            {
                var maiorCodigo = 0;
                foreach (var item in conferirProdutos)
                {
                    if (item.Codigo > maiorCodigo) { maiorCodigo = item.Codigo; }
                }
                produto.Codigo = maiorCodigo += 1;
            }


            if (produto.ValorDeCusto == null) { produto.ValorDeCusto = 0; }
            if (produto.MargemLucro == null) { produto.MargemLucro = 0; }
            if (produto.PrecoAutomatico)
            {
                if (!compra)
                {

                    if (produto.MargemLucro == 0)
                    {
                        if (produto.ValorDeCusto > 0)
                        { produto.MargemLucro = (double?)((produto.Valor - produto.ValorDeCusto) / produto.ValorDeCusto); }

                    }
                    else
                    {
                        decimal? margemDeLucro = (decimal?)produto.MargemLucro;
                        produto.Valor = produto.ValorDeCusto * (margemDeLucro + 1);
                    }
                }
                else
                {
                        if (produto.ValorDeCusto > 0)
                        { produto.MargemLucro = (double?)((produto.Valor - produto.ValorDeCusto) / produto.ValorDeCusto); }
                }
            }
            if (produto.IsVolume)
            {
                var produtoParent = await GetProdutosByIdAsync((Guid)produto.CodigoDoProdutoVolume);
                if (volumeQuantidade)
                {
                    produtoParent.Quantidade = produtoParent.Quantidade % produto.QuantidadeVolume;
                    produtoParent.Quantidade = produtoParent.Quantidade + (produto.Quantidade * produto.QuantidadeVolume);
                    await UpdateProdutoAsync(produtoParent);
                }
                else
                {
                    var produtoVolume = await GetProdutosByIdAsync((Guid)produto.CodigoDoProdutoVolume);
                    if (produtoVolume.Quantidade < produto.QuantidadeVolume) produto.Quantidade = 0;
                    else produto.Quantidade = produtoVolume.Quantidade / produto.QuantidadeVolume;
                }
                
            }
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            if (produtosDependentes.Count > 0)
            {
                foreach (var item in produtosDependentes)
                {
                    await UpdateProdutoAsync(item);
                }
            }
        }
    }
}
