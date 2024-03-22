using Azure;
using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Services;
using MercadoPago.Config;
using MercadoPago.Http;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Comandas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HookController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsuarioServices _usuarioServices;

        public HookController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUsuarioServices usuarioServices)
        {
            _context = context;
            _userManager = userManager;
            _usuarioServices = usuarioServices;
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string accessToken = "APP_USR-5464872456427218-030716-8100569b27e5d6529a1b410e1a3fc3cd-145263082";
            string paymentId = data.data.id;

            if (data.action == "payment.updated")
            {

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        HttpResponseMessage response = await client.GetAsync("https://api.mercadopago.com/v1/payments/" + paymentId);

                        if (response.IsSuccessStatusCode)
                        {
                            var pag = await _context.PagamentosCartao.FirstOrDefaultAsync(x => x.IdCompra == paymentId);
                            string email = pag.EmailUser;
                            var usuario = await _userManager.FindByEmailAsync(email);

                            string responseData = await response.Content.ReadAsStringAsync();
                            dynamic paymentData = JsonConvert.DeserializeObject(responseData);

                            if(paymentData.status == "approved")
                            {
                                usuario.IsAtivo = true;
                                usuario.estadoDePagamento = "aprovado";
                                usuario.DiaDeVencimento = 10;
                                usuario.TotalDeUsuarios = 2;
                                usuario.nivelAdmin = 1;
                                usuario.DataDeAdesao = DateTime.Now;
                                _context.Entry(usuario).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                                await _usuarioServices.AtualizarPerfilUsuario(usuario.Id, "Admin");

                            }
                            else
                            {
                                usuario.estadoDePagamento = "pendente";
                                usuario.DataDeBloqueio = DateTime.Now.AddDays(5);
                                _context.Entry(usuario).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                            }

                            return Ok(responseData);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
                }
            }
            else if (data.action == "updated")
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        HttpResponseMessage response = await client.GetAsync("https://api.mercadopago.com/preapproval/" + paymentId);

                        if (response.IsSuccessStatusCode)
                        {
                            var pag = await _context.PagamentosCartao.FirstOrDefaultAsync(x => x.IdCompra == paymentId);
                            string email = pag.EmailUser;
                            var usuario = await _userManager.FindByEmailAsync(email);

                            string responseData = await response.Content.ReadAsStringAsync();
                            dynamic paymentData = JsonConvert.DeserializeObject(responseData);

                            if (paymentData.status == "authorized")
                            {
                                usuario.IsAtivo = true;
                                usuario.estadoDePagamento = "aprovado";
                                usuario.DiaDeVencimento = 10;
                                usuario.TotalDeUsuarios = 2;
                                usuario.nivelAdmin = 1;
                                usuario.DataDeAdesao = DateTime.Now.Date;
                                _context.Entry(usuario).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                                await _usuarioServices.AtualizarPerfilUsuario(usuario.Id, "Admin");

                            }
                            else
                            {
                                usuario.estadoDePagamento = "pendente";
                                usuario.DataDeBloqueio = DateTime.Now.Date.AddDays(5);
                                _context.Entry(usuario).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                            }

                            return Ok(responseData);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
                }

            }
            return BadRequest();
        }


    }
}
