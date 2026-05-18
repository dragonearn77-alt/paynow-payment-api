# 🏦 PayNow 簡易第三方支付金流 API 系統

這是一個專為模擬第三方支付（金流網關）核心邏輯而設計的後端 API 系統。專案採用 **Code-First** 流程開發，並全面整合容器化技術，支援一鍵本地端環境部署。

## 🚀 技術棧 (Tech Stack)

* **後端框架**：.NET 10.0 / ASP.NET Core (Minimal APIs)
* **資料庫 ORM**：Entity Framework Core (EF Core)
* **實體資料庫**：Microsoft SQL Server 2022
* **容器化技術**：Docker / Docker Compose (支援環境編排與跨平台晶片架構對齊)
* **版本控制**：Git / GitHub

---

## 🏗️ 系統架構 (Architecture)

本專案採用 **多容器獨立封裝架構 (Multi-Container Architecture)**：
1.  **`web` 服務容器**：運行 ASP.NET Core 10.0 應用程式，負責處理 HTTP 請求、路由分發與業務邏輯驗證。
2.  **`db` 服務容器**：運行微軟官方 MS SQL Server 2022 鏡像，負責數據的持久化安全存儲。

兩個容器透過 Docker Compose 自動建立的內部虛擬網路進行高效、安全的端到端通訊。

---

## ✨ 核心功能與 API 端點 (Endpoints)

### 1. 模擬刷卡扣款 (POST)
* **URL**: `/api/pay`
* **功能**: 接收前端電商網站的付款請求，內建「防禦性驗證邏輯」（金額必須大於 0 元），驗證通過後自動將交易紀錄寫入 MS SQL 資料庫硬碟。

### 2. 調閱歷史交易帳本 (GET)
* **URL**: `/api/history`
* **功能**: 穿透雙層貨櫃，利用 EF Core 將資料庫內部的所有歷史付款紀錄以 JSON 陣列型態完整吐回。

### 3. 精準單筆訂單查詢 (GET)
* **URL**: `/api/history/{id}`
* **功能**: 根據主鍵 (Id) 精準檢索單筆訂單。若查詢不存在之單號，系統將優雅拋出標準的 **HTTP 404 (Not Found)** 客製化錯誤通知。

---

## 📦 如何在本地端快速運行？

本專案已完成全面的容器化編排，您不需要在本地電腦安裝任何 .NET SDK 或資料庫環境，只需確保電腦已啟動 **Docker Desktop**，並在專案根目錄執行以下單一指令：

```bash
docker-compose up --build