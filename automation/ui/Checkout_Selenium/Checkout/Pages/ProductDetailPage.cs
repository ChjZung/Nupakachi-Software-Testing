using OpenQA.Selenium;

namespace CheckOut_Sele.Checkout.Pages
{
    public class ProductDetailPage : BasePage
    {
        private readonly By _quantityInput = By.Id("soluong");
        private readonly By _addToCartButton = By.CssSelector("#frmAddCart button[type='submit']");
        private readonly By _sizeButtons = By.CssSelector(".size-btn");

        public ProductDetailPage(IWebDriver driver) : base(driver)
        {
        }

        public void EnterQuantity(string quantity)
        {
            var element = WaitForElementToBeVisible(_quantityInput);
            element.Clear();
            element.SendKeys(quantity);
        }

        public void SelectFirstAvailableSize()
        {
            try
            {
                var elements = Driver.FindElements(_sizeButtons);
                if (elements.Count > 0)
                {
                    ClickViaJavaScript(elements[0]);
                }
            }
            catch (NoSuchElementException)
            {
                // Size selection might not be present for all items
            }
        }

        public void ClickAddToCart()
        {
            var element = WaitForElementToBeClickable(_addToCartButton);
            ClickViaJavaScript(element);
        }
    }
}
