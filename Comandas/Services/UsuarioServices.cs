using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace Comandas.Services
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CurrentUserService _user;
        public UsuarioServices(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, CurrentUserService user)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _user = user;
        }

        public async Task AddPerfil(string nome)
        {
            if (await _roleManager.FindByNameAsync(nome) == null)
            {
                // Role não existe, então criamos
                var role = new IdentityRole(nome);

                // Adicionamos a role à tabela
                var result = await _roleManager.CreateAsync(role);

                // Verificamos se a operação foi bem-sucedida
                if (!result.Succeeded)
                {
                    throw new Exception("Erro ao criar a role.");
                }
            }
        }

        public async Task AddUsuário(Usuario usuario)
        {
            var newUser = new ApplicationUser
            {
                UserName = usuario.Email,
                Email = usuario.Email,
                NomeEmpresa = usuario.Empresa,
                nivelAdmin = usuario.NivelAdmin,
                EmailConfirmed = true,
                IsAtivo = usuario.Ativo,
                DataDeAdesao = DateTime.Now,
                DiaDeVencimento = usuario.DiaDeVencimento,
                TotalDeUsuarios = usuario.UsuariosAltorizados,
                
            };
            var result = await _userManager.CreateAsync(newUser, usuario.Senha);
            if(usuario.NivelAdmin == 1)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == usuario.Email);
                await AtualizarPerfilUsuario(user.Id, "Admin");
            }

        }

        public async Task<bool> AddUsuárioDependente(Usuario usuario)
        {
            List<Usuario> usuarios = new List<Usuario>();
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var users = await _context.Users.Where(x => x.IdDoProprietario == userId).ToListAsync();
            usuarios = await GetUsuariosDependentes();
            string proprietarioId = userId;
            if (userCurrent.nivelAdmin == 2) proprietarioId = userCurrent.IdDoProprietario;


                if (usuarios.Count != userCurrent.TotalDeUsuarios)
                {
                var newUser = new ApplicationUser
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    NomeEmpresa = userCurrent.NomeEmpresa,
                    nivelAdmin = 2,
                    EmailConfirmed = true,
                    IsAtivo = true,
                    TotalDeUsuarios = userCurrent.TotalDeUsuarios,
                    IdDoProprietario = proprietarioId,
                    porcentagemDefinida = usuario.porcentagemUsuario,
                    DataDeAdesao = DateTime.Now,
                    DiaDeVencimento = 0,

                };
                    var result = await _userManager.CreateAsync(newUser, usuario.Senha);
                    return true;
                }
                else { return false; }
            
        }

        public async Task AtivarLogado(string id = null, bool logado = true)
        {
            if (!logado)
            {

                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                userCurrent.IsLogados = false;
                _context.Entry(userCurrent).State = EntityState.Modified;
                await _context.SaveChangesAsync();

               
            }
            else
            {
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                userCurrent.IsLogados = true;
                if (userCurrent.estadoDePagamento == "pendente" && userCurrent.DataDeBloqueio == DateTime.Now.Date)
                {
                    userCurrent.IsAtivo = false;
                }
                _context.Entry(userCurrent).State = EntityState.Modified;
                await _context.SaveChangesAsync();


            }
            
        }

        public async Task AtualizarPerfilUsuario(string IdUsuario, string nomePerfil)
        {
            var user = await _userManager.FindByIdAsync(IdUsuario);

            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, nomePerfil);

                if (result.Succeeded)
                {
                    // O perfil foi atribuído com sucesso ao usuário
                }
                else
                {
                    // Ocorreu um erro ao atribuir o perfil ao usuário
                }
            }
            else
            {
                // Usuário não encontrado
            }
        }

        public async Task AtualizarPorcentagemUsuario(string idUsuario, decimal porcent)
        {
            var user = await _userManager.FindByIdAsync(idUsuario);
            user.porcentagemDefinida = porcent;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarUsuario(Usuario usuario, bool novaSenha = false)
        {
            var user = await _userManager.FindByIdAsync(usuario.Id);
            user.Email = usuario.Email;
            user.NomeEmpresa = usuario.Empresa;
            user.DiaDeVencimento = usuario.DiaDeVencimento;
            user.IsAtivo = usuario.Ativo;
            user.nivelAdmin = usuario.NivelAdmin;
            if (novaSenha) 
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, usuario.Senha);
            }

            user.TotalDeUsuarios = usuario.UsuariosAltorizados;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmaPagamento()
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            userCurrent.IsAtivo = true;
            _context.Entry(userCurrent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletarUsuario(string id)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
             _context.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePerfilDoUsuario(string PerfilNome, string IdUsuario)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == IdUsuario);
            await _userManager.RemoveFromRoleAsync(user, PerfilNome);
        }

        public async Task<List<PerfilUsuario>> GetPerfils()
        {
            PerfilUsuario perfil = new PerfilUsuario();
            List<PerfilUsuario> perfils = new List<PerfilUsuario>();
            var roles = await _context.Roles.ToListAsync();
            foreach (var role in roles)
            {
                perfil.Id = role.Id;
                perfil.Nome = role.Name;
                perfils.Add(perfil);
                perfil = new PerfilUsuario();
            }
            return perfils;
        }

        public async Task<Usuario> GetUsuario(string Id = null)
        {
            if (Id == null)
            {
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                Usuario usuario = new Usuario();
                usuario.Id = userCurrent.Id;
                usuario.NivelAdmin = userCurrent.nivelAdmin;
                usuario.porcentagemUsuario = userCurrent.porcentagemDefinida;
                return usuario;
            }
            else
            {
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
                Usuario usuario = new Usuario();
                usuario.Id = userCurrent.Id;
                usuario.Email = userCurrent.Email;
                usuario.Empresa = userCurrent.NomeEmpresa;
                usuario.DiaDeVencimento = userCurrent.DiaDeVencimento;
                usuario.Ativo = userCurrent.IsAtivo;
                usuario.NivelAdmin = userCurrent.nivelAdmin;
                usuario.porcentagemUsuario = userCurrent.porcentagemDefinida;
                usuario.UsuariosAltorizados = userCurrent.TotalDeUsuarios;
                return usuario;
            }
        }

        public async Task<bool> GetUsuarioAtivo()
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.IsAtivo) return true;
            else return false;

        }

        public async Task<List<Usuario>> GetUsuarios()
        {
            Usuario usuario = new Usuario();
            List<Usuario> usuarios = new List<Usuario>();
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                usuario.PerfisDesteUser = new List<PerfilUsuario>();
                var rolesUsers = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
                foreach (var item in rolesUsers)
                {
                    PerfilUsuario perfilUsuario = new PerfilUsuario();
                    var perfil = await _context.Roles.FirstOrDefaultAsync(x => x.Id == item.RoleId);
                    perfilUsuario.Id = perfil.Id;
                    perfilUsuario.Nome = perfil.Name;
                    usuario.PerfisDesteUser.Add(perfilUsuario);
                }
                usuario.Id = user.Id;
                usuario.Email = user.Email;
                usuario.Empresa = user.NomeEmpresa;
                usuario.NivelAdmin = user.nivelAdmin;
                usuario.Ativo = user.IsAtivo;
                usuario.Logado = user.IsLogados;
                usuario.porcentagemUsuario = user.porcentagemDefinida;
                usuario.UsuariosAltorizados = user.TotalDeUsuarios;
                usuarios.Add(usuario);
                usuario = new Usuario();
            }
            return usuarios;
        }

        public async Task<List<Usuario>> GetUsuariosDependentes()
        {
            Usuario usuario = new Usuario();
            List<Usuario> usuarios = new List<Usuario>();
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var users = await _context.Users.Where(x => x.IdDoProprietario == userId).ToListAsync();

            if (userCurrent.nivelAdmin == 2)
            {
                users = await _context.Users.Where(x => x.IdDoProprietario == userCurrent.IdDoProprietario).ToListAsync();
            }
            foreach (var user in users)
            {
                usuario.PerfisDesteUser = new List<PerfilUsuario>();
                var rolesUsers = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
                foreach (var item in rolesUsers)
                {
                    PerfilUsuario perfilUsuario = new PerfilUsuario();
                    var perfil = await _context.Roles.FirstOrDefaultAsync(x => x.Id == item.RoleId);
                    perfilUsuario.Id = perfil.Id;
                    perfilUsuario.Nome = perfil.Name;
                    usuario.PerfisDesteUser.Add(perfilUsuario);
                }
                usuario.Id = user.Id;
                usuario.Email = user.Email;
                usuario.Empresa = user.NomeEmpresa;
                usuario.NivelAdmin = user.nivelAdmin;
                usuario.Ativo = user.IsAtivo;
                usuario.porcentagemUsuario = user.porcentagemDefinida;
                usuarios.Add(usuario);
                usuario = new Usuario();
            }
            return usuarios;
        }
    }
}
