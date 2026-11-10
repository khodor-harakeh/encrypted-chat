# python library for BSD socket API
import socket

# Get my real IP
# SOCK_DGRAM -> UDP
s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
# Doesn't actually send data
s.connect(("8.8.8.8", 80))
# Returns server's real LAN IP
my_real_IP = s.getsockname()[0]   
# close socket s
s.close()

# 1. Create a socket (ask the OS for a communication endpoint)
# AF_INET -> IPv4
# SOCK_STREAM -> TCP
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# Prevents "Address already in use" when restarting the server quickly
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

# 2. Bind to an address (reserve this building + appartment)
server_socket.bind(("0.0.0.0", 5000))

# 3. Listen for incomming connections
server_socket.listen(1)
print("Server listening on 0.0.0.0:5000 ...")
# print server's real LAN IP
print(f"My real IP: {my_real_IP}")   

# 4. Accept a client (blocks until someone connects)
client_socket, client_address = server_socket.accept()
print(f"Client connected from {client_address}")

# This loop stays running until someone disconnects or ends the chat by sending "/quit"
while True:
    # 5. Wait for client message
    data = client_socket.recv(1024)
    # if empty bytes where sent (ctrl + c)
    if not data:
        print("Client disconnected.")
        break
    # convert recieved raw bytes to a string
    message = data.decode("utf-8")
    # if the recieved  string was actually equal to '/quit' end the chat
    if message == "/quit":
        print("Client ended the chat.")
        break

    # print client's message
    print(f"Client: {message}")

    # 6. Send server reply
    reply = input("You (Server): ")
    # convert the string to raw bytes to send
    client_socket.send(reply.encode("utf-8"))
    # if the sent string was equal to "/quit" end the chat
    if reply == "/quit":
        print("Closing chat.")
        break

# 7. Clean up
# Close the client's socket
client_socket.close()
# Close the server's socket
server_socket.close()
