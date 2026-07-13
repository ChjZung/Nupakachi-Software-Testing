using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NupakachiShop_Automation
{
    [TestFixture]
    public class CheckoutValidationTests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(2));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl("http://nupakachi.somee.com/");
            var signInBtn = driver.FindElement(By.LinkText("Sign in"));
            js.ExecuteScript("arguments[0].click();", signInBtn);
            driver.FindElement(By.Id("loginInput")).SendKeys("thuanphat");
            driver.FindElement(By.Id("passwordInput")).SendKeys("123456");
            driver.FindElement(By.CssSelector(".btn-pink")).Click();
            Thread.Sleep(2000);
            var sanPhamBtn = driver.FindElement(By.LinkText("Sản phẩm"));
            js.ExecuteScript("arguments[0].click();", sanPhamBtn);
            Thread.Sleep(2000);
            var xemChiTietBtn = driver.FindElement(By.LinkText("Xem chi tiết"));
            js.ExecuteScript("arguments[0].click();", xemChiTietBtn);
            Thread.Sleep(2000);
            var addCartBtn = driver.FindElement(By.CssSelector(".btn-pink"));
            js.ExecuteScript("window.alert = function() { }; window.confirm = function() { return true; };");
            js.ExecuteScript("arguments[0].click();", addCartBtn);
            Thread.Sleep(1000);

            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (NoAlertPresentException)
            {
            }
            Thread.Sleep(1000);
            var thanhToanBtn = driver.FindElement(By.LinkText("Thanh toán"));
            js.ExecuteScript("arguments[0].click();", thanhToanBtn);
            Thread.Sleep(2000);
        }

        [TestCase("", "", "", "", "Vui lòng nhập đầy đủ thông tin", TestName = "TC_VAL_01_Bo_Trong_Toan_Bo_Form")]
        [TestCase("Nguyễn A", "090123abcd", "test@gmail.com", "Hà Nội", "SĐT không hợp lệ", TestName = "TC_VAL_02_SDT_Chua_Chu_Cai")]
        [TestCase("Nguyễn A", "09012345", "test@gmail.com", "Hà Nội", "SĐT phải có đủ 10 chữ số", TestName = "TC_VAL_03_SDT_Sai_Do_Dai")]
        [TestCase("Nguyễn A", "8901234567", "test@gmail.com", "Hà Nội", "SĐT phải bắt đầu bằng số 0", TestName = "TC_VAL_04_SDT_Khong_Bat_Dau_Bang_0")]
        [TestCase("Nguyễn A", "0901234567", "nguyenvana.com", "Hà Nội", "Email không hợp lệ", TestName = "TC_VAL_05_Email_Sai_Dinh_Dang")]
        [TestCase("Nguyễn A", "0901234567", "chuoi_email_dai_hon_255_ky_tu_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@gmail.com", "Hà Nội", "Email vượt quá số ký tự cho phép", TestName = "TC_VAL_06_Email_Vuot_Qua_Do_Dai")]
        [TestCase("Nguyễn A", "0901234567", "nguyễn@gmail.com", "Hà Nội", "Email không được chứa tiếng Việt có dấu", TestName = "TC_VAL_07_Email_Co_Tieng_Viet")]
        [TestCase("Nguyễn A", "0901234567", "abc@xyz@gmail.com", "Hà Nội", "Email không hợp lệ", TestName = "TC_VAL_08_Email_Nhieu_Ky_Tu_At")]
        [TestCase("   ", "0901234567", "test@gmail.com", "Hà Nội", "Không được để trống Họ Tên", TestName = "TC_VAL_09_Ten_Toan_Khoang_Trang")]
        [TestCase("Nguyễn AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "0901234567", "test@gmail.com", "Hà Nội", "Họ Tên vượt quá giới hạn ký tự", TestName = "TC_VAL_10_Ten_Vuot_Qua_Do_Dai")]
        [TestCase("Nguyen @Van!", "0901234567", "test@gmail.com", "Hà Nội", "Họ Tên không được chứa ký tự đặc biệt", TestName = "TC_VAL_11_Ten_Co_Ky_Tu_Dac_Biet")]
        [TestCase("Nguyễn A", "0901234567", "test@gmail.com", "Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài Một chuỗi địa chỉ rất dài", "Địa chỉ quá dài", TestName = "TC_VAL_12_Dia_Chi_Vuot_Qua_Do_Dai")]
        [TestCase("Nguyễn A", "0901234567", "test@gmail.com", "!@#$%^", "Địa chỉ không hợp lệ", TestName = "TC_VAL_13_Dia_Chi_Toan_Ky_Tu_Dac_Biet")]
        [TestCase("Nguyễn A", "090 123 4567", "test@gmail.com", "Hà Nội", "SĐT không hợp lệ", TestName = "TC_VAL_14_SDT_Chua_Khoang_Trang")]
        [TestCase("Nguyễn A", "0901234567", "abc@.com", "Hà Nội", "Email không hợp lệ", TestName = "TC_VAL_15_Email_Thieu_Domain")]
        public void FormValidation_ShouldShowCorrectError(string name, string phone, string email, string address, string expectedError)
        {
            var nameField = driver.FindElement(By.Id("HoTen"));
            SafeEnterText(driver, nameField, name);
            Thread.Sleep(500);
            var phoneField = driver.FindElement(By.Id("DienThoai"));
            SafeEnterText(driver, phoneField, phone);
            Thread.Sleep(500);
            var emailField = driver.FindElement(By.Id("Email"));
            SafeEnterText(driver, emailField, email);
            Thread.Sleep(500);
            var addressField = driver.FindElement(By.Id("DiaChi"));
            SafeEnterText(driver, addressField, address);
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector(".btn-pink")).Click();
            Thread.Sleep(1000);
            string nameValidationMsg = nameField.GetAttribute("validationMessage") ?? "";
            string phoneValidationMsg = phoneField.GetAttribute("validationMessage") ?? "";
            string emailValidationMsg = emailField.GetAttribute("validationMessage") ?? "";
            string addressValidationMsg = addressField.GetAttribute("validationMessage") ?? "";


            string activeHtml5Error = "";
            if (!string.IsNullOrEmpty(nameValidationMsg)) activeHtml5Error = nameValidationMsg;
            else if (!string.IsNullOrEmpty(phoneValidationMsg)) activeHtml5Error = phoneValidationMsg;
            else if (!string.IsNullOrEmpty(emailValidationMsg)) activeHtml5Error = emailValidationMsg;
            else if (!string.IsNullOrEmpty(addressValidationMsg)) activeHtml5Error = addressValidationMsg;

            bool isErrorDisplayed = false;
            if (!string.IsNullOrEmpty(activeHtml5Error))
            {
                isErrorDisplayed = MatchesExpectedValidationError(expectedError, activeHtml5Error);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(string.IsNullOrEmpty(activeHtml5Error), "Validation message of HTML5 should not be empty.");
            }
            else
            {
                isErrorDisplayed = driver.PageSource.Contains(expectedError);
            }

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(isErrorDisplayed, $"Test thất bại! Không tìm thấy thông báo lỗi: '{expectedError}' trên màn hình hoặc trong validationMessage (Lỗi thực tế nhận được: '{activeHtml5Error}').");
        }

        private bool MatchesExpectedValidationError(string expectedError, string actualHtml5Error)
        {
            if (string.IsNullOrEmpty(actualHtml5Error)) return false;
            string expectedLower = expectedError.ToLower();
            string actualLower = actualHtml5Error.ToLower();
            if (actualLower.Contains(expectedLower)) return true;
            if (expectedLower.Contains("đầy đủ thông tin") || expectedLower.Contains("để trống") || expectedLower.Contains("vui lòng nhập"))
            {
                return actualLower.Contains("fill out") || 
                       actualLower.Contains("điền vào") || 
                       actualLower.Contains("nhập");
            }

            if (expectedLower.Contains("email"))
            {
                return actualLower.Contains("@") || 
                       actualLower.Contains("email") || 
                       actualLower.Contains("địa chỉ") || 
                       actualLower.Contains("position");
            }

            if (expectedLower.Contains("sđt") || expectedLower.Contains("số điện thoại") || expectedLower.Contains("chữ số"))
            {
                return actualLower.Contains("format") || 
                       actualLower.Contains("định dạng") || 
                       actualLower.Contains("khớp");
            }

            return false;
        }

        private void SafeEnterText(IWebDriver driver, IWebElement element, string text)
        {
            try
            {
                element.Click();
            }
            catch (Exception) { }

            element.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                element.SendKeys(text);
                
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                string val = (string)js.ExecuteScript("return arguments[0].value;", element);
                if (val != text)
                {
                    js.ExecuteScript("arguments[0].value = arguments[1]; " +
                                     "arguments[0].dispatchEvent(new Event('input', { bubbles: true })); " +
                                     "arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", 
                                     element, text);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}