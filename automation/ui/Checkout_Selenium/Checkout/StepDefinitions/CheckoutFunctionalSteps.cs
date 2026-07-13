using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Checkout.StepDefinitions
{
    [Binding]
    public class CheckoutFunctionalSteps
    {
        private IWebDriver _driver;
        private readonly ScenarioContext _scenarioContext;

        private const string BaseUrl = "http://nupakachi.somee.com/";
        private const string TestUser = "thuanphat";
        private const string TestPass = "123456";

        public CheckoutFunctionalSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var options = new OpenQA.Selenium.Chrome.ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            
            _driver = new OpenQA.Selenium.Chrome.ChromeDriver(
                OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService(), 
                options, 
                TimeSpan.FromMinutes(2)
            );
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _driver.Manage().Window.Maximize();
            
            _scenarioContext["WebDriver"] = _driver;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        private void WaitForUrlToContain(string fraction, int seconds = 25)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
            try
            {
                wait.Until(d => d.Url.Contains(fraction));
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine("--- TIMEOUT WAITING FOR URL TO CONTAIN: " + fraction + " ---");
                Console.WriteLine("Current URL: " + _driver.Url);
                try
                {
                    string bodyText = _driver.FindElement(By.TagName("body")).Text;
                    Console.WriteLine("Body Text Preview: " + (bodyText.Length > 2000 ? bodyText.Substring(0, 2000) : bodyText));
                }
                catch (Exception bodyEx)
                {
                    Console.WriteLine("Could not get body text: " + bodyEx.Message);
                }
                throw;
            }
        }

        private void PerformLoginAndNavigateToCheckout()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            
            _driver.Navigate().GoToUrl(BaseUrl);
            
            var signInBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Sign in")));
            js.ExecuteScript("arguments[0].click();", signInBtn);

            var loginInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("loginInput")));
            loginInput.SendKeys(TestUser);
            _driver.FindElement(By.Id("passwordInput")).SendKeys(TestPass);
            _driver.FindElement(By.CssSelector(".btn-pink")).Click();
            
            // Wait until logged in (Sign out link appears)
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Đăng Xuất")));

            _driver.Navigate().GoToUrl(BaseUrl + "Home/SanPham");
            
            var xemChiTietBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Xem chi tiết")));
            js.ExecuteScript("arguments[0].click();", xemChiTietBtn);

            var addCartBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#frmAddCart button.btn-pink")));
            js.ExecuteScript("window.alert = function() { }; window.confirm = function() { return true; };");
            js.ExecuteScript("arguments[0].click();", addCartBtn);
            
            var thanhToanBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.LinkText("Thanh toán")));
            js.ExecuteScript("arguments[0].click();", thanhToanBtn);
            
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("HoTen")));
            Thread.Sleep(2000);
        }

        // --- TC_FUN_01 ---
        [Given(@"Người dùng điền đầy đủ thông tin giao hàng hợp lệ")]
        public void GivenNguoiDungDienDayDuThongTinGiaoHangHopLe()
        {
            PerformLoginAndNavigateToCheckout();
            
            var hoTen = _driver.FindElement(By.Id("HoTen"));
            hoTen.Clear();
            hoTen.SendKeys(GenerateUniqueName());
            
            var email = _driver.FindElement(By.Id("Email"));
            email.Clear();
            email.SendKeys(GenerateUniqueEmail());
            
            var dienThoai = _driver.FindElement(By.Id("DienThoai"));
            dienThoai.Clear();
            dienThoai.SendKeys(GenerateUniquePhone());
            
            var diaChi = _driver.FindElement(By.Id("DiaChi"));
            diaChi.Clear();
            diaChi.SendKeys("Hà Nội");
            
            Console.WriteLine("--- FORM VALUES IMMEDIATELY AFTER FILLING ---");
            Console.WriteLine("HoTen: '" + hoTen.GetAttribute("value") + "'");
            Console.WriteLine("Email: '" + email.GetAttribute("value") + "'");
            Console.WriteLine("DienThoai: '" + dienThoai.GetAttribute("value") + "'");
            Console.WriteLine("DiaChi: '" + diaChi.GetAttribute("value") + "'");
        }

        [Given(@"Người dùng chọn phương thức thanh toán là ""(.*)""")]
        public void GivenNguoiDungChonPhuongThucThanhToanLa(string method)
        {
            var selectPayment = _driver.FindElement(By.Name("PhuongThucThanhToan"));
            selectPayment.Click();
            var option = selectPayment.FindElement(By.XPath($".//option[@value='{method}']"));
            option.Click();
        }

        [When(@"Người dùng bấm đặt hàng")]
        public void WhenNguoiDungBamDatHang()
        {
            var hoTen = _driver.FindElement(By.Id("HoTen"));
            var email = _driver.FindElement(By.Id("Email"));
            var dienThoai = _driver.FindElement(By.Id("DienThoai"));
            var diaChi = _driver.FindElement(By.Id("DiaChi"));
            
            Console.WriteLine("--- FORM VALUES BEFORE SUBMIT ---");
            Console.WriteLine("HoTen: '" + hoTen.GetAttribute("value") + "'");
            Console.WriteLine("Email: '" + email.GetAttribute("value") + "'");
            Console.WriteLine("DienThoai: '" + dienThoai.GetAttribute("value") + "'");
            Console.WriteLine("DiaChi: '" + diaChi.GetAttribute("value") + "'");
            
            _driver.FindElement(By.CssSelector("#checkoutForm button[type='submit']")).Click();
            
            Thread.Sleep(2000);
            if (_driver.Url.Contains("ThanhToan"))
            {
                Console.WriteLine("--- SUBMIT FAILED (STILL ON THANHTOAN) ---");
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                Console.WriteLine("HoTen validationMessage: '" + js.ExecuteScript("return arguments[0].validationMessage;", hoTen) + "'");
                Console.WriteLine("Email validationMessage: '" + js.ExecuteScript("return arguments[0].validationMessage;", email) + "'");
                Console.WriteLine("DienThoai validationMessage: '" + js.ExecuteScript("return arguments[0].validationMessage;", dienThoai) + "'");
                Console.WriteLine("DiaChi validationMessage: '" + js.ExecuteScript("return arguments[0].validationMessage;", diaChi) + "'");
            }
        }

        [Then(@"Hệ thống chuyển hướng sang trang ""Đặt hàng thành công"" và hiển thị mã đơn hàng dạng ""HD""")]
        public void ThenHeThongChuyenHuongSangTrangDatHangThanhCongVaHienThiMaDonHangDangHD()
        {
            WaitForUrlToContain("DatHangThanhCong", 25);
            string maDon = GetOrderCodeFromUrlOrDb();
            Assert.IsFalse(string.IsNullOrEmpty(maDon), "Không lấy được mã đơn hàng từ URL hoặc Database.");
            Assert.IsTrue(maDon.StartsWith("HD") || maDon.StartsWith("DH"), $"Mã đơn hàng '{maDon}' không bắt đầu bằng 'HD' hoặc 'DH'");
            _scenarioContext["LastCreatedOrderCode"] = maDon;
        }

        // --- TC_FUN_02 ---
        [Given(@"Người dùng đã tạo đơn hàng thành công")]
        public void GivenNguoiDungDaTaoDonHangThanhCong()
        {
            GivenNguoiDungDienDayDuThongTinGiaoHangHopLe();
            GivenNguoiDungChonPhuongThucThanhToanLa("COD");
            WhenNguoiDungBamDatHang();
            ThenHeThongChuyenHuongSangTrangDatHangThanhCongVaHienThiMaDonHangDangHD();
        }

        [When(@"Người dùng quay về trang chủ và vào lại Giỏ hàng")]
        public void WhenNguoiDungQuayVeTrangChuVaVaoLaiGioHang()
        {
            _driver.Navigate().GoToUrl(BaseUrl + "Home/GioHang");
            WaitForUrlToContain("GioHang");
        }

        [Then(@"Giỏ hàng hiển thị trống với thông báo giỏ hàng trống")]
        public void ThenGioHangHienThiTrongVoiThongBaoGioHangTrong()
        {
            Assert.IsTrue(_driver.PageSource.Contains("Giỏ hàng của bạn đang trống!"), "Không hiển thị giỏ hàng trống.");
        }

        // --- TC_FUN_03 ---
        [Given(@"Người dùng chọn mua một sản phẩm với số lượng lớn hơn số lượng tồn kho")]
        public void GivenNguoiDungChonMuaMotSanPhamVoiSoLuongLonHonSoLuongTonKho()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _driver.Navigate().GoToUrl(BaseUrl + "Account/DangNhap");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("loginInput"))).SendKeys(TestUser);
            _driver.FindElement(By.Id("passwordInput")).SendKeys(TestPass);
            _driver.FindElement(By.CssSelector(".btn-pink")).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Đăng Xuất")));

            _driver.Navigate().GoToUrl(BaseUrl + "Home/SanPham");
            
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            var xemChiTietBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Xem chi tiết")));
            js.ExecuteScript("arguments[0].click();", xemChiTietBtn);
            
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("soluong")));

            // Bypass HTML5 validation limit in Chrome using JS submit
            js.ExecuteScript("document.getElementById('soluong').value = '99999'; document.getElementById('frmAddCart').submit();");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.LinkText("Thanh toán")));
        }

        [When(@"Người dùng bấm tiến hành đặt hàng")]
        public void WhenNguoiDungBamTienHanhDatHang()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _driver.Navigate().GoToUrl(BaseUrl + "Home/ThanhToan");
            
            var hoTen = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("HoTen")));
            Thread.Sleep(2000);
            hoTen.Clear();
            hoTen.SendKeys("Test Stock Limit");
            
            var email = _driver.FindElement(By.Id("Email"));
            email.Clear();
            email.SendKeys(GenerateUniqueEmail());
            
            var dienThoai = _driver.FindElement(By.Id("DienThoai"));
            dienThoai.Clear();
            dienThoai.SendKeys(GenerateUniquePhone());
            
            var diaChi = _driver.FindElement(By.Id("DiaChi"));
            diaChi.Clear();
            diaChi.SendKeys("Hà Nội");
            
            _driver.FindElement(By.CssSelector("#checkoutForm button[type='submit']")).Click();
        }

        [Then(@"Hệ thống chuyển hướng người dùng về trang Giỏ hàng để điều chỉnh số lượng")]
        public void ThenHeThongChuyenHuongNguoiDungVeTrangGioHangDeDieuChinhSoLuong()
        {
            WaitForUrlToContain("GioHang", 25);
        }

        // --- TC_FUN_04 ---
        [Given(@"Người dùng thêm 1 sản phẩm vào giỏ hàng với số lượng là 2")]
        public void GivenNguoiDungThem1SanPhamVaoGioHangVoiSoLuongLa2()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            _driver.Navigate().GoToUrl(BaseUrl + "Account/DangNhap");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("loginInput"))).SendKeys(TestUser);
            _driver.FindElement(By.Id("passwordInput")).SendKeys(TestPass);
            _driver.FindElement(By.CssSelector(".btn-pink")).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Đăng Xuất")));

            _driver.Navigate().GoToUrl(BaseUrl + "Home/SanPham");
            
            var xemChiTietBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Xem chi tiết")));
            js.ExecuteScript("arguments[0].click();", xemChiTietBtn);

            var frm = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("frmAddCart")));
            string priceAttr = frm.GetAttribute("data-price");
            decimal price = decimal.Parse(priceAttr);
            _scenarioContext["ProductPrice"] = price;
            _scenarioContext["BuyQty"] = 2;

            var qtyInput = _driver.FindElement(By.Id("soluong"));
            qtyInput.Clear();
            qtyInput.SendKeys("2");
            
            var addCartBtn = _driver.FindElement(By.CssSelector(".btn-pink"));
            js.ExecuteScript("window.alert = function() { }; window.confirm = function() { return true; };");
            js.ExecuteScript("arguments[0].click();", addCartBtn);
            
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.LinkText("Thanh toán")));
        }

        [When(@"Người dùng mở màn hình thanh toán")]
        public void WhenNguoiDungMoManHinhThanhToan()
        {
            _driver.Navigate().GoToUrl(BaseUrl + "Home/ThanhToan");
            WaitForUrlToContain("ThanhToan");
        }

        [Then(@"Dòng tạm tính hiển thị chính xác bằng giá bán của sản phẩm nhân với 2")]
        public void ThenDongTamTinhHienThiChinhXacBangGiaBanCuaSanPhamNhanVoi2()
        {
            var subtotalElement = _driver.FindElement(By.XPath("//span[text()='Subtotal']/following-sibling::span"));
            string subtotalText = subtotalElement.Text.Replace(" đ", "").Replace(",", "").Trim();
            decimal actualSubtotal = decimal.Parse(subtotalText);
            
            decimal price = (decimal)_scenarioContext["ProductPrice"];
            int qty = (int)_scenarioContext["BuyQty"];
            decimal expectedSubtotal = price * qty;
            
            Assert.AreEqual(expectedSubtotal, actualSubtotal, "Dòng tạm tính (Subtotal) hiển thị không chính xác.");
        }

        // --- TC_FUN_05 ---
        [Given(@"Người dùng thêm 1 sản phẩm vào giỏ hàng")]
        public void GivenNguoiDungThem1SanPhamVaoGioHang()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            _driver.Navigate().GoToUrl(BaseUrl + "Account/DangNhap");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("loginInput"))).SendKeys(TestUser);
            _driver.FindElement(By.Id("passwordInput")).SendKeys(TestPass);
            _driver.FindElement(By.CssSelector(".btn-pink")).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Đăng Xuất")));

            _driver.Navigate().GoToUrl(BaseUrl + "Home/SanPham");
            
            var xemChiTietBtn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText("Xem chi tiết")));
            js.ExecuteScript("arguments[0].click();", xemChiTietBtn);

            var frm = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("frmAddCart")));
            string priceAttr = frm.GetAttribute("data-price");
            _scenarioContext["ProductPrice"] = decimal.Parse(priceAttr);
            _scenarioContext["BuyQty"] = 1;

            var qtyInput = _driver.FindElement(By.Id("soluong"));
            qtyInput.Clear();
            qtyInput.SendKeys("1");
            
            var addCartBtn = _driver.FindElement(By.CssSelector(".btn-pink"));
            js.ExecuteScript("window.alert = function() { }; window.confirm = function() { return true; };");
            js.ExecuteScript("arguments[0].click();", addCartBtn);
            
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.LinkText("Thanh toán")));
        }

        [Then(@"Dòng giảm giá mặc định hiển thị ""(.*)"" và phí vận chuyển mặc định hiển thị ""(.*)""")]
        public void ThenDongGiamGiaMacDinhHienThiVaPhiVanChuyenMacDinhHienThi(string discountVal, string shippingVal)
        {
            var discountElement = _driver.FindElement(By.XPath("//span[text()='Discount']/following-sibling::span"));
            var shippingElement = _driver.FindElement(By.XPath("//span[text()='Delivery fee']/following-sibling::span"));
            
            Assert.AreEqual(discountVal, discountElement.Text.Trim(), "Dòng giảm giá (Discount) mặc định hiển thị sai.");
            Assert.AreEqual(shippingVal, shippingElement.Text.Trim(), "Dòng phí vận chuyển (Delivery fee) mặc định hiển thị sai.");
        }

        [Then(@"Tổng tiền hiển thị bằng đúng dòng tạm tính")]
        public void ThenTongTienHienThiBangDungDongTamTinh()
        {
            var subtotalElement = _driver.FindElement(By.XPath("//span[text()='Subtotal']/following-sibling::span"));
            var totalElement = _driver.FindElement(By.XPath("//span[text()='Total']/following-sibling::span"));
            
            Assert.AreEqual(subtotalElement.Text.Trim(), totalElement.Text.Trim(), "Tổng tiền (Total) không bằng Tạm tính (Subtotal).");
        }

        // --- TC_FUN_06 ---
        [Given(@"Người dùng can thiệp sửa đổi giá sản phẩm trên HTML thành 1 VND")]
        public void GivenNguoiDungCanThiepSuaDoiGiaSanPhamTrenHTMLThanh1VND()
        {
            GivenNguoiDungThem1SanPhamVaoGioHang();
            _driver.Navigate().GoToUrl(BaseUrl + "Home/ThanhToan");
            WaitForUrlToContain("ThanhToan");
            
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("var spans = document.querySelectorAll('span'); for(var i=0; i<spans.length; i++) { if(spans[i].innerText.indexOf('Subtotal') !== -1 || spans[i].innerText.indexOf('Total') !== -1) { spans[i].nextElementSibling.innerText = '1 đ'; } }");
            Thread.Sleep(1000);
        }

        [When(@"Người dùng điền đầy đủ thông tin giao hàng hợp lệ và bấm đặt hàng")]
        public void WhenNguoiDungDienDayDuThongTinGiaoHangHopLeVaBamDatHang()
        {
            var hoTen = _driver.FindElement(By.Id("HoTen"));
            hoTen.Clear();
            hoTen.SendKeys(GenerateUniqueName());
            
            var email = _driver.FindElement(By.Id("Email"));
            email.Clear();
            email.SendKeys(GenerateUniqueEmail());
            
            var dienThoai = _driver.FindElement(By.Id("DienThoai"));
            dienThoai.Clear();
            dienThoai.SendKeys(GenerateUniquePhone());
            
            var diaChi = _driver.FindElement(By.Id("DiaChi"));
            diaChi.Clear();
            diaChi.SendKeys("Hà Nội");
            
            _driver.FindElement(By.CssSelector("#checkoutForm button[type='submit']")).Click();
        }

        [Then(@"Hệ thống vẫn tạo đơn hàng thành công với giá trị tổng tiền tính theo giá gốc trên database")]
        public void ThenHeThongVanTaoDonHangThanhCongVoiGiaTriTongTienTinhTheoGiaGocTrenDatabase()
        {
            WaitForUrlToContain("DatHangThanhCong", 25);
            string maDon = GetOrderCodeFromUrlOrDb();
            Assert.IsFalse(string.IsNullOrEmpty(maDon), "Không lấy được mã đơn hàng từ URL hoặc Database.");
            _scenarioContext["LastCreatedOrderCode"] = maDon;
            
            decimal expectedPrice = (decimal)_scenarioContext["ProductPrice"];
            
            string connString = "Server=localhost;Database=QL_BANHANG_TRUCTUYEN;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var conn = new System.Data.SqlClient.SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TONGTIEN FROM DONHANG WHERE MADH_MAHOA = @code";
                    cmd.Parameters.AddWithValue("@code", maDon);
                    var val = cmd.ExecuteScalar();
                    Assert.IsNotNull(val, "Không tìm thấy đơn hàng vừa đặt trong database.");
                    decimal actualPriceInDB = Convert.ToDecimal(val);
                    Assert.AreEqual(expectedPrice, actualPriceInDB, "Giá bán của đơn hàng bị thay đổi theo Client-side.");
                }
            }
        }

        // --- TC_FUN_07 ---
        [Given(@"Người dùng đăng nhập và đã có một đơn hàng chờ xác nhận")]
        public void GivenNguoiDungDangNhapVaDaCoVotDonHangChoXacNhan()
        {
            GivenNguoiDungDienDayDuThongTinGiaoHangHopLe();
            GivenNguoiDungChonPhuongThucThanhToanLa("COD");
            WhenNguoiDungBamDatHang();
            ThenHeThongChuyenHuongSangTrangDatHangThanhCongVaHienThiMaDonHangDangHD();
            
            string maDon = (string)_scenarioContext["LastCreatedOrderCode"];
            Assert.IsFalse(string.IsNullOrEmpty(maDon));
        }

        [Given(@"Người dùng truy cập trang chi tiết đơn hàng đó")]
        public void GivenNguoiDungTruyCapTrangChiTietDonHangDo()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _driver.Navigate().GoToUrl(BaseUrl + "Home/DonHang");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(), 'Đơn hàng')] | //h3[contains(text(), 'Đơn hàng')] | //th[text()='Mã đơn hàng']")));
            
            string maDon = (string)_scenarioContext["LastCreatedOrderCode"];
            
            int orderId = 0;
            string connString = "Server=localhost;Database=QL_BANHANG_TRUCTUYEN;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var conn = new System.Data.SqlClient.SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT MADH FROM DONHANG WHERE MADH_MAHOA = @code";
                    cmd.Parameters.AddWithValue("@code", maDon);
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            
            _driver.Navigate().GoToUrl(BaseUrl + "Home/ChiTietDonHang?id=" + orderId);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath($"//strong[contains(text(), '{maDon}')]")));
        }

        [When(@"Người dùng bấm nút Hủy đơn hàng và xác nhận")]
        public void WhenNguoiDungBamNutHuyDonHangVaXacNhan()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("window.confirm = function() { return true; };");
            
            var cancelBtn = _driver.FindElement(By.CssSelector("form[action*='HuyDonHang'] button.btn-danger"));
            js.ExecuteScript("arguments[0].click();", cancelBtn);
        }

        [Then(@"Trạng thái của đơn hàng chuyển sang hiển thị là ""Đã hủy""")]
        public void ThenTrangThaiCuaDonHangChuyenSangHienThiLaDaHuy()
        {
            Thread.Sleep(3000);
            var pageSource = _driver.PageSource;
            Assert.IsTrue(pageSource.Contains("Đã hủy") || pageSource.Contains("DA HUY"), "Trạng thái đơn hàng không đổi thành 'Đã hủy' trên UI.");
        }

        // --- TC_FUN_08 ---
        [Given(@"Người dùng để trống trường ""Họ và tên"" trên Form thanh toán")]
        public void GivenNguoiDungDeTrongTruongHoVaTenTrenFormThanhToan()
        {
            PerformLoginAndNavigateToCheckout();
            _driver.FindElement(By.Id("HoTen")).Clear();
            
            var email = _driver.FindElement(By.Id("Email"));
            email.Clear();
            email.SendKeys(GenerateUniqueEmail());
            
            var dienThoai = _driver.FindElement(By.Id("DienThoai"));
            dienThoai.Clear();
            dienThoai.SendKeys(GenerateUniquePhone());
            
            var diaChi = _driver.FindElement(By.Id("DiaChi"));
            diaChi.Clear();
            diaChi.SendKeys("Hà Nội");
        }

        [Then(@"HTML5 yêu cầu điền thông tin hiển thị thông báo lỗi tại trường ""Họ và tên""")]
        public void ThenHTML5YeuCauDienThongTinHienThiThongBaoLoiTaiTruongHoVaTen()
        {
            var hoTenInput = _driver.FindElement(By.Id("HoTen"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            
            string validationMsg = (string)js.ExecuteScript("return arguments[0].validationMessage;", hoTenInput);
            
            Assert.IsFalse(string.IsNullOrEmpty(validationMsg), "Không có thông báo lỗi HTML5 validationMessage.");
            Assert.IsTrue(validationMsg.Length > 0, "Thông báo lỗi HTML5 bị trống.");
        }

        private string GetOrderCodeFromUrlOrDb()
        {
            string url = _driver.Url;
            
            // 1. Try query param id=
            int idIdx = url.IndexOf("id=");
            if (idIdx != -1)
            {
                string rawId = url.Substring(idIdx + 3);
                int ampIdx = rawId.IndexOf('&');
                if (ampIdx != -1)
                {
                    rawId = rawId.Substring(0, ampIdx);
                }
                if (rawId.StartsWith("HD") || rawId.StartsWith("DH"))
                {
                    return rawId.Trim();
                }
            }

            // 2. Try URL path segments
            string[] segments = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0)
            {
                string lastSeg = segments[segments.Length - 1];
                int qMarkIdx = lastSeg.IndexOf('?');
                if (qMarkIdx != -1)
                {
                    lastSeg = lastSeg.Substring(0, qMarkIdx);
                }
                if (lastSeg.StartsWith("HD") || lastSeg.StartsWith("DH"))
                {
                    return lastSeg.Trim();
                }
            }

            // 3. Fallback to SQL Database
            string connString = "Server=localhost;Database=QL_BANHANG_TRUCTUYEN;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var conn = new System.Data.SqlClient.SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 MADH_MAHOA FROM DONHANG ORDER BY NGAYDAT DESC, MADH DESC";
                    object res = cmd.ExecuteScalar();
                    if (res != null)
                    {
                        return res.ToString().Trim();
                    }
                }
            }
            return "";
        }

        private static string GenerateUniquePhone()
        {
            Random rand = new Random();
            string digits = "";
            for (int i = 0; i < 8; i++)
            {
                digits += rand.Next(0, 10).ToString();
            }
            return "09" + digits;
        }

        private static string GenerateUniqueEmail()
        {
            return $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@gmail.com";
        }

        private static string GenerateUniqueName()
        {
            return "Khach Hang " + Guid.NewGuid().ToString().Substring(0, 5);
        }
    }
}
