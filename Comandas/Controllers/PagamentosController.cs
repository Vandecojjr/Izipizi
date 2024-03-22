using Microsoft.AspNetCore.Mvc;
using System;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Newtonsoft.Json;
using MercadoPago.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Comandas.Services;
using MercadoPago.Resource.User;
using Comandas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MercadoPago.Resource.PreApproval;
using MercadoPago.Client.Preapproval;
using Azure.Core;
using System.Text;
using System.Dynamic;
using Azure;
using Comandas.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Comandas.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsuarioServices _usuarioServices;


        public PagamentosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager  , IUsuarioServices usuarioServices)
        {
            _context = context;
            _userManager = userManager;
            _usuarioServices = usuarioServices;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string accessToken = "APP_USR-5464872456427218-030716-8100569b27e5d6529a1b410e1a3fc3cd-145263082";
            MercadoPagoConfig.AccessToken = accessToken;
           

            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string url = "https://api.mercadopago.com/preapproval";

            string email = data.payer.email;
            var usuario = await _userManager.FindByEmailAsync(email);

            if (data.transaction_amount == 60 && usuario != null)
            {
                dynamic resposta = new ExpandoObject();
                string jsonPayload = @"{
                                ""back_url"": ""https://vendas.izipizi.site/"",
                                ""card_token_id"": """ + data.token + @""",
                                ""payer_email"": """ + data.payer.email + @""",
                                ""preapproval_plan_id"": ""2c9380848e1d0a43018e2175b3cf0392"",
                                ""status"": ""authorized""
                                }";

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    // Enviar a requisição POST
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        resposta = JsonConvert.DeserializeObject(responseContent);
                        Console.WriteLine("Resposta do Servidor: " + responseContent);

                        if (resposta.status == "authorized")
                        {
                            

                            usuario.IsAtivo = true;
                            usuario.estadoDePagamento = "aprovado";
                            usuario.DiaDeVencimento = 10;
                            usuario.TotalDeUsuarios = 2;
                            usuario.nivelAdmin = 1;
                            usuario.DataDeAdesao = DateTime.Now;
                            _context.Entry(usuario).State = EntityState.Modified;
                            PagMercadoPad cartao = new();
                            cartao.EmailUser = data.payer.email;
                            cartao.IdCompra = resposta.id;
                            _context.Add(cartao);
                            await _context.SaveChangesAsync();
                            await _usuarioServices.AtualizarPerfilUsuario(usuario.Id, "Admin");
                            return Ok(resposta);
                        }
                    }
                    else
                    {
                        Console.WriteLine((response.StatusCode, "Falha ao processar a solicitação"));
                        string responseContent = await response.Content.ReadAsStringAsync();
                        resposta = JsonConvert.DeserializeObject(responseContent);
                        return BadRequest(responseContent);
                    }
                }
            }
            else if (data.transaction_amount == 600 && usuario != null)
            {
                var paymentRequest = new PaymentCreateRequest
                {
                    TransactionAmount = data.transaction_amount,
                    Token = data.token,
                    Installments = data.installments,
                    Description = "Izipizi vendas",
                    IssuerId = data.issuer,
                    PaymentMethodId = data.payment_method_id,
                    Payer = new PaymentPayerRequest
                    {
                        Email = data.payer.email,
                        Identification = new IdentificationRequest
                        {
                            Type = data.identificationType,
                            Number = data.identificationNumber,
                        },
                    },
                    NotificationUrl = "https://vendas.izipizi.site/api/hook",
                };

                if (data.payment_method_id == "pix")
                {
                    paymentRequest.PaymentMethodId = "pix";
                }

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest);
                dynamic teste = JsonConvert.DeserializeObject(payment.ApiResponse.Content);

                    PagMercadoPad cartao = new();
                    cartao.EmailUser = data.payer.email;
                    cartao.IdCompra = teste.id;
                    _context.Add(cartao);
                    await _context.SaveChangesAsync();

                if (teste.status == "approved")
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

                Console.WriteLine(payment.ApiResponse.Content);
                return Ok(payment.ApiResponse.Content);
            }
            else
            {
                return BadRequest("teste");
            }
            return Ok();
        }



    }
}
