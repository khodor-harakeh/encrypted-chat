# built-in python library to interact with low-level BSD socket API
import socket
# threading
import threading
# import from protocol
from protocol import send_encrypted_message, recv_encrypted_message, SHARED_KEY

# Shared State Flag
running = True

# Receiver Threads Function
def receive_loop(sock):
    global running
    while running:
        try:
            msg = recv_encrypted_message(sock, SHARED_KEY)
            if msg is None or msg == "/quit":
                print("\n[Client] Server disconnected or ended the chat.")
                running = False
                break
            print(f"\n[Server]: {msg}\nYou (Client): ", end="", flush=True)
        except Exception:
            break

# 1. Create a socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# 2. Connect to the server (destination IP + port)
client_socket.connect(("192.168.0.110", 5000))
print("[Client] Connected to server! Type /quit to exit.\n")

# Spawns and starts receiver thread
threading.Thread(target=receive_loop, args=(client_socket,), daemon=True).start()

# Runs until server/client disconnects by sending empty bytes b"" (ctrl + c) or sending "/quit"
while running:
    # 3. Send Client message
    message = input("You (Client): ")
    if not running:
        break
    send_encrypted_message(client_socket, message, SHARED_KEY)
    # if message string matched the pattern "/quit" closes the chat
    if message == "/quit":
        print("[Client] Closing chat.")
        break

# 5. Clean up
# close client's socket
client_socket.close()
