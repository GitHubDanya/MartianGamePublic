#1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src
COPY ["ServerAlphaWebsite/ServerAlphaWebsite.csproj", "ServerAlphaWebsite/"]
RUN dotnet restore "ServerAlphaWebsite/ServerAlphaWebsite.csproj"
COPY . .
WORKDIR "/src/ServerAlphaWebsite"
RUN dotnet publish -c Release -o /app/publish

#2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy
workdir /app

#3 - Install Python dependencies
RUN apt-get update && apt-get install -y \
	software-properties-common \
	&& add-apt-repository ppa:deadsnakes/ppa \
	&& apt-get update && apt-get install -y \
	python3.12 \
	python3.12-dev \
	python3.12-venv \
	&& rm -rf /var/lib/apt/lists/*

RUN python3.12 -m ensurepip --upgrade \
	&& python3.12 -m pip install --upgrade pip

RUN python3.12 -m pip install openai

#4 - Copy published .NET files
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServerAlphaWebsite.dll"]
