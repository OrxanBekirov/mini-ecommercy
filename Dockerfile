FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Layihə fayllarını kopyalayırıq və bərpa edirik
COPY . .
RUN dotnet restore "WebApi/WebApi.csproj"

# Proqramı yığırıq (Publish)
RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish

# 2. İşləmə mərhələsi (Runtime Stage) - Əsas bura lazımdır!
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# PostgreSQL-in tələb etdiyi kitabxananı RUNTIME mühitinə yükləyirik
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

# Yığılmış faylları birinci mərhələdən bura kopyalayırıq
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Fayl adını dırnaq içində düzgün yazdığından əmin ol (Məsələn: WebApi.dll)
ENTRYPOINT ["dotnet", "WebApi.dll"]