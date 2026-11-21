# built-in python library to interact with low-level BSD socket API
import socket
# import from protocol
from protocol import send_encrypted_message, recv_encrypted_message, SHARED_KEY

# 1. Create a socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Connect to the server (destination IP + port)
client_socket.connect(("192.168.0.110", 5000))
print("[Client] Connected to server! Type /quit to exit.\n")

# Runs until server/client disconnects by sending empty bytes b"" (ctrl + c) or sending "/quit"
while True:
    # 3. Send Client message
    message = input("You (Client): ")
    send_encrypted_message(client_socket, message, SHARED_KEY)
    # if message string matched the pattern "/quit" closes the chat
    if message == "/quit":
        print("[Client] Closing chat.")
        break
    # recieved raw bytes decoded into a string
    reply = recv_encrypted_message(client_socket, SHARED_KEY)
    if reply is None:
        print("[Client] Server closed the connection.")
        break
    # if string equals "/quit" close chat
    if reply == "/quit":
        print("[Client] Server ended the chat.")
        break

    # print the server's message
    print(f"Server: {reply}")

# 5. Clean up
# close client's socket
client_socket.close()
