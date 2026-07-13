using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CheckOut_Sele.Checkout.Pages
{
    public class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        protected IWebElement WaitForElementToBeVisible(By locator)
        {
            return Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
        }

        protected IWebElement WaitForElementToBeClickable(By locator)
        {
            return Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
        }

        protected void ClickViaJavaScript(IWebElement element)
        {
            var jsExecutor = (IJavaScriptExecutor)Driver;
            jsExecutor.ExecuteScript("arguments[0].click();", element);
        }

        protected void ScrollToElement(IWebElement element)
        {
            var jsExecutor = (IJavaScriptExecutor)Driver;
            jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }
    }
}
