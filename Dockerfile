# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM dhi.io/dotnet:10-sdk AS build
ARG TARGETARCH
COPY . /source
WORKDIR /source/src/Api
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

FROM dhi.io/aspnetcore:10 AS final
ENV ConnectionStrings:Database="Host=db;Port=5432;Database=dev;Username=DB_USER;Password=DB_PASSWORD"
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Api.dll"]
