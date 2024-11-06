using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;
using QueueManagementSystem.MVC.Services;

namespace QueueManagementSystem.MVC.Controllers
{
    public class FingerprintCallbackController : Controller
    {
        private readonly IFingerprintService _fingerprintService;
        private readonly ILogger<FingerprintCallbackController> _logger;
        private readonly IHubContext<FingerprintHub> _hubContext;

        public FingerprintCallbackController(
            IFingerprintService fingerprintService,
            ILogger<FingerprintCallbackController> logger,
            IHubContext<FingerprintHub> hubContext)
        {
            _fingerprintService = fingerprintService;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpPost("callback/{sessionId}")]
        public async Task<IActionResult> ReceiveFingerprint(string sessionId, [FromBody] FingerprintCallbackData data)
        {
            try
            {
                var fingerprintBytes = Convert.FromBase64String(data.FingerprintData);
                var customer = new CustomerInfo { FingerPrintData = fingerprintBytes };

                var response = await _fingerprintService.MatchFingerPrintAsync(customer);

                // Notify the client through SignalR
                await _hubContext.Clients.Group(sessionId).SendAsync("FingerprintProcessed", new
                {
                    IsAuthenticated = response.IsAuthenticated,
                    Message = response.Message
                });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing fingerprint callback");
                return StatusCode(500, "Failed to process fingerprint");
            }
        }

        [HttpPost("error/{sessionId}")]
        public async Task<IActionResult> ReceiveError(string sessionId, [FromBody] FingerprintErrorData error)
        {
            await _hubContext.Clients.Group(sessionId).SendAsync("FingerprintError", error);
            return Ok();
        }
    }
}
