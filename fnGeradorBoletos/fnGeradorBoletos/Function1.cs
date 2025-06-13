using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BarcodeStandard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fnGeradorBoletos
{
    public class GeradorCodigoBarras
    {
        private readonly ILogger<GeradorCodigoBarras> _logger;
        private readonly string _serviceBusConnectionString;
        private readonly string _queueName = "gerador-codigo-barras";
        public GeradorCodigoBarras(ILogger<GeradorCodigoBarras> logger)
        {
            _logger = logger;
            _serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
        }

        [Function("barcode-generate")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string valor = data?.valor;
                string dataVencimento = data?.dataVencimento;

                string barcodeData;

                //Validação dos dados
                if (string.IsNullOrEmpty(valor) || string.IsNullOrEmpty(dataVencimento))
                {
                    return new BadRequestObjectResult("Os campos 'valor' e 'dataVencimento' são obrigatórios.");
                }

                //Validar formato da Data de Vencimento YYYY-MM-DD
                if (!DateTime.TryParseExact(dataVencimento, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dateObj))
                {
                    return new BadRequestObjectResult("O campo 'dataVencimento' deve estar no formato YYYY-MM-DD.");
                }
                string dateStr = dateObj.ToString("yyyyMMdd"); 

              //Conversão do Valor para centavos e formatação até 8 dígitos
                if (!decimal.TryParse(valor, out decimal valorDecimal))
                {
                    return new BadRequestObjectResult("O campo 'valor' deve ser um número válido.");
                }
                int valorCentavos = (int)(valorDecimal * 10);
                string valorStr = valorCentavos.ToString("D8"); // 8 dígitos, preenchido com zeros à esquerda

                string bankCode = "008";
                string baseCode = string.Concat(bankCode, dateStr, valorStr);
                //Preenchimento do barCode para ter 44 caracteres
                // O código de barras deve ter 44 caracteres, preenchendo com zeros à direita
                barcodeData = baseCode.Length < 44 ? baseCode.PadRight(44, '0') : baseCode.Substring(0, 44);
                _logger.LogInformation($"Barcode gerado: {barcodeData}");



                Barcode barcode = new Barcode();
                var skImage = barcode.Encode(BarcodeStandard.Type.Code128, barcodeData);
                if (skImage == null)
                {
                    _logger.LogError($"ERRO: skImage é null! barcodeData inválido: {barcodeData}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                _logger.LogInformation("skImage gerado com sucesso.");

                using (var encodeData = skImage.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                {
                    if (encodeData == null)
                    {
                        _logger.LogError(" ERRO: encodeData retornou null após skImage.Encode()");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                    _logger.LogInformation(" encodeData gerado com sucesso.");

                    var imageBytes = encodeData.ToArray();


                    string base64String = Convert.ToBase64String(imageBytes);

                    var resultObject = new
                    {
                        barcode = barcodeData,
                        valorOriginal = valorDecimal,
                        DataVencimento = DateTime.Now.AddDays(5),
                        ImagemBase64 = base64String
                    };
                    if (string.IsNullOrWhiteSpace(_serviceBusConnectionString))
                    {
                        _logger.LogError(" ServiceBusConnectionString está vazia ou inválida.");
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                    _logger.LogInformation("Enviando mensagem para o Service Bus...");


                    await SendFileFallback(resultObject, _serviceBusConnectionString, _queueName);
                    return new OkObjectResult(resultObject);
                }

            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
           
        }
        
        private async Task SendFileFallback(object resultObject, string serviceBusConnectionString, string queueName)
        {
            await using var client = new ServiceBusClient(serviceBusConnectionString);

            ServiceBusSender sender = client.CreateSender(queueName);

            string messageBody = JsonConvert.SerializeObject(resultObject);
            _logger.LogInformation("Serializando objeto...");
            _logger.LogInformation($"Conteúdo: {JsonConvert.SerializeObject(resultObject)}");

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
            _logger.LogInformation("Mensagem enviada com sucesso.");


            _logger.LogInformation($"Message sent to queue {queueName} with content: {messageBody}");
        }
    }
}
