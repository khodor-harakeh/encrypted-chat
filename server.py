import socket

# 1. Create a socket (ask the OS for a communication endpoint)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Bind to an address (reserve this building + appartment)
server_socket.bind(("127.0.0.1", 5000))

# 3. Listen for incomming connections
server_socket.listen(1)
print("Server listening on 127.0.0.1:5000 ...")

# 4. Accept a client (blocks until someone connects)
client_socket, client_address = server_socket.accept()
print(f"Client connected from {client_address}")

# 5. Recieve data
data = client_socket.recv(1024)
print(f"Recieved: {data.decode()}")

# 6. Send a reply
client_socket.send("Hello from server!".encode())

# 7. Clean up
client_socket.close()
server_socket.close()
print("Server closed.")
