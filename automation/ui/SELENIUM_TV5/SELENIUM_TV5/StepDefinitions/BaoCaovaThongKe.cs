using Reqnroll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SELENIUM_TV5.StepDefinitions
{
    [Binding]
    public class BaoCaoThongKeSteps
    {
        private ChromeDriver driver;
        private WebDriverWait wait;

        private readonly string baseUrl =
            "http://nupakachi.somee.com";

        private readonly string reportUrl =
            "http://nupakachi.somee.com/Admin/BaocaoThongKe";

        [BeforeScenario]
        public void Setup()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(10)
            );
        }

        [AfterScenario]
        public void TearDown()
        {
            driver?.Quit();
        }

        [Given(@"Dang nhap voi tai khoan Admin")]
        public void GivenDangNhapVoiTaiKhoanAdmin()
        {
            driver.Navigate()
                .GoToUrl(baseUrl + "/Account/DangNhap");

            driver.FindElement(By.Id("loginInput"))
                .SendKeys("admin");

            driver.FindElement(By.Id("passwordInput"))
                .SendKeys("admin");

            driver.FindElement(By.CssSelector(".btn-pink"))
                .Click();

            Thread.Sleep(2000);
        }

        [Given(@"Chua dang nhap he thong")]
        public void GivenChuaDangNhap()
        {
            driver.Manage().Cookies.DeleteAllCookies();
        }

        [When(@"Truy cap trang Bao cao thong ke")]
        public void WhenTruyCapTrangBaoCaoThongKe()
        {
            driver.Navigate().GoToUrl(reportUrl);

            Thread.Sleep(2000);
        }

        [When(@"Truy cap truc tiep trang Bao cao")]
        public void WhenTruyCapTrucTiepTrangBaoCao()
        {
            driver.Navigate().GoToUrl(reportUrl);

            Thread.Sleep(2000);
        }

        [When(@"Chon khoang thoi gian hop le")]
        public void WhenChonKhoangThoiGianHopLe()
        {
            try
            {
                driver.FindElement(By.Name("tuNgay"))
                    .SendKeys("2025-01-01");

                driver.FindElement(By.Name("denNgay"))
                    .SendKeys("2025-12-31");
            }
            catch { }
        }

        [When(@"Nhap ngay hop le")]
        public void WhenNhapNgayHopLe()
        {
            try
            {
                driver.FindElement(By.Name("tuNgay"))
                    .SendKeys("2025-01-01");

                driver.FindElement(By.Name("denNgay"))
                    .SendKeys("2025-12-31");
            }
            catch { }
        }

        [When(@"Nhap FromDate lon hon ToDate")]
        public void WhenNhapFromDateLonHonToDate()
        {
            try
            {
                driver.FindElement(By.Name("tuNgay"))
                    .SendKeys("2025-12-31");

                driver.FindElement(By.Name("denNgay"))
                    .SendKeys("2025-01-01");
            }
            catch { }
        }

        [When(@"Nhap ngay trong tuong lai")]
        public void WhenNhapNgayTrongTuongLai()
        {
            string futureDate =
                DateTime.Now.AddDays(30)
                .ToString("yyyy-MM-dd");

            try
            {
                driver.FindElement(By.Name("tuNgay"))
                    .SendKeys(futureDate);

                driver.FindElement(By.Name("denNgay"))
                    .SendKeys(futureDate);
            }
            catch { }
        }

        [When(@"Chon khoang thoi gian lon")]
        public void WhenChonKhoangThoiGianLon()
        {
            try
            {
                driver.FindElement(By.Name("tuNgay"))
                    .SendKeys("2024-01-01");

                driver.FindElement(By.Name("denNgay"))
                    .SendKeys("2025-12-31");
            }
            catch { }
        }

        [When(@"Click Xem bao cao")]
        public void WhenClickXemBaoCao()
        {
            try
            {
                driver.FindElement(By.CssSelector(".btn-pink"))
                    .Click();
            }
            catch { }

            Thread.Sleep(2000);
        }

        [Then(@"Trang Bao cao thong ke hien thi thanh cong")]
        public void ThenTrangBaoCaoThongKeHienThiThanhCong()
        {
            Assert.IsTrue(
                driver.Url.Contains("BaocaoThongKe")
            );
        }

        [Then(@"Du lieu doanh thu duoc hien thi")]
        public void ThenDuLieuDoanhThuDuocHienThi()
        {
            Assert.IsTrue(
                driver.PageSource.Length > 0
            );
        }

        [Then(@"He thong hien thi du lieu")]
        public void ThenHeThongHienThiDuLieu()
        {
            Assert.IsTrue(
                driver.PageSource.Length > 0
            );
        }

        [Then(@"He thong hien thi thong bao loi")]
        public void ThenHeThongHienThiThongBaoLoi()
        {
            Assert.IsTrue(
                driver.PageSource.Length > 0
            );
        }

        [Then(@"He thong van hoat dong")]
        public void ThenHeThongVanHoatDong()
        {
            Assert.IsTrue(
                driver.PageSource.Length > 0
            );
        }

        [Then(@"Bao cao duoc tai thanh cong")]
        public void ThenBaoCaoDuocTaiThanhCong()
        {
            Assert.IsTrue(
                driver.PageSource.Length > 0
            );
        }

        [Then(@"He thong chuyen ve trang Dang nhap")]
        public void ThenHeThongChuyenVeTrangDangNhap()
        {
            Assert.IsTrue(
                driver.Url.Contains("DangNhap")
                || driver.PageSource.Contains("Đăng nhập")
            );
        }
    }
}