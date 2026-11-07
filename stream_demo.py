import socket
import threading
import time

def server():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.bind(("127.0.0.1", 5001))
    s.listen(1)
    conn, _ = s.accept()

    # Small delay to let client send BOTH messages before we read 
    time.sleep(1)

    data = conn.recv(1024)
    print(f"Server recieved ONE recv(): {data}")
    print(f"Length: {len(data)} bytes")

    conn.close()
    s.close()

def client():
    time.sleep(0.5) # Wait for server to be ready
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect(("127.0.0.1", 5001))

    # Send two seperate messages quickly
    s.send(b"Hello")
    s.send(b"World")
    print("Client sent: b'Hello' and b'World' as two seperate sends")

    time.sleep(2) # Wait for server to finish
    s.close()

# Run server in a thread, client in main
t = threading.Thread(target=server)
t.start()
client()
t.join()
