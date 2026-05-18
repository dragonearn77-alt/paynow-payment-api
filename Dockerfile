# 1. 強制指定下載給 Intel 晶片專用的 linux/amd64 版本
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 2. 把專案檔案複製進去，並下載零件 (Restore)
COPY *.csproj ./
RUN dotnet restore

# 3. 複製所有程式碼，並打包發布成最輕量的執行檔
COPY . ./
RUN dotnet publish -c Release -o out

# 4. 執行環境也同樣強制對齊 amd64
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# 5. 告訴貨櫃：對外開啟 5275 連接埠
EXPOSE 5275
ENV ASPNETCORE_URLS=http://+:5275
ENTRYPOINT ["dotnet", "asp_csharp.dll"]