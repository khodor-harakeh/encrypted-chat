import socket

# 1. Create a socket (ask the OS for a communication endpoint)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# Prevents "Address already in use" when restarting the server quickly
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

# 2. Bind to an address (reserve this building + appartment)
server_socket.bind(("0.0.0.0", 5000))

# 3. Listen for incomming connections
server_socket.listen(1)
print("Server listening on 0.0.0.0:5000 ...")
print(f"My real IP: {socket.gethostbyname(socket.gethostname())}")

# 4. Accept a client (blocks until someone connects)
client_socket, client_address = server_socket.accept()
print(f"Client connected from {client_address}")

while True:
    # 5. Wait for client message
    data = client_socket.recv(1024)
    if not data:
        print("Client disconnected.")
        break
    
    message = data.decode("utf-8")
    if message == "/quit":
        print("Client ended the chat.")
        break

    print(f"Client: {message}")

    # 6. Send server reply
    reply = input("You (Server): ")
    client_socket.send(reply.encode("utf-8"))
    if reply == "/quit":
        print("Closing chat.")
        break

# 7. Clean up
client_socket.close()
server_socket.close()
