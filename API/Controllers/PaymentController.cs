using Microsoft.AspNetCore.Mvc;
   using MockUPIPaymentGateway.Services;

   namespace MockUPIPaymentGateway.Controllers
   {
       [Route("api/[controller]")]
       [ApiController]
       public class PaymentController : ControllerBase
       {
           private readonly MockUPIPaymentService _paymentService;

           public PaymentController(MockUPIPaymentService paymentService)
           {
               _paymentService = paymentService;
           }

           [HttpPost("create")]
           public IActionResult CreatePayment()
           {
               try
               {
                   var (qrCodeImageBase64, paymentId) = _paymentService.CreatePayment();
                   return Ok(new { qrCodeImage = qrCodeImageBase64, paymentId });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { error = ex.Message });
               }
           }

           [HttpGet("details/{paymentId}")]
           public IActionResult GetPaymentDetails(string paymentId)
           {
               try
               {
                   var payment = _paymentService.GetPaymentDetails(paymentId);
                   return Ok(new { amount = payment.Amount, description = payment.Description, status = payment.Status });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { error = ex.Message });
               }
           }

           [HttpGet("status/{paymentId}")]
           public IActionResult GetPaymentStatus(string paymentId)
           {
               try
               {
                   var status = _paymentService.CheckPaymentStatus(paymentId);
                   return Ok(new { status });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { error = ex.Message });
               }
           }

           [HttpPost("update-status")]
           public IActionResult UpdatePaymentStatus([FromBody] UpdateStatusRequest request)
           {
               try
               {
                   _paymentService.UpdatePaymentStatus(request.PaymentId, request.Status);
                   return Ok(new { success = true });
               }
               catch (Exception ex)
               {
                   return StatusCode(500, new { error = ex.Message });
               }
           }
       }

       public class UpdateStatusRequest
       {
           public string PaymentId { get; set; }
           public string Status { get; set; }
       }
   }