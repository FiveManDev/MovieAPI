using Newtonsoft.Json.Linq;

namespace MovieAPI.Services.MomoPayment
{
    public class MomoConnection
    {
        private static string partnerCode;
        private static string accessKey;
        private static string serectkey;
        private static string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
        private static string returnUrl = "https://localhost:7251/process/payment/confirm";
        private static string notifyurl = "https://localhost:7251/process/payment/save";
        public static void MomoConfig(String PartnerCode, String AccessKey, String Serectkey)
        {
            partnerCode = PartnerCode;
            accessKey = AccessKey;
            serectkey = Serectkey;
        }
        public static String MomoResponse(String OrderInfo, String Amount, String ExtraData)
        {
            Console.WriteLine("OrderInfo: " + OrderInfo);
            Console.WriteLine("Amount: " + Amount);
            Console.WriteLine("ExtraData: " + ExtraData);

            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();


            //Before sign HMAC SHA256 signature
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
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", Amount },
                { "orderId", orderid },
                { "orderInfo", OrderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", ExtraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.SendPaymentRequest(endpoint, message.ToString());
            Console.WriteLine(responseFromMomo);
            JObject jmessage = JObject.Parse(responseFromMomo);

            if (jmessage.GetValue("payUrl") != null)
            {
                return jmessage.GetValue("payUrl").ToString();
            }
            return "/";

        }
    }
}
