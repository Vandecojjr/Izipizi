using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IUsuarioServices
    {
        Task<List<Usuario>> GetUsuarios();
        Task<List<Usuario>> GetUsuariosDependentes();
        Task<List<Usuario>> GetUsuariosDaConta();
        Task<Usuario> GetUsuario(string Id = null);

        Task<List<PerfilUsuario>> GetPerfils();
        Task AddPerfil(string nome);
        Task DeletePerfilDoUsuario(string PerfilNome, string IdUsuario);


        Task<bool> GetUsuarioAtivo();
        Task DeletarUsuario(string id);
        Task AtualizarPerfilUsuario(string idUsuario, string nomePerfil);
        Task AtualizarPorcentagemUsuario(string idUsuario, decimal porcent);

        Task AddUsuário(Usuario usuario);
        Task<bool> AddUsuárioDependente(Usuario usuario);
        Task AtualizarUsuario(Usuario usuario, bool novaSenha = false);

        Task AtivarLogado(string id = null ,bool logado = true);
        Task ConfirmaPagamento();
    }
}
