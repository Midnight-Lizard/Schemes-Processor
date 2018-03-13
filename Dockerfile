#===========================================#
#				DOTNET	BUILD				#
#===========================================#
FROM microsoft/aspnetcore-build:2-jessie as dotnet-build
ARG DOTNET_CONFIG=Release
WORKDIR /build
COPY *.sln .
COPY /app/MidnightLizard.Schemes.Processor.csproj ./app/
COPY /domain/MidnightLizard.Schemes.Domain.csproj ./domain/
COPY /infrastructure/MidnightLizard.Schemes.Infrastructure.csproj ./infrastructure/
RUN dotnet restore
COPY . .
RUN dotnet publish app -c ${DOTNET_CONFIG} -o ./results

#===========================================#
#				DOTNET	TEST				#
#===========================================#
FROM microsoft/aspnetcore-build:2-jessie as dotnet-test
WORKDIR /test
COPY --from=dotnet-build /build .
RUN dotnet test -c Test

#===========================================#
#				IMAGE	BUILD				#
#===========================================#
FROM microsoft/aspnetcore:2-jessie as image
ARG INSTALL_CLRDBG
RUN bash -c "${INSTALL_CLRDBG}"
WORKDIR /app
EXPOSE 80
COPY --from=dotnet-build /build/app/results .
ENTRYPOINT ["dotnet", "MidnightLizard.Schemes.Processor.dll"]
