# FarmIdle
Thiết kế tách layer: dễ maintain, dễ test
UI sample chỉ là 1 adapter, dễ thay thế bằng UI khác
Data config dùng CSV, dễ để game designer cập nhật
Có xử lý thời gian chính xác, kể cả khi app tắt
Hạn chế dùng singleton global, để dễ test và tránh coupling
