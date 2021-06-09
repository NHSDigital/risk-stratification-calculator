#!/bin/bash
set -e

declare connectionString="Server=localhost;User Id=docker;Password=docker;Database=postgres";
declare directory="binary";

echo "=========== STARTING postgres =============="
    service postgresql start
echo "postgres service started..."
echo "============================================"

echo "============= STARTING dotnet =============="
    dotnet NHSD.RiskStratification.Calculator.TownsendIndexConverter.dll "${connectionString}" "${directory}"
echo "TownsendIndexConverter completed successfully..."
echo "directory for [townsend binary index files] has been created at [/app/${directory}]"
echo "============================================"
