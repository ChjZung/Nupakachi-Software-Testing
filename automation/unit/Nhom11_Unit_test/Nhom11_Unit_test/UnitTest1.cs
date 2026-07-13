using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nhom11_Unit_test
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class OrderDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class InventoryResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class ReportRow
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class ExportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
    }

    public interface IReportRepository
    {
        List<Order> GetOrders(DateTime fromDate, DateTime toDate);
    }

    public class RevenueService
    {
        public decimal CalculateTotalRevenue(List<Order> orders)
        {
            if (orders == null || orders.Count == 0)
                return 0;

            return orders
                .Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Delivered)
                .Sum(o => o.TotalAmount);
        }
    }

    public class StatisticsService
    {
        public int CountOrders(List<Order> orders)
        {
            return orders == null ? 0 : orders.Count;
        }

        public Dictionary<OrderStatus, int> CountOrdersByStatus(List<Order> orders)
        {
            return orders
                .GroupBy(o => o.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public List<OrderDetail> GetTopSellingProducts(List<OrderDetail> orderDetails, int top = 5)
        {
            return orderDetails
                .GroupBy(d => new { d.ProductId, d.ProductName })
                .Select(g => new OrderDetail
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .ThenBy(x => x.ProductId)
                .Take(top)
                .ToList();
        }

        public List<InventoryResult> GetInventoryStatistics(List<Product> products)
        {
            return products.Select(p => new InventoryResult
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                StockQuantity = p.StockQuantity
            }).ToList();
        }

        public void ValidateInventory(Product product)
        {
            if (product.StockQuantity < 0)
                throw new ArgumentException("Số lượng tồn kho phải >= 0");
        }
    }

    public class ReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository = null)
        {
            _repository = repository;
        }

        public bool ValidateDateRange(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
                throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

            if (toDate.Date > DateTime.Today)
                throw new ArgumentException("Không được chọn ngày sau ngày hiện tại");

            return true;
        }

        public List<Order> GenerateRevenueReport(DateTime fromDate, DateTime toDate)
        {
            if (_repository == null)
                return new List<Order>();

            try
            {
                return _repository.GetOrders(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Không thể tạo báo cáo doanh thu do lỗi cơ sở dữ liệu",
                    ex
                );
            }
        }

        public ExportResult ExportReport(List<ReportRow> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new ExportResult
                {
                    Success = true,
                    Message = "Không có dữ liệu, file chỉ có header",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Name,Value\n")
                };
            }

            return new ExportResult
            {
                Success = true,
                Message = "Xuất báo cáo thành công",
                FileContent = System.Text.Encoding.UTF8.GetBytes(
                    "Name,Value\n" +
                    string.Join("\n", rows.Select(r => $"{r.Name},{r.Value}"))
                )
            };
        }
    }

    public class FakeErrorReportRepository : IReportRepository
    {
        public List<Order> GetOrders(DateTime fromDate, DateTime toDate)
        {
            throw new Exception("Database timeout");
        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TC01_TinhTongDoanhThu_DonHangHopLe_TraVeDungTongTien()
        {
            var service = new RevenueService();

            var orders = new List<Order>
            {
                new Order { Id = 1, TotalAmount = 500000, Status = OrderStatus.Paid },
                new Order { Id = 2, TotalAmount = 300000, Status = OrderStatus.Delivered }
            };

            var result = service.CalculateTotalRevenue(orders);

            Assert.AreEqual(800000, result);
        }

        [TestMethod]
        public void TC02_TinhTongDoanhThu_CoDonHangDaHuy_KhongTinhDonDaHuy()
        {
            var service = new RevenueService();

            var orders = new List<Order>
            {
                new Order { Id = 1, TotalAmount = 500000, Status = OrderStatus.Paid },
                new Order { Id = 2, TotalAmount = 200000, Status = OrderStatus.Cancelled }
            };

            var result = service.CalculateTotalRevenue(orders);

            Assert.AreEqual(500000, result);
        }

        [TestMethod]
        public void TC03_TinhTongDoanhThu_KhongCoDuLieu_TraVe0()
        {
            var service = new RevenueService();
            var orders = new List<Order>();

            var result = service.CalculateTotalRevenue(orders);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TC04_KiemTraKhoangNgay_HopLe_TraVeTrue()
        {
            var service = new ReportService();

            var fromDate = new DateTime(2026, 5, 1);
            var toDate = new DateTime(2026, 5, 10);

            var result = service.ValidateDateRange(fromDate, toDate);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TC05_KiemTraKhoangNgay_NgayBatDauLonHonNgayKetThuc_BaoLoi()
        {
            var service = new ReportService();

            var fromDate = new DateTime(2026, 5, 30);
            var toDate = new DateTime(2026, 5, 1);

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                service.ValidateDateRange(fromDate, toDate)
            );

            StringAssert.Contains(exception.Message, "Ngày bắt đầu");
        }

        [TestMethod]
        public void TC06_KiemTraKhoangNgay_ChonNgayTuongLai_BaoLoi()
        {
            var service = new ReportService();

            var fromDate = DateTime.Today;
            var toDate = DateTime.Today.AddDays(1);

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                service.ValidateDateRange(fromDate, toDate)
            );

            StringAssert.Contains(exception.Message, "ngày sau ngày hiện tại");
        }

        [TestMethod]
        public void TC07_DemSoDonHang_CoDanhSachDonHang_TraVeDungSoLuong()
        {
            var service = new StatisticsService();

            var orders = new List<Order>
            {
                new Order { Id = 1, Status = OrderStatus.Pending },
                new Order { Id = 2, Status = OrderStatus.Paid },
                new Order { Id = 3, Status = OrderStatus.Cancelled }
            };

            var result = service.CountOrders(orders);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TC08_ThongKeDonHangTheoTrangThai_TraVeDungSoLuongTungTrangThai()
        {
            var service = new StatisticsService();

            var orders = new List<Order>
            {
                new Order { Id = 1, Status = OrderStatus.Pending },
                new Order { Id = 2, Status = OrderStatus.Delivered },
                new Order { Id = 3, Status = OrderStatus.Delivered },
                new Order { Id = 4, Status = OrderStatus.Cancelled }
            };

            var result = service.CountOrdersByStatus(orders);

            Assert.AreEqual(1, result[OrderStatus.Pending]);
            Assert.AreEqual(2, result[OrderStatus.Delivered]);
            Assert.AreEqual(1, result[OrderStatus.Cancelled]);
        }

        [TestMethod]
        public void TC09_LaySanPhamBanChay_DuLieuHopLe_TraVeSanPhamBanNhieuNhat()
        {
            var service = new StatisticsService();

            var details = new List<OrderDetail>
            {
                new OrderDetail { ProductId = 1, ProductName = "Áo thun", Quantity = 5 },
                new OrderDetail { ProductId = 2, ProductName = "Quần jean", Quantity = 12 },
                new OrderDetail { ProductId = 3, ProductName = "Váy", Quantity = 7 }
            };

            var result = service.GetTopSellingProducts(details, 3);

            Assert.AreEqual("Quần jean", result[0].ProductName);
            Assert.AreEqual(12, result[0].Quantity);
        }

        [TestMethod]
        public void TC10_LaySanPhamBanChay_SoLuongBangNhau_TraVeDanhSachHopLe()
        {
            var service = new StatisticsService();

            var details = new List<OrderDetail>
            {
                new OrderDetail { ProductId = 2, ProductName = "Quần jean", Quantity = 10 },
                new OrderDetail { ProductId = 1, ProductName = "Áo thun", Quantity = 10 }
            };

            var result = service.GetTopSellingProducts(details, 2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(10, result[0].Quantity);
            Assert.AreEqual(10, result[1].Quantity);
        }

        [TestMethod]
        public void TC11_ThongKeTonKho_DuLieuHopLe_TraVeDungSoLuongTon()
        {
            var service = new StatisticsService();

            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Áo sơ mi", StockQuantity = 20 },
                new Product { ProductId = 2, ProductName = "Chân váy", StockQuantity = 15 }
            };

            var result = service.GetInventoryStatistics(products);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(item => item.StockQuantity >= 0));
            Assert.AreEqual(20, result.First(x => x.ProductId == 1).StockQuantity);
        }

        [TestMethod]
        public void TC12_KiemTraTonKho_SoLuongAm_BaoLoi()
        {
            var service = new StatisticsService();

            var product = new Product
            {
                ProductId = 1,
                ProductName = "Áo khoác",
                StockQuantity = -1
            };

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                service.ValidateInventory(product)
            );

            StringAssert.Contains(exception.Message, ">= 0");
        }

        [TestMethod]
        public void TC13_TaoBaoCaoDoanhThu_LoiCoSoDuLieu_TraVeLoiDeHieu()
        {
            var repository = new FakeErrorReportRepository();
            var service = new ReportService(repository);

            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                service.GenerateRevenueReport(
                    new DateTime(2026, 5, 1),
                    new DateTime(2026, 5, 10)
                )
            );

            StringAssert.Contains(exception.Message, "Không thể tạo báo cáo");
            Assert.IsNotNull(exception.InnerException);
        }

        [TestMethod]
        public void TC14_TinhTongDoanhThu_DuLieuLon_TraVeDungVaKhongQuaCham()
        {
            var service = new RevenueService();

            var orders = Enumerable.Range(1, 10000)
                .Select(i => new Order
                {
                    Id = i,
                    TotalAmount = 100000,
                    Status = OrderStatus.Paid
                })
                .ToList();

            var stopwatch = Stopwatch.StartNew();

            var result = service.CalculateTotalRevenue(orders);

            stopwatch.Stop();

            Assert.AreEqual(1000000000, result);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 3000);
        }

        [TestMethod]
        public void TC15_XuatBaoCao_KhongCoDuLieu_TraVeFileChiCoHeader()
        {
            var service = new ReportService();

            var rows = new List<ReportRow>();

            var result = service.ExportReport(rows);

            Assert.IsTrue(result.Success);
            StringAssert.Contains(result.Message, "Không có dữ liệu");
            Assert.IsTrue(result.FileContent.Length > 0);
        }
    }
}