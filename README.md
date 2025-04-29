# Simba POC Project

This project demonstrates the use of a .NET application with Simba ODBC drivers in a Dockerized environment. We try to show the difference in behavior of the two versions. Version 2.8.0 does not result in out-of-memory whereas 2.8.2 results in the container going down due to memory restrictions.

## Building the Docker Image

You can build the Docker image using two different versions of the Simba ODBC driver: 2.8.0 and 2.8.2. Follow the steps below for each version.

### Prerequisites

- Docker must be installed on your system.
- The appropriate Simba ODBC `.deb` package (e.g., `simbaspark_2.8.0_amd64.deb` or `simbaspark_2.8.2_amd64.deb`) must be available in the project root directory.

### Steps to Build

1. Open a terminal and navigate to the project root directory.

2. Use the following commands to build the Docker image:

#### For Simba ODBC Driver Version 2.8.0

```bash
docker build --platform=linux/amd64 --build-arg SIMBA_PACKAGE=simbaspark_2.8.0_amd64.deb -t simba-poc:2.8.0 .
```

#### For Simba ODBC Driver Version 2.8.2

```bash
docker build --platform=linux/amd64 --build-arg SIMBA_PACKAGE=simbaspark_2.8.2_amd64.deb -t simba-poc:2.8.2 .
```

### Notes

- Ensure that the `.deb` package specified in the `SIMBA_PACKAGE` build argument matches the version you want to use.
- The Dockerfile is configured to copy the `.deb` package and install it during the build process.

## Running the Application

After building the Docker image, you can run the application using the following command:

```bash
docker run --platform=linux/amd64 -p 8080:8080 simba-poc:<version>
```

Replace `<version>` with the appropriate tag (e.g., `2.8.0` or `2.8.2`).

### Note

The application within the container is hosted on port 8080.

## Configuring the ODBC.ini File

Ensure that the `odbc.ini` file is configured with the correct values for your Databricks instance. This includes setting the appropriate server hostname, port, and authentication details. Incorrect configuration may result in connection failures.

## Expected Outcome

When downloading the file from `http://localhost:8080/lineitems`, the container should not crash, and the output file should be created as a valid JSON file.