using Newtonsoft.Json.Linq;

namespace MovieAPI.Services.MomoPayment
{
    public class MomoConnection
    {
        private static string partnerCode = AppSettings.PartnerCode;
        private static string accessKey = AppSettings.MomoAccessKey;
        private static string serectkey = AppSettings.MomoSerectkey;
        private static string endpoint = AppSettings.Endpoint;
        private static string returnUrl = AppSettings.ReturnUrl;
        private static string notifyurl = AppSettings.NotifyUrl;
        public static async Task<string> MomoResponse(string OrderInfo, string Amount, string ExtraData)
        {
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                Amount + "&orderId=" +
                orderid + "&orderInfo=" +
                OrderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                ExtraData;
            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);
            var message = new
            {
                accessKey = accessKey,
                partnerCode = partnerCode,
                requestType = "captureMoMoWallet",
                notifyUrl = notifyurl,
                returnUrl = returnUrl,
                orderId = orderid,
                amount = Amount,
                orderInfo = OrderInfo,
                requestId = requestId,
                extraData = ExtraData,
                signature = signature
            };
            var responseFromMomo = await PaymentRequest.SendPaymentRequest(endpoint, message);
            JObject jmessage = JObject.Parse(responseFromMomo);
            if (jmessage.GetValue("payUrl") != null)
            {
                return jmessage.GetValue("payUrl").ToString();
            }
            return "/";
        }
    }
}
