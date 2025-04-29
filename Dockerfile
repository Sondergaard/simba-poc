# Use a .NET SDK base image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Define build argument with default value
ARG SIMBA_PACKAGE

# Install dependencies
RUN apt-get update && apt-get install -y \
    unixodbc \
    odbcinst \
    libodbc1 \ 
    libsasl2-modules-gssapi-mit \
    && rm -rf /var/lib/apt/lists/*

# Copy the simbaspark_2.9.1.1001-2_amd64.deb package into the container
COPY *.deb /tmp/

# # Install the .deb package
RUN dpkg -i /tmp/${SIMBA_PACKAGE} || apt-get -f install -y

# # Configure ODBC
COPY odbcinst.ini /etc/odbcinst.ini
COPY odbc.ini /etc/odbc.ini

# # Set environment variables
ENV LD_LIBRARY_PATH=/opt/simba/spark/lib/64:/usr/lib:/usr/local/lib

# Optional: Set the ODBC config file locations
ENV ODBCINI=/etc/odbc.ini
ENV ODBCSYSINI=/etc

# Copy and build the .NET application
WORKDIR /app
COPY ./src/SimbaNetApi .
RUN dotnet publish -c Release -o out

# Run the application
CMD ["dotnet", "out/SimbaNetApi.dll"]