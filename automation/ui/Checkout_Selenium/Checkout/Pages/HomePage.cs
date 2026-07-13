using OpenQA.Selenium;

namespace CheckOut_Sele.Checkout.Pages
{
    public class HomePage : BasePage
    {
        private readonly By _signInLink = By.LinkText("Sign in");
        private readonly By _usernameInput = By.Id("loginInput");
        private readonly By _passwordInput = By.Id("passwordInput");
        private readonly By _loginButton = By.CssSelector(".btn-pink");
        private readonly By _productsLink = By.LinkText("Sản phẩm");
        private readonly By _loggedInUserBadge = By.CssSelector(".user-info-badge"); // hypothetical tag based on standard layout

        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        public void NavigateTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public void ClickSignInLink()
        {
            var element = WaitForElementToBeClickable(_signInLink);
            ClickViaJavaScript(element);
        }

        public void EnterUsername(string username)
        {
            var element = WaitForElementToBeVisible(_usernameInput);
            element.Clear();
            element.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            var element = WaitForElementToBeVisible(_passwordInput);
            element.Clear();
            element.SendKeys(password);
        }

        public void ClickLogin()
        {
            var element = WaitForElementToBeClickable(_loginButton);
            element.Click();
        }

        public void ClickProductsLink()
        {
            var element = WaitForElementToBeClickable(_productsLink);
            ClickViaJavaScript(element);
        }

        public bool IsUserLoggedIn()
        {
            try
            {
                return Driver.FindElement(_loggedInUserBadge).Displayed;
            }
            catch (NoSuchElementException)
            {
                // Fallback check if standard badge does not exist, look for "Log out" text
                try
                {
                    return Driver.FindElement(By.LinkText("Log out")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
        }
    }
}
