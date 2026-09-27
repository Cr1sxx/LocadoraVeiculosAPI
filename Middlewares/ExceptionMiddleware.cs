using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace LocadoraApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException ex)
            {
                // Erros de integridade do banco (FK, chave duplicada, etc.)
                _logger.LogError(ex, "Erro de atualização no banco de dados.");
                await EscreverErro(context, HttpStatusCode.BadRequest,
                    "Não foi possível concluir a operação no banco de dados. Verifique se os dados enviados respeitam as chaves e restrições existentes.");
            }
            catch (Exception ex)
            {
                // Qualquer outro erro não tratado
                _logger.LogError(ex, "Erro não tratado na aplicação.");
                await EscreverErro(context, HttpStatusCode.InternalServerError,
                    "Ocorreu um erro interno inesperado. Tente novamente mais tarde.");
            }
        }

        private static Task EscreverErro(HttpContext context, HttpStatusCode statusCode, string mensagem)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var resposta = new
            {
                status = (int)statusCode,
                erro = mensagem,
                data = DateTime.UtcNow
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
        }
    }
}