import socket

# 1. Create a socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Connect to the server (festination IP + port)
client_socket.connect(("192.168.0.107", 5000))
print("Connected to server! Type /quit to exit.\n")

while True:
    # 3. Send Client message
    message = input("You (Client): ")
    client_socket.send(message.encode("utf-8"))
    if message == "/quit":
        print("Closing chat.")
        break

    # 4. Wait for server reply
    data = client_socket.recv(1024)
    if not data:
        print("Server disconnected.")
        break
    
    reply = data.decode("utf-8")
    if reply == "/quit":
        print("Server ended the chat.")
        break

    print(f"Server: {reply}")

# 5. Clean up
client_socket.close()
