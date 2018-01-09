#===========================================#
#				DOTNET	BUILD				#
#===========================================#
FROM microsoft/aspnetcore-build:2-jessie as dotnet-build
COPY *.sln /build/
COPY /app/MidnightLizard.Schemes.Processor.csproj /build/app/
COPY /application/MidnightLizard.Schemes.Application.csproj /build/application/
COPY /domain/MidnightLizard.Schemes.Domain.csproj /build/domain/
COPY /infrastructure/MidnightLizard.Schemes.Infrastructure.csproj /build/infrastructure/
WORKDIR /build
RUN dotnet restore
COPY . /build/
ARG DOTNET_CONFIG=Release
RUN dotnet publish app -c ${DOTNET_CONFIG} -o ./results

#===========================================#
#				IMAGE	BUILD				#
#===========================================#
FROM microsoft/aspnetcore:2-jessie as image
ARG INSTALL_CLRDBG
RUN bash -c "${INSTALL_CLRDBG}"
WORKDIR /app
EXPOSE 80
COPY --from="dotnet-build" /build/app/results .
ENTRYPOINT ["dotnet", "MidnightLizard.Schemes.Processor.dll"]
