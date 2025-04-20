# Quy trình phát triển

# Giai đoạn 1: Thiết lập nền tảng
	•	Khởi tạo project Unity, cấu trúc folder rõ ràng.
	•	Khởi tạo các class cơ bản: popup manager, observer pattern, safeArea, DoTween,...
	•	Khởi tạo các lớp đối tượng chính: Crop, Animal, worker,...

# Giai đoạn 2-1: Xây dựng hệ thống trồng trọt
	•	Thiết kế FarmEntity gồm:
	•	Thời gian sinh sản (yield interval)
	•	Số lượng sản phẩm tối đa (max yield)
	•	Thời gian tồn tại sau khi đầy trái
	•	Tạo FarmSlot có thể chứa nhiều FarmEntity.
	•	Viết FarmService để xử lý trồng/thu hoạch từng slot riêng.
	•	Thiết kế hệ thống Inventory để lưu hạt giống và sản phẩm.

# Giai đoạn 3: Hệ thống công nhân và auto
	•	WorkerEntity mỗi công nhân chỉ làm việc cố định với 1 slot.
	•	WorkerService quản lý:
	•	Tick online mỗi 2 phút
	•	Gán việc trồng và thu hoạch tự động
	•	OfflineWork:
	•	Tính toán số lượng job thực hiện được trong thời gian offline
	•	Ưu tiên trồng trước, sau đó thu hoạch
	•	Mô phỏng giống behavior online

# Giai đoạn 4: Giao diện và Popup
	•	Giao diện từng slot đất có:
	•	Tên cây/con
	•	Countdown thời gian thu hoạch
	•	Nút trồng/thu hoạch riêng
	•	Các popup:
	•	Inventory: hiển thị hạt giống + sản phẩm + bán all sản phẩm có trong giỏ hàng(trừ hạt giống)
	•	Shop: mua hạt giống, animal, đất, thuê công nhân
	•	Tools: hiển thị thiết bị và nút nâng cấp

# Giai đoạn 5: Lưu trữ dữ liệu
	•	UserData lưu toàn bộ trạng thái game
	•	Dùng JSON + FileManager để đọc/ghi local file
	•	Tự động lưu mỗi 40s
	•	Khi tắt game sẽ lưu LastExitTime để xử lý offline

# Giai đoạn 6: Tối ưu offline giống như online
	•	Mỗi công nhân chỉ xử lý một slot cố định.
	•	Ưu tiên trồng trước, sau đó thu hoạch liên tục trong thời gian offline.
	•	Cây không bị chết nếu có công nhân chăm sóc, tương tự nếu không có thì sẽ bị héo khi quá thời gian 1h
