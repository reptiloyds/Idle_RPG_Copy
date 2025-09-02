using System;

namespace NutakuUnitySdk
{
    public static class NutakuConfigHandler
    {
        public static event Action<bool> OnLoginAction;
        public static event Action<bool> OnPaymentAction;

        public static void OnLogin(bool success)
        {
            OnLoginAction?.Invoke(success);
        }

        
        // необходимо если по старой логике подтверждение покупки выполняется в браузере на android
        // когда в новом сдк по стандарту - покупка выполняется сразу при нажатии в игре и успешном ответе от сервера
        public static void Callback_PaymentResultFromBrowser(string paymentId, string statusFromBrowser)
        {
            // if (_instance == null)
            // {
            //     return; // for this sample code, we're not going to do instance salvaging. In a real game you want to try to ensure that this callback is always able to process, because the user could have spent his gold
            // }
            //
            // if (paymentId != _instance._trackedPayment.paymentId)
            // {
            //     _instance.LogMessage("Received payment event with status " + statusFromBrowser + " for paymentId " +
            //                          paymentId +
            //                          " which is not the latest payment id tracked by the sample app. For the purpose of the sample app, we are just logging this as information text, but in a real game you should implement full and appropriate processing, no matter the payment id.");
            // }
            // else
            // {
            //     if (statusFromBrowser == "purchase")
            //     {
            //         _instance.LogMessage("Received Payment Successful completion event from browser for payment ID " +
            //                              paymentId);
            //         _instance.LogMessage(
            //             "Remember, this is just an event from the browser, it could have been tampered with, so you should check with the Nutaku API server2server for validity before awarding anything to the user!");
            //     }
            //     else if (statusFromBrowser == "errorFromGPHS")
            //     {
            //         _instance.LogMessage("Received errorFromGPHS event from browser for payment ID " + paymentId);
            //         _instance.LogMessage(
            //             "This is similar to generic error event, but it was caused by your Game Payment Handler Server not responding positively to the S2S PUT Request.");
            //     }
            //     else if (statusFromBrowser == "cancel")
            //     {
            //         _instance.LogMessage("Received Payment cancellation event from browser for payment ID " +
            //                              paymentId);
            //         _instance.LogMessage("This can be trusted, no need for server2server validation with Nutaku API.");
            //     }
            //     else
            //     {
            //         _instance.LogMessage("Received a generic error event from browser for payment ID " + paymentId);
            //         _instance.LogMessage("Error details (no need to show to the user): " + statusFromBrowser);
            //         _instance.LogMessage(
            //             "This can be trusted, no need for server2server validation with Nutaku API. You can inform the user about there having been a problem, and that's it.");
            //     }
            //
            //     _instance.OpenTransactionUrlButton.interactable = false;
            //     _instance.PutPaymentButton.interactable = false;
            // }
        }
    }
}