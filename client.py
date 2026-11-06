import socket

# 1. Create a socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Connect to the server (festination IP + port)
client_socket.connect(("127.0.0.1", 5000))
print("Connected to server!")

# 3. Send data
client_socket.send("Hello from client!".encode())

# 4. Recieve reply
data = client_socket.recv(1024)
print(f"Recieved: {data.decode()}")

# 5. Clean up
client_socket.close()
print("Client closed.")
