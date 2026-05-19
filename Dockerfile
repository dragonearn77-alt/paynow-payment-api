# 1. 拿取微軟官方的 .NET SDK 引擎來進行編譯
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 2. 複製專案檔並下載零件
COPY ["asp_csharp.csproj", "./"]
RUN dotnet restore "asp_csharp.csproj"

# 3. 複製所有檔案（包含新創的 Models, Data, Controllers 資料夾！）
COPY . .
RUN dotnet build "asp_csharp.csproj" -c Release -o /app/build

# 4. 打包發布成執行檔
FROM build AS publish
RUN dotnet publish "asp_csharp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. 換成最乾淨的執行環境
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5275
ENV ASPNETCORE_URLS=http://+:5275
ENTRYPOINT ["dotnet", "asp_csharp.dll"]