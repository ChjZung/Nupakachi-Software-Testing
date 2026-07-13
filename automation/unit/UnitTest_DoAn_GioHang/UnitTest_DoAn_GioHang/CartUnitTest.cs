using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest_DoAn_GioHang
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CartService
    {
        public List<CartItem> AddToCart(List<CartItem> cart, CartItem item)
        {
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            if (item.StockQuantity > 0 && item.Quantity > item.StockQuantity)
                throw new ArgumentException("Quantity exceeds stock");

            var existing = cart.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (existing != null)
            {
                if (item.StockQuantity > 0 && existing.Quantity + item.Quantity > item.StockQuantity)
                    throw new ArgumentException("Total quantity exceeds stock");

                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            return cart;
        }

        public List<CartItem> RemoveFromCart(List<CartItem> cart, int productId)
        {
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
            }

            return cart;
        }

        public List<CartItem> UpdateQuantity(List<CartItem> cart, int productId, int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                throw new ArgumentException("Product not found in cart");

            if (item.StockQuantity > 0 && newQuantity > item.StockQuantity)
                throw new ArgumentException("Quantity exceeds stock");

            item.Quantity = newQuantity;
            return cart;
        }

        public int GetTotalQuantity(List<CartItem> cart)
        {
            if (cart == null || cart.Count == 0)
                return 0;

            return cart.Sum(x => x.Quantity);
        }

        public decimal GetTotalPrice(List<CartItem> cart)
        {
            if (cart == null || cart.Count == 0)
                return 0;

            return cart.Sum(x => x.Quantity * x.Price);
        }

        public bool IsCartEmpty(List<CartItem> cart)
        {
            return cart == null || cart.Count == 0;
        }

        public void ClearCart(List<CartItem> cart)
        {
            cart.Clear();
        }

        public void ValidateStock(CartItem item)
        {
            if (item.StockQuantity <= 0)
                throw new ArgumentException("Product is out of stock");
        }

        public List<CartItem> LoadCartFromDatabase()
        {
            throw new Exception("Database connection failed");
        }
    }

    [TestClass]
    public class CartUnitTest
    {
        [TestMethod]
        public void TC01_ThemSanPhamMoiVaoGioHang_ThanhCong()
        {
            var service = new CartService();
            var cart = new List<CartItem>();

            var item = new CartItem
            {
                ProductId = 1,
                Quantity = 1,
                Price = 100000,
                StockQuantity = 10
            };

            var result = service.AddToCart(cart, item);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].ProductId);
            Assert.AreEqual(1, result[0].Quantity);
        }

        [TestMethod]
        public void TC02_ThemSanPhamDaTonTaiTrongGioHang_CongDonSoLuong()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = 1,
                    Quantity = 1,
                    Price = 100000,
                    StockQuantity = 10
                }
            };

            var item = new CartItem
            {
                ProductId = 1,
                Quantity = 2,
                Price = 100000,
                StockQuantity = 10
            };

            var result = service.AddToCart(cart, item);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].ProductId);
            Assert.AreEqual(3, result[0].Quantity);
        }

        [TestMethod]
        public void TC03_XoaSanPhamKhoiGioHang_ThanhCong()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, Price = 100000, StockQuantity = 10 },
                new CartItem { ProductId = 2, Quantity = 1, Price = 200000, StockQuantity = 10 }
            };

            var result = service.RemoveFromCart(cart, 1);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].ProductId);
        }

        [TestMethod]
        public void TC04_CapNhatSoLuongSanPham_ThanhCong()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 1, Price = 100000, StockQuantity = 10 }
            };

            var result = service.UpdateQuantity(cart, 1, 3);

            Assert.AreEqual(3, result[0].Quantity);
        }

        [TestMethod]
        public void TC05_KhongChoSoLuongAm_BaoLoi()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 1, Price = 100000, StockQuantity = 10 }
            };

            var ex = Assert.ThrowsException<ArgumentException>(() =>
                service.UpdateQuantity(cart, 1, -1)
            );

            StringAssert.Contains(ex.Message, "greater than 0");
        }

        [TestMethod]
        public void TC06_KhongChoSoLuongBangKhong_BaoLoi()
        {
            var service = new CartService();

            var cart = new List<CartItem>();

            var item = new CartItem
            {
                ProductId = 1,
                Quantity = 0,
                Price = 100000,
                StockQuantity = 10
            };

            var ex = Assert.ThrowsException<ArgumentException>(() =>
                service.AddToCart(cart, item)
            );

            StringAssert.Contains(ex.Message, "greater than 0");
        }

        [TestMethod]
        public void TC07_KhongChoVuotTonKho_BaoLoi()
        {
            var service = new CartService();

            var cart = new List<CartItem>();

            var item = new CartItem
            {
                ProductId = 1,
                Quantity = 15,
                Price = 100000,
                StockQuantity = 10
            };

            var ex = Assert.ThrowsException<ArgumentException>(() =>
                service.AddToCart(cart, item)
            );

            StringAssert.Contains(ex.Message, "exceeds stock");
        }

        [TestMethod]
        public void TC08_TinhTongTienGioHang_DungKetQua()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, Price = 100000, StockQuantity = 10 },
                new CartItem { ProductId = 2, Quantity = 1, Price = 200000, StockQuantity = 10 }
            };

            var result = service.GetTotalPrice(cart);

            Assert.AreEqual(400000, result);
        }

        [TestMethod]
        public void TC09_TinhTongSoLuongSanPham_DungKetQua()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, Price = 100000, StockQuantity = 10 },
                new CartItem { ProductId = 2, Quantity = 3, Price = 200000, StockQuantity = 10 }
            };

            var result = service.GetTotalQuantity(cart);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TC10_GioHangRong_TraVeTrue()
        {
            var service = new CartService();
            var cart = new List<CartItem>();

            var result = service.IsCartEmpty(cart);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TC11_XoaSanPhamCuoiCung_GioHangTroThanhRong()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 1, Price = 100000, StockQuantity = 10 }
            };

            service.RemoveFromCart(cart, 1);

            Assert.IsTrue(service.IsCartEmpty(cart));
        }

        [TestMethod]
        public void TC12_SanPhamHetHang_BaoLoi()
        {
            var service = new CartService();

            var item = new CartItem
            {
                ProductId = 1,
                Quantity = 1,
                Price = 100000,
                StockQuantity = 0
            };

            var ex = Assert.ThrowsException<ArgumentException>(() =>
                service.ValidateStock(item)
            );

            StringAssert.Contains(ex.Message, "out of stock");
        }

        [TestMethod]
        public void TC13_DatabaseException_KhiTaiGioHang()
        {
            var service = new CartService();

            var ex = Assert.ThrowsException<Exception>(() =>
                service.LoadCartFromDatabase()
            );

            StringAssert.Contains(ex.Message, "Database connection failed");
        }

        [TestMethod]
        public void TC14_DuLieuLon_GioHang100SanPham_TinhTongDung()
        {
            var service = new CartService();
            var cart = new List<CartItem>();

            for (int i = 1; i <= 100; i++)
            {
                cart.Add(new CartItem
                {
                    ProductId = i,
                    Quantity = 1,
                    Price = 10000,
                    StockQuantity = 100
                });
            }

            var totalQuantity = service.GetTotalQuantity(cart);
            var totalPrice = service.GetTotalPrice(cart);

            Assert.AreEqual(100, totalQuantity);
            Assert.AreEqual(1000000, totalPrice);
        }

        [TestMethod]
        public void TC15_CapNhatNhieuLan_TongTienVanChinhXac()
        {
            var service = new CartService();

            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 1, Price = 100000, StockQuantity = 10 }
            };

            service.UpdateQuantity(cart, 1, 2);
            service.UpdateQuantity(cart, 1, 3);
            service.UpdateQuantity(cart, 1, 4);

            var result = service.GetTotalPrice(cart);

            Assert.AreEqual(400000, result);
        }
    }
}