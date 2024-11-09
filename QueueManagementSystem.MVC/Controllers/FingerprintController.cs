using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QueueManagementSystem.MVC.Data;
using QueueManagementSystem.MVC.Models;
using QueueManagementSystem.MVC.Services;

namespace QueueManagementSystem.MVC.Controllers
{
    [ApiController]
    [Route("api/fingerprint")]
    public class FingerprintController : Controller
    {
        private readonly IFingerprintService _fingerprintService;
        private readonly ILogger<FingerprintController> _logger;
        private readonly FingerprintState _fingerprintState;

        public FingerprintController(
            IFingerprintService fingerprintService,
            ILogger<FingerprintController> logger,
            FingerprintState fingerprintState)
        {
            _fingerprintService = fingerprintService;
            _logger = logger;
            _fingerprintState = fingerprintState;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResult>> AuthenticateFingerprint([FromBody] Fingerprint data)
        {
            try
            {
                if (string.IsNullOrEmpty(data?.FingerprintData))
                {
                    return BadRequest("Invalid fingerprint data");
                }

                var fingerprintBytes = Convert.FromBase64String(data.FingerprintData);

                var customer = new CustomerInfo { FingerPrintData = fingerprintBytes };

                var result = await _fingerprintService.MatchFingerPrintAsync(customer);

                // Notify subscribers about the authentication result
                _fingerprintState.NotifyAuthenticationCompleted(result);

                return Ok(new AuthenticationResult
                {
                    IsAuthenticated = result.IsAuthenticated,
                    Message = result.Message
                });
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid base64 string received");
                return BadRequest("Invalid fingerprint data format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating fingerprint");
                return StatusCode(500, new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Message = "An error occurred while processing the fingerprint"
                });
            }
        }

        public class Fingerprint
        {
            public string FingerprintData { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
