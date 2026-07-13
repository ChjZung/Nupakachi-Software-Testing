using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CheckOut_Sele.Checkout.Pages
{
    public class CheckoutPage : BasePage
    {
        private readonly By _fullNameInput = By.Id("HoTen");
        private readonly By _emailInput = By.Id("Email");
        private readonly By _phoneInput = By.Id("DienThoai");
        private readonly By _addressInput = By.Id("DiaChi");
        private readonly By _paymentSelect = By.Name("PhuongThucThanhToan");
        private readonly By _submitOrderButton = By.CssSelector("#checkoutForm button[type='submit']");
        
        // Target elements for vouchers based on standard test cases
        private readonly By _voucherInput = By.Id("voucherCode");
        private readonly By _applyVoucherButton = By.Id("btnApplyVoucher");
        private readonly By _voucherErrorText = By.CssSelector(".voucher-error-msg");

        // Order summary elements
        private readonly By _subtotalText = By.XPath("//span[text()='Subtotal']/following-sibling::span");
        private readonly By _discountText = By.XPath("//span[text()='Discount']/following-sibling::span");
        private readonly By _shippingText = By.XPath("//span[text()='Delivery fee']/following-sibling::span");
        private readonly By _totalText = By.XPath("//span[text()='Total']/following-sibling::span");

        public CheckoutPage(IWebDriver driver) : base(driver)
        {
        }

        public void EnterFullName(string name)
        {
            var element = WaitForElementToBeVisible(_fullNameInput);
            element.Clear();
            element.SendKeys(name);
        }

        public void EnterEmail(string email)
        {
            var element = WaitForElementToBeVisible(_emailInput);
            element.Clear();
            element.SendKeys(email);
        }

        public void EnterPhone(string phone)
        {
            var element = WaitForElementToBeVisible(_phoneInput);
            element.Clear();
            element.SendKeys(phone);
        }

        public void EnterAddress(string address)
        {
            var element = WaitForElementToBeVisible(_addressInput);
            element.Clear();
            element.SendKeys(address);
        }

        public void SelectPaymentMethod(string method)
        {
            var element = WaitForElementToBeVisible(_paymentSelect);
            element.Click();
            var option = element.FindElement(By.XPath($".//option[@value='{method}']"));
            option.Click();
        }

        public void EnterVoucher(string voucherCode)
        {
            try
            {
                var element = WaitForElementToBeVisible(_voucherInput);
                element.Clear();
                element.SendKeys(voucherCode);
            }
            catch (WebDriverTimeoutException)
            {
                // Graceful fallback if fields do not exist dynamically
            }
        }

        public void ClickApplyVoucher()
        {
            try
            {
                var element = WaitForElementToBeClickable(_applyVoucherButton);
                ClickViaJavaScript(element);
            }
            catch (WebDriverTimeoutException)
            {
                // Graceful fallback
            }
        }

        public void ClickSubmitOrder()
        {
            var element = WaitForElementToBeClickable(_submitOrderButton);
            ClickViaJavaScript(element);
        }

        public string GetSubtotalText()
        {
            return WaitForElementToBeVisible(_subtotalText).Text;
        }

        public string GetDiscountText()
        {
            return WaitForElementToBeVisible(_discountText).Text;
        }

        public string GetShippingText()
        {
            return WaitForElementToBeVisible(_shippingText).Text;
        }

        public string GetTotalText()
        {
            return WaitForElementToBeVisible(_totalText).Text;
        }

        public string GetVoucherErrorText()
        {
            try
            {
                return WaitForElementToBeVisible(_voucherErrorText).Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }

        public bool IsErrorTextDisplayed(string errorMessage)
        {
            try
            {
                // Check in page source for generic message representation
                return Driver.PageSource.Contains(errorMessage);
            }
            catch (WebDriverException)
            {
                return false;
            }
        }
    }
}
