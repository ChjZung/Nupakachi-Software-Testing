Feature: Bao cao thong ke

Scenario: TC31 Mo trang bao cao thong ke
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	Then Trang Bao cao thong ke hien thi thanh cong

Scenario: TC32 Hien thi bao cao doanh thu theo thang
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	And Chon khoang thoi gian hop le
	And Click Xem bao cao
	Then Du lieu doanh thu duoc hien thi

Scenario: TC33 Loc bao cao theo ngay hop le
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	And Nhap ngay hop le
	And Click Xem bao cao
	Then He thong hien thi du lieu

Scenario: TC34 Ngay bat dau lon hon ngay ket thuc
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	And Nhap FromDate lon hon ToDate
	And Click Xem bao cao
	Then He thong hien thi thong bao loi

Scenario: TC35 Chon ngay tuong lai
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	And Nhap ngay trong tuong lai
	And Click Xem bao cao
	Then He thong van hoat dong

Scenario: TC43 Tai du lieu lon
	Given Dang nhap voi tai khoan Admin
	When Truy cap trang Bao cao thong ke
	And Chon khoang thoi gian lon
	And Click Xem bao cao
	Then Bao cao duoc tai thanh cong

Scenario: TC45 User chua dang nhap truy cap bao cao
	Given Chua dang nhap he thong
	When Truy cap truc tiep trang Bao cao
	Then He thong chuyen ve trang Dang nhap