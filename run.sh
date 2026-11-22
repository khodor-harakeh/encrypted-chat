#!/usr/bin/env bash
set -e

REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

function show_usage() {
    echo "====================================================="
    echo "  Encrypted TCP Chat Application - Runner Script"
    echo "====================================================="
    echo "Usage: ./run.sh [server|client|test|sniff]"
    echo ""
    echo "Commands:"
    echo "  server   - Build and start the C# .NET 8 Server"
    echo "  client   - Start the Python 3 Client"
    echo "  test     - Run the standalone protocol verification tests"
    echo "  sniff    - Run tcpdump to inspect encrypted traffic (requires sudo)"
    echo "====================================================="
}

case "$1" in
    server)
        echo "[*] Building and starting C# Server on port 5000..."
        cd "$REPO_DIR/csharp-server"
        dotnet run
        ;;
    client)
        echo "[*] Starting Python 3 Client..."
        cd "$REPO_DIR"
        python3 client.py
        ;;
    test)
        echo "[*] Running Python protocol unit test..."
        cd "$REPO_DIR"
        python3 protocol.py
        echo "[*] Running C# crypto demo..."
        cd "$REPO_DIR/crypto-demo-csharp"
        dotnet run
        ;;
    sniff)
        echo "[*] Starting tcpdump on port 5000 (Ctrl+C to stop)..."
        sudo tcpdump -i any -X -s 0 port 5000
        ;;
    *)
        show_usage
        exit 1
        ;;
esac
