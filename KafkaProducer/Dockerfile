FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update && \
    apt install unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["KafkaProducer/KafkaProducer.csproj", "KafkaProducer/"]
RUN dotnet restore "KafkaProducer/KafkaProducer.csproj"
COPY . .
WORKDIR "/src/KafkaProducer"
RUN dotnet build "KafkaProducer.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "KafkaProducer.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KafkaProducer.dll"]