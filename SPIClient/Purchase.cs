﻿using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SPIClient
{
    public class PurchaseRequest
    {
        public string PosRefId { get; }

        public int PurchaseAmount { get; }
        public int TipAmount { get; set; }
        public int CashoutAmount { get; set; }
        public bool PromptForCashout { get; set; }
        public int SurchargeAmount { get; set; }

        [Obsolete("Id is deprecated. Use PosRefId instead.")]
        public string Id { get; }

        [Obsolete("AmountCents is deprecated. Use PurchaseAmount instead.")]
        public int AmountCents { get; }

        internal SpiConfig Config = new SpiConfig();

        internal TransactionOptions Options = new TransactionOptions();

        public PurchaseRequest(int amountCents, string posRefId)
        {
            PosRefId = posRefId;
            PurchaseAmount = amountCents;

            // Library Backwards Compatibility
            Id = posRefId;
            AmountCents = amountCents;
        }

        public string AmountSummary()
        {
            return $"Purchase: ${PurchaseAmount / 100.0:.00}; " +
                $"Tip: ${TipAmount / 100.0:.00}; " +
                $"Cashout: ${CashoutAmount / 100.0:.00};";
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("purchase_amount", PurchaseAmount),
                new JProperty("tip_amount", TipAmount),
                new JProperty("cash_amount", CashoutAmount),
                new JProperty("prompt_for_cashout", PromptForCashout),
                new JProperty("surcharge_amount", SurchargeAmount)

                );

            Config.EnabledPrintMerchantCopy = true;
            Config.EnabledPromptForCustomerCopyOnEftpos = true;
            Config.EnabledSignatureFlowOnEftpos = true;
            Config.AddReceiptConfig(data);
            Options.AddOptions(data);
            return new Message(RequestIdHelper.Id("prchs"), Events.PurchaseRequest, data, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PurchaseResponse
    {
        public bool Success { get; }
        public string RequestId { get; }
        public string PosRefId { get; }
        public string SchemeName { get; }

        /// <summary>
        /// Deprecated. Use SchemeName instead
        /// </summary>
        public string SchemeAppName { get; }

        private readonly Message _m;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public PurchaseResponse() { }

        public PurchaseResponse(Message m)
        {
            _m = m;
            RequestId = _m.Id;
            PosRefId = _m.GetDataStringValue("pos_ref_id");
            SchemeName = _m.GetDataStringValue("scheme_name");
            SchemeAppName = _m.GetDataStringValue("scheme_name");
            Success = m.GetSuccessState() == Message.SuccessState.Success;
        }

        public string GetRRN()
        {
            return _m.GetDataStringValue("rrn");
        }

        public int GetPurchaseAmount()
        {
            return _m.GetDataIntValue("purchase_amount");
        }

        public int GetTipAmount()
        {
            return _m.GetDataIntValue("tip_amount");
        }

        public int GetSurchargeAmount()
        {
            return _m.GetDataIntValue("surcharge_amount");
        }

        public int GetCashoutAmount()
        {
            return _m.GetDataIntValue("cash_amount");
        }

        public int GetBankNonCashAmount()
        {
            return _m.GetDataIntValue("bank_noncash_amount");
        }

        public int GetBankCashAmount()
        {
            return _m.GetDataIntValue("bank_cash_amount");
        }

        public string GetCustomerReceipt()
        {
            return _m.GetDataStringValue("customer_receipt");
        }

        public string GetMerchantReceipt()
        {
            return _m.GetDataStringValue("merchant_receipt");
        }

        public string GetResponseText()
        {
            return _m.GetDataStringValue("host_response_text");
        }

        public string GetResponseCode()
        {
            return _m.GetDataStringValue("host_response_code");
        }

        public string GetTerminalReferenceId()
        {
            return _m.GetDataStringValue("terminal_ref_id");
        }

        public string GetCardEntry()
        {
            return _m.GetDataStringValue("card_entry");
        }

        public string GetAccountType()
        {
            return _m.GetDataStringValue("account_type");
        }

        public string GetAuthCode()
        {
            return _m.GetDataStringValue("auth_code");
        }

        public string GetBankDate()
        {
            return _m.GetDataStringValue("bank_date");
        }

        public string GetBankTime()
        {
            return _m.GetDataStringValue("bank_time");
        }

        public string GetMaskedPan()
        {
            return _m.GetDataStringValue("masked_pan");
        }

        public string GetTerminalId()
        {
            return _m.GetDataStringValue("terminal_id");
        }

        public bool WasMerchantReceiptPrinted()
        {
            return _m.GetDataBoolValue("merchant_receipt_printed", false);
        }

        public bool WasCustomerReceiptPrinted()
        {
            return _m.GetDataBoolValue("customer_receipt_printed", false);
        }

        public DateTime? GetSettlementDate()
        {
            //"bank_settlement_date":"20042018"
            var dateStr = _m.GetDataStringValue("bank_settlement_date");
            if (string.IsNullOrEmpty(dateStr)) return null;
            return DateTime.ParseExact(dateStr, "ddMMyyyy", CultureInfo.InvariantCulture).Date;
        }

        public string GetResponseValue(string attribute)
        {
            return _m.GetDataStringValue(attribute);
        }

        internal JObject ToPaymentSummary()
        {
            return new JObject(
                new JProperty("account_type", GetAccountType()),
                new JProperty("auth_code", GetAuthCode()),
                new JProperty("bank_date", GetBankDate()),
                new JProperty("bank_time", GetBankTime()),
                new JProperty("host_response_code", GetResponseCode()),
                new JProperty("host_response_text", GetResponseText()),
                new JProperty("masked_pan", GetMaskedPan()),
                new JProperty("purchase_amount", GetPurchaseAmount()),
                new JProperty("rrn", GetRRN()),
                new JProperty("scheme_name", SchemeName),
                new JProperty("terminal_id", GetTerminalId()),
                new JProperty("terminal_ref_id", GetTerminalReferenceId()),
                new JProperty("tip_amount", GetTipAmount()),
                new JProperty("surcharge_amount", GetSurchargeAmount())
                );
        }
    }

    public class CancelTransactionRequest
    {

        public Message ToMessage()
        {
            return new Message(RequestIdHelper.Id("ctx"), Events.CancelTransactionRequest, null, true);
        }
    }

    public class CancelTransactionResponse
    {
        public bool Success { get; }
        public string PosRefId { get; }

        private readonly Message _m;

        public CancelTransactionResponse() { }

        public CancelTransactionResponse(Message m)
        {
            _m = m;
            PosRefId = _m.GetDataStringValue("pos_ref_id");
            Success = m.GetSuccessState() == Message.SuccessState.Success;
        }

        public string GetErrorReason()
        {
            return _m.GetDataStringValue("error_reason");
        }

        public string GetErrorDetail()
        {
            return _m.GetDataStringValue("error_detail");
        }

        public bool WasTxnPastPointOfNoReturn()
        {
            return _m.GetError().StartsWith("TXN_PAST_POINT_OF_NO_RETURN");
        }

        public string GetResponseValueWithAttribute(string attribute)
        {
            return _m.GetDataStringValue(attribute);
        }
    }

    public class GetTransactionRequest
    {
        public string PosRefId { get; }

        public GetTransactionRequest(string posRefId)
        {
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {

            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId));

            return new Message(RequestIdHelper.Id("gt"), Events.GetTransactionRequest, data, true);
        }
    }

    public class GetTransactionResponse
    {
        public bool Success;

        private readonly Message _m;

        public GetTransactionResponse() { }

        public GetTransactionResponse(Message m)
        {
            _m = m;
        }

        public string GetPosRefId()
        {
            return _m.GetDataStringValue("pos_ref_id");
        }

        public string GetError()
        {
            JToken e = null;
            var found = _m.Data.TryGetValue("error_reason", out e);
            if (found) return (string)e;
            return null;
        }

        /// <summary>
        /// Tx is a sub object of the payload, so we will copy this into a message for convenience. 
        /// </summary>
        public Message GetTxMessage()
        {
            var txFound = _m.Data.TryGetValue("tx", out var tx);
            if (txFound)
            {
                return new Message(_m.Id, "gt", (JObject)tx, false);
            }

            return null;
        }

        public bool PosRefIdNotFound()
        {
            if (_m.GetError() != null)
                return  _m.GetError().StartsWith("POS_REF_ID_NOT_FOUND");

            return false;
        }

        public bool PosRefIdInvalid()
        {
            if (_m.GetError() != null)
                return _m.GetError().StartsWith("INVALID_ARGUMENTS");

            return false;
        }

        internal bool PosRefIdMissing()
        {
            if (_m.GetError() != null)
                return _m.GetError().StartsWith("MISSING_ARGUMENTS");

            return false;
        }

        public bool WasRetrievedSuccessfully()
        {
            return _m.GetSuccessState() == Message.SuccessState.Success;
        }
     
        public bool IsTransactionInProgress()
        {
            return _m.GetError().Equals("TRANSACTION_IN_PROGRESS");
        }
        
        public bool IsWaitingForSignatureResponse()
        {
            return _m.GetError().Equals("TRANSACTION_IN_PROGRESS_AWAITING_SIGNATURE");
        }

        public bool IsWaitingForAuthCode()
        {
            return _m.GetError().Equals("TRANSACTION_IN_PROGRESS_AWAITING_PHONE_AUTH_CODE");
        }

        public bool IsSomethingElseBlocking()
        {
            return _m.GetError().Equals("OPERATION_IN_PROGRESS");
        }
        
        public void CopyMerchantReceiptToCustomerReceipt()
        {
            var cr = _m.GetDataStringValue("customer_receipt");
            var mr = _m.GetDataStringValue("merchant_receipt");
            if (mr != "" && cr == "")
            {
                _m.Data["customer_receipt"] = new JValue(mr);
            }
        }
    }

    public class GetLastTransactionRequest
    {
        public Message ToMessage()
        {
            return new Message(RequestIdHelper.Id("glt"), Events.GetLastTransactionRequest, null, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class GetLastTransactionResponse
    {

        private readonly Message _m;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public GetLastTransactionResponse() { }

        public GetLastTransactionResponse(Message m)
        {
            _m = m;
        }

        public bool WasRetrievedSuccessfully()
        {
            // We can't rely on checking "success" flag or "error" fields here,
            // as retrieval may be successful, but the retrieved transaction was a fail.
            // So we check if we got back an ResponseCode.
            // (as opposed to say an operation_in_progress_error)
            return !string.IsNullOrEmpty(GetResponseCode());
        }

        public bool WasOperationInProgressError()
        {
            return _m.GetError().StartsWith("OPERATION_IN_PROGRESS");
        }

        public bool IsWaitingForSignatureResponse()
        {
            return _m.GetError().StartsWith("OPERATION_IN_PROGRESS_AWAITING_SIGNATURE");
        }

        public bool IsWaitingForAuthCode()
        {
            return _m.GetError().StartsWith("OPERATION_IN_PROGRESS_AWAITING_PHONE_AUTH_CODE");
        }

        public bool IsStillInProgress(string posRefId)
        {
            return WasOperationInProgressError() && (GetPosRefId().Equals(posRefId) || GetPosRefId() == null);
        }

        public Message.SuccessState GetSuccessState()
        {
            return _m.GetSuccessState();
        }

        public bool WasSuccessfulTx()
        {
            return _m.GetSuccessState() == Message.SuccessState.Success;
        }

        public string GetTxType()
        {
            return _m.GetDataStringValue("transaction_type");
        }

        public string GetPosRefId()
        {
            return _m.GetDataStringValue("pos_ref_id");
        }

        public int GetBankNonCashAmount()
        {
            return _m.GetDataIntValue("bank_noncash_amount");
        }

        public int GetPurchaseAmount()
        {
            return _m.GetDataIntValue("purchase_amount");
        }

        [Obsolete("Should not need to look at this in a GLT Response")]
        public string GetSchemeApp()
        {
            return _m.GetDataStringValue("scheme_name");
        }

        [Obsolete("Should not need to look at this in a GLT Response")]
        public string GetSchemeName()
        {
            return _m.GetDataStringValue("scheme_name");
        }

        [Obsolete("Should not need to look at this in a GLT Response")]
        public int GetAmount()
        {
            return _m.GetDataIntValue("amount_purchase");
        }

        [Obsolete("Should not need to look at this in a GLT Response")]
        public int GetTransactionAmount()
        {
            return _m.GetDataIntValue("amount_transaction_type");
        }

        public string GetBankDateTimeString()
        {

            var ds = _m.GetDataStringValue("bank_date") + _m.GetDataStringValue("bank_time");
            return ds;
        }

        [Obsolete("Should not need to look at this in a GLT Response")]
        public string GetRRN()
        {
            return _m.GetDataStringValue("rrn");
        }

        public string GetResponseText()
        {
            return _m.GetDataStringValue("host_response_text");
        }

        public string GetResponseCode()
        {
            return _m.GetDataStringValue("host_response_code");
        }

        /// <summary>
        /// There is a bug, VSV-920, whereby the customer_receipt is missing from a glt response.
        /// The current recommendation is to use the merchant receipt in place of it if required.
        /// This method modifies the underlying incoming message data by copying
        /// the merchant receipt into the customer receipt only if there 
        /// is a merchant_receipt and there is not a customer_receipt.   
        /// </summary>
        public void CopyMerchantReceiptToCustomerReceipt()
        {
            var cr = _m.GetDataStringValue("customer_receipt");
            var mr = _m.GetDataStringValue("merchant_receipt");
            if (mr != "" && cr == "")
            {
                _m.Data["customer_receipt"] = new JValue(mr);
            }
        }
    }

    public class RefundRequest
    {
        public int AmountCents { get; }
        public string PosRefId { get; }
        public bool SuppressMerchantPassword { get; }

        internal SpiConfig Config = new SpiConfig();

        internal TransactionOptions Options = new TransactionOptions();

        [Obsolete("Id is deprecated. Use PosRefId instead.")]
        public string Id { get; }

        public RefundRequest(int amountCents, string posRefId, bool suppressMerchantPassword)
        {
            AmountCents = amountCents;
            PosRefId = posRefId;
            SuppressMerchantPassword = suppressMerchantPassword;
            Id = RequestIdHelper.Id("refund");
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("refund_amount", AmountCents),
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("suppress_merchant_password", SuppressMerchantPassword)
            );

            Config.EnabledPrintMerchantCopy = true;
            Config.EnabledPromptForCustomerCopyOnEftpos = true;
            Config.EnabledSignatureFlowOnEftpos = true;
            Config.AddReceiptConfig(data);
            Options.AddOptions(data);
            return new Message(RequestIdHelper.Id("refund"), Events.RefundRequest, data, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RefundResponse
    {
        public bool Success { get; }
        public string RequestId { get; }
        public string PosRefId { get; }
        public string SchemeName { get; }

        /// <summary>
        /// Deprecated. Use SchemeName instead
        /// </summary>
        public string SchemeAppName { get; }

        private readonly Message _m;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public RefundResponse() { }

        public RefundResponse(Message m)
        {
            _m = m;
            RequestId = m.Id;
            PosRefId = _m.GetDataStringValue("pos_ref_id");
            SchemeName = _m.GetDataStringValue("scheme_name");
            SchemeAppName = _m.GetDataStringValue("scheme_name");
            Success = m.GetSuccessState() == Message.SuccessState.Success;
        }

        public int GetRefundAmount()
        {
            return _m.GetDataIntValue("refund_amount");
        }

        public string GetRRN()
        {
            return _m.GetDataStringValue("rrn");
        }

        public string GetCustomerReceipt()
        {
            return _m.GetDataStringValue("customer_receipt");
        }

        public string GetMerchantReceipt()
        {
            return _m.GetDataStringValue("merchant_receipt");
        }

        public string GetResponseText()
        {
            return _m.GetDataStringValue("host_response_text");
        }

        public string GetResponseCode()
        {
            return _m.GetDataStringValue("host_response_code");
        }

        public string GetTerminalReferenceId()
        {
            return _m.GetDataStringValue("terminal_ref_id");
        }

        public string GetCardEntry()
        {
            return _m.GetDataStringValue("card_entry");
        }

        public string GetAccountType()
        {
            return _m.GetDataStringValue("account_type");
        }

        public string GetAuthCode()
        {
            return _m.GetDataStringValue("auth_code");
        }

        public string GetBankDate()
        {
            return _m.GetDataStringValue("bank_date");
        }

        public string GetBankTime()
        {
            return _m.GetDataStringValue("bank_time");
        }

        public string GetMaskedPan()
        {
            return _m.GetDataStringValue("masked_pan");
        }

        public string GetTerminalId()
        {
            return _m.GetDataStringValue("terminal_id");
        }

        public bool WasMerchantReceiptPrinted()
        {
            return _m.GetDataBoolValue("merchant_receipt_printed", false);
        }

        public bool WasCustomerReceiptPrinted()
        {
            return _m.GetDataBoolValue("customer_receipt_printed", false);
        }

        public DateTime? GetSettlementDate()
        {
            //"bank_settlement_date":"20042018"
            var dateStr = _m.GetDataStringValue("bank_settlement_date");
            if (string.IsNullOrEmpty(dateStr)) return null;
            return DateTime.ParseExact(dateStr, "ddMMyyyy", CultureInfo.InvariantCulture).Date;
        }

        public string GetResponseValue(string attribute)
        {
            return _m.GetDataStringValue(attribute);
        }

    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SignatureRequired
    {
        public string RequestId { get; }
        public string PosRefId { get; }
        private string _receiptToSign;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public SignatureRequired() { }

        public SignatureRequired(Message m)
        {
            RequestId = m.Id;
            PosRefId = m.GetDataStringValue("pos_ref_id");
            _receiptToSign = m.GetDataStringValue("merchant_receipt");
        }

        public SignatureRequired(string posRefId, string requestId, string receiptToSign)
        {
            RequestId = requestId;
            PosRefId = posRefId;
            _receiptToSign = receiptToSign;
        }

        public string GetMerchantReceipt()
        {
            return _receiptToSign;
        }
    }

    public class SignatureDecline
    {
        public string PosRefId { get; }
        public SignatureDecline(string posRefId)
        {
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId)
            );
            return new Message(RequestIdHelper.Id("sigdec"), Events.SignatureDeclined, data, true);
        }
    }

    public class SignatureAccept
    {
        public string PosRefId { get; }

        public SignatureAccept(string posRefId)
        {
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId)
            );
            return new Message(RequestIdHelper.Id("sigacc"), Events.SignatureAccepted, data, true);
        }
    }

    public class MotoPurchaseRequest
    {
        public string PosRefId { get; }
        public int PurchaseAmount { get; }
        public int SurchargeAmount { get; set; }
        public bool SuppressMerchantPassword { get; set; }

        internal SpiConfig Config = new SpiConfig();

        internal TransactionOptions Options = new TransactionOptions();

        public MotoPurchaseRequest(int amountCents, string posRefId)
        {
            PosRefId = posRefId;
            PurchaseAmount = amountCents;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("purchase_amount", PurchaseAmount),
                new JProperty("surcharge_amount", SurchargeAmount),
                new JProperty("suppress_merchant_password", SuppressMerchantPassword)
            );

            Config.EnabledPrintMerchantCopy = true;
            Config.EnabledPromptForCustomerCopyOnEftpos = true;
            Config.EnabledSignatureFlowOnEftpos = true;
            Config.AddReceiptConfig(data);
            Options.AddOptions(data);
            return new Message(RequestIdHelper.Id("moto"), Events.MotoPurchaseRequest, data, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class MotoPurchaseResponse
    {
        public string PosRefId { get; }
        public PurchaseResponse PurchaseResponse { get; }

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public MotoPurchaseResponse() { }

        public MotoPurchaseResponse(Message m)
        {
            PurchaseResponse = new PurchaseResponse(m);
            PosRefId = PurchaseResponse.PosRefId;
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PhoneForAuthRequired
    {
        public string RequestId { get; }
        public string PosRefId { get; }

        private string _phoneNumber;
        private string _merchantId;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public PhoneForAuthRequired() { }

        public PhoneForAuthRequired(Message m)
        {
            RequestId = m.Id;
            PosRefId = m.GetDataStringValue("pos_ref_id");
            _phoneNumber = m.GetDataStringValue("auth_centre_phone_number");
            _merchantId = m.GetDataStringValue("merchant_id");
        }

        public PhoneForAuthRequired(string posRefId, string requestId, string phoneNumber, string merchantId)
        {
            RequestId = requestId;
            PosRefId = posRefId;
            _phoneNumber = phoneNumber;
            _merchantId = merchantId;
        }

        public string GetPhoneNumber()
        {
            return _phoneNumber;
        }

        public string GetMerchantId()
        {
            return _merchantId;
        }
    }

    public class AuthCodeAdvice
    {
        public string PosRefId { get; }
        public string AuthCode { get; }

        public AuthCodeAdvice(string posRefId, string authCode)
        {
            PosRefId = posRefId;
            AuthCode = authCode;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("auth_code", AuthCode)
            );
            return new Message(RequestIdHelper.Id("authad"), Events.AuthCodeAdvice, data, true);
        }
    }

    public class TransactionUpdate
    {
        public string DisplayMessageCode { get; }
        public string DisplayMessageText { get; }

        public TransactionUpdate() { }

        public TransactionUpdate(Message m)
        {
            DisplayMessageCode = m.GetDataStringValue("display_message_code");
            DisplayMessageText = m.GetDataStringValue("display_message_text");
        }
    }
}