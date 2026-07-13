Feature: Checkout Functional Tests
  Kiểm thử các chức năng liên quan đến việc Đặt hàng (Checkout) trên trang bán hàng Nhom04_CongNghePhanMem.

  Scenario: TC_FUN_01 - Đặt hàng thành công với thông tin hợp lệ
    Given Người dùng điền đầy đủ thông tin giao hàng hợp lệ
    And Người dùng chọn phương thức thanh toán là "COD"
    When Người dùng bấm đặt hàng
    Then Hệ thống chuyển hướng sang trang "Đặt hàng thành công" và hiển thị mã đơn hàng dạng "HD"

  Scenario: TC_FUN_02 - Xóa giỏ hàng sau thanh toán thành công
    Given Người dùng đã tạo đơn hàng thành công
    When Người dùng quay về trang chủ và vào lại Giỏ hàng
    Then Giỏ hàng hiển thị trống với thông báo giỏ hàng trống

  Scenario: TC_FUN_03 - Mua sản phẩm vượt quá số lượng tồn kho
    Given Người dùng chọn mua một sản phẩm với số lượng lớn hơn số lượng tồn kho
    When Người dùng bấm tiến hành đặt hàng
    Then Hệ thống chuyển hướng người dùng về trang Giỏ hàng để điều chỉnh số lượng

  Scenario: TC_FUN_04 - Hiển thị chính xác giá trị tạm tính (Subtotal) cho sản phẩm trong giỏ hàng
    Given Người dùng thêm 1 sản phẩm vào giỏ hàng với số lượng là 2
    When Người dùng mở màn hình thanh toán
    Then Dòng tạm tính hiển thị chính xác bằng giá bán của sản phẩm nhân với 2

  Scenario: TC_FUN_05 - Kiểm tra hiển thị mặc định của giảm giá và phí vận chuyển
    Given Người dùng thêm 1 sản phẩm vào giỏ hàng
    When Người dùng mở màn hình thanh toán
    Then Dòng giảm giá mặc định hiển thị "-0 đ" và phí vận chuyển mặc định hiển thị "0 đ"
    And Tổng tiền hiển thị bằng đúng dòng tạm tính

  Scenario: TC_FUN_06 - Ràng buộc giá bán ngăn chặn can thiệp phía client
    Given Người dùng can thiệp sửa đổi giá sản phẩm trên HTML thành 1 VND
    When Người dùng điền đầy đủ thông tin giao hàng hợp lệ và bấm đặt hàng
    Then Hệ thống vẫn tạo đơn hàng thành công với giá trị tổng tiền tính theo giá gốc trên database

  Scenario: TC_FUN_07 - Hủy đơn hàng thành công từ trang chi tiết đơn hàng
    Given Người dùng đăng nhập và đã có một đơn hàng chờ xác nhận
    And Người dùng truy cập trang chi tiết đơn hàng đó
    When Người dùng bấm nút Hủy đơn hàng và xác nhận
    Then Trạng thái của đơn hàng chuyển sang hiển thị là "Đã hủy"

  Scenario: TC_FUN_08 - Kiểm tra ràng buộc trường bắt buộc trên Form thanh toán
    Given Người dùng để trống trường "Họ và tên" trên Form thanh toán
    When Người dùng bấm đặt hàng
    Then HTML5 yêu cầu điền thông tin hiển thị thông báo lỗi tại trường "Họ và tên"
