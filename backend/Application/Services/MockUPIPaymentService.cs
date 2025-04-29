using QRCoder;
using System;
using System.Collections.Generic;

namespace MockUPIPaymentGateway.Services
{
    public class MockUPIPaymentService
    {
        private readonly Dictionary<string, Payment> _payments = new Dictionary<string, Payment>();
        private const string GOOGLE_PAY_UPI_ID = "example@okhdfcbank"; // Mock UPI ID
        private const decimal FIXED_AMOUNT = 100.00m; // Fixed amount in INR
        private const string MERCHANT_NAME = "ExampleMerchant";

        public (string QrCodeImageBase64, string PaymentId) CreatePayment()
        {
            var paymentId = Guid.NewGuid().ToString();

            // Construct UPI payment URL
            var upiUrl = $"upi://pay?pa={GOOGLE_PAY_UPI_ID}&pn={MERCHANT_NAME}&am={FIXED_AMOUNT}&cu=INR&tn=MockPayment&tr={paymentId}";
            var paymentAppUrl = $"https://sanskrit-vedangna.onrender.com/mock-payment-app?paymentId={paymentId}&amount={FIXED_AMOUNT}";

            _payments[paymentId] = new Payment
            {
                PaymentId = paymentId,
                Amount = FIXED_AMOUNT,
                Description = "Mock Payment via Google Pay",
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpiUrl = upiUrl
            };

            // Generate QR code with PngByteQRCode
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(upiUrl, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20); // 20 pixels per module
            var base64String = Convert.ToBase64String(qrCodeBytes);

            return ($"data:image/png;base64,{base64String}", paymentId);
        }

        public Payment GetPaymentDetails(string paymentId)
        {
            if (_payments.TryGetValue(paymentId, out var payment))
            {
                return payment;
            }
            throw new Exception("Payment not found");
        }

        public string CheckPaymentStatus(string paymentId)
        {
            if (_payments.TryGetValue(paymentId, out var payment))
            {
                return payment.Status;
            }
            throw new Exception("Payment not found");
        }

        public void UpdatePaymentStatus(string paymentId, string status)
        {
            if (_payments.TryGetValue(paymentId, out var payment))
            {
                payment.Status = status;
            }
            else
            {
                throw new Exception("Payment not found");
            }
        }
    }

    public class Payment
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpiUrl { get; set; }
    }
}