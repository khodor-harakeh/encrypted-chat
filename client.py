# built-in python library to interact with low-level BSD socket API
import socket

# 1. Create a socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Connect to the server (festination IP + port)
client_socket.connect(("192.168.0.107", 5000))
print("Connected to server! Type /quit to exit.\n")

# Runs until server/client disconnects by sending empty bytes b"" (ctrl + c) or sending "/quit"
while True:
    # 3. Send Client message
    message = input("You (Client): ")
    # encodes string into raw bytes (utf-8 standard)
    client_socket.send(message.encode("utf-8"))
    # if message string matched the pattern "/quit" closes the chat
    if message == "/quit":
        print("Closing chat.")
        break

    # 4. Wait for server reply
    data = client_socket.recv(1024)
    # the server sent b"" empty bytes
    if not data:
        print("Server disconnected.")
        break
    
    # recieved raw bytes decoded into a string
    reply = data.decode("utf-8")
    # if string equals "/quit" close chat
    if reply == "/quit":
        print("Server ended the chat.")
        break

    # print the server's message
    print(f"Server: {reply}")

# 5. Clean up
# close client's socket
client_socket.close()
