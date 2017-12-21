#===========================================#
#				DOTNET	BUILD				#
#===========================================#
FROM microsoft/aspnetcore-build:2-jessie as dotnet-build
COPY *.csproj /build/
WORKDIR /build
RUN dotnet restore
COPY . .
ARG DOTNET_CONFIG=Release
RUN dotnet publish -c ${DOTNET_CONFIG} -o ./results

#===========================================#
#				IMAGE	BUILD				#
#===========================================#
FROM microsoft/aspnetcore:2-jessie as image
ARG INSTALL_CLRDBG
RUN bash -c "${INSTALL_CLRDBG}"
WORKDIR /app
EXPOSE 80
COPY --from="dotnet-build" /build/results .
ENTRYPOINT ["dotnet", "MidnightLizard.Schemes.Processor.dll"]
